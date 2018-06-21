using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Miic.Manage.Org
{
    public class ToCommentView:CommentView
    {
        /// <summary>
        /// 对方ID
        /// </summary>
        public string ToCommenterID { get; set; }
        /// <summary>
        /// 对方名称
        /// </summary>
        public string ToCommenterName { get; set; }
    }
}
