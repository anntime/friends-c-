using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miic.Common
{
    public  class BoolWithMessageView
    {
        /// <summary>
        /// 返回类型
        /// </summary>
        public bool result { get; set; }
        /// <summary>
        /// 错误说明
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
