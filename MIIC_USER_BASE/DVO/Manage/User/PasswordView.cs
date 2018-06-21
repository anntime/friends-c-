
namespace Miic.Manage.User
{
    public class PasswordView
    {
        /// <summary>
        /// 旧密码
        /// </summary>
        public string OldPassword { get; set; }
        /// <summary>
        /// 新密码
        /// </summary>
        public string NewPassword { get; set; }
        /// <summary>
        /// MD5加密密码
        /// </summary>
        public string Md5 { get; set; }
        /// <summary>
        /// sm3加密密码
        /// </summary>
        public string Sm3 { get; set; }
    }
}
