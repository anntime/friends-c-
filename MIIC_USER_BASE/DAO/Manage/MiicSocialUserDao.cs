using Miic;
using Miic.Base;
using Miic.Base.Setting;
using Miic.BaseStruct;
using Miic.Common;
using Miic.DB;
using Miic.DB.Setting;
using Miic.DB.SqlObject;
using Miic.Email;
using Miic.Log;
using Miic.Manage.Org;
using Miic.Manage.User.Setting;
using Miic.MiicException;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net.Mail;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using Miic.IM.User;
using Miic.Base.ConfigSection;
namespace Miic.Manage.User
{
    public class MiicSocialUserDao : NoRelationCommon<MiicSocialUserInfo>, IMiicSocialUser
    {
        private static readonly string ClassName = MethodBase.GetCurrentMethod().DeclaringType.Name;
        private static readonly string NamespaceName = MethodBase.GetCurrentMethod().DeclaringType.Namespace;
        private static readonly string DefaultPhotoUrl = WebConfigurationManager.AppSettings["DefaultPhotoUrl"].ToString();
        private static readonly string DefaultMicroThemeID = WebConfigurationManager.AppSettings["DefaultMicroThemeID"].ToString();
        private static readonly InitPasswordConfigSection InitPasswordSection = (InitPasswordConfigSection)WebConfigurationManager.GetSection("InitPasswordConfigSection");
        private static readonly string ManageUrl = WebConfigurationManager.AppSettings["ManageUrl"].ToString();
        //private static readonly IOpenIMUserInfo IopenIMUserInfo = new OpenUserInfoService();
        /// <summary>
        /// 当前用户ID
        /// </summary>
        public string UserID { get; private set; }
        /// <summary>
        /// 当前用户名
        /// </summary>
        public string UserName { get; private set; }
        public bool IsCached { get; private set; }
        public MiicSocialUserDao()
            : this(true)
        {

        }
        public MiicSocialUserDao(bool isCached)
        {
            this.IsCached = isCached;
            Cookie cookie = new Cookie();
            string message = string.Empty;
            this.UserID = cookie.GetCookie("MiicID", out message);
            this.UserName = HttpUtility.UrlDecode(cookie.GetCookie("MiicUserName", out message));
        }
        bool ICommon<MiicSocialUserInfo>.Insert(MiicSocialUserInfo socialUserInfo)
        {
            Contract.Requires<ArgumentNullException>(socialUserInfo != null, "参数miicSocialUserInfo：不能为空！");
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(socialUserInfo.ID), "参数miicSocialUserInfo.ID：不能为空！");
            bool result = false;
            int count = 0;
            string message = string.Empty;
            /*------------------------------用户初始化值-----------------------------------*/
            //初始密码
            socialUserInfo.Password = InitPasswordSection.Password; 
            //初始MD5密码
            socialUserInfo.MD5Password = InitPasswordSection.MD5;
            //初始MD5密码
            socialUserInfo.SM3Password = InitPasswordSection.SM3;
            //Email是否绑定"
            socialUserInfo.EmailBind = ((int)MiicYesNoSetting.Yes).ToString();
            //用户激活状态
            //如果激活状态不是激活状态，则为待激活状态
            if (socialUserInfo.ActivateFlag != ((int)UserActivateSetting.Agree).ToString())
            {
                socialUserInfo.ActivateFlag = ((int)UserActivateSetting.Waiting).ToString();
            }
            //登录次数
            socialUserInfo.LoginNum = 0;
            //创建时间
            socialUserInfo.CreateTime = DateTime.Now;
            //默认头像
            socialUserInfo.MicroUserUrl = DefaultPhotoUrl;
            //微博主题样式ID
            socialUserInfo.MicroThemeID = DefaultMicroThemeID;
            //有效
            socialUserInfo.Valid = ((int)MiicValidTypeSetting.Valid).ToString();
            //用户等级
            socialUserInfo.Level = ((int)UserLevelSetting.Level1).ToString();
            //是否能被搜索
            socialUserInfo.CanSearch = ((int)MiicYesNoSetting.Yes).ToString();
            /*------------------------------用户初始化值-----------------------------------*/
            try
            {
                result = dbService.Insert(socialUserInfo, out count, out message);
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
                    InsertCache(socialUserInfo);
                }
            }
            return result;
        }

        bool ICommon<MiicSocialUserInfo>.Update(MiicSocialUserInfo socialUserInfo)
        {
            Contract.Requires<ArgumentNullException>(socialUserInfo != null, "参数miicSocialUserInfo:不能为空！");
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(socialUserInfo.ID), "参数miicSocialUserInfo.ID:不能为空，因为是主键");
            int count = 0;
            string message = string.Empty;
            bool result = false;
            bool fileResult = false;
            socialUserInfo.UpdateTime = DateTime.Now;
            try
            {
                MiicSocialUserInfo temp = ((ICommon<MiicSocialUserInfo>)this).GetInformation(socialUserInfo.ID);
                if (!string.IsNullOrEmpty(socialUserInfo.MicroUserUrl)
                   && temp.MicroUserUrl != socialUserInfo.MicroUserUrl)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(temp.MicroUserUrl) && temp.MicroUserUrl != DefaultPhotoUrl)
                        {
                            File.Delete(HttpContext.Current.Server.MapPath(temp.MicroUserUrl));
                            File.Delete(HttpContext.Current.Server.MapPath("/file/temp/User/" + Path.GetFileName(temp.MicroUserUrl)));
                        }
                        string dest = HttpContext.Current.Server.MapPath(socialUserInfo.MicroUserUrl);
                        string source = HttpContext.Current.Server.MapPath("/file/temp/User/" + Path.GetFileName(dest));
                        File.Copy(source, dest, true);
                        fileResult = true;
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
                }
                else
                {
                    fileResult = true;
                }
                if (fileResult == true)
                {
                    result = dbService.Update<MiicSocialUserInfo>(socialUserInfo, out count, out message);
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
            if (result == true)
            {
                if (this.IsCached == true)
                {
                    DeleteCache(o => o.ID == socialUserInfo.ID);
                }
            }
            return result;
        }
        /// <summary>
        /// 删除操作
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>Yes/No</returns>
        /// <remarks>如果是Person则级联删除用户信息，否则级联删除企业信息</remarks>
        bool ICommon<MiicSocialUserInfo>.Delete(string id)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(id), "参数id:不能为空");
            bool result = false;
            string message = string.Empty;
            bool fileResult = false;
            bool orgProductFileResult = true;
            List<string> sqls = new List<string>();
            MiicSocialUserInfo temp = ((ICommon<MiicSocialUserInfo>)this).GetInformation(id);
            try
            {
                try
                {
                    if (!string.IsNullOrEmpty(temp.MicroUserUrl) && temp.MicroUserUrl != DefaultPhotoUrl)
                    {
                        File.Delete(HttpContext.Current.Server.MapPath(temp.MicroUserUrl));
                        File.Delete(HttpContext.Current.Server.MapPath("/file/temp/User/" + Path.GetFileName(temp.MicroUserUrl)));
                    }
                    fileResult = true;
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
                if (fileResult == true)
                {
                    string message1 = string.Empty;
                    string message2 = string.Empty;
                    sqls.Add(DBService.DeleteSql(new MiicSocialUserInfo()
                     {
                         ID = id
                     }, out message1));
                    if (temp.UserType == ((int)UserTypeSetting.Administrator).ToString()
                        || temp.UserType == ((int)UserTypeSetting.LocalAdmin).ToString()
                        || temp.UserType == ((int)UserTypeSetting.CountryAdmin).ToString()
                        || temp.UserType == ((int)UserTypeSetting.PersonUser).ToString())
                    {
                        //删除用户
                        sqls.Add(DBService.DeleteSql(new UserInfo()
                        {
                            UserID = id
                        }, out message2));
                    }
                    else
                    {
                        orgProductFileResult = false;
                        string message3 = string.Empty;
                        //删除企业
                        sqls.Add(DBService.DeleteSql(new OrganizationInfo()
                        {
                            OrgID = id
                        }, out message2));
                        IOrganizationInfo IorgInfo = new OrganizationInfoDao();
                        List<OrgProductInfo> orgProductInfoList = IorgInfo.GetOrgProductList(id);
                        try
                        {
                            foreach (OrgProductInfo item in orgProductInfoList)
                            {
                                File.Delete(HttpContext.Current.Server.MapPath(item.FilePath));
                                File.Delete(HttpContext.Current.Server.MapPath("/file/temp/OrgProduct/" + Path.GetFileName(item.FilePath)));
                            }
                            orgProductFileResult = true;
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
                        if (orgProductInfoList.Count > 0)
                        {
                            MiicCondition orgIDCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<OrgProductInfo, string>(o => o.OrgID),
                                id,
                                DbType.String,
                                MiicDBOperatorSetting.Equal);
                            MiicConditionSingle condition = new MiicConditionSingle(orgIDCondition);
                            sqls.Add(DBService.DeleteConditionSql<OrgProductInfo>(condition, out message3));
                        }

                    }
                    if (orgProductFileResult == true)
                    {
                        result = dbService.excuteSqls(sqls, out message);
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
            if (result == true)
            {
                if (IsCached == true)
                {
                    DeleteCache(o => o.ID == id);
                    if (temp.UserType == ((int)UserTypeSetting.Administrator).ToString()
                         || temp.UserType == ((int)UserTypeSetting.LocalAdmin).ToString()
                         || temp.UserType == ((int)UserTypeSetting.CountryAdmin).ToString()
                         || temp.UserType == ((int)UserTypeSetting.PersonUser).ToString())
                    {
                        UserInfoDao.DeleteCache(o => o.UserID == id);
                    }
                    else
                    {
                        OrganizationInfoDao.DeleteCache(o => o.OrgID == id);
                    }
                }
                try
                {
                    if (temp.UserType == ((int)UserTypeSetting.Administrator).ToString()
                       || temp.UserType == ((int)UserTypeSetting.LocalAdmin).ToString()
                       || temp.UserType == ((int)UserTypeSetting.CountryAdmin).ToString()
                       || temp.UserType == ((int)UserTypeSetting.PersonUser).ToString())
                    {
                        //IopenIMUserInfo.Delete(id);
                    }
                }
                finally { }
               
            }

            return result;
        }

        MiicSocialUserInfo ICommon<MiicSocialUserInfo>.GetInformation(string id)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(id), "参数id:不能为空");
            MiicSocialUserInfo result = null;
            string message = string.Empty;
            try
            {
                result = items.Find(o => o.ID == id);
                if (result == null)
                {
                    result = dbService.GetInformation(new MiicSocialUserInfo
                    {
                        ID = id
                    }, out message);
                    if (result != null)
                    {
                        if (IsCached == true)
                        {
                            InsertCache(result);
                        }
                    }
                }
                else
                {
                    string serializer = Config.Serializer.Serialize(result);
                    result = Config.Attribute.ConvertObjectWithDateTime(Config.Serializer.Deserialize<MiicSocialUserInfo>(serializer));
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
        public DataTable GetInformationBySocialCode(string socialCode)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(socialCode), "参数id:不能为空");
            DataTable result = null;
            string message = string.Empty;
            MiicCondition socialCodeCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.SocialCode),
                    socialCode,
                    DbType.String,
                    MiicDBOperatorSetting.Equal);
            MiicCondition flagCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.ActivateFlag),
               ((int)UserActivateSetting.Agree).ToString(),
                DbType.String,
                MiicDBOperatorSetting.Equal);
            MiicCondition disableCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.DisabledFlag),
               ((int)MiicYesNoSetting.No).ToString(),
                DbType.String,
                MiicDBOperatorSetting.Equal);
            MiicConditionCollections condition = new MiicConditionCollections();
            condition.Add(new MiicConditionLeaf(MiicDBLogicSetting.No, socialCodeCondition));
            condition.Add(new MiicConditionLeaf(flagCondition));
            condition.Add(new MiicConditionLeaf(disableCondition));
            List<MiicOrderBy> order = new List<MiicOrderBy>();
            order.Add(new MiicOrderBy()
            {
                Desc = true,
                PropertyName = Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.ID)
            }); 
            condition.order = order;
            try
            {
                result = dbService.GetInformations<MiicSocialUserInfo>(null, condition, out message);
                
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
        /// 找回密码
        /// </summary>
        /// <param name="psView">密码视图</param>
        /// <returns>Yes/No</returns>
        public bool FindPassword(FindPasswordView psView)
        {
            Contract.Requires<ArgumentNullException>(psView != null, "参数psView:不能为空");
            bool result = false;
            string message = string.Empty;
            string mailType = "Miic.Config.email.xml";
            MiicConditionCollections condition = psView.Vistor(this);
            try
            {
                DataTable dt = dbService.GetInformations<MiicSocialUserInfo>(null, condition, out message);
                if (dt.Rows.Count >= 1)
                {
                    string content = string.Empty;
                    if (dt.Rows.Count == 1)
                    {
                        string socialCode = dt.Rows[0][Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.SocialCode)].ToString();
                        string password = dt.Rows[0][Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.Password)].ToString();
                        content = "尊敬的用户" + socialCode + "您好：您注册的" + Config.AppName + "账户已经找回，账号为：" + socialCode + "；密码为：" + password + " ,感谢您的支持与关注！";
                    }
                    else
                    {
                        content = "尊敬的用户，您好：您在" + Config.AppName + "上共注册" + dt.Rows.Count + "个账号，分别如下";
                        foreach (DataRow dr in dt.AsEnumerable())
                        {
                            string socialCode = dr[Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.SocialCode)].ToString();
                            string password = dr[Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.Password)].ToString();
                            content += "账号为：" + socialCode + "；密码为：" + password;
                        }
                        content += "请酌情选择您所需要的账号进行登录，感谢您的支持与关注！";
                    }
                    MiicEmail email = new MiicEmail(Config.AppName, "rhadamanthys0407@126.com", psView.Value, "找回密码", content);
                    email.Priority = MailPriority.High;
                    Reflection reflection = new Reflection();
                    Assembly assembly = Assembly.LoadFrom(HttpContext.Current.Server.MapPath("/Bin/MiicLibrary.dll"));
                    Stream stream = assembly.GetManifestResourceStream(mailType);
                    if (stream != null)
                    {
                        IEmailService Iemail = reflection.initInterface<IEmailService>(stream, "01");
                        result = Iemail.SendMail(email, out message);
                    }
                    else
                    {
                        throw new Exception("邮件初始化配置存在异常");
                    }
                }

                else
                {
                    throw new Exception("联系方式不存在或未绑定");
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
        /// 登录
        /// </summary>
        /// <param name="loginView">登录视图</param>
        /// <returns>登录响应视图</returns>
        public LoginResponseView Login(LoginRequestView loginRequestView, UserLoginTypeSetting userLoginType)
        {
            Contract.Requires<ArgumentNullException>(loginRequestView != null, "参数loginView:不能为空！");
            LoginResponseView result = new LoginResponseView()
            {
                //是否登录成功
                IsLogin = false,
                //密码是否错误？
                CheckPassword = true,
                //账号是否错误？
                CheckUserCode = true,
                //是否用户失效
                CheckValid = true,
                //管理系统登录 是否为管理员
                CheckAdmin = true
            };

            int count = 0;
            DataTable dt = new DataTable();
            string message1 = string.Empty;
            string message2 = string.Empty;
            string message3 = string.Empty;
            bool loginNumAdd = false;
            MiicConditionCollections conditions = loginRequestView.visitor(this);
            //如果是管理员，添加条件判断，用于登录管理系统使用
            if (userLoginType == UserLoginTypeSetting.Manage)
            {
                MiicCondition userTypeCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.UserType),
                    ((int)UserTypeSetting.Administrator).ToString(),
                    DbType.String,
                    MiicDBOperatorSetting.Equal);
                conditions.Add(new MiicConditionLeaf(userTypeCondition));
            }
            else if (userLoginType == UserLoginTypeSetting.Friends)
            {
                MiicConditionCollections userTypeCondition = new MiicConditionCollections();
                MiicCondition adminCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.UserType),
                    ((int)UserTypeSetting.Administrator).ToString(),
                    DbType.String,
                    MiicDBOperatorSetting.Equal);
                userTypeCondition.Add(new MiicConditionLeaf(MiicDBLogicSetting.No, adminCondition));
                MiicCondition localAdminCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.UserType),
                   ((int)UserTypeSetting.LocalAdmin).ToString(),
                   DbType.String,
                   MiicDBOperatorSetting.Equal);
                userTypeCondition.Add(new MiicConditionLeaf(MiicDBLogicSetting.Or, localAdminCondition));
                MiicCondition countryAdminCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.UserType),
                  ((int)UserTypeSetting.CountryAdmin).ToString(),
                  DbType.String,
                  MiicDBOperatorSetting.Equal);
                userTypeCondition.Add(new MiicConditionLeaf(MiicDBLogicSetting.Or, countryAdminCondition));
                MiicCondition personCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.UserType),
                  ((int)UserTypeSetting.PersonUser).ToString(),
                  DbType.String,
                  MiicDBOperatorSetting.Equal);
                userTypeCondition.Add(new MiicConditionLeaf(MiicDBLogicSetting.Or, personCondition));
                conditions.Add(userTypeCondition);
            }
            try
            {
                dt = dbService.GetInformations<MiicSocialUserInfo>(null, conditions, out message1);
                if (dt.Rows.Count == 1)
                {
                    result.IsLogin = true;

                    MiicSocialUserInfo miicSocialUserInfo = new MiicSocialUserInfo()
                    {
                        ID = dt.Rows[0][Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.ID)].ToString(),
                        LoginNum = Convert.ToInt64(dt.Rows[0][Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, long?>(o => o.LoginNum)])
                    };
                    miicSocialUserInfo.LoginNum = miicSocialUserInfo.LoginNum.GetValueOrDefault(0) + 1;

                    loginNumAdd = dbService.Update(miicSocialUserInfo, out count, out message2);
                }
                else
                {
                    conditions.Clear();
                    if (userLoginType == UserLoginTypeSetting.Manage)
                    {
                        conditions = loginRequestView.visitor(this);
                        dt = dbService.GetInformations<MiicSocialUserInfo>(null, conditions, out message2);
                        if (dt.Rows.Count == 1)
                        {
                            result.CheckAdmin = false;
                        }
                        else
                        {
                            conditions.Clear();
                            conditions.Add(new MiicConditionLeaf(MiicDBLogicSetting.No,
                                               new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.SocialCode),
                                                                    loginRequestView.SocialCode,
                                                                    DbType.String,
                                                                    MiicDBOperatorSetting.Equal
                                                                )
                                                             ));
                            dt = dbService.GetInformations<MiicSocialUserInfo>(null, conditions, out message2);
                            if (dt.Rows.Count == 1)
                            {
                                if (dt.Rows[0][Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.Valid)].ToString() != ((int)MiicValidTypeSetting.InValid).ToString())
                                {
                                    result.CheckPassword = false;
                                }
                                else
                                {
                                    result.CheckValid = false;
                                }
                            }
                            else
                            {
                                result.CheckUserCode = false;
                            }
                        }
                    }
                    else
                    {
                        conditions.Add(new MiicConditionLeaf(MiicDBLogicSetting.No,
                                               new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.SocialCode),
                                                                    loginRequestView.SocialCode,
                                                                    DbType.String,
                                                                    MiicDBOperatorSetting.Equal
                                                                )
                                                             ));
                        dt = dbService.GetInformations<MiicSocialUserInfo>(null, conditions, out message2);
                        if (dt.Rows.Count == 1)
                        {
                            if (dt.Rows[0][Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.Valid)].ToString() != ((int)MiicValidTypeSetting.InValid).ToString())
                            {
                                result.CheckPassword = false;
                            }
                            else
                            {
                                result.CheckValid = false;
                            }
                        }
                        else
                        {
                            result.CheckUserCode = false;
                        }
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

            if (result.IsLogin == true && loginNumAdd == true)
            {
                result.IsLogin = this.LoginSuccessHandle(dt.Rows[0][Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.ID)].ToString());
            }
            return result;
        }

        public bool LoginSuccessHandle(string userID)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(userID))
            {
                try
                {
                    MiicSocialUserInfo temp = ((ICommon<MiicSocialUserInfo>)this).GetInformation(userID);
                    if (temp != null)
                    {
                        //设置cookie
                        Cookie cookie = new Cookie();
                        string cookieMessage = string.Empty;
                        cookie.Clear(out cookieMessage, MiicClientOrServerSetting.Client);
                        //用户编码
                        MiicKeyValue socialCodeCookie = new MiicKeyValue()
                        {
                            Name = "SNS_SocialCode",
                            Value = temp.SocialCode
                        };
                        cookie.SetCookie(socialCodeCookie, out cookieMessage);
                        Config.Oper = socialCodeCookie.Value.ToString();
                        //用户ID
                        MiicKeyValue userIDCookie = new MiicKeyValue()
                        {
                            Name = "MiicID",
                            Value = temp.ID
                        };
                        cookie.SetCookie(userIDCookie, out cookieMessage);
                        //md5密码
                        MiicKeyValue passwordCookie = new MiicKeyValue()
                        {
                            Name = "SNS_One",
                            Value = temp.SM3Password
                        };
                        cookie.SetCookie(passwordCookie, out cookieMessage);
                        //用户类型
                        MiicKeyValue userTypeCookie = new MiicKeyValue()
                        {
                            Name = "SNS_UserType",
                            Value = temp.UserType
                        };
                        cookie.SetCookie(userTypeCookie, out cookieMessage);
                        //微博头像
                        MiicKeyValue microUserUrlCookie = new MiicKeyValue()
                        {
                            Name = "SNS_UserUrl",
                            Value = ManageUrl + temp.MicroUserUrl
                        };
                        cookie.SetCookie(microUserUrlCookie, out cookieMessage);
                        MiicKeyValue microThemeIDCookie = new MiicKeyValue()
                        {
                            Name = "SNS_UserThemeID",
                            Value = temp.MicroThemeID
                        };
                        cookie.SetCookie(microThemeIDCookie, out cookieMessage);

                        //用户名
                        MiicKeyValue userNameCookie = new MiicKeyValue()
                        {
                            Name = "MiicUserName",
                            Value = string.Empty
                        };
                        if (int.Parse(temp.UserType) == (int)UserTypeSetting.PersonUser
                            || int.Parse(temp.UserType) == (int)UserTypeSetting.LocalAdmin
                            || int.Parse(temp.UserType) == (int)UserTypeSetting.CountryAdmin
                            || int.Parse(temp.UserType) == (int)UserTypeSetting.Administrator
                            )
                        {
                            IUserInfo IuserInfo = new UserInfoDao(IsCached);
                            UserInfo userInfo = IuserInfo.GetInformation(userIDCookie.Value.ToString());
                            userNameCookie.Value = HttpUtility.UrlEncode(userInfo.UserName);

                            //朋友圈主题ID
                            MiicKeyValue userThemeIDCookie = new MiicKeyValue()
                            {
                                Name = "SNS_FriendsUserThemeID",
                                Value = userInfo.FriendsThemeID
                            };
                            cookie.SetCookie(userThemeIDCookie, out cookieMessage);

                        }
                        else if (int.Parse(temp.UserType) == (int)UserTypeSetting.OrgUser
                            || int.Parse(temp.UserType) == (int)UserTypeSetting.LocalDepartUser
                            || int.Parse(temp.UserType) == (int)UserTypeSetting.CountryDepartUser)
                        {
                            IOrganizationInfo IorgInfo = new OrganizationInfoDao();
                            OrganizationInfo orgInfo = IorgInfo.GetInformation(userIDCookie.Value.ToString());
                            userNameCookie.Value = HttpUtility.UrlEncode(orgInfo.OrgName);
                        }
                        cookie.SetCookie(userNameCookie, out cookieMessage);

                        result = true;
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
            }
            return result;
        }

        /// <summary>
        /// 设置激活
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>Yes/No</returns>
        public bool SetActivateAgree(string id)
        {
            bool result = false;
            int count = 0;
            string message = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(this.UserID))
                {
                    throw new MiicCookieArgumentNullException("UserID不能为空，Cookie失效");
                }
                result = dbService.Update<MiicSocialUserInfo>(new MiicSocialUserInfo()
                {
                    ID = id,
                    CreaterID = this.UserID,
                    CreaterName = this.UserName,
                    ActivateFlag = ((int)UserActivateSetting.Agree).ToString(),
                    ActivateTime = DateTime.Now

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
                    DeleteCache(o => o.ID == id);
                }
                try
                {
                    //IopenIMUserInfo.Update(new OpenIMUserInfo()
                    //{
                    //    UserID = id,
                    //    Status = OpenIMUserActivationSetting.Activated,
                    //    UpdateTime = DateTime.Now
                    //});
                }
                finally { }
            }
            return result;
        }
        /// <summary>
        /// 设置是否禁用
        /// </summary>
        /// <param name="yesNoView">用户禁用视图</param>
        /// <returns>Yes/No</returns>
        public bool SetDisabled(UserDisabledView userDisabledView)
        {
            bool result = false;
            int count = 0;
            string message = string.Empty;
            try
            {
                result = dbService.Update<MiicSocialUserInfo>(new MiicSocialUserInfo()
                {
                    ID = userDisabledView.ID,
                    DisabledFlag = ((int)userDisabledView.YesNo).ToString(),
                    DisabledReason = userDisabledView.Reason,
                    UpdaterID = this.UserID,
                    UpdaterName = this.UserName,
                    UpdateTime = DateTime.Now

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
                    DeleteCache(o => o.ID == userDisabledView.ID);
                }
                try
                {
                    if (userDisabledView.YesNo == MiicYesNoSetting.Yes)
                    {
                        //IopenIMUserInfo.Update(new OpenIMUserInfo()
                        //{
                        //    UserID = userDisabledView.ID,
                        //    Status = OpenIMUserActivationSetting.UnActivated,
                        //    UpdateTime = DateTime.Now
                        //});
                    }
                    else if (userDisabledView.YesNo == MiicYesNoSetting.No)
                    {
                        //IopenIMUserInfo.Update(new OpenIMUserInfo()
                        //{
                        //    UserID = userDisabledView.ID,
                        //    Status = OpenIMUserActivationSetting.Activated,
                        //    UpdateTime = DateTime.Now
                        //});
                    }
                }
                finally 
                {

                }
            }
            return result;
        }
        /// <summary>
        /// 设置用户有效性
        /// </summary>
        /// <param name="userValidView">有效性视图</param>
        /// <returns>Yes/No</returns>
        public bool SetValid(ValidView validView)
        {
            bool result = false;
            int count = 0;
            string message = string.Empty;
            try
            {
                result = dbService.Update<MiicSocialUserInfo>(new MiicSocialUserInfo()
                {
                    ID = validView.ID,
                    Valid = ((int)validView.Valid).ToString(),
                    UpdaterID = UserID,
                    UpdaterName = UserName,
                    UpdateTime = DateTime.Now
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
                    DeleteCache(o => o.ID == validView.ID);
                }
                try
                {
                    if (validView.Valid == MiicValidTypeSetting.Valid)
                    {
                        //IopenIMUserInfo.Update(new OpenIMUserInfo()
                        //{
                        //    UserID = validView.ID,
                        //    Status = OpenIMUserActivationSetting.Activated,
                        //    UpdateTime = DateTime.Now
                        //});
                    }
                    else
                    {
                        //IopenIMUserInfo.Update(new OpenIMUserInfo()
                        //{
                        //    UserID = validView.ID,
                        //    Status = OpenIMUserActivationSetting.UnActivated,
                        //    UpdateTime = DateTime.Now
                        //});
                    }
                }
                finally 
                {

                }
            }
            return result;
        }
        /// <summary>
        /// 是否激活
        /// </summary>
        /// <param name="ID">ID</param>
        /// <returns>YesNo</returns>
        public bool IsActivate(string id)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(id), "参数id:不能为空");
            bool result = false;
            if (items.FindAll(o => o.ID == id).Count > 0)
            {
                MiicSocialUserInfo item = items.Find(o => o.ID == id);
                if (item.ActivateFlag == ((int)UserActivateSetting.Agree).ToString())
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
                return result;
            }

            string message = string.Empty;
            MiicColumn column = new MiicColumn(Config.Attribute.GetSqlTableNameByClassName<MiicSocialUserInfo>(), Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.ID));
            MiicCondition idCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.ID),
                id,
                DbType.String,
                MiicDBOperatorSetting.Equal);
            MiicCondition flagCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.ActivateFlag),
               ((int)UserActivateSetting.Agree).ToString(),
                DbType.String,
                MiicDBOperatorSetting.Equal);
            MiicConditionCollections condition = new MiicConditionCollections();
            condition.Add(new MiicConditionLeaf(MiicDBLogicSetting.No, idCondition));
            condition.Add(new MiicConditionLeaf(flagCondition));
            try
            {
                int count = dbService.GetCount<MiicSocialUserInfo>(column, condition, out message);
                if (count == 1)
                {
                    result = true;
                }
                else
                {
                    result = false;
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
        /// 是否有效
        /// </summary>
        /// <param name="ID">ID</param>
        /// <returns>YesNo</returns>
        public bool IsValid(string id)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(id), "参数id:不能为空");
            bool result = false;
            if (items.FindAll(o => o.ID == id).Count > 0)
            {
                MiicSocialUserInfo item = items.Find(o => o.ID == id);
                if (item.Valid == ((int)MiicValidTypeSetting.Valid).ToString())
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            else
            {
                string message = string.Empty;
                MiicConditionCollections condition = new MiicConditionCollections(MiicDBLogicSetting.No);
                MiicCondition idCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.ID),
                    id,
                    DbType.String,
                    MiicDBOperatorSetting.Equal);
                condition.Add(new MiicConditionLeaf(MiicDBLogicSetting.No, idCondition));
                MiicCondition flagCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.Valid),
                    ((int)MiicValidTypeSetting.Valid).ToString(),
                    DbType.String,
                    MiicDBOperatorSetting.Equal);
                condition.Add(new MiicConditionLeaf(flagCondition));
                MiicColumn column = new MiicColumn(Config.Attribute.GetSqlTableNameByClassName<MiicSocialUserInfo>(),
                    Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.ID));
                try
                {
                    int count = dbService.GetCount<MiicSocialUserInfo>(column, condition, out message);
                    if (count == 1)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
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
            }
            return result;
        }
        /// <summary>
        /// 用户名唯一性确认
        /// </summary>
        /// <param name="socialCode"></param>
        /// <returns>true 无记录可插入 false 有记录不可插入</returns>
        public bool UniqueSocialCode(string socialCode)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(socialCode), "参数socialCode:不能为空");
            bool result = false;
            if (items.FindAll(o => o.SocialCode == socialCode).Count > 0) return result;
            string message = string.Empty;
            MiicColumn column = new MiicColumn(Config.Attribute.GetSqlTableNameByClassName<MiicSocialUserInfo>(), Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.ID));
            MiicCondition socialCodeCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.SocialCode),
                socialCode,
                DbType.String,
                MiicDBOperatorSetting.Equal);
            int count = 0;
            try
            {
                count = dbService.GetCount<MiicSocialUserInfo>(column, new MiicConditionSingle(socialCodeCondition), out message);
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
            if (count == 0)
            {
                result = true;
            }
            return result;
        }
        /// <summary>
        /// 邮箱唯一性确认
        /// </summary>
        /// <param name="email"></param>
        /// <returns>true 无记录可插入 false 有记录不可插入</returns>
        public bool UniqueEmail(string email)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(email), "参数email:不能为空");
            bool result = false;
            if (items.FindAll(o => o.Email == email).Count > 0) return result;
            string message = string.Empty;
            MiicColumn column = new MiicColumn(Config.Attribute.GetSqlTableNameByClassName<MiicSocialUserInfo>(), Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.ID));
            MiicCondition emailCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.Email),
                email,
                DbType.String,
                MiicDBOperatorSetting.Equal);
            int count = 0;
            try
            {
                count = dbService.GetCount<MiicSocialUserInfo>(column, new MiicConditionSingle(emailCondition), out message);
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
            if (count == 0)
            {
                result = true;
            }
            return result;
        }
        /// <summary>
        /// 手机唯一性确认
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns>true 无记录可插入 false 有记录不可插入</returns>
        public bool UniqueMobile(string mobile)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(mobile), "参数mobile:不能为空");
            bool result = false;
            if (items.FindAll(o => o.Mobile == mobile).Count > 0) return result;
            string message = string.Empty;
            MiicColumn column = new MiicColumn(Config.Attribute.GetSqlTableNameByClassName<MiicSocialUserInfo>(), Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.ID));
            MiicCondition mobileCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.Mobile),
                mobile,
                DbType.String,
                MiicDBOperatorSetting.Equal);
            int count = 0;
            try
            {
                count = dbService.GetCount<MiicSocialUserInfo>(column, new MiicConditionSingle(mobileCondition), out message);

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
            if (count == 0)
            {
                result = true;
            }
            return result;
        }


        public DataTable GetUserCountStatistics()
        {
            DataTable result = new DataTable();
            string message = string.Empty;
            try
            {
                result = dbService.querySql("select * from MIIC_SOCIAL_COMMON.dbo.GetUserCountStatistics()", out message);
                DataRow allRow = result.NewRow();
                allRow["USER_KEY"] = UserTypeSetting.All.ToString();
                allRow["USER_VALUE"] = result.Compute("sum(USER_VALUE)", "true");
                result.Rows.Add(allRow);
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
        /// 获取用户展示提示信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="userType">用户类别</param>
        /// <returns>用户信息</returns>
        public DataTable GetUserMicroblogTip(string userID, UserTypeSetting userType)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(userID), "参数userID:不能为空！");
            Contract.Requires<ArgumentException>(userType != UserTypeSetting.AllAdminDeparter && userType != UserTypeSetting.AllAdminPerson && userType != UserTypeSetting.All && userType != UserTypeSetting.AnonymousUser, "参数userType:非法收入");
            DataTable result = new DataTable();
            Dictionary<String, String> paras = new Dictionary<String, String>();
            paras.Add("USER_ID", userID);
            paras.Add("USER_TYPE", ((int)userType).ToString());
            string message = string.Empty;
            try
            {
                result = dbService.QueryStoredProcedure<string>("MIIC_MICRO_BLOG.dbo.GetUserTipInfo", paras, out message);
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
        /// 获取用户展示提示信息（朋友圈）
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="userType">用户类别</param>
        /// <returns>用户信息</returns>
        public DataTable GetUserFriendTip(string userID, UserTypeSetting userType)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(userID), "参数userID:不能为空！");
            Contract.Requires<ArgumentException>(userType != UserTypeSetting.AllAdminDeparter && userType != UserTypeSetting.AllAdminPerson && userType != UserTypeSetting.All && userType != UserTypeSetting.AnonymousUser, "参数userType:非法收入");
            DataTable result = new DataTable();
            Dictionary<String, String> paras = new Dictionary<String, String>();
            paras.Add("USER_ID", userID);
            paras.Add("USER_TYPE", ((int)userType).ToString());
            string message = string.Empty;
            try
            {
                result = dbService.QueryStoredProcedure<string>("MIIC_FRIENDS.dbo.GetUserTipInfo", paras, out message);
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
            throw new NotImplementedException();
        }
        public DataTable Search(UserSearchView userSearchView, MiicPage page = null)
        {
            DataTable result = new DataTable();
            string message = string.Empty;
            MiicConditionCollections condition = userSearchView.visitor(this);
            List<MiicOrderBy> order = new List<MiicOrderBy>();
            order.Add(new MiicOrderBy()
            {
                Desc = true,
                PropertyName = Config.Attribute.GetSqlColumnNameByPropertyName<SearchUserView, string>(o => o.UserType)
            });

            order.Add(new MiicOrderBy()
            {
                Desc = true,
                PropertyName = Config.Attribute.GetSqlColumnNameByPropertyName<SearchUserView, int?>(o => o.SortNo)
            });
            condition.order = order;
            try
            {
                if (page != null)
                {
                    result = dbService.GetInformationsPage<SearchUserView>(null, condition, page, out message);
                }
                else
                {
                    result = dbService.GetInformations<SearchUserView>(null, condition, out message);
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

        public int GetSearchCount(UserSearchView userSearchView)
        {
            int result = 0;
            string message = string.Empty;
            MiicConditionCollections condition = userSearchView.visitor(this);
            try
            {
                result = dbService.GetCount<SearchUserView>(null, condition, out message);
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

        public DataTable ManageSearch(UserSearchView userSearchView, MiicPage page = null)
        {
            DataTable result = new DataTable();
            string message = string.Empty;
            MiicConditionCollections condition = userSearchView.visitor(this);
            List<MiicOrderBy> order = new List<MiicOrderBy>();
            order.Add(new MiicOrderBy()
            {
                Desc = true,
                PropertyName = Config.Attribute.GetSqlColumnNameByPropertyName<SearchAllUserView, string>(o => o.UserType)
            });

            order.Add(new MiicOrderBy()
            {
                Desc = true,
                PropertyName = Config.Attribute.GetSqlColumnNameByPropertyName<SearchAllUserView, int?>(o => o.SortNo)
            });
            condition.order = order;
            try
            {
                if (page != null)
                {
                    result = dbService.GetInformationsPage<SearchAllUserView>(null, condition, page, out message);
                }
                else
                {
                    result = dbService.GetInformations<SearchAllUserView>(null, condition, out message);
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

        public int GetManageSearchCount(UserSearchView userSearchView)
        {
            int result = 0;
            string message = string.Empty;
            MiicConditionCollections condition = userSearchView.visitor(this);
            try
            {
                result = dbService.GetCount<SearchAllUserView>(null, condition, out message);
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
        /// 重置密码
        /// </summary>
        /// <param name="ids">ids</param>
        /// <returns>Yes/No</returns>
        public bool ResetPassword(List<string> ids)
        {
            Contract.Requires<ArgumentNullException>(ids != null, "参数ids:不能为空！");
            bool result = false;
            string message = string.Empty;
            string message1 = string.Empty;
            List<string> sqls = new List<string>();
            try
            {
                foreach (var id in ids)
                {
                    sqls.Add(DBService.UpdateSql(new MiicSocialUserInfo()
                    {
                        ID = id,
                        Password = InitPasswordSection.Password,
                        SM3Password = InitPasswordSection.SM3
                    }, out message1));
                }
                result = dbService.excuteSqls(sqls, out message);
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
                    foreach (var id in ids)
                    {
                        DeleteCache(o => o.ID == id);
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 删除未激活的用户
        /// </summary>
        /// <param name="userTypeViews">用户类别视图集合</param>
        /// <returns>Yes/No</returns>

        public bool Delete(List<UserTypeView> userTypeViews)
        {
            Contract.Requires<ArgumentNullException>(userTypeViews != null, "参数userTypeViews:不能为空！");
            bool result = false;
            string message1 = string.Empty;
            string message2 = string.Empty;
            string message3 = string.Empty;
            string message4 = string.Empty;
            string message = string.Empty;
            List<string> sqls = new List<string>();
            foreach (var item in userTypeViews)
            {
                sqls.Add(DBService.DeleteSql<MiicSocialUserInfo>(new MiicSocialUserInfo()
                {
                    ID = item.ID
                }, out message1));
                if (item.UserType == UserTypeSetting.Administrator
                    || item.UserType == UserTypeSetting.CountryAdmin
                    || item.UserType == UserTypeSetting.LocalAdmin
                    || item.UserType == UserTypeSetting.PersonUser)
                {
                    sqls.Add(DBService.DeleteSql<UserInfo>(new UserInfo()
                    {
                        UserID = item.ID
                    }, out message2));
                }
                else if (item.UserType == UserTypeSetting.LocalDepartUser
                    || item.UserType == UserTypeSetting.CountryDepartUser
                    || item.UserType == UserTypeSetting.OrgUser)
                {
                    sqls.Add(DBService.DeleteSql<OrganizationInfo>(new OrganizationInfo()
                    {
                        OrgID = item.ID
                    }, out message2));
                    MiicCondition orgIDDimCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DimOrganizationInfo, string>(o => o.OrgID),
                        item.ID,
                        DbType.String,
                        MiicDBOperatorSetting.Equal);
                    MiicConditionSingle condition0 = new MiicConditionSingle(orgIDDimCondition);
                    sqls.Add(DBService.DeleteConditionSql<DimOrganizationInfo>(condition0, out message4));
                    MiicCondition orgIDCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<OrgProductInfo, string>(o => o.OrgID),
                        item.ID,
                        DbType.String,
                        MiicDBOperatorSetting.Equal);
                    MiicConditionSingle condition1 = new MiicConditionSingle(orgIDCondition);
                    sqls.Add(DBService.DeleteConditionSql<OrgProductInfo>(condition1, out message3));
                }
            }
            try
            {
                result = dbService.excuteSqls(sqls, out message);
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
                    foreach (var item in userTypeViews)
                    {
                        DeleteCache(o => o.ID == item.ID);
                        if (item.UserType == UserTypeSetting.Administrator
                  || item.UserType == UserTypeSetting.CountryAdmin
                  || item.UserType == UserTypeSetting.LocalAdmin
                  || item.UserType == UserTypeSetting.PersonUser)
                        {
                            UserInfoDao.DeleteCache(o => o.UserID == item.ID);
                            
                        }
                        else if (item.UserType == UserTypeSetting.LocalDepartUser
                   || item.UserType == UserTypeSetting.CountryDepartUser
                   || item.UserType == UserTypeSetting.OrgUser)
                        {
                            OrganizationInfoDao.DeleteCache(o => o.OrgID == item.ID);
                        }
                    }
                }
                List<string> ids=new List<string>();
                 foreach (var item in userTypeViews)
                 {
                       if (item.UserType == UserTypeSetting.Administrator
                  || item.UserType == UserTypeSetting.CountryAdmin
                  || item.UserType == UserTypeSetting.LocalAdmin
                  || item.UserType == UserTypeSetting.PersonUser)
                       {
                           ids.Add(item.ID);
                       }
                 }
                 if (ids.Count > 0)
                 {
                     try
                     {
                         //IopenIMUserInfo.Delete(ids);
                     }
                     finally { }
                 }
            }
            return result;
        }


        public bool SetActivateRefuse(string id)
        {
            bool result = false;
            int count = 0;
            string message = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(this.UserID))
                {
                    throw new MiicCookieArgumentNullException("UserID不能为空，Cookie失效");
                }
                result = dbService.Update<MiicSocialUserInfo>(new MiicSocialUserInfo()
                {
                    ID = id,
                    CreaterID = this.UserID,
                    CreaterName = this.UserName,
                    ActivateFlag = ((int)MiicSimpleApproveStatusSetting.Refuse).ToString(),
                    ActivateTime = DateTime.Now

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
                    DeleteCache(o => o.ID == id);
                }
                try
                {
                    //IopenIMUserInfo.Delete(id);
                }
                finally { }
            }
            return result;
        }

        /// <summary>
        /// 获取最新注册用户
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        DataTable IMiicSocialUser.GetNewestUserList(int top)
        {
            DataTable result = new DataTable();
            string message = string.Empty;
            MiicConditionCollections condition = new MiicConditionCollections(MiicDBLogicSetting.No);
            MiicCondition validCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<SearchUserView, string>(o => o.Valid),
                     ((int)MiicValidTypeSetting.Valid).ToString(),
                     DbType.String,
                     MiicDBOperatorSetting.Equal);
            condition.Add(new MiicConditionLeaf(MiicDBLogicSetting.No, validCondition));
            MiicCondition activateFlagCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<SearchUserView, string>(o => o.ActivateFlag),
                     ((int)UserActivateSetting.Agree).ToString(),
                     DbType.String,
                     MiicDBOperatorSetting.Equal);
            condition.Add(new MiicConditionLeaf(activateFlagCondition));
            MiicCondition disabledFlagCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<SearchUserView, string>(o => o.DisabledFlag),
                      ((int)MiicYesNoSetting.No).ToString(),
                      DbType.String,
                      MiicDBOperatorSetting.Equal);
            condition.Add(new MiicConditionLeaf(disabledFlagCondition));
            List<MiicOrderBy> order = new List<MiicOrderBy>();
            order.Add(new MiicOrderBy()
            {
                PropertyName = Config.Attribute.GetSqlColumnNameByPropertyName<SearchUserView, DateTime?>(o => o.ActivateTime),
                Desc = true
            });
            condition.order = order;
            MiicColumnCollections columns = new MiicColumnCollections(new MiicTop(top));
            MiicColumn column = new MiicColumn(Config.Attribute.GetSqlTableNameByClassName<SearchUserView>());
            columns.Add(column);
            try
            {
                result = dbService.GetInformations<SearchUserView>(columns, condition, out message);
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
        /// 批量设置用户等级
        /// </summary>
        /// <param name="userLevelViews">设置人员级别视图列表</param>
        /// <param name="level">设置等级</param>
        /// <returns>Yes/No</returns>
        public bool SetUsersLevel(List<UserLevelView> userLevelViews, UserLevelSetting level)
        {
            Contract.Requires<ArgumentNullException>(userLevelViews != null && userLevelViews.Count != 0, "参数userLevelViews:不能为空！");
            bool result = false;
            string message = string.Empty;
            string message1 = string.Empty;
            string message2 = string.Empty;
            List<string> sqls = new List<string>();
            try
            {
                foreach (var item in userLevelViews)
                {
                    //更新主表
                    sqls.Add(DBService.UpdateSql(new MiicSocialUserInfo()
                    {
                        ID = item.UserID,
                        Level = ((int)level).ToString()
                    }, out message1));

                    //插入更新记录历史
                    sqls.Add(DBService.InsertSql(new UserLevelUpdateHistory()
                    {
                        ID = Guid.NewGuid().ToString(),
                        EditerID = UserID,
                        EditerName = UserName,
                        EditerIP = HttpContext.Current.Request.UserHostAddress,
                        EditTime = DateTime.Now,
                        OriginalLevel = ((int)item.UserLevel).ToString(),
                        NowLevel = ((int)level).ToString(),
                        UpdatedUserID = item.UserID,
                        UpdatedUserName = item.UserName
                    }, out message2));
                }
                result = dbService.excuteSqls(sqls, out message);
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

        DataTable IMiicSocialUser.DownloadSearch(DownloadSearchView downloadSearchView)
        {
            DataTable result = new DataTable();
            string message = string.Empty;
            MiicConditionCollections condition = downloadSearchView.visitor(this);
            List<MiicOrderBy> order = new List<MiicOrderBy>();
            order.Add(new MiicOrderBy()
            {
                Desc = true,
                PropertyName = Config.Attribute.GetSqlColumnNameByPropertyName<DownloadAllUserView, string>(o => o.UserType)
            });

            order.Add(new MiicOrderBy()
            {
                Desc = true,
                PropertyName = Config.Attribute.GetSqlColumnNameByPropertyName<DownloadAllUserView, int?>(o => o.SortNo)
            });
            condition.order = order;
            try
            {
                if (downloadSearchView.DownloadType == MiicYesNoSetting.Unknown)
                {
                    result = dbService.GetInformations<DownloadAllUserView>(null, order, out message);
                }
                else
                {
                    result = dbService.GetInformations<DownloadAllUserView>(null, condition, out message);
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
    }
}
