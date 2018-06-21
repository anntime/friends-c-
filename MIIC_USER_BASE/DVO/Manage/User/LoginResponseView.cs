using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miic.Manage.User
{
    public class LoginResponseView
    {
        /// <summary>
        /// 检测用户名
        /// </summary>
        public bool CheckUserCode { get; set; }
        /// <summary>
        /// 检测密码
        /// </summary>
        public bool CheckPassword { get; set; }
        /// <summary>
        /// 检测用户是否失效
        /// </summary>
        public bool CheckValid { get; set; }
        /// <summary>
        /// 管理系统登录用户是否为管理员
        /// </summary>
        public bool CheckAdmin { get; set; }
        /// <summary>
        /// 是否登录成功
        /// </summary>
        public bool IsLogin { get; set; }
        public LoginResponseView() { }
    }
}
