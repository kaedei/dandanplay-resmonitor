using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ResourceMonitor.Models.Web;
using ResourceMonitor.Services.Declaration;

namespace ResourceMonitor.Controllers
{
    [ApiController]
    [Route("server")]
    public class ServerController: ControllerBase
    {
        private readonly IRulesContainer _rulesContainer;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ServerController> _logger;

        public ServerController(IRulesContainer rulesContainer, IConfiguration configuration, ILogger<ServerController> logger)
        {
            _rulesContainer = rulesContainer;
            _configuration = configuration;
            _logger = logger;
        }
        
        [Route("status")]
        public ServerStatusViewModel GetServerStatus()
        {
            _logger.LogInformation("(Web)刷新服务器当前状态");
            var model = new ServerStatusViewModel
            {
                version = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                downloader = _configuration["Downloader"],
                rules = _rulesContainer.LocalRules.ToArray()
            };
            return model;
        }
        
    }
}