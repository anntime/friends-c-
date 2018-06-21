using Miic.Base;
using System.Collections.Generic;
using System.Data;

namespace Miic.Manage.Org
{
    public interface IOrganizationInfo:ICommon<OrganizationInfo>
    {
        /// <summary>
        /// 新增企业信息
        /// </summary>
        /// <param name="organizationInfo">企业信息</param>
        /// <param name="orgProductInfos">企业产品附件</param>
        /// <returns>Yes/No</returns>
        bool Insert(OrganizationInfo organizationInfo, List<OrgProductInfo> orgProductInfos);
        /// <summary>
        /// 获取企业详细信息
        /// </summary>
        /// <param name="orgID">企业ID</param>
        /// <returns>企业详细信息</returns>
        DataTable GetOrganizationDetailInfo(string orgID);
        /// <summary>
        /// 根据企业ID查询企业产品展示附件列表
        /// </summary>
        /// <param name="orgID">企业ID</param>
        /// <returns>产品展示附件列表</returns>
        List<OrgProductInfo> GetOrgProductList(string orgID);
        /// <summary>
        /// 更新企业信息
        /// </summary>
        /// <param name="organizationInfo">企业</param>
        /// <param name="removeSimpleOrgProductViews">删除企业产品附件</param>
        /// <returns></returns>
        bool Update(OrganizationInfo organizationInfo, List<SimpleOrgProductView> removeSimpleOrgProductViews);
        /// <summary>
        /// 更新企业信息
        /// </summary>
        /// <param name="organizationInfo">企业</param>
        /// <param name="orgProductInfos">企业产品附件</param>
        /// <param name="removeSimpleOrgProductViews">删除企业产品附件</param>
        /// <returns></returns>
        bool Update(OrganizationInfo organizationInfo, List<OrgProductInfo> orgProductInfos, List<SimpleOrgProductView> removeSimpleOrgProductViews = null);
    }
}
