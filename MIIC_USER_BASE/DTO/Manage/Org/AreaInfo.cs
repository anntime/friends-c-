using Miic.Attribute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miic.Manage.Org
{
    [MiicTable(MiicParentStorageName = "MIIC_SOCIAL_COMMON", MiicStorageName = "AREA_INFO", Description = "区域信息表")]
    public partial class AreaInfo
    {
        [MiicField(MiicStorageName = "ID",IsIdentification=true,IsPrimaryKey=true, IsNotNull = true, MiicDbType = DbType.Int32, Description = "区域ID")]
        public int? ID { get; set; }
        [MiicField(MiicStorageName = "PID",  MiicDbType = DbType.Int32, Description = "区域父ID")]
        public int? PID { get; set; }
        [MiicField(MiicStorageName = "CODE", IsNotNull = true, MiicDbType = DbType.String, Description = "编码")]
        public string Code { get; set; }
        [MiicField(MiicStorageName = "NAME", IsNotNull = true, MiicDbType = DbType.String, Description = "名称")]
        public string Name { get; set; }
        [MiicField(MiicStorageName = "FULL_NAME", IsNotNull = true, MiicDbType = DbType.String, Description = "全称")]
        public string FullName { get; set; }
        [MiicField(MiicStorageName = "TYPE", IsNotNull = true, MiicDbType = DbType.String, Description = "区域类型")]
        public string Type { get; set; }
        [MiicField(MiicStorageName = "BEGIN_TIME", IsNotNull = true, MiicDbType = DbType.DateTime, Description = "开始时间")]
        public DateTime? BeginTime { get; set; }
        [MiicField(MiicStorageName = "END_TIME",  MiicDbType = DbType.DateTime, Description = "失效时间")]
        public DateTime? EndTime { get; set; }
        [MiicField(MiicStorageName = "VALID", IsNotNull = true, MiicDbType = DbType.String, Description = "有效性")]
        public string Valid { get; set; }
    }
}
