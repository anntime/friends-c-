using Miic.Manage.User.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miic.Manage.User
{
    public partial class UserTypeView
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 用户类别
        /// </summary>
        public UserTypeSetting UserType { get; set; }
    }
}
