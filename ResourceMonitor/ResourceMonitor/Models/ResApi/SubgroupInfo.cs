using System.Collections.Generic;
using System.Xml.Serialization;

namespace ResourceMonitor.Models.ResApi
{
    /// <summary>
    /// 字幕组详细信息
    /// </summary>
    public class SubgroupInfo
    {
        /// <summary>
        /// 字幕组ID
        /// </summary>
        [XmlAttribute("Id")]
        public int Id { get; set; }
        /// <summary>
        /// 字幕组名称
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        #region IdEqualityComparer

        private sealed class IdEqualityComparer : IEqualityComparer<SubgroupInfo>
        {
            public bool Equals(SubgroupInfo x, SubgroupInfo y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Id == y.Id;
            }

            public int GetHashCode(SubgroupInfo obj)
            {
                return obj.Id;
            }
        }

        private static readonly IEqualityComparer<SubgroupInfo> m_idComparerInstance = new IdEqualityComparer();

        public static IEqualityComparer<SubgroupInfo> IdComparer
        {
            get { return m_idComparerInstance; }
        }

        #endregion
    }
}