using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Refit;
using ResourceMonitor.Models.DandanplayApi;
using ResourceMonitor.Services.Declaration;

namespace ResourceMonitor.Services.Implementation
{
    /// <summary>
    /// 后台同步最新的下载规则
    /// </summary>
    public class SyncRulesBackgroundService : BackgroundService
    {
        private int _executionCount = 0;
        private readonly IDandanplayApi _dandanplayApi;
        private readonly IRulesContainer _rulesContainer;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SyncRulesBackgroundService> _logger;
        private LoginResponse _lastLoginResponse; //上次登录成功的请求，包含 jwt token 和 token 的过期时间

        public SyncRulesBackgroundService(IDandanplayApi dandanplayApi, 
            IRulesContainer rulesContainer,
            IConfiguration configuration,
            ILogger<SyncRulesBackgroundService> logger)
        {
            _dandanplayApi = dandanplayApi;
            _rulesContainer = rulesContainer;
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[规则同步] 开始运行.");

            while (!stoppingToken.IsCancellationRequested)
            {
                _rulesContainer.IsUpdating = true;
                bool success = await DoWork();
                _rulesContainer.IsUpdating = false;
                //每十分钟运行一次
                _logger.LogInformation($"[规则同步] 在后台运行完成，等待 10 分钟后重新执行。此次运行结果为：{success}");
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }

            _logger.LogInformation("因触发取消，[规则同步] 终止运行.");
        }

        private async Task<bool> DoWork()
        {
            //增加计数器
            var count = Interlocked.Increment(ref _executionCount);
            _logger.LogInformation($"[规则同步] 在后台第 {count} 次运行");

           //之前登录过，先尝试直接刷新 jwt token
            if (_lastLoginResponse != null && _lastLoginResponse.tokenExpireTime >= DateTime.UtcNow)
            {
                try
                {
                    _lastLoginResponse = await _dandanplayApi.Renew("Bearer " + _lastLoginResponse.token);
                }
                catch (ApiException ex)
                {
                    _logger.LogDebug(ex, "尝试刷新 jwt token 失败");
                    _lastLoginResponse = null;
                }
            }

            //尝试登录，填充登录参数
            if (_lastLoginResponse == null)
            {
                //读取 appsettings 中的用户名、密码、AppID、AppSecret等信息
                var userName = _configuration["Api:UserName"];
                var password = _configuration["Api:Password"];
                var appId = _configuration["Api:AppId"];
                var appSecret = _configuration["Api:AppSecret"];
                
                var loginRequest = new LoginRequest
                {
                    userName = userName,
                    password = password,
                    appId = appId,
                    unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                };
                loginRequest.hash = CalculateLoginRequestHash(loginRequest, appSecret);
                try
                {
                    var loginResponse = await _dandanplayApi.Login(loginRequest);

                    //先判断是否登录失败了
                    if (loginResponse.registerRequired || !loginResponse.success ||
                        string.IsNullOrWhiteSpace(loginResponse.token))
                    {
                        _logger.LogWarning($"登录失败，原因: {loginResponse.errorCode} {loginResponse.errorMessage}");
                        return false;
                    }
                    
                    //存储 jwt token
                    _lastLoginResponse = loginResponse;
                }
                catch (ApiException ex)
                {
                    _logger.LogDebug(ex, "尝试登录失败");
                    return false;
                }

            }

            //尝试同步下载规则
            //获取服务器上当前用户最新版本的规则
            var serverRules = await _dandanplayApi.SyncAutoDownloadRules(new AutoDownloadRuleSyncRequest
                {
                    currentRuleIds = _rulesContainer.LocalRules.Select(r => r.id).ToArray()
                }, "Bearer " + _lastLoginResponse.token);
            //合并本地和服务器端的规则
            _rulesContainer.SyncWithServerRules(serverRules);
            
            return true;
        }

        /// <summary>
        /// 计算登录参数中的 hash 参数
        /// </summary>
        public string CalculateLoginRequestHash(LoginRequest loginRequest, string appSecret)
        {
            //算法参考 https://api.acplay.net/swagger/ui/index#!/Auth/Auth_Login
            var concat = loginRequest.appId + loginRequest.password + loginRequest.unixTimestamp +
                         loginRequest.userName + appSecret;
            using var md5 = MD5.Create();
            var byteArray = md5.ComputeHash(Encoding.UTF8.GetBytes(concat));
            var sb = new StringBuilder(32);
            foreach (var b in byteArray)
            {
                sb.AppendFormat("{0:X2}", b);
            }
            return sb.ToString();
        }
    }
}