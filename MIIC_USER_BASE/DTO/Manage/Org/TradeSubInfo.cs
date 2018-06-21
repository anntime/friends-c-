using Miic.Attribute;
using System.Data;

namespace Miic.Manage.Org
{
    [MiicTable(MiicParentStorageName = "MIIC_SOCIAL_COMMON", MiicStorageName = "TRADE_SUB_INFO", Description = "子行业表")]
    public class TradeSubInfo
    {
        [MiicField(MiicStorageName = "ID", IsNotNull = true, IsPrimaryKey = true, MiicDbType = DbType.String, Description = "唯一码")]
        public string ID { get; set; }
        [MiicField(MiicStorageName = "NAME", IsNotNull = true, MiicDbType = DbType.String, Description = "子行业名称")]
        public string Name { get; set; }
        [MiicField(MiicStorageName = "PID", MiicDbType = DbType.String, Description = "父ID")]
        public string PID { get; set; }
        [MiicField(MiicStorageName = "TRADE_ID", MiicDbType = DbType.String, Description = "父行业ID")]
        public string TradeID { get; set; }
        [MiicField(MiicStorageName = "DESCRIPTION", MiicDbType = DbType.String, Description = "描述")]
        public string Description { get; set; }
        [MiicField(MiicStorageName = "SORT_NO", IsNotNull = true, IsIdentification = true, MiicDbType = DbType.Int32, Description = "唯一码")]
        public int? SortNo { get; set; }
    }
}
