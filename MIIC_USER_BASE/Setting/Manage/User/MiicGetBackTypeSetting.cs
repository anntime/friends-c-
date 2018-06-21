using System.ComponentModel;

namespace Miic.Manage.User.Setting
{
    public enum MiicGetBackTypeSetting
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知类型")]
        Unknow = 0,
        /// <summary>
        /// 邮箱查找
        /// </summary>
        [Description("邮箱查找")]
        Email = 1,
        /// <summary>
        /// 手机查找
        /// </summary>
        [Description("手机查找")]
        Mobile = 2
    }
}
