using Miic.Attribute;
using System.Data;

namespace Miic.Manage.Org
{
    [MiicTable(MiicParentStorageName = "MIIC_SOCIAL_COMMON", MiicStorageName = "SCALE_INFO", Description = "规模表")]
    public partial class ScaleInfo
    {
        [MiicField(MiicStorageName = "ID", IsNotNull = true, IsPrimaryKey = true, MiicDbType = DbType.String, Description = "唯一码")]
        public string ID { get; set; }
        [MiicField(MiicStorageName = "NAME", IsNotNull = true, MiicDbType = DbType.String, Description = "规模名称")]
        public string Name { get; set; }
        [MiicField(MiicStorageName = "DESCRIPTION", MiicDbType = DbType.String, Description = "描述")]
        public string Description { get; set; }
        [MiicField(MiicStorageName = "SORT_NO", IsNotNull = true, IsIdentification = true, MiicDbType = DbType.Int32, Description = "唯一码")]
        public int? SortNo { get; set; }
    }
}
