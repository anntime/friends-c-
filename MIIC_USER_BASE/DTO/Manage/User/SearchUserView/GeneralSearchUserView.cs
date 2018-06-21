using Miic.Attribute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miic.Manage.User
{
    
    public abstract class GeneralSearchUserView
    {
        [MiicField(MiicStorageName = "ID", IsNotNull = true, MiicDbType = DbType.String, Description = "唯一码")]
        public string ID { get; set; }
        [MiicField(MiicStorageName = "MIIC_SOCIAL_CODE", IsNotNull = true, MiicDbType = DbType.String, Description = "用户编码")]
        public string SocialCode { get; set; }
        [MiicField(MiicStorageName = "USER_LEVEL", IsNotNull = true, MiicDbType = DbType.String, Description = "用户等级")]
        public string Level { get; set; }
        [MiicField(MiicStorageName = "NAME", IsNotNull = true, MiicDbType = DbType.String, Description = "用户名")]
        public string Name { get; set; }
        [MiicField(MiicStorageName = "USER_NAME", IsNotNull = true, MiicDbType = DbType.String, Description = "用户名(含部门)")]
        public string UserName { get; set; }
        [MiicField(MiicStorageName = "MICRO_USER_URL", IsNotNull = true, MiicDbType = DbType.String, Description = "用户头像")]
        public string UserUrl { get; set; }
        [MiicField(MiicStorageName = "MOBILE", MiicDbType = DbType.String, Description = "手机")]
        public string Mobile { get; set; }
        [MiicField(MiicStorageName = "MOBILE_BIND", IsNotNull = true, MiicDbType = DbType.String, Description = "手机绑定")]
        public string MobileBind { get; set; }
        [MiicField(MiicStorageName = "EMAIL", MiicDbType = DbType.String, Description = "Email")]
        public string Email { get; set; }
        [MiicField(MiicStorageName = "EMAIL_BIND", IsNotNull = true, MiicDbType = DbType.String, Description = "邮箱绑定")]
        public string EmailBind { get; set; }
        [MiicField(MiicStorageName = "USER_TYPE", IsNotNull = true, MiicDbType = DbType.String, Description = "用户类型")]
        public string UserType { get; set; }
        [MiicField(MiicStorageName = "ACTIVATE_FLAG", IsNotNull = true, MiicDbType = DbType.String, Description = "是否激活")]
        public string ActivateFlag { get; set; }
        [MiicField(MiicStorageName = "DISABLED_FLAG", IsNotNull = true, MiicDbType = DbType.String, Description = "是否禁用")]
        public string DisabledFlag { get; set; }
        [MiicField(MiicStorageName = "ACTIVATE_TIME", MiicDbType = DbType.DateTime, Description = "激活时间")]
        public DateTime? ActivateTime { get; set; }
        [MiicField(MiicStorageName = "DISABLED_TIME", MiicDbType = DbType.DateTime, Description = "禁用时间")]
        public DateTime? DisabledTime { get; set; }
        [MiicField(MiicStorageName = "DISABLED_REASON", MiicDbType = DbType.String, Description = "禁用原因")]
        public string DisabledReason { get; set; }
        [MiicField(MiicStorageName = "VALID", IsNotNull = true, MiicDbType = DbType.String, Description = "有效性")]
        public string Valid { get; set; }
        [MiicField(MiicStorageName = "REMARK", MiicDbType = DbType.String, Description = "备注")]
        public string Remark { get; set; }
        [MiicField(MiicStorageName = "SORT_NO", MiicDbType = DbType.Int32, IsIdentification=true,IsNotNull=true, Description = "排序")]
        public int? SortNo { get; set; }

    }
}
