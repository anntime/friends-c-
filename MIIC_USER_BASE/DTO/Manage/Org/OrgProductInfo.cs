using Miic.Attribute;
using System;
using System.Data;

namespace Miic.Manage.Org
{
    [MiicTable(MiicParentStorageName = "MIIC_SOCIAL_COMMON", MiicStorageName = "ORG_PRODUCT_INFO", Description = "微博企业账号长篇展示附件")]
    public class OrgProductInfo
    {
        [MiicField(MiicStorageName = "ID", IsNotNull = true, IsPrimaryKey = true, MiicDbType = DbType.String, Description = "唯一码")]
        public string ID { get; set; }
        [MiicField(MiicStorageName = "ORG_ID", IsNotNull = true, MiicDbType = DbType.String, Description = "企业用户ID")]
        public string OrgID { get; set; }
        [MiicField(MiicStorageName = "FILE_NAME", IsNotNull = true, MiicDbType = DbType.String, Description = "文件原名称")]
        public string FileName { get; set; }
        [MiicField(MiicStorageName = "FILE_PATH", IsNotNull = true, MiicDbType = DbType.String, Description = "文件路径")]
        public string FilePath { get; set; }
        [MiicField(MiicStorageName = "UPLOAD_TIME", IsNotNull = true, MiicDbType = DbType.DateTime, Description = "上传时间")]
        public DateTime? UploadTime { get; set; }
       
    }
}
