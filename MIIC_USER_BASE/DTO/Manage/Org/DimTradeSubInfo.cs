using Miic.Attribute;
using System;
using System.Data;

namespace Miic.Manage.Org
{
    [MiicTable(MiicParentStorageName = "MIIC_SOCIAL_COMMON", MiicStorageName = "DIM_TRADE_SUB", Description = "小行业维度表")]
    public partial class DimTradeSubInfo
    {
        [MiicField(MiicStorageName = "TRADE_SUB_ID", IsNotNull = true, MiicDbType = DbType.String, Description = "小行业编码")]
        public string TradeSubID { get; set; }
        [MiicField(MiicStorageName = "NAME", IsNotNull = true, MiicDbType = DbType.String, Description = "小行业名称")]
        public string Name { get; set; }
        [MiicField(MiicStorageName = "PID", IsNotNull = true, MiicDbType = DbType.String, Description = "小行业父ID")]
        public string PID { get; set; }
        [MiicField(MiicStorageName = "TRADE_ID", IsNotNull = true, MiicDbType = DbType.String, Description = "大行业编码")]
        public string TradeID { get; set; }
        [MiicField(MiicStorageName = "BEGIN_TIME", IsNotNull = true, MiicDbType = DbType.DateTime, Description = "开始时间")]
        public DateTime? BeginTime { get; set; }
        [MiicField(MiicStorageName = "END_TIME", MiicDbType = DbType.DateTime, Description = "结束时间")]
        public DateTime? EndTime { get; set; }
        [MiicField(MiicStorageName = "VALID", IsNotNull = true, MiicDbType = DbType.String, Description = "有效性")]
        public string Valid { get; set; }
        [MiicField(MiicStorageName = "SORT_NO", IsIdentification = true, IsNotNull = true, MiicDbType = DbType.Int32, Description = "排序码")]
        public int? SortNo { get; set; }
    }
}
