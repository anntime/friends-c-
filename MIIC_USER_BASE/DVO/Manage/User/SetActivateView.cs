using Miic.Manage.User.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miic.Manage.User
{
    public class SetActivateView
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// 激活状态
        /// </summary>
        public UserActivateSetting Activate { get; set; }
        public SetActivateView()
        {

        }
    }
}
