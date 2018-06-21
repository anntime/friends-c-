using System;

namespace Miic.Manage.User
{
    public class SimpleUserInfoView
    { 
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string SocialCode { get; set; }
        /// <summary>
        /// 是否可被搜索
        /// </summary>
        public string CanSearch { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 手机是否绑定
        /// </summary>
        public string MobileBind { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Email是否绑定
        /// </summary>
        public string EmailBind { get; set; }
        /// <summary>
        /// 用户类型
        /// </summary>
        public string UserType { get; set; }
        /// <summary>
        /// 用户头像
        /// </summary>
        public string MicroUserUrl { get; set; }
        /// <summary>
        /// 微博主题样式ID
        /// </summary>
        public string MicroThemeID { get; set; }
        /// <summary>
        /// 用户激活状态
        /// </summary>
        public string Flag { get; set; }
        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreaterID { get; set; }
        /// <summary>
        /// 创建人姓名
        /// </summary>
        public string CreaterName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 登录次数
        /// </summary>
        public long? LoginNum { get; set; }
        /// <summary>
        /// 最后修改人ID
        /// </summary>
        public string UpdaterID { get; set; }
        /// <summary>
        /// 最后修改人姓名
        /// </summary>
        public string UpdaterName { get; set; }
        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }
        /// <summary>
        /// 激活时间
        /// </summary>
        public DateTime? ActivateTime { get; set; }
        /// <summary>
        /// 禁用时间
        /// </summary>
        public DateTime? DisabledTime { get; set; }
        /// <summary>
        /// 禁用原因
        /// </summary>
        public string DisabledReason { get; set; }
        /// <summary>
        /// 有效性
        /// </summary>
        public string Valid { get; set; }
        /// <summary>
        /// 个人简介
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 用户等级
        /// </summary>
        public string UserLevel { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int? SortNo { get; set; }
    }
}
