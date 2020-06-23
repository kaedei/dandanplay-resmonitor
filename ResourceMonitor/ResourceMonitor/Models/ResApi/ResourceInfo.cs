using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace ResourceMonitor.Models.ResApi
{
    /// <summary>
    /// 资源详细信息
    /// </summary>
    [XmlRoot("Resource", Namespace = "")]
    public class ResourceInfo
    {
        /// <summary>
        /// 资源标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 类型ID
        /// </summary>
        public int TypeId { get; set; }

        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 字幕组ID
        /// </summary>
        public int SubgroupId { get; set; }

        /// <summary>
        /// 字幕组名称
        /// </summary>
        public string SubgroupName { get; set; }

        /// <summary>
        /// 磁力链接
        /// </summary>
        public string Magnet { get; set; }

        /// <summary>
        /// 发布页
        /// </summary>
        public string PageUrl { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public string FileSize { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public string PublishDate { get; set; }

        /// <summary>
        /// 发布时间（DateTime类型）
        /// </summary>
        public DateTime PublishDateTime => DateTime.Parse(PublishDate, CultureInfo.InvariantCulture);

        /// <summary>
        /// 文件大小（MB)
        /// </summary>
        public decimal FileSizeInMB
        {
            get
            {
                var r = Regex.Match(FileSize, @"^(?<size>\d+(?:\.\d{1,3})?)(?<unit>MB|GB)$", RegexOptions.IgnoreCase);
                if (!r.Success)
                {
                    return 0;
                }
                var size = decimal.Parse(r.Groups["size"].Value, CultureInfo.InvariantCulture);
                var unit = r.Groups["unit"].Value.Equals("MB", StringComparison.InvariantCultureIgnoreCase) ? 1 : 1024;
                return size * unit;
            }
        }
    }
}