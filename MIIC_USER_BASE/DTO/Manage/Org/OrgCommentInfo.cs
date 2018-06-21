using Miic.Attribute;
using System;
using System.Data;

namespace Miic.Manage.Org
{
    [MiicTable(MiicParentStorageName = "MIIC_SOCIAL_COMMON", MiicStorageName = "ORG_COMMENT_INFO", Description = "微博企业账号留言记录表")]
    public partial class OrgCommentInfo
    {
        [MiicField(MiicStorageName = "ID", IsNotNull = true, IsPrimaryKey = true, MiicDbType = DbType.String, Description = "唯一码")]
        public string ID { get; set; }
        [MiicField(MiicStorageName = "COMMENT_CONTENT", IsNotNull = true, MiicDbType = DbType.String, Description = "留言")]
        public string Content { get; set; }
        [MiicField(MiicStorageName = "ORG_ID", IsNotNull = true, MiicDbType = DbType.String, Description = "企业账号ID")]
        public string OrgID { get; set; }
        [MiicField(MiicStorageName = "FROM_COMMENTER_ID", IsNotNull = true, MiicDbType = DbType.String, Description = "评论人ID")]
        public string FromCommenterID { get; set; }
        [MiicField(MiicStorageName = "FROM_COMMENTER_NAME", IsNotNull = true, MiicDbType = DbType.String, Description = "评论人名称")]
        public string FromCommenterName { get; set; }
        [MiicField(MiicStorageName = "TO_COMMENTER_ID", IsNotNull = true, MiicDbType = DbType.String, Description = "指向某人ID")]
        public string ToCommenterID { get; set; }
        [MiicField(MiicStorageName = "TO_COMMENTER_NAME", IsNotNull = true, MiicDbType = DbType.String, Description = "指向某人名称")]
        public string ToCommenterName { get; set; }
        [MiicField(MiicStorageName = "COMMENT_TIME", IsNotNull = true, MiicDbType = DbType.DateTime, Description = "评论时间")]
        public DateTime? CommentTime { get; set; }
        [MiicField(MiicStorageName = "SORT_NO", IsNotNull = true, IsIdentification = true, MiicDbType = DbType.Int32, Description = "排序")]
        public int? SortNo { get; set; }
    }
}
