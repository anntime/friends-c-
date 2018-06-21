using Miic.Base;
using Miic.DB.SqlObject;
using System.Data;

namespace Miic.Manage.Org
{
    public interface IOrgCommentInfo : ICommon<OrgCommentInfo>
    {
        /// <summary>
        /// 获得企业留言列表
        /// </summary>
        /// <param name="orgID">企业ID</param>
        /// <param name="page">分页（默认：不分页）</param>
        /// <returns>留言列表信息</returns>
        DataTable GetOrgCommentInfos(string orgID, MiicPage page);
        /// <summary>
        /// 根据机构ID获取企业留言信息数
        /// </summary>
        /// <param name="orgID">机构ID</param>
        /// <returns>留言信息数</returns>
        int GetOrgCommentCount(string orgID);
    }
}
