using Miic.Base;
using System.Collections.Generic;
using System.Data;

namespace Miic.Manage.User
{
    public interface IUserInfo:ICommon<UserInfo>
    {
        /// <summary>
        /// 根据用户唯一标识集合获取用户基本信息集合
        /// </summary>
        /// <param name="ids">用户ID集合</param>
        /// <returns>用户基本信息集合</returns>
        List<SimplePersonUserView> GetSimplePersonUserInfos(List<string> ids);
        /// <summary>
        /// 获取全部用户信息集合
        /// </summary>
        /// <returns>全部用户信息集合</returns>
        DataTable GetAllUserInfos();
        /// <summary>
        /// 获取全部用户信息个数
        /// </summary>
        /// <returns>全部用户信息个数</returns>
        int GetAllUserInfosCount();
    }
}
