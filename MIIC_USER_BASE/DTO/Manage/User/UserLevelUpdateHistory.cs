using Miic.Attribute;
using System;
using System.Data;

namespace Miic.Manage.User
{
    [MiicTable(MiicParentStorageName = "MIIC_SOCIAL_COMMON", MiicStorageName = "USER_LEVEL_UPDATE_HISTORY", Description = "用户级别修改历史信息表")]
    public class UserLevelUpdateHistory
    {
        [MiicField(MiicStorageName = "ID", IsNotNull = true, IsPrimaryKey = true, MiicDbType = DbType.String, Description = "唯一码")]
        public string ID { get; set; }
        [MiicField(MiicStorageName = "EDITER_ID", IsNotNull = true, MiicDbType = DbType.String, Description = "用户级别编辑人员ID")]
        public string EditerID { get; set; }
        [MiicField(MiicStorageName = "EDITER_NAME", IsNotNull = true, MiicDbType = DbType.String, Description = "用户级别编辑人员名称")]
        public string EditerName { get; set; }
        [MiicField(MiicStorageName = "EDITER_IP", IsNotNull = true, MiicDbType = DbType.String, Description = "用户级别编辑人员IP")]
        public string EditerIP { get; set; }
        [MiicField(MiicStorageName = "EDIT_TIME", IsNotNull = true, MiicDbType = DbType.DateTime, Description = "编辑时间")]
        public DateTime? EditTime { get; set; }
        [MiicField(MiicStorageName = "UPDATED_USER_ID", IsNotNull = true, MiicDbType = DbType.String, Description = "被编辑人员ID")]
        public string UpdatedUserID { get; set; }
        [MiicField(MiicStorageName = "UPDATED_USER_NAME", IsNotNull = true, MiicDbType = DbType.String, Description = "被编辑人员名称")]
        public string UpdatedUserName { get; set; }
        [MiicField(MiicStorageName = "ORIGINAL_LEVEL", IsNotNull = true, MiicDbType = DbType.String, Description = "初始等级")]
        public string OriginalLevel { get; set; }
        [MiicField(MiicStorageName = "NOW_LEVEL", IsNotNull = true, MiicDbType = DbType.String, Description = "修改后等级")]
        public string NowLevel { get; set; }
        [MiicField(MiicStorageName = "SORT_NO", IsNotNull = true, IsIdentification = true, MiicDbType = DbType.Int32, Description = "排序")]
        public int? SortNo { get; set; }
    }
}
