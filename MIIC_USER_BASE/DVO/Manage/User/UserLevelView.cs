using Miic.Manage.User.Setting;

namespace Miic.Manage.User
{
    public class UserLevelView
    {
        /// <summary>
        /// ID
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户级别
        /// </summary>
        public UserLevelSetting UserLevel { get; set; }
    }
}
