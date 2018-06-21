using System.ComponentModel;

namespace Miic.Manage.User.Setting
{
    public enum UserTypeSetting
    {
        /// <summary>
        /// 超级管理员
        /// </summary>
        [Description("平台管理员")]
        Administrator = 0,
        /// <summary>
        /// 国家管理员
        /// </summary>
        [Description("国家管理员")]
        CountryAdmin = 11,
        /// <summary>
        /// 地方管理员
        /// </summary>
        [Description("地方管理员")]
        LocalAdmin = 12,
        /// <summary>
        /// 国家主管部门
        /// </summary>
        [Description("国家主管部门")]
        CountryDepartUser = 21,
        /// <summary>
        /// 地方主管部门
        /// </summary>
        [Description("地方主管部门")]
        LocalDepartUser = 22,
        /// <summary>
        /// 企业用户
        /// </summary>
        [Description("企业用户")]
        OrgUser = 31,
        /// <summary>
        /// 个人用户
        /// </summary>
        [Description("个人用户")]
        PersonUser = 41,
        /// <summary>
        /// 匿名用户
        /// </summary>
       [Description("匿名用户")]
        AnonymousUser=51 ,
        /// <summary>
        /// 所有管理员
        /// </summary>
        [Description("所有管理员")]
        AllAdminPerson=98,
        /// <summary>
        /// 所有管理部门
        /// </summary>
        [Description("所有管部门")]
        AllAdminDeparter=97,
        /// <summary>
        /// 所有
        /// </summary>
       [Description("所有类别用户")]
        All=99
    }
}
