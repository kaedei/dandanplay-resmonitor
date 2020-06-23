using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AriaNet.Attributes;
using Newtonsoft.Json;

namespace AriaNet
{
	public class AriaManager
	{
		private HttpClient m_client;

		private readonly string m_rpcUrl;

		private readonly string m_token;

		public AriaManager(string rpcUrl, string token, HttpClient client)
		{
			m_rpcUrl = rpcUrl;
			m_token = token;
			m_client = client;
		}

		private async Task<T> RpcInvoke<T>(string method, params object[] parameters)
		{
			var p = parameters?.ToList() ?? new List<object>(1);
			if (!string.IsNullOrWhiteSpace(m_token))
			{
				p.Insert(0, $"token:{m_token}");
			}
			var content = new StringContent(JsonConvert.SerializeObject(new
			{
				jsonrpc = "2.0",
				id = "resmonitor",
				method = method,
				@params = p
			}));
			content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
			var response = await m_client.PostAsync(m_rpcUrl, content);
			var responseString = await response.Content.ReadAsStringAsync();
			var result = JsonConvert.DeserializeAnonymousType(responseString, new
			{
				result = default(T),
				error = new
				{
					code = 0,
					message = string.Empty
				}
			});
			if (result.error == null || result.error.code == 0)
			{
				return result.result;
			}
			else
			{
				throw new Exception($"[{result.error.code}] {result.error.message}");
			}
		}

		public async Task<string> AddUri(List<string> uriList)
		{
			return await RpcInvoke<string>("aria2.addUri", uriList);
		}

		public async Task<string> AddUri(List<string> uriList, string userAgent, string referrer)
		{
			return await RpcInvoke<string>("aria2.addUri", uriList,
				new Dictionary<string, string>
				{
					{"user-agent", userAgent},
					{"referer", referrer}
				});
		}

		public async Task<string> AddMetaLink(string filePath)
		{
			var metaLinkBase64 = Convert.ToBase64String(File.ReadAllBytes(filePath));
			return await RpcInvoke<string>("aria2.addMetalink", metaLinkBase64);
		}

		public async Task<string> AddTorrent(string filePath)
		{
			return await AddTorrent(File.ReadAllBytes(filePath));
		}

		public async Task<string> AddTorrent(byte[] torrentBytes)
		{
			var torrentBase64 = Convert.ToBase64String(torrentBytes);
			return await RpcInvoke<string>("aria2.addTorrent", torrentBase64);
		}

		public async Task<string> RemoveTask(string gid, bool forceRemove = false)
		{
			if (!forceRemove)
			{
				return await RpcInvoke<string>("aria2.remove", gid);
			}
			else
			{
				return await RpcInvoke<string>("aria2.forceRemove", gid);
			}
		}

		public async Task<string> PauseTask(string gid, bool forcePause = false)
		{
			if (!forcePause)
			{
				return await RpcInvoke<string>("aria2.pause", gid);
			}
			else
			{
				return await RpcInvoke<string>("aria2.forcePause", gid);
			}
		}

		public async Task<bool> PauseAllTasks()
		{
			return (await RpcInvoke<string>("aria2.pauseAll")).Contains("OK");
		}

		public async Task<bool> UnpauseAllTasks()
		{
			return (await RpcInvoke<string>("aria2.unpauseAll")).Contains("OK");
		}

		public async Task<string> UnpauseTask(string gid)
		{
			return await RpcInvoke<string>("aria2.unpause", gid);
		}

		public async Task<AriaStatus> GetStatus(string gid)
		{
			return await RpcInvoke<AriaStatus>("aria2.tellStatus", gid);
		}

		public async Task<AriaStatus[]> GetAllStatus()
		{
			var task1 = GetActiveStatus();
			var task2 = GetWaitingStatus();
			var task3 = GetStoppedStatus();
			await Task.WhenAll(task1, task2, task3);
			return task1.Result.Concat(task2.Result).Concat(task3.Result).ToArray();
		}

		public async Task<AriaUri> GetUris(string gid)
		{
			return await RpcInvoke<AriaUri>("aria2.getUris", gid);
		}

		public async Task<AriaFile> GetFiles(string gid)
		{
			return await RpcInvoke<AriaFile>("aria2.getFiles", gid);
		}

		public async Task<AriaTorrent> GetPeers(string gid)
		{
			return await RpcInvoke<AriaTorrent>("aria2.getPeers", gid);
		}

		public async Task<AriaServer> GetServers(string gid)
		{
			return await RpcInvoke<AriaServer>("aria2.getServers", gid);
		}

		public async Task<AriaStatus> GetActiveStatus(string gid)
		{
			return await RpcInvoke<AriaStatus>("aria2.tellActive", gid);
		}

		public async Task<AriaStatus[]> GetActiveStatus()
		{
			return await RpcInvoke<AriaStatus[]>("aria2.tellActive");
		}

		public async Task<AriaStatus[]> GetWaitingStatus()
		{
			return await RpcInvoke<AriaStatus[]>("aria2.tellWaiting", 0, 999);
		}

		public async Task<AriaStatus[]> GetStoppedStatus()
		{
			return await RpcInvoke<AriaStatus[]>("aria2.tellStopped", 0, 999);
		}

		public async Task<AriaOption> GetOption(string gid)
		{
			return await RpcInvoke<AriaOption>("aria2.getOption", gid);
		}


		public async Task<bool> ChangeOption(string gid, AriaOption option)
		{
			return (await RpcInvoke<string>("aria2.changeOption", gid, option))
				.Contains("OK");
		}

		public async Task<AriaOption> GetGlobalOption()
		{
			return await RpcInvoke<AriaOption>("aria2.getGlobalOption");
		}

		public async Task<bool> ChangeGlobalOption(AriaOption option)
		{
			return (await RpcInvoke<string>("aria2.changeGlobalOption", option))
				.Contains("OK");
		}

		public async Task<AriaGlobalStatus> GetGlobalStatus()
		{
			return await RpcInvoke<AriaGlobalStatus>("aria2.getGlobalStat");
		}

		public async Task<bool> PurgeDownloadResult()
		{
			return (await RpcInvoke<string>("aria2.purgeDownloadResult")).Contains("OK");
		}

		public async Task<bool> RemoveDownloadResult(string gid)
		{
			return (await RpcInvoke<string>("aria2.removeDownloadResult", gid))
				.Contains("OK");
		}

		public async Task<AriaVersionInfo> GetVersion()
		{
			return await RpcInvoke<AriaVersionInfo>("aria2.getVersion");
		}

		public async Task<AriaSession> GetSessionInfo()
		{
			return await RpcInvoke<AriaSession>("aria2.getSessionInfo");
		}

		public async Task<bool> Shutdown(bool forceShutdown = false)
		{
			if (!forceShutdown)
			{
				return (await RpcInvoke<string>("aria2.shutdown")).Contains("OK");
			}
			else
			{
				return (await RpcInvoke<string>("aria2.forceShutdown")).Contains("OK");
			}
		}

		public async Task<bool> SaveSession()
		{
			return (await RpcInvoke<string>("aria2.saveSession")).Contains("OK");
		}
	}
}