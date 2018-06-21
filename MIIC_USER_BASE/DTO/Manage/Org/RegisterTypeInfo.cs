using Miic.Attribute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miic.Manage.Org
{
    [MiicTable(MiicParentStorageName = "MIIC_SOCIAL_COMMON", MiicStorageName = "REGISTER_TYPE_INFO", Description = "注册类型信息表")]
    public partial class RegisterTypeInfo
    {
        [MiicField(MiicStorageName = "ID", IsNotNull = true, IsPrimaryKey = true, MiicDbType = DbType.String, Description = "唯一码")]
        public string ID { get; set; }
        [MiicField(MiicStorageName = "NAME", IsNotNull = true, MiicDbType = DbType.String, Description = "名称")]
        public string Name { get; set; }
        [MiicField(MiicStorageName = "PID", MiicDbType = DbType.String, Description = "父ID")]
        public string PID { get; set; }
        [MiicField(MiicStorageName = "DESCRIPTION", MiicDbType = DbType.String, Description = "描述")]
        public string Description { get; set; }
        [MiicField(MiicStorageName = "SORT_NO", IsIdentification = true, IsNotNull = true, MiicDbType = DbType.Int32, Description = "排序码")]
        public int? SortNo { get; set; }
    }
}
