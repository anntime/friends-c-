using Miic.Attribute;
using System.Data;
namespace Miic.Manage.User
{
    [MiicTable(MiicParentStorageName = "MIIC_SOCIAL_COMMON", MiicStorageName = "USER_INFO", Description = "用户信息表")]
    public partial class UserInfo
    {
        public UserInfo() { }
        [MiicField(MiicStorageName = "USER_ID", IsPrimaryKey = true, IsNotNull = true, MiicDbType = DbType.String, Description = "唯一码")]
        public string UserID { get; set; }
        [MiicField(MiicStorageName = "USER_NAME", IsNotNull = true, MiicDbType = DbType.String, Description = "昵称")]
        public string UserName { get; set; }
        [MiicField(MiicStorageName = "REAL_NAME", IsNotNull = true, MiicDbType = DbType.String, Description = "姓名")]
        public string RealName { get; set; }
        [MiicField(MiicStorageName = "SEX", IsNotNull = true, MiicDbType = DbType.String, Description = "性别")]
        public string Sex { get; set; }
        [MiicField(MiicStorageName = "NATION", IsNotNull = true, MiicDbType = DbType.String, Description = "民族")]
        public string Nation { get; set; }
        [MiicField(MiicStorageName = "QQ", MiicDbType = DbType.String, Description = "QQ")]
        public string qq { get; set; }
        [MiicField(MiicStorageName = "CAN_SEE_QQ", IsNotNull = true, MiicDbType = DbType.String, Description = "QQ是否可见")]
        public string CanSeeQQ { get; set; }
        [MiicField(MiicStorageName = "TEL", MiicDbType = DbType.String, Description = "联系电话")]
        public string Tel { get; set; }
        [MiicField(MiicStorageName = "CAN_SEE_TEL", IsNotNull = true, MiicDbType = DbType.String, Description = "联系电话是否可见")]
        public string CanSeeTel { get; set; }
        [MiicField(MiicStorageName = "FAX", MiicDbType = DbType.String, Description = "传真")]
        public string Fax { get; set; }
        [MiicField(MiicStorageName = "MOBILE", MiicDbType = DbType.String, Description = "移动电话")]
        public string Mobile { get; set; }
        [MiicField(MiicStorageName = "CAN_SEE_MOBILE", IsNotNull = true, MiicDbType = DbType.String, Description = "移动电话是否可见")]
        public string CanSeeMobile { get; set; }
        [MiicField(MiicStorageName = "EMAIL", IsNotNull = true, MiicDbType = DbType.String, Description = "Email")]
        public string Email { get; set; }
        [MiicField(MiicStorageName = "ORG_NAME", IsNotNull = true, MiicDbType = DbType.String, Description = "单位名称")]
        public string OrgName { get; set; }
        [MiicField(MiicStorageName = "MAJOR_DUTY", MiicDbType = DbType.String, Description = "主要职责")]
        public string MajorDuty { get; set; }
        [MiicField(MiicStorageName = "UNIVERSITY", MiicDbType = DbType.String, Description = "毕业院校")]
        public string University { get; set; }
        [MiicField(MiicStorageName = "MOTTO", MiicDbType = DbType.String, Description = "座右铭")]
        public string Motto { get; set; }
        [MiicField(MiicStorageName = "FRIENDS_THEME_ID", IsNotNull = true, MiicDbType = DbType.String, Description = "朋友圈主题样式ID")]
        public string FriendsThemeID { get; set; }
        [MiicField(MiicStorageName = "TOTAL", IsNotNull = true, MiicDbType = DbType.Int64, Description = "总积分")]
        public long Total { get; set; }
        [MiicField(MiicStorageName = "REMARK", MiicDbType = DbType.String, Description = "唯一码")]
        public string Remark { get; set; }
    }
}
