using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miic.Manage.User.Setting
{
    /// <summary>
    /// 登录类别枚举
    /// </summary>
    public enum UserLoginTypeSetting
    {
        /// <summary>
        /// 管理系统
        /// </summary>
        Manage=0,
       /// <summary>
       /// 微博系统
       /// </summary>
        Microblog=1,
        /// <summary>
        /// 朋友圈系统
        /// </summary>
        Friends=2,
        /// <summary>
        /// 活动系统
        /// </summary>
        Activity=3,
        /// <summary>
        /// 其他
        /// </summary>
        Other=9
    }
}
