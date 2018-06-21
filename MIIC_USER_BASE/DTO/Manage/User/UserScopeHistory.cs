using Miic.Attribute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miic.Manage.User
{
    [MiicTable(MiicParentStorageName = "MIIC_SOCIAL_COMMON", MiicStorageName = "USER_SCORE_HISTORY", Description = "用户积分历史表")]
    public partial class UserScopeHistory
    {
        public UserScopeHistory()
        {

        }
        [MiicField(MiicStorageName = "ID", IsPrimaryKey = true, IsNotNull = true, MiicDbType = DbType.String, Description = "唯一码")]
        public string ID { get; set; }
        [MiicField(MiicStorageName = "SERVICE_ID", IsPrimaryKey = true, IsNotNull = true, MiicDbType = DbType.String, Description = "平台服务ID")]
        public string ServiceID { get; set; }
        [MiicField(MiicStorageName = "GET_WAY", IsNotNull = true, MiicDbType = DbType.String, Description = "获取方式")]
        public string GetWay { get; set; }
         [MiicField(MiicStorageName = "BUSINESS_ID", IsNotNull = true, MiicDbType = DbType.String, Description = "业务ID（可能是点赞ID、发布ID等）")]
        public string BusinessID { get; set; }
        [MiicField(MiicStorageName = "USER_ID", IsNotNull = true, MiicDbType = DbType.String, Description = "用户ID")]
        public string UserID { get; set; }
        [MiicField(MiicStorageName = "USER_NAME", IsNotNull = true, MiicDbType = DbType.String, Description = "用户名称")]
        public string UserName { get; set; }
        [MiicField(MiicStorageName = "SCORE", MiicDbType = DbType.Int32, Description = "积分")]
        public int? Score { get; set; }
        [MiicField(MiicStorageName = "CREATE_TIME", IsNotNull = true, MiicDbType = DbType.DateTime, Description = "创建时间")]
        public DateTime? CreateTime { get; set; }
        [MiicField(MiicStorageName = "SORT_NO", IsIdentification = true, IsNotNull = true, MiicDbType = DbType.Int32, Description = "排序")]
        public int? SortNo { get; set; }
    }
}
