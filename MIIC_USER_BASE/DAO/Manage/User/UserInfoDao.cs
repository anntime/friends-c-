using Miic.Base;
using Miic.Base.Setting;
using Miic.DB.Setting;
using Miic.DB.SqlObject;
using Miic.IM.User;
using Miic.Log;
using Miic.Manage.User.Setting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Web.Configuration;

namespace Miic.Manage.User
{
    public partial class UserInfoDao : NoRelationCommon<UserInfo>, IUserInfo
    {
        private static readonly string ClassName = MethodBase.GetCurrentMethod().DeclaringType.Name;
        private static readonly string NamespaceName = MethodBase.GetCurrentMethod().DeclaringType.Namespace;
        private static readonly string DefaultFriendsThemeID = WebConfigurationManager.AppSettings["DefaultFriendsThemeID"].ToString();
        //private static readonly IOpenIMUserInfo IopenIMUserInfo = new OpenUserInfoService();
        public bool IsCached { get; private set; }
        public UserInfoDao() : this(true) { }
        public UserInfoDao(bool isCached)
        {
            this.IsCached = isCached;
        }

        bool ICommon<UserInfo>.Insert(UserInfo userInfo)
        {
            Contract.Requires<ArgumentNullException>(userInfo != null, "参数userInfo：不能为空！");
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(userInfo.UserID), "参数userInfo.UserID：不能为空！");
            bool result = false;
            int count = 0;
            string message = string.Empty;
            /*------------------------------用户初始化值-----------------------------------*/
            //朋友圈主题样式ID
            userInfo.FriendsThemeID = DefaultFriendsThemeID;
            //总积分
            userInfo.Total = 0;
            /*------------------------------用户初始化值-----------------------------------*/
            try
            {
                result = dbService.Insert(userInfo, out count, out message);
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
                if (this.IsCached == true)
                {
                    InsertCache(userInfo);
                }
                try
                {
                    //IopenIMUserInfo.Insert(new OpenIMUserInfo()
                    //{
                    //    UserID = userInfo.UserID,
                    //    Status = OpenIMUserActivationSetting.UnActivated,
                    //    NickName = userInfo.UserName,
                    //    Sex = (MiicSexSetting)int.Parse(userInfo.Sex),
                    //    RealName = userInfo.RealName,
                    //    UpdateTime = DateTime.Now,
                      
                    //});
                }
                finally { }
            }
            return result;
        }

