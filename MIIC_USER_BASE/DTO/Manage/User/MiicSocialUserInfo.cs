using Miic.Attribute;
using System;
using System.Data;

namespace Miic.Manage.User
{
    [MiicTable(MiicParentStorageName = "MIIC_SOCIAL_COMMON", MiicStorageName = "MIIC_SOCIAL_USER", Description = "用户信息表")]
    public partial class MiicSocialUserInfo
    {
        public MiicSocialUserInfo() { }
        [MiicField(MiicStorageName = "ID", IsPrimaryKey = true, IsNotNull = true, MiicDbType = DbType.String, Description = "唯一码")]
        public string ID { get; set; }
        [MiicField(MiicStorageName = "MIIC_SOCIAL_CODE", IsNotNull = true, MiicDbType = DbType.String, Description = "用户名")]
        public string SocialCode { get; set; }
        [MiicField(MiicStorageName = "PASSWORD", IsNotNull = true, MiicDbType = DbType.String, Description = "密码")]
        public string Password { get; set; }
        [MiicField(MiicStorageName = "MD5_PASSWORD", IsNotNull = true, MiicDbType = DbType.String, Description = "MD5密钥")]
        public string MD5Password { get; set; }
        [MiicField(MiicStorageName = "SM3_PASSWORD", IsNotNull = true, MiicDbType = DbType.String, Description = "SM3密钥")]
        public string SM3Password { get; set; }
        [MiicField(MiicStorageName = "MOBILE", MiicDbType = DbType.String, Description = "手机")]
        public string Mobile { get; set; }
        [MiicField(MiicStorageName = "MOBILE_BIND", IsNotNull = true, MiicDbType = DbType.String, Description = "手机是否绑定")]
        public string MobileBind { get; set; }
        [MiicField(MiicStorageName = "EMAIL", MiicDbType = DbType.String, Description = "Email")]
        public string Email { get; set; }
        [MiicField(MiicStorageName = "EMAIL_BIND", IsNotNull = true, MiicDbType = DbType.String, Description = "Email是否绑定")]
        public string EmailBind { get; set; }
        [MiicField(MiicStorageName = "USER_TYPE", IsNotNull = true, MiicDbType = DbType.String, Description = "用户类型")]
        public string UserType { get; set; }
        [MiicField(MiicStorageName = "MICRO_USER_URL", IsNotNull = true, MiicDbType = DbType.String, Description = "用户头像")]
        public string MicroUserUrl { get; set; }
        [MiicField(MiicStorageName = "MICRO_THEME_ID", IsNotNull = true, MiicDbType = DbType.String, Description = "微博主题样式ID")]
        public string MicroThemeID { get; set; }
        [MiicField(MiicStorageName = "ACTIVATE_FLAG", IsNotNull = true, MiicDbType = DbType.String, Description = "用户激活状态")]
        public string ActivateFlag { get; set; }
        [MiicField(MiicStorageName = "DISABLED_FLAG", IsNotNull = true, MiicDbType = DbType.String, Description = "用户激活状态")]
        public string DisabledFlag { get; set; }
        [MiicField(MiicStorageName = "CREATER_ID", MiicDbType = DbType.String, Description = "创建人ID")]
        public string CreaterID { get; set; }
        [MiicField(MiicStorageName = "CREATER_NAME", MiicDbType = DbType.String, Description = "创建人姓名")]
        public string CreaterName { get; set; }
        [MiicField(MiicStorageName = "CREATE_TIME", IsNotNull = true, MiicDbType = DbType.DateTime, Description = "创建时间")]
        public DateTime? CreateTime { get; set; }
        [MiicField(MiicStorageName = "LOGIN_NUM", IsNotNull = true, MiicDbType = DbType.Int64, Description = "登录次数")]
        public long? LoginNum { get; set; }
        [MiicField(MiicStorageName = "UPDATER_ID", MiicDbType = DbType.String, Description = "最后修改人ID")]
        public string UpdaterID { get; set; }
        [MiicField(MiicStorageName = "UPDATER_NAME", MiicDbType = DbType.String, Description = "最后修改人姓名")]
        public string UpdaterName { get; set; }
        [MiicField(MiicStorageName = "UPDATE_TIME", MiicDbType = DbType.DateTime, Description = "最后修改时间")]
        public DateTime? UpdateTime { get; set; }
        [MiicField(MiicStorageName = "ACTIVATE_TIME", MiicDbType = DbType.DateTime, Description = "激活时间")]
        public DateTime? ActivateTime { get; set; }
        [MiicField(MiicStorageName = "DISABLED_TIME", MiicDbType = DbType.DateTime, Description = "禁用时间")]
        public DateTime? DisabledTime { get; set; }
        [MiicField(MiicStorageName = "DISABLED_REASON", MiicDbType = DbType.String, Description = "禁用原因")]
        public string DisabledReason { get; set; }
        [MiicField(MiicStorageName = "REMARK", MiicDbType = DbType.String, Description = "简介")]
        public string Remark { get; set; }
        [MiicField(MiicStorageName = "VALID", IsNotNull = true, MiicDbType = DbType.String, Description = "有效性")]
        public string Valid { get; set; }
        [MiicField(MiicStorageName = "CAN_SEARCH", IsNotNull = true, MiicDbType = DbType.String, Description = "是否能被搜索")]
        public string CanSearch { get; set; }
        [MiicField(MiicStorageName = "USER_LEVEL", IsNotNull = true, MiicDbType = DbType.String, Description = "用户等级")]
        public string Level { get; set; }
        [MiicField(MiicStorageName = "SORT_NO", IsIdentification = true, IsNotNull = true, MiicDbType = DbType.Int32, Description = "排序")]
        public int? SortNo { get; set; }
    }
}
