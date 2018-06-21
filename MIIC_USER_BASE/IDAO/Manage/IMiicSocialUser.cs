using Miic.Base;
using Miic.Common;
using Miic.DB.SqlObject;
using Miic.Manage.User.Setting;
using System.Collections.Generic;
using System.Data;

namespace Miic.Manage.User
{
    public interface IMiicSocialUser:ICommon<MiicSocialUserInfo>
    {
        /// <summary>
        /// 找回密码
        /// </summary>
        /// <param name="psView">密码视图</param>
        /// <returns>Yes/No</returns>
        bool FindPassword(FindPasswordView psView);
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="loginView">登录视图</param>
        /// <param name="userLoginType">用户登录类别</param>
        /// <returns>登录响应视图</returns>
        LoginResponseView Login(LoginRequestView loginView,UserLoginTypeSetting userLoginType=UserLoginTypeSetting.Other);
        /// <summary>
        /// 设置激活
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>Yes/No</returns>
        bool SetActivateAgree(string id);
        /// <summary>
        /// 设置拒绝激活
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>Yes/No</returns>
        bool SetActivateRefuse(string id);
        /// <summary>
        /// 设置是否禁用
        /// </summary>
        /// <param name="userDisabledView">用户禁用视图</param>
        /// <returns>Yes/No</returns>
        bool SetDisabled(UserDisabledView userDisabledView);
        /// <summary>
        /// 设置用户有效性
        /// </summary>
        /// <param name="validView">有效性视图</param>
        /// <returns>Yes/No</returns>
        bool SetValid(ValidView validView);
        /// <summary>
        /// 是否激活
        /// </summary>
        /// <param name="ID">ID</param>
        /// <returns>YesNo</returns>
        bool IsActivate(string ID);
        /// <summary>
        /// 是否有效
        /// </summary>
        /// <param name="ID">ID</param>
        /// <returns>YesNo</returns>
        bool IsValid(string ID);
        /// <summary>
        /// 用户名唯一性确认
        /// </summary>
        /// <param name="socialCode"></param>
        /// <returns>true 无记录可插入 false 有记录不可插入</returns>
        bool UniqueSocialCode(string socialCode);
        /// <summary>
        /// 邮箱唯一性确认
        /// </summary>
        /// <param name="email"></param>
        /// <returns>true 无记录可插入 false 有记录不可插入</returns>
        bool UniqueEmail(string email);
        /// <summary>
        /// 手机唯一性确认
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns>true 无记录可插入 false 有记录不可插入</returns>
        bool UniqueMobile(string mobile);
        /// <summary>
        /// 获取用户数统计信息
        /// </summary>
        /// <returns>用户数统计信息</returns>
        DataTable GetUserCountStatistics();
        /// <summary>
        /// 搜索用户(用于后台管理系统)
        /// </summary>
        /// <param name="userSearchView">用户搜索视图</param>
        /// <param name="page">分页，默认不分页</param>
        /// <returns>用户搜索集合</returns>
        DataTable Search(UserSearchView userSearchView,MiicPage page=null);
        /// <summary>
        /// 用户搜索集合数（用于后台管理系统）
        /// </summary>
        /// <param name="userSearchView">用户搜索视图</param>
        /// <returns>用户搜索集合数</returns>
        int GetSearchCount(UserSearchView userSearchView);
        /// <summary>
        /// 搜索用户(用于后台管理系统)
        /// </summary>
        /// <param name="userSearchView">用户搜索视图</param>
        /// <param name="page">分页，默认不分页</param>
        /// <returns>用户搜索集合</returns>
        DataTable ManageSearch(UserSearchView userSearchView, MiicPage page = null);
        /// <summary>
        /// 用户搜索集合数（用于后台管理系统）
        /// </summary>
        /// <param name="userSearchView">用户搜索视图</param>
        /// <returns>用户搜索集合数</returns>
        int GetManageSearchCount(UserSearchView userSearchView);
        /// <summary>
        /// 获取用户展示提示信息（微博）
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="userType">用户类别</param>
        /// <returns>用户信息</returns>
        DataTable GetUserMicroblogTip(string userID, UserTypeSetting userType);
        /// <summary>
        /// 获取用户展示提示信息（朋友圈）
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="userType">用户类别</param>
        /// <returns>用户信息</returns>
        DataTable GetUserFriendTip(string userID, UserTypeSetting userType);
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="ids">ids</param>
        /// <returns>Yes/No</returns>
        bool ResetPassword(List<string> ids);
       /// <summary>
        /// 删除未激活的用户
        /// </summary>
        /// <param name="userTypeViews">用户类别视图集合</param>
        /// <returns>Yes/No</returns>
        bool Delete(List<UserTypeView> userTypeViews);
        /// <summary>
        /// 获取最新注册用户
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        DataTable GetNewestUserList(int top = 10);
        /// <summary>
        /// 批量设置用户等级
        /// </summary>
        /// <param name="userLevelViews">设置人员级别视图列表</param>
        /// <param name="level">设置等级</param>
        /// <returns>Yes/No</returns>
        bool SetUsersLevel(List<UserLevelView> userLevelViews, UserLevelSetting level);
        /// <summary>
        /// 下载用户列表
        /// </summary>
        /// <param name="downloadSearchView"></param>
        /// <returns></returns>
        DataTable DownloadSearch(DownloadSearchView downloadSearchView);
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userID">socialCode</param>
        /// <returns></returns>
        DataTable GetInformationBySocialCode(string socialCode);
    }
}