        bool ICommon<UserInfo>.Update(UserInfo userInfo)
        {
            Contract.Requires<ArgumentNullException>(userInfo != null, "参数userInfo:不能为空！");
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(userInfo.UserID), "参数userInfo.UserID:不能为空，因为是主键");
            int count = 0;
            string message = string.Empty;
            bool result = false;
            try
            {
                result = dbService.Update(userInfo, out count, out message);
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
                if (this.IsCached == true)
                {
                    DeleteCache(o => o.UserID == userInfo.UserID);
                }
                try
                {
                    //IopenIMUserInfo.Update(new OpenIMUserInfo()
                    //{
                    //    UserID = userInfo.UserID,
                    //    NickName = userInfo.UserName,
                    //    RealName = userInfo.RealName,
                    //    UpdateTime = DateTime.Now,

                    //});
                }
                finally { }
            }
            return result;
        }

        bool ICommon<UserInfo>.Delete(string id)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(id), "参数id:不能为空");
            bool result = false;
            int count = 0;
            string message = string.Empty;
            try
            {
                result = dbService.Delete<UserInfo>(new UserInfo()
                {
                    UserID = id
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
                if (IsCached == true)
                {
                    DeleteCache(o => o.UserID == id);
                }
            }
            return result;
        }

        UserInfo ICommon<UserInfo>.GetInformation(string id)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(id), "参数id:不能为空");
            UserInfo result = null;
            string message = string.Empty;
            try
            {
                result = items.Find(o => o.UserID == id);
                if (result == null)
                {
                    result = dbService.GetInformation(new UserInfo
                    {
                        UserID = id
                    }, out message);
                    if (result != null)
                    {
                        if (this.IsCached == true)
                        {
                            InsertCache(result);
                        }
                    }
                }
                else
                {
                    string serializer = Config.Serializer.Serialize(result);
                    result = Config.Serializer.Deserialize<UserInfo>(serializer);
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

        List<SimplePersonUserView> IUserInfo.GetSimplePersonUserInfos(List<string> ids)
        {
            List<SimplePersonUserView> result = new List<SimplePersonUserView>();
            string message = string.Empty;
            MiicConditionCollections conditions = new MiicConditionCollections(MiicDBLogicSetting.No);
            MiicCondition idsCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<SimplePersonUserView, string>(o => o.UserID),
                ids,
                DbType.String,
                MiicDBOperatorSetting.In);
            conditions.Add(new MiicConditionLeaf(MiicDBLogicSetting.No, idsCondition));
            MiicColumnCollections columns = new MiicColumnCollections();
            MiicColumn column = new MiicColumn(Config.Attribute.GetSqlTableNameByClassName<SimplePersonUserView>());
            columns.Add(column);
            try
            {
                DataTable dt = dbService.GetInformations<SimplePersonUserView>(columns, conditions, out message);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.AsEnumerable())
                    {
                        result.Add(new SimplePersonUserView()
                        {
                            UserID = dr[Config.Attribute.GetSqlColumnNameByPropertyName<SimplePersonUserView, string>(o => o.UserID)].ToString(),
                            OrgName = dr[Config.Attribute.GetSqlColumnNameByPropertyName<SimplePersonUserView, string>(o => o.OrgName)].ToString(),
                            SocialCode = dr[Config.Attribute.GetSqlColumnNameByPropertyName<SimplePersonUserView, string>(o => o.SocialCode)].ToString(),
                            UserLevel = dr[Config.Attribute.GetSqlColumnNameByPropertyName<SimplePersonUserView, string>(o => o.UserLevel)].ToString(),
                            UserName = dr[Config.Attribute.GetSqlColumnNameByPropertyName<SimplePersonUserView, string>(o => o.UserName)].ToString(),
                            UserType = dr[Config.Attribute.GetSqlColumnNameByPropertyName<SimplePersonUserView, string>(o => o.UserType)].ToString(),
                            UserUrl = dr[Config.Attribute.GetSqlColumnNameByPropertyName<SimplePersonUserView, string>(o => o.UserUrl)].ToString()
                        });
                    }
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
        /// 获得用户所有信息
        /// </summary> 
        /// <returns></returns>
        DataTable IUserInfo.GetAllUserInfos()
        {
            DataTable result = new DataTable();
            string message = string.Empty;
            MiicRelation relation = new MiicRelation(Config.Attribute.GetSqlTableNameByClassName<UserInfo>(),
                Config.Attribute.GetSqlColumnNameByPropertyName<UserInfo, string>(o => o.UserID),
                Config.Attribute.GetSqlTableNameByClassName<MiicSocialUserInfo>(),
                Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.ID),
                MiicDBOperatorSetting.Equal,
                MiicDBRelationSetting.LeftJoin);
            MiicConditionCollections conditions = new MiicConditionCollections(MiicDBLogicSetting.No); 
            //选择有效地，没有被禁止的，已经被激活的 
            MiicCondition disabledFlagCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.DisabledFlag),
                        ((int)MiicYesNoSetting.No).ToString(),
                        DbType.String,
                        MiicDBOperatorSetting.Equal);
            conditions.Add(new MiicConditionLeaf(MiicDBLogicSetting.No, disabledFlagCondition));

            MiicCondition activateFlagCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.ActivateFlag),
                        ((int)UserActivateSetting.Agree).ToString(),
                        DbType.String,
                        MiicDBOperatorSetting.Equal);
            conditions.Add(new MiicConditionLeaf(activateFlagCondition));

            MiicCondition validCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.Valid),
                        ((int)MiicValidTypeSetting.Valid).ToString(),
                        DbType.String,
                        MiicDBOperatorSetting.Equal);
            conditions.Add(new MiicConditionLeaf(validCondition));
            MiicColumnCollections columns = new MiicColumnCollections();
            MiicColumn column = new MiicColumn(Config.Attribute.GetSqlTableNameByClassName<UserInfo>());
            columns.Add(column);
            try
            {
                result = dbService.GetInformations(columns,relation, conditions, out message);
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

        int IUserInfo.GetAllUserInfosCount()
        {
            int result = 0;
            string message = string.Empty;
            MiicRelation relation = new MiicRelation(Config.Attribute.GetSqlTableNameByClassName<UserInfo>(),
                Config.Attribute.GetSqlColumnNameByPropertyName<UserInfo, string>(o => o.UserID),
                Config.Attribute.GetSqlTableNameByClassName<MiicSocialUserInfo>(),
                Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.ID),
                MiicDBOperatorSetting.Equal,
                MiicDBRelationSetting.LeftJoin);
            MiicConditionCollections conditions = new MiicConditionCollections(MiicDBLogicSetting.No);
            //选择有效地，没有被禁止的，已经被激活的 
            MiicCondition disabledFlagCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.DisabledFlag),
                        ((int)MiicYesNoSetting.No).ToString(),
                        DbType.String,
                        MiicDBOperatorSetting.Equal); 
            conditions.Add(new MiicConditionLeaf(MiicDBLogicSetting.No, disabledFlagCondition));

            MiicCondition activateFlagCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.ActivateFlag),
                        ((int)UserActivateSetting.Agree).ToString(),
                        DbType.String,
                        MiicDBOperatorSetting.Equal);
            conditions.Add(new MiicConditionLeaf(activateFlagCondition));

            MiicCondition validCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.Valid),
                        ((int)MiicValidTypeSetting.Valid).ToString(),
                        DbType.String,
                        MiicDBOperatorSetting.Equal);
            conditions.Add(new MiicConditionLeaf(validCondition));
            MiicColumn column = new MiicColumn(Config.Attribute.GetSqlTableNameByClassName<UserInfo>(),
                Config.Attribute.GetSqlColumnNameByPropertyName<UserInfo, string>(o => o.UserID)); 
            try
            {
                result = dbService.GetCount(column,relation, conditions, out message);
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
