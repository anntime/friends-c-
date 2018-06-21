using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miic.Common
{
    public class BoolWithPrimaryKeyView
    {
        /// <summary>
        /// 返回值
        /// </summary>
        public bool result { get; set; }
        /// <summary>
        /// 主键ID
        /// </summary>
        public string PrimaryID { get; set; }
    }
}
