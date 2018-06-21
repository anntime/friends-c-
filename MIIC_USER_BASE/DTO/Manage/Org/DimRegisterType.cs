using Miic.Attribute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miic.Manage.Org
{
    [MiicTable(MiicParentStorageName = "MIIC_SOCIAL_COMMON", MiicStorageName = "DIM_REGISTER_TYPE", Description = "注册类型维度表")]
    public class DimRegisterType
    {
        [MiicField(MiicStorageName = "REGISTER_TYPE_ID", IsNotNull = true, MiicDbType = DbType.String, Description = "注册类型编码")]
        public string RegisterTypeID { get; set; }
        [MiicField(MiicStorageName = "NAME", IsNotNull = true, MiicDbType = DbType.String, Description = "注册类型名称")]
        public string Name { get; set; }
        [MiicField(MiicStorageName = "PID", MiicDbType = DbType.String, Description = "注册类型父编码")]
        public string PID { get; set; }
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
