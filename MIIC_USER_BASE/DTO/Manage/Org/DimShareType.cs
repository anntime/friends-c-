using Miic.Attribute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miic.Manage.Org
{
    [MiicTable(MiicParentStorageName = "MIIC_SOCIAL_COMMON", MiicStorageName = "DIM_SHARE_TYPE", Description = "控股维度表")]
    public partial class DimShareType
    {
        [MiicField(MiicStorageName = "SHARE_TYPE_ID", IsNotNull = true, MiicDbType = DbType.String, Description = "控股编码")]
        public string ShareTypeID { get; set; }
        [MiicField(MiicStorageName = "NAME", IsNotNull = true, MiicDbType = DbType.String, Description = "控股名称")]
        public string Name { get; set; }
        [MiicField(MiicStorageName = "BEGIN_TIME", IsNotNull = true, MiicDbType = DbType.DateTime, Description = "开始时间")]
        public DateTime? BeginTime { get; set; }
        [MiicField(MiicStorageName = "END_TIME", MiicDbType = DbType.String, Description = "结束时间")]
        public DateTime? EndTime { get; set; }
        [MiicField(MiicStorageName = "VALID", IsNotNull = true, MiicDbType = DbType.String, Description = "有效性")]
        public string Valid { get; set; }
        [MiicField(MiicStorageName = "SORT_NO", IsIdentification = true, IsPrimaryKey = true, IsNotNull = true, MiicDbType = DbType.Int32, Description = "主键编码")]
        public int? SortNo { get; set; }
    }
}
