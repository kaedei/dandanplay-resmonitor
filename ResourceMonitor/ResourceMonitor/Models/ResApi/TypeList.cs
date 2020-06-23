using System.Xml.Serialization;

namespace ResourceMonitor.Models.ResApi
{
    /// <summary>
    /// 资源类型列表
    /// </summary>
    [XmlRoot("Types")]
    public class TypeList
    {
        /// <summary>
        /// 包含资源类型信息的集合
        /// </summary>
        [XmlElement("Type")]
        public TypeInfo[] Types { get; set; }
    }
}