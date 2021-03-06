﻿using Miic.Base;
using Miic.Base.Setting;
using Miic.DB.Setting;
using Miic.DB.SqlObject;
using Miic.Manage.User.Setting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miic.Manage.User
{
    public partial class DownloadSearchView
    {
         /// <summary>
        /// 关键字
        /// </summary>
        public string Keyword { get; set; }
        /// <summary>
        /// 用户类别
        /// </summary>
        public UserTypeSetting UserType { get; set; }
        /// <summary>
        /// 是否禁用
        /// </summary>
        public MiicYesNoSetting IsDisabled { get; set; }
        /// <summary>
        /// 是否激活
        /// </summary>
        public UserActivateSetting IsActived { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        public MiicValidTypeSetting IsValid { get; set; }
        /// <summary>
        /// 下载类别
        /// 所有用户：unknown;筛选用户：yes;勾选用户：no
        /// </summary>
        public MiicYesNoSetting DownloadType { get; set; }
        /// <summary>
        /// 勾选用户时选中的用户ID集合
        /// </summary>
        public List<string> CheckIDs { get; set; }
        public DownloadSearchView()
        {
            IsDisabled = MiicYesNoSetting.Unknown;
            IsActived = UserActivateSetting.Unknown;
            IsValid = MiicValidTypeSetting.Unknown;
            DownloadType = MiicYesNoSetting.Unknown;
            CheckIDs = new List<string>();
        }

        public MiicConditionCollections visitor(MiicSocialUserDao socialUserDao)
        {
            MiicConditionCollections condition = new MiicConditionCollections(MiicDBLogicSetting.No);
            if (DownloadType == MiicYesNoSetting.Yes)
            {
                MiicConditionCollections keywordCondition = new MiicConditionCollections(MiicDBLogicSetting.No);
                MiicCondition socialCode = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DownloadAllUserView, string>(o => o.SocialCode),
                    Keyword,
                    DbType.String,
                    MiicDBOperatorSetting.Like);
                keywordCondition.Add(new MiicConditionLeaf(MiicDBLogicSetting.No, socialCode));
                MiicCondition userName = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DownloadAllUserView, string>(o => o.UserName),
                    Keyword,
                    DbType.String,
                    MiicDBOperatorSetting.Like);
                keywordCondition.Add(new MiicConditionLeaf(MiicDBLogicSetting.Or, userName));
                MiicCondition email = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DownloadAllUserView, string>(o => o.Email),
                    Keyword,
                    DbType.String,
                    MiicDBOperatorSetting.Like);
                keywordCondition.Add(new MiicConditionLeaf(MiicDBLogicSetting.Or, email));
                condition.Add(keywordCondition);
                if (UserType != UserTypeSetting.All)
                {
                    if (UserType == UserTypeSetting.AllAdminDeparter)
                    {
                        MiicConditionCollections userTypeCondition = new MiicConditionCollections();
                        MiicCondition contryDepart = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DownloadAllUserView, string>(o => o.UserType),
                            ((int)UserTypeSetting.CountryDepartUser).ToString(),
                            DbType.String,
                            MiicDBOperatorSetting.Equal);
                        userTypeCondition.Add(new MiicConditionLeaf(MiicDBLogicSetting.No, contryDepart));
                        MiicCondition localDepart = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DownloadAllUserView, string>(o => o.UserType),
                            ((int)UserTypeSetting.LocalDepartUser).ToString(),
                            DbType.String,
                            MiicDBOperatorSetting.Equal);
                        userTypeCondition.Add(new MiicConditionLeaf(MiicDBLogicSetting.Or, localDepart));
                        condition.Add(userTypeCondition);
                    }
                    else if (UserType == UserTypeSetting.AllAdminPerson)
                    {
                        MiicConditionCollections userTypeCondition = new MiicConditionCollections();
                        MiicCondition countryAdmin = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DownloadAllUserView, string>(o => o.UserType),
                            ((int)UserTypeSetting.CountryAdmin).ToString(),
                            DbType.String,
                            MiicDBOperatorSetting.Equal);
                        userTypeCondition.Add(new MiicConditionLeaf(MiicDBLogicSetting.No, countryAdmin));
                        MiicCondition localAdmin = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DownloadAllUserView, string>(o => o.UserType),
                            ((int)UserTypeSetting.LocalAdmin).ToString(),
                            DbType.String,
                            MiicDBOperatorSetting.Equal);
                        userTypeCondition.Add(new MiicConditionLeaf(MiicDBLogicSetting.Or, localAdmin));
                        MiicCondition admin = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DownloadAllUserView, string>(o => o.UserType),
                            ((int)UserTypeSetting.Administrator).ToString(),
                            DbType.String,
                            MiicDBOperatorSetting.Equal);
                        userTypeCondition.Add(new MiicConditionLeaf(MiicDBLogicSetting.Or, admin));
                        condition.Add(userTypeCondition);
                    }
                    else
                    {
                        MiicCondition userTypeCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DownloadAllUserView, string>(o => o.UserType),
                                ((int)UserType).ToString(),
                                DbType.String,
                                MiicDBOperatorSetting.Equal);
                        condition.Add(new MiicConditionLeaf(userTypeCondition));
                    }
                }

                if (IsDisabled != MiicYesNoSetting.Unknown)
                {
                    MiicCondition disabledFlagCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DownloadAllUserView, string>(o => o.DisabledFlag),
                              ((int)IsDisabled).ToString(),
                              DbType.String,
                              MiicDBOperatorSetting.Equal);
                    condition.Add(new MiicConditionLeaf(disabledFlagCondition));
                }

                if (IsActived != UserActivateSetting.Unknown)
                {
                    MiicCondition activateFlagCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DownloadAllUserView, string>(o => o.ActivateFlag),
                              ((int)IsActived).ToString(),
                              DbType.String,
                              MiicDBOperatorSetting.Equal);
                    condition.Add(new MiicConditionLeaf(activateFlagCondition));

                }

                if (IsValid != MiicValidTypeSetting.Unknown)
                {
                    MiicCondition validCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DownloadAllUserView, string>(o => o.Valid),
                             ((int)IsValid).ToString(),
                             DbType.String,
                             MiicDBOperatorSetting.Equal);
                    condition.Add(new MiicConditionLeaf(validCondition));
                }
            }
            else if(DownloadType == MiicYesNoSetting.No)
            {
                MiicConditionCollections userIDConditions = new MiicConditionCollections(MiicDBLogicSetting.No);
                if (CheckIDs.Count != 0)
                {
                    bool first = true;
                    foreach (string userID in CheckIDs)
                    {
                        if (first == true)
                        {
                            MiicCondition userIDCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DownloadAllUserView, string>(o => o.ID),
                            userID,
                            DbType.String,
                            MiicDBOperatorSetting.Equal);
                            userIDConditions.Add(new MiicConditionLeaf(MiicDBLogicSetting.No, userIDCondition));
                            first = false;
                        }
                        else
                        {
                            MiicCondition userIDCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DownloadAllUserView, string>(o => o.ID),
                            userID,
                            DbType.String,
                            MiicDBOperatorSetting.Equal);
                            userIDConditions.Add(new MiicConditionLeaf(MiicDBLogicSetting.Or, userIDCondition));
                        }
                    }
                    condition.Add(userIDConditions);
                }
                else
                {
                    throw new Exception("勾选用户不能为空");
                }
            }
            return condition;
        }
    }
}
