using System.Xml.Serialization;

namespace ResourceMonitor.Models.ResApi
{
    /// <summary>
    /// 字幕组列表
    /// </summary>
    [XmlRoot("Subgroups")]
    public class SubgroupList
    {
        /// <summary>
        /// 包含字幕组详细信息的集合
        /// </summary>
        [XmlElement("Subgroup")]
        public SubgroupInfo[] Subgroups { get; set; }
    }
}