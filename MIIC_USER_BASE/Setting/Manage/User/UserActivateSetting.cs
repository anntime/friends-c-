using Miic.Base.Setting;
using System.ComponentModel;

namespace Miic.Manage.User.Setting
{
    public enum UserActivateSetting
    {
        /// <summary>
        /// 激活
        /// </summary>
        [Description("激活")]
        Agree = MiicSimpleApproveStatusSetting.Agree,
        /// <summary>
        /// 拒绝激活
        /// </summary>
        [Description("拒绝激活")]
        Refuse = MiicSimpleApproveStatusSetting.Refuse,
        /// <summary>
        /// 待激活
        /// </summary>
        [Description("待激活")]
        Waiting = MiicSimpleApproveStatusSetting.Waiting,
        /// <summary>
        /// 未知状态
        /// </summary>
        Unknown = 3
    }
}
