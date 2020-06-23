using System.Xml.Serialization;

namespace ResourceMonitor.Models.ResApi
{
    /// <summary>
	/// 资源列表
	/// </summary>
	[XmlRoot("Resources")]
	public class ResourceList
	{
		[XmlAttribute("HasMore")]
		public bool HasMore { get; set; }
		/// <summary>
		/// 包含每个资源详细信息的集合
		/// </summary>
		[XmlElement("Resource")]
		public ResourceInfo[] Resources { get; set; }
	}
}