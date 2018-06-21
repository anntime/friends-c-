using Miic.Base;
using Miic.Base.Setting;
using Miic.BaseStruct;
using Miic.Cryptographic;
using Miic.Cryptographic.Md5;
using Miic.Cryptographic.SM2;
using Miic.DB.SqlObject;
using Miic.Email;
using Miic.Friends.AddressBook;
using Miic.Friends.Common;
using Miic.Friends.Community;
using Miic.Friends.Group;
using Miic.Log;
using Miic.Manage.Org;
using Miic.Manage.User;
using Miic.Manage.User.Setting;
using Miic.MiicException;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography;
using System.Transactions;
using System.Web;
using System.Web.Configuration;
using System.Web.Script.Services;
using System.Web.Services;
using OAService = cn.com.miic.oa;

/// <summary>
/// 用户服务 包含登录 查询用户信息 修改用户信息等
/// </summary>
[WebService(Namespace = "http://share.miic.com.cn/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[ScriptService]
public class UserService : WebService
{

    private static readonly string ClassName = MethodBase.GetCurrentMethod().DeclaringType.Name;
    private static readonly string NamespaceName = MethodBase.GetCurrentMethod().DeclaringType.Namespace;
    private static readonly string ManageUrl = WebConfigurationManager.AppSettings["ManageUrl"].ToString();
    private readonly IMiicSocialUser ImiicSocialUser = new MiicSocialUserDao(false);
    private static readonly IUserInfo IuserInfo = new UserInfoDao(false);
    public string UserID { get; private set; }
    public string UserName { get; private set; }
    public UserService()
    {
        string message = string.Empty;
        Cookie cookie = new Cookie();
        this.UserID = cookie.GetCookie("MiicID", out message);
        this.UserName = HttpUtility.UrlDecode(cookie.GetCookie("MiicUserName", out message));

    }


    [WebMethod(Description = "用户名唯一性检测", BufferResponse = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public bool UniqueSocialCode(string socialCode)
    {
        return ImiicSocialUser.UniqueSocialCode(socialCode);
    }

    [WebMethod(Description = "邮箱唯一性检测", BufferResponse = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public bool UniqueEmail(string email)
    {
        return ImiicSocialUser.UniqueEmail(email);
    }

    [WebMethod(Description = "手机唯一性检测", BufferResponse = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public bool UniqueMobile(string mobile)
    {
        return ImiicSocialUser.UniqueMobile(mobile);
    }


    [WebMethod(BufferResponse = true, Description = "获取朋友圈注册用户类别列表")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    [GenerateScriptType(typeof(MiicKeyValue))]
    public List<MiicKeyValue> GetRegisterUserTypes()
    {
        List<MiicKeyValue> temp = Config.Attribute.ConvertEnumToList(typeof(UserTypeSetting), e => e.GetDescription());
        temp.RemoveAll(o => (o.Name == ((int)UserTypeSetting.All).ToString()));
        temp.RemoveAll(o => (o.Name == ((int)UserTypeSetting.AllAdminPerson).ToString()));
        temp.RemoveAll(o => (o.Name == ((int)UserTypeSetting.AllAdminDeparter).ToString()));
        temp.RemoveAll(o => (o.Name == ((int)UserTypeSetting.AnonymousUser).ToString()));
        temp.RemoveAll(o => (o.Name == ((int)UserTypeSetting.CountryDepartUser).ToString()));
        temp.RemoveAll(o => (o.Name == ((int)UserTypeSetting.LocalDepartUser).ToString()));
        temp.RemoveAll(o => (o.Name == ((int)UserTypeSetting.OrgUser).ToString()));
        temp.RemoveAll(o => (o.Name == ((int)UserTypeSetting.Administrator).ToString()));
        List<MiicKeyValue> result = temp.OrderByDescending(o => o.Name).ToList<MiicKeyValue>();
        return result;
    }

    //获取RSA公钥
    [WebMethod(MessageName = "GetRSAPublicKey", EnableSession = true, Description = "获取RSA公钥", BufferResponse = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetPublicKey()
    {
        string result = string.Empty;
        RSACryptoServiceProvider rsaKeyGenerator = new RSACryptoServiceProvider(1024);
        string privatekey = privatekey = rsaKeyGenerator.ToXmlString(true);
        //存入私钥
        Context.Session["PrivateKey"] = privatekey;
        RSAParameters publickey = rsaKeyGenerator.ExportParameters(true);
        string publicKeyExponent = Config.Convert.BytesToHexString(publickey.Exponent);
        string publicKeyModulus = Config.Convert.BytesToHexString(publickey.Modulus);
        result = Config.Serializer.Serialize(new { publickeyexponent = publicKeyExponent, publickeymodulus = publicKeyModulus });
        return result;
    }
    [WebMethod(MessageName = "GetSM2EncryptPassword", EnableSession = true, Description = "获取SM2密文", BufferResponse = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetEncryptPassword(string password)
    {
        string result = string.Empty;
        IAsymmetricEncryptionService IasymmetricEncryptionService = Sm2Service.Instance;
        try
        {
            MiicCryptoKeyPair keyPair = IasymmetricEncryptionService.CreateKeyPair();
            result = IasymmetricEncryptionService.Encrypt(keyPair.PublicKey, password);
            Context.Session["PrivateKey"] = keyPair.PrivateKey;
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
    [WebMethod(Description = "获取SM3报文信息", BufferResponse = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetSm3Digest(string id)
    {
        IDigestEncryptionService Iservice = Miic.Cryptographic.SM3.Sm3Service.Instance;
        IAsymmetricEncryptionService IasymmetricEncryptionService = Sm2Service.Instance;
        string result = Iservice.Encrypt(id);
        return result;
    }

    [WebMethod(Description = "登录", EnableSession = true, BufferResponse = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    [GenerateScriptType(typeof(LoginRequestView))]
    [GenerateScriptType(typeof(LoginResponseView))]
    public LoginResponseView Login(LoginRequestView loginView)
    {
        LoginResponseView result = ImiicSocialUser.Login(loginView, UserLoginTypeSetting.Friends);
        //如果登录成功，清除session
        if (result.IsLogin == true)
        {
            //登录成功，清楚私钥SESSION
            Context.Session.Remove("PrivateKey");
        }
        return result;
    }

    [WebMethod(Description = "通过邮箱找回密码", BufferResponse = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public bool FindPasswordByEmail(string myEmail)
    {
        if (string.IsNullOrEmpty(myEmail) == true)
        {
            throw new ArgumentNullException("myEmail", "参数myEmail：不能为空！");
        }
        bool result = ImiicSocialUser.FindPassword(new FindPasswordView()
        {
            Type = MiicGetBackTypeSetting.Email,
            Value = myEmail,
            LoginType = UserLoginTypeSetting.Friends
        });


        return result;
    }

    [WebMethod(BufferResponse = true, Description = "验证密码")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    [GenerateScriptType(typeof(PasswordView))]
    public bool CheckPassword(PasswordView passwordView)
    {
        bool result = false;
        if (!string.IsNullOrEmpty(UserID))
        {
            MiicSocialUserInfo socialUserInfo = ImiicSocialUser.GetInformation(UserID);
            if (socialUserInfo.Password != passwordView.OldPassword)
            {
                result = false;
            }
            else
            {
                result = true;
            }
        }
        else
        {
            throw new MiicCookieArgumentNullException("UserID失效！");
        }
        return result;
    }


    [WebMethod(BufferResponse = true, Description = "修改密码")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    [GenerateScriptType(typeof(PasswordView))]
    public string ModifyPassword(PasswordView passwordView, bool isEmail)
    {
        string result = string.Empty;
        string MailType = "Miic.Config.email.xml";
        string message = string.Empty;
        try
        {
            if (!string.IsNullOrEmpty(passwordView.NewPassword))
            {
                if (!string.IsNullOrEmpty(UserID))
                {
                    MiicSocialUserInfo miicSocialUserInfo = ImiicSocialUser.GetInformation(UserID);
                    miicSocialUserInfo.Password = passwordView.NewPassword;
                    //添加MD5加密密码
                    miicSocialUserInfo.SM3Password = passwordView.Sm3;
                    bool temp = ImiicSocialUser.Update(new MiicSocialUserInfo()
                    {
                        ID = miicSocialUserInfo.ID,
                        Password = miicSocialUserInfo.Password,
                        SM3Password = miicSocialUserInfo.SM3Password
                    });


                    OAService.UserService service = new OAService.UserService();
                    IDigestEncryptionService IdigestEncryptionService = Md5Service.Instance;
                    bool synchroOa = service.SynchroUserInfo(new OAService.UserInfo()
                    {
                        ID = miicSocialUserInfo.ID,
                        Password = miicSocialUserInfo.Password,
                        Md5 = IdigestEncryptionService.Encrypt(IdigestEncryptionService.Encrypt(miicSocialUserInfo.Password))
                    });

                    if (temp == true && synchroOa == true)
                    {
                        if (isEmail == true)
                        {
                            bool tempEmail = false;
                            string content = "尊敬的用户" + miicSocialUserInfo.SocialCode + "您好：您的" + Config.AppName + "账户密码已经修改，账号为：" + miicSocialUserInfo.SocialCode + "；新密码为：" + miicSocialUserInfo.Password + "，感谢您的支持，谢谢！";
                            MiicEmail email = new MiicEmail(Config.AppName, "rhadamanthys0407@126.com", miicSocialUserInfo.Email, "找回密码", content);
                            email.Priority = MailPriority.High;
                            Reflection reflection = new Reflection();
                            Assembly a = Assembly.LoadFrom(HttpContext.Current.Server.MapPath("/Bin/MiicLibrary.dll"));
                            Stream stream = a.GetManifestResourceStream(MailType);
                            IEmailService Iemail = reflection.initInterface<IEmailService>(stream, "01");
                            tempEmail = Iemail.SendMail(email, out message);
                            if (tempEmail == true)
                            {
                                result = Config.Serializer.Serialize(new { result = true, message = "您的密码修改成功，请查阅您的Email" });
                            }
                            else
                            {

                                result = Config.Serializer.Serialize(new { result = true, message = "您的密码修改成功，Email发送失败" });
                            }
                        }
                        else
                        {
                            result = Config.Serializer.Serialize(new { result = true, message = "您的密码修改成功" });
                        }
                    }
                    else
                    {
                        result = Config.Serializer.Serialize(new { result = false, message = "您的密码修改失败" });
                    }
                }
                else
                {
                    throw new MiicCookieArgumentNullException("UserID失效！");
                }
            }
            else
            {
                throw new ArgumentNullException("新密码为空！");
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

    [WebMethod(BufferResponse = true, Description = "获取某人的统计信息")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public List<MiicKeyValue> GetPersonStatisticsCount(string userID)
    {
        Miic.Friends.User.IUserInfo IuserInfo = new Miic.Friends.User.UserInfoDao();
        return IuserInfo.GetPersonStatisticsCount(userID);
    }
    [WebMethod(BufferResponse = true, Description = "搜索用户信息(添加好友使用)")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    [GenerateScriptType(typeof(MyKeywordView))]
    [GenerateScriptType(typeof(MiicPage))]
    public string SearchFriends(MyKeywordView keywordView, MiicPage page)
    {
        string result = CommonService.InitialJsonList;
        Miic.Friends.User.IUserInfo IuserInfo = new Miic.Friends.User.UserInfoDao();
        DataTable dt = IuserInfo.Search(keywordView, page);
        if (dt.Rows.Count > 0)
        {
            var temp = from dr in dt.AsEnumerable()
                       select new
                       {
                           ID = dr[Config.Attribute.GetSqlColumnNameByPropertyName<Miic.Friends.User.SimpleUserView, string>(o => o.UserID)],
                           UserType = dr[Config.Attribute.GetSqlColumnNameByPropertyName<Miic.Friends.User.SimpleUserView, string>(o => o.UserType)],
                           UserName = dr[Config.Attribute.GetSqlColumnNameByPropertyName<Miic.Friends.User.SimpleUserView, string>(o => o.UserName)],
                           RealName = dr[Config.Attribute.GetSqlColumnNameByPropertyName<Miic.Friends.User.SimpleUserView, string>(o => o.RealName)],
                           Sex = dr[Config.Attribute.GetSqlColumnNameByPropertyName<Miic.Friends.User.SimpleUserView, string>(o => o.Sex)],
                           OrgName = dr[Config.Attribute.GetSqlColumnNameByPropertyName<Miic.Friends.User.SimpleUserView, string>(o => o.OrgName)],
                           UserUrl = CommonService.GetManageFullUrl(dr[Config.Attribute.GetSqlColumnNameByPropertyName<Miic.Friends.User.SimpleUserView, string>(o => o.UserUrl)].ToString()),
                           IsMyFriend = Convert.IsDBNull(dr["ADDRESS_BOOK_" + Config.Attribute.GetSqlColumnNameByPropertyName<AddressBookInfo, string>(o => o.ID)]) == false || (!string.IsNullOrEmpty(this.UserID) && dr[Config.Attribute.GetSqlColumnNameByPropertyName<Miic.Friends.User.SimpleUserView, string>(o => o.UserID)].ToString() == this.UserID) ? true : false
                       };
            result = Config.Serializer.Serialize(temp);
        }
        return result;
    }
    [WebMethod(BufferResponse = true, Description = "搜索用户数(添加好友使用)")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    [GenerateScriptType(typeof(MyKeywordView))]
    public int GetSearchFriendsCount(MyKeywordView keywordView)
    {
        Miic.Friends.User.IUserInfo IuserInfo = new Miic.Friends.User.UserInfoDao();
        return IuserInfo.GetSearchCount(keywordView);
    }
    [WebMethod(BufferResponse = true, Description = "获取我的讨论组/圈子/通讯录数")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    [GenerateScriptType(typeof(MiicKeyValue))]
    public List<MiicKeyValue> GetMyShareCount()
    {
        List<MiicKeyValue> result = new List<MiicKeyValue>();
        IAddressBookInfo IaddressBookInfo = new AddressBookInfoDao();
        IGroupInfo IgroupInfo = new GroupInfoDao();
        ICommunityInfo IcommunityInfo = new CommunityInfoDao();
        result.Add(new MiicKeyValue()
        {
            Name = "AddressBookCount",
            Value = IaddressBookInfo.GetPersonAllAddressBookCount(this.UserID)
        });
        result.Add(new MiicKeyValue()
        {
            Name = "GroupCount",
            Value = IgroupInfo.GetPersonAllGroupCount(this.UserID)
        });
        result.Add(new MiicKeyValue()
        {
            Name = "CommunityCount",
            Value = IcommunityInfo.GetPersonAllCommunityCount(this.UserID)
        });
        return result;
    }
  
}
