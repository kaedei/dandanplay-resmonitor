using System.Collections.Generic;
using System.Xml.Serialization;

namespace ResourceMonitor.Models.ResApi
{
    /// <summary>
    /// 资源类型信息
    /// </summary>
    public class TypeInfo
    {
        /// <summary>
        /// 资源类型ID
        /// </summary>
        [XmlAttribute("Id")]
        public int Id { get; set; }
        
        /// <summary>
        /// 资源类型名称
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        #region IdEqualityComparer

        private sealed class IdEqualityComparer : IEqualityComparer<TypeInfo>
        {
            public bool Equals(TypeInfo x, TypeInfo y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Id == y.Id;
            }

            public int GetHashCode(TypeInfo obj)
            {
                return obj.Id;
            }
        }

        private static readonly IEqualityComparer<TypeInfo> m_idComparerInstance = new IdEqualityComparer();

        public static IEqualityComparer<TypeInfo> IdComparer
        {
            get { return m_idComparerInstance; }
        }

        #endregion
    }
}