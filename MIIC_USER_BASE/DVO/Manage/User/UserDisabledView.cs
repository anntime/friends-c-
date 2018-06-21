using Miic.Base.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miic.Manage.User
{
    public class UserDisabledView
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// YesNo
        /// </summary>
        public MiicYesNoSetting YesNo { get; set; }
        /// <summary>
        /// 禁用原因
        /// </summary>
        public string Reason { get; set; }
        public UserDisabledView() 
        {
            if (YesNo == MiicYesNoSetting.No) 
            {
                this.Reason = string.Empty;
            }
        }
    }
}
