using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Miic.Manage.Org
{
    public abstract  class CommentView
    {
        /// <summary>
        /// 企业ID
        /// </summary>
        public string OrgID { get; set; }
        /// <summary>
        /// 评论内容
        /// </summary>
        public string Content { get; set; }
        public CommentView() { }
      
    }
}
