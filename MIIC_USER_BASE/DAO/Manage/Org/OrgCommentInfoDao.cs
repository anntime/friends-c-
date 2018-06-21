using Miic.Base;
using Miic.DB.Setting;
using Miic.DB.SqlObject;
using Miic.DB.SqlStruct;
using Miic.Log;
using Miic.Manage.User;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace Miic.Manage.Org
{
    public partial class OrgCommentInfoDao : NoRelationCommon<OrgCommentInfo>, IOrgCommentInfo
    {
        private static readonly string ClassName = MethodBase.GetCurrentMethod().DeclaringType.Name;
        private static readonly string NamespaceName = MethodBase.GetCurrentMethod().DeclaringType.Namespace;
        public OrgCommentInfoDao() { }

        bool ICommon<OrgCommentInfo>.Insert(OrgCommentInfo orgCommentInfo)
        {
            Contract.Requires<ArgumentNullException>(orgCommentInfo != null, "参数microOrgMessageInfo：不能为空！");
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(orgCommentInfo.ID), "参数microOrgMessageInfo.ID：不能为空！");
            bool result = false;
            int count = 0;
            string message = string.Empty;
            try
            {
                result = dbService.Insert(orgCommentInfo, out count, out message);
            }
            catch (Exception ex)
            {
                Config.IlogicLogService.Write(new LogicLog()
                {
                    AppName = Config.AppName,
                    ClassName = ClassName,
                    NamespaceName = NamespaceName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    Message = ex.Message,
                    Oper = Config.Oper
                });
            }
            if (result == true)
            {
                InsertCache(orgCommentInfo);
            }
            return result;
        }

        bool ICommon<OrgCommentInfo>.Update(OrgCommentInfo orgCommentInfo)
        {
            Contract.Requires<ArgumentNullException>(orgCommentInfo != null, "参数microOrgMessageInfo:不能为空！");
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(orgCommentInfo.ID), "参数microOrgMessageInfo.ID:不能为空，因为是主键");
            int count = 0;
            string message = string.Empty;
            bool result = false;
            try
            {
                result = dbService.Update(orgCommentInfo, out count, out message);
            }
            catch (Exception ex)
            {
                Config.IlogicLogService.Write(new LogicLog()
                {
                    AppName = Config.AppName,
                    ClassName = ClassName,
                    NamespaceName = NamespaceName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    Message = ex.Message,
                    Oper = Config.Oper
                });
            }
            if (result == true)
            {
                DeleteCache(o => o.ID == orgCommentInfo.ID);
            }
            return result;
        }

        bool ICommon<OrgCommentInfo>.Delete(string id)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(id), "参数id:不能为空");
            bool result = false;
            int count = 0;
            string message = string.Empty;
            try
            {
                result = dbService.Delete<OrgCommentInfo>(new OrgCommentInfo()
                {
                    ID = id
                }, out count, out message);
            }
            catch (Exception ex)
            {
                Config.IlogicLogService.Write(new LogicLog()
                {
                    AppName = Config.AppName,
                    ClassName = ClassName,
                    NamespaceName = NamespaceName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    Message = ex.Message,
                    Oper = Config.Oper
                });
            }
            if (result == true)
            {
                DeleteCache(o => o.ID == id);
            }
            return result;
        }

        OrgCommentInfo ICommon<OrgCommentInfo>.GetInformation(string id)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(id), "参数id:不能为空");
            OrgCommentInfo result = null;
            string message = string.Empty;
            try
            {
                result = items.Find(o => o.ID == id);
                if (result == null)
                {
                    result = dbService.GetInformation(new OrgCommentInfo
                    {
                        ID = id
                    }, out message);
                    if (result != null)
                    {
                        InsertCache(result);
                    }
                }
                else
                {
                    string serializer = Config.Serializer.Serialize(result);
                    result =Config.Attribute.ConvertObjectWithDateTime(Config.Serializer.Deserialize<OrgCommentInfo>(serializer));
                }
            }
            catch (Exception ex)
            {
                Config.IlogicLogService.Write(new LogicLog()
                {
                    AppName = Config.AppName,
                    ClassName = ClassName,
                    NamespaceName = NamespaceName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    Message = ex.Message,
                    Oper = Config.Oper
                });
            }
            return result;
        }
        /// <summary>
        /// 获得企业留言列表
        /// </summary>
        /// <param name="orgID">企业ID</param>
        /// <param name="page">分页</param>
        /// <returns>留言列表信息</returns>
        public DataTable GetOrgCommentInfos(string orgID, MiicPage page)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(orgID), "参数orgID:不能为空");
            DataTable result = new DataTable();
            string message = string.Empty;
            MiicRelationCollections relation = new MiicRelationCollections(Config.Attribute.GetSqlTableNameByClassName<OrgCommentInfo>());
            MiicFriendRelation fromRelation = new MiicFriendRelation(Config.Attribute.GetSqlColumnNameByPropertyName<OrgCommentInfo, string>(o => o.FromCommenterID),
                  new MiicTableName()
                  {
                      TableAliasName = "FROM_USER",
                      TableName = Config.Attribute.GetSqlTableNameByClassName<MiicSocialUserInfo>()
                  },
                Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.ID),
                MiicDBOperatorSetting.Equal,
                MiicDBRelationSetting.LeftJoin);
            relation.Add(fromRelation);
            MiicFriendRelation toRelation = new MiicFriendRelation(Config.Attribute.GetSqlColumnNameByPropertyName<OrgCommentInfo, string>(o => o.ToCommenterID),
                new MiicTableName()
                {
                    TableAliasName = "TO_USER",
                    TableName = Config.Attribute.GetSqlTableNameByClassName<MiicSocialUserInfo>()
                },
                Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.ID),
                MiicDBOperatorSetting.Equal,
                MiicDBRelationSetting.LeftJoin);

            relation.Add(toRelation);
            MiicCondition orgIDCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<OrgCommentInfo, string>(o => o.OrgID),
                orgID,
                DbType.String,
                MiicDBOperatorSetting.Equal);
            MiicConditionCollections conditions = new MiicConditionCollections();
            conditions.Add(new MiicConditionLeaf(MiicDBLogicSetting.No, orgIDCondition));
            List<MiicOrderBy> order = new List<MiicOrderBy>();
            MiicOrderBy timeOrder = new MiicOrderBy()
            {
                Desc = true,
                PropertyName = Config.Attribute.GetSqlColumnNameByPropertyName<OrgCommentInfo, DateTime?>(o => o.CommentTime)
            };
            order.Add(timeOrder);
            conditions.order = order;
            MiicColumn allColumn = new MiicColumn(Config.Attribute.GetSqlTableNameByClassName<OrgCommentInfo>());
            MiicColumnCollections column = new MiicColumnCollections();
            column.Add(allColumn);
            MiicColumn fromUserUrlColumn = new MiicColumn(Config.Attribute.GetSqlTableNameByClassName<MiicSocialUserInfo>(),
                "FROM_USER",
                Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.MicroUserUrl),
                "FROM_USER_URL");
            column.Add(fromUserUrlColumn);
            MiicColumn fromUserTypeColumn = new MiicColumn(Config.Attribute.GetSqlTableNameByClassName<MiicSocialUserInfo>(),
                "FROM_USER",
                Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.UserType),
                "FROM_USER_TYPE");
            column.Add(fromUserTypeColumn);
            MiicColumn toUserUrlColumn = new MiicColumn(Config.Attribute.GetSqlTableNameByClassName<MiicSocialUserInfo>(),
              "TO_USER",
              Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.MicroUserUrl),
              "TO_USER_URL");
            column.Add(toUserUrlColumn);
            MiicColumn toUserTypeColumn = new MiicColumn(Config.Attribute.GetSqlTableNameByClassName<MiicSocialUserInfo>(),
                "TO_USER",
                Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.UserType),
                "TO_USER_TYPE");
            column.Add(toUserTypeColumn);
            try
            {
                if (page == null)
                {
                    result = dbService.GetInformations(column, relation, conditions, out message);
                }
                else
                {
                    result = dbService.GetInformationsPage(column, relation, conditions, page, out message);
                }
            }
            catch (Exception ex)
            {
                Config.IlogicLogService.Write(new LogicLog()
                {
                    AppName = Config.AppName,
                    ClassName = ClassName,
                    NamespaceName = NamespaceName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    Message = ex.Message,
                    Oper = Config.Oper
                });
            }
            return result;
        }
        /// <summary>
        /// 根据机构ID获取企业留言信息数
        /// </summary>
        /// <param name="orgID">机构ID</param>
        /// <returns>留言信息数</returns>
        public int GetOrgCommentCount(string orgID)
        {
            int result = 0;
            string message = string.Empty;
            MiicCondition orgIDCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<OrgCommentInfo, string>(o => o.OrgID),
                orgID,
                DbType.String,
                MiicDBOperatorSetting.Equal);
            MiicColumn idColumn = new MiicColumn(Config.Attribute.GetSqlTableNameByClassName<OrgCommentInfo>(),
                Config.Attribute.GetSqlColumnNameByPropertyName<OrgCommentInfo, string>(o => o.ID));
            try
            {
                result = dbService.GetCount<OrgCommentInfo>(idColumn, new MiicConditionSingle(orgIDCondition), out message);
            }
            catch (Exception ex)
            {
                Config.IlogicLogService.Write(new LogicLog()
                {
                    AppName = Config.AppName,
                    ClassName = ClassName,
                    NamespaceName = NamespaceName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    Message = ex.Message,
                    Oper = Config.Oper
                });
            }
            return result;
        }
    }
}
