using Miic.Attribute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miic.Manage.User
{
    public abstract class GeneralSimpleUserView
    {
        [MiicField(MiicStorageName = "USER_ID", MiicDbType = DbType.String, Description = "唯一码")]
        public string UserID { get; set; }
        [MiicField(MiicStorageName = "MIIC_SOCIAL_CODE", MiicDbType = DbType.String, Description = "用户名")]
        public string SocialCode { get; set; }
        [MiicField(MiicStorageName = "USER_NAME", MiicDbType = DbType.String, Description = "姓名")]
        public string UserName { get; set; }
        [MiicField(MiicStorageName = "ORG_NAME", MiicDbType = DbType.String, Description = "企业名称")]
        public string OrgName { get; set; }
        [MiicField(MiicStorageName = "USER_TYPE", MiicDbType = DbType.String, Description = "用户类型")]
        public string UserType { get; set; }
        [MiicField(MiicStorageName = "MICRO_USER_URL", MiicDbType = DbType.String, Description = "用户头像")]
        public string UserUrl { get; set; }
        [MiicField(MiicStorageName = "USER_LEVEL", MiicDbType = DbType.String, Description = "用户Level")]
        public string UserLevel { get; set; }
    }
}
