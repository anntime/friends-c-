using Miic.Attribute;
using System.Data;

namespace Miic.Manage.Org
{
    [MiicTable(MiicParentStorageName = "MIIC_SOCIAL_COMMON", MiicStorageName = "TRADE_INFO", Description = "行业表")]
    public class TradeInfo
    {
        [MiicField(MiicStorageName = "ID", IsNotNull = true, IsPrimaryKey = true, MiicDbType = DbType.String, Description = "唯一码")]
        public string ID { get; set; }
        [MiicField(MiicStorageName = "NAME", IsNotNull = true, MiicDbType = DbType.String, Description = "行业名称")]
        public string Name { get; set; }
        [MiicField(MiicStorageName = "DESCRIPTION", MiicDbType = DbType.String, Description = "描述")]
        public string Description { get; set; }
        [MiicField(MiicStorageName = "SORT_NO", IsNotNull = true, IsIdentification = true, MiicDbType = DbType.Int32, Description = "排序码")]
        public int? SortNo { get; set; }
    }
}
