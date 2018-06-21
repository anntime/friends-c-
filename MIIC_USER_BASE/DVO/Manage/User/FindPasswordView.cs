using Miic.Base;
using Miic.Base.Setting;
using Miic.DB.Setting;
using Miic.DB.SqlObject;
using Miic.Manage.User.Setting;
using System.Data;

namespace Miic.Manage.User
{
    public class FindPasswordView
    {
        /// <summary>
        /// 找回方式
        /// </summary>
        public MiicGetBackTypeSetting Type { get; set; }
        /// <summary>
        /// 方式值：邮箱、手机
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 用户来源
        /// </summary>
        public UserLoginTypeSetting LoginType { get; set; }
        public FindPasswordView() 
        {
            this.LoginType = UserLoginTypeSetting.Other;
        }

        /// <summary>
        /// 找回密码访问器
        /// </summary>
        /// <param name="miicSocialUserDao">微博用户DAO</param>
        /// <returns>生成查找条件</returns>
        public MiicConditionCollections Vistor(MiicSocialUserDao miicSocialUserDao)
        {
            MiicConditionCollections condition = new MiicConditionCollections();
            switch (Type)
            {
                case MiicGetBackTypeSetting.Email:
                    MiicCondition emailCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.Email),
                                                              Value,
                                                              DbType.String,
                                                              MiicDBOperatorSetting.Equal);
                    MiicCondition isEmailBindConditon = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.EmailBind),
                                                              ((int)MiicYesNoSetting.Yes).ToString(),
                                                              DbType.String,
                                                              MiicDBOperatorSetting.Equal);
                    condition.Add(new MiicConditionLeaf(MiicDBLogicSetting.No, emailCondition));
                    condition.Add(new MiicConditionLeaf(isEmailBindConditon));
                    break;
                case MiicGetBackTypeSetting.Mobile:
                    MiicCondition mobileCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.Mobile),
                                                              Value,
                                                              DbType.String,
                                                              MiicDBOperatorSetting.Equal);
                    MiicCondition isMobileBindConditon = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.MobileBind),
                                                              ((int)MiicYesNoSetting.Yes).ToString(),
                                                              DbType.String,
                                                              MiicDBOperatorSetting.Equal);
                    condition.Add(new MiicConditionLeaf(MiicDBLogicSetting.No, mobileCondition));
                    condition.Add(new MiicConditionLeaf(isMobileBindConditon));
                    break;
            }
            if (LoginType == UserLoginTypeSetting.Friends) 
            {
                MiicConditionCollections userTypeCondition = new MiicConditionCollections(MiicDBLogicSetting.And);
                MiicCondition user = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.UserType),
                    ((int)UserTypeSetting.PersonUser).ToString(),
                    DbType.String,
                    MiicDBOperatorSetting.Equal);
                userTypeCondition.Add(new MiicConditionLeaf(MiicDBLogicSetting.No, user));
                MiicCondition administrator = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.UserType),
                    ((int)UserTypeSetting.Administrator).ToString(),
                    DbType.String,
                    MiicDBOperatorSetting.Equal);
                userTypeCondition.Add(new MiicConditionLeaf(MiicDBLogicSetting.Or, administrator));
                MiicCondition countryAdmin = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.UserType),
                   ((int)UserTypeSetting.CountryAdmin).ToString(),
                   DbType.String,
                   MiicDBOperatorSetting.Equal);
                userTypeCondition.Add(new MiicConditionLeaf(MiicDBLogicSetting.Or, countryAdmin));
                MiicCondition localAdmin = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.UserType),
                  ((int)UserTypeSetting.LocalAdmin).ToString(),
                  DbType.String,
                  MiicDBOperatorSetting.Equal);
                userTypeCondition.Add(new MiicConditionLeaf(MiicDBLogicSetting.Or,localAdmin));
                condition.Add(userTypeCondition);
            }
            else if (LoginType == UserLoginTypeSetting.Manage) 
            {
                MiicCondition administrator = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.UserType),
                  ((int)UserTypeSetting.Administrator).ToString(),
                  DbType.String,
                  MiicDBOperatorSetting.Equal);
                condition.Add(new MiicConditionLeaf(administrator));
            }
            return condition;
        }
    }
}
