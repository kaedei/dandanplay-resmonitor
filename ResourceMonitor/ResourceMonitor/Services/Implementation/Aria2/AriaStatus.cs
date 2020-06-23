using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AriaNet.Attributes
{
	public class AriaStatus
	{

		[JsonProperty("bitfield")]
		public string Bitfield { get; set; }

		[JsonProperty("completedLength")]
		public string CompletedLength { get; set; }

		[JsonProperty("connections")]
		public string Connections { get; set; }

		[JsonProperty("dir")]
		public string Dir { get; set; }

		[JsonProperty("downloadSpeed")]
		public string DownloadSpeed { get; set; }

		[JsonProperty("files")]
		public List<AriaFile> Files { get; set; }

		[JsonProperty("gid")]
		public string TaskId { get; set; }

		[JsonProperty("numPieces")]
		public string NumPieces { get; set; }

		[JsonProperty("pieceLength")]
		public string PieceLength { get; set; }

		[JsonProperty("status")]
		public string Status { get; set; }

		[JsonProperty("totalLength")]
		public string TotalLength { get; set; }

		[JsonProperty("uploadLength")]
		public string UploadLength { get; set; }

		[JsonProperty("uploadSpeed")]
		public string UploadSpeed { get; set; }

		[JsonProperty("infoHash")]
		public string InfoHash { get; set; }
	}
}
