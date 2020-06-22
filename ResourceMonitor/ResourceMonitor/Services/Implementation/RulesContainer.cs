using System.Collections.Concurrent;
using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using ResourceMonitor.Models.DandanplayApi;
using ResourceMonitor.Services.Declaration;

namespace ResourceMonitor.Services.Implementation
{
    public class RulesContainer : IRulesContainer
    {
        private readonly ILogger<RulesContainer> _logger;
        private readonly ConcurrentDictionary<string, AutoDownloadRule> _localRules = new ConcurrentDictionary<string, AutoDownloadRule>();
        private bool _isUpdating = true;

        public RulesContainer(ILogger<RulesContainer> logger)
        {
            _logger = logger;
        }

        public bool IsUpdating
        {
            get => _isUpdating;
            set
            {
                _logger.LogDebug($"RulesContainer.IsUpdating 设置为 {value}");
                _isUpdating = value;
            }
        }

        public ImmutableList<AutoDownloadRule> LocalRules => _localRules.Values.ToImmutableList();
        
        public void SyncWithServerRules(AutoDownloadRuleListResponse serverRules)
        {
            _logger.LogInformation("开始同步服务器规则");
            //移除服务器端已经删除的规则
            foreach (var removedRuleId in serverRules.removedRuleIds ?? new string[0])
            {
                _localRules.TryRemove(removedRuleId, out var removedRule);
                _logger.LogInformation($"删除规则 {removedRule}");
            }
            
            //添加或更新规则
            foreach (var serverRule in serverRules.rules)
            {
                //新增规则
                if (!_localRules.ContainsKey(serverRule.id))
                {
                    _localRules.TryAdd(serverRule.id, serverRule);
                    _logger.LogInformation($"新增规则 {serverRule}");
                }
                else //更新规则
                {
                    var localRule = _localRules[serverRule.id];
                    if (serverRule.version > localRule.version)
                    {
                        _localRules[serverRule.id] = serverRule;
                        _logger.LogInformation($"更新规则-原规则 {localRule}");
                        _logger.LogInformation($"更新规则-替换为 {serverRule}");
                    }
                }
            }
            
            _logger.LogInformation($"规则同步完毕，当前本地规则共有 {_localRules.Count} 个");
        }

    }
}