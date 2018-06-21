using Miic.Base;
using Miic.Base.Setting;
using Miic.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Top.Api;
using Top.Api.Domain;
using Top.Api.Request;
using Top.Api.Response;
using System.Web.Configuration; 
using Miic.IM.User;
namespace Miic.IM.User1
{
    public class OpenUserInfoService : IOpenIMUserInfo
    {
        private static readonly string ClassName = MethodBase.GetCurrentMethod().DeclaringType.Name;
        private static readonly string NamespaceName = MethodBase.GetCurrentMethod().DeclaringType.Namespace;
        public static readonly string AppKey;
        public static readonly string AppSecret;
        public static readonly string Url = "http://gw.api.taobao.com/router/rest";
        private static readonly ITopClient client ;
        static OpenUserInfoService() 
        {
             
            if (WebConfigurationManager.AppSettings["OpenIMAppKey"] != null && WebConfigurationManager.AppSettings["OpenIMAppSecret"] != null)
            {
                AppKey = WebConfigurationManager.AppSettings["OpenIMAppKey"].ToString();
                AppSecret = WebConfigurationManager.AppSettings["OpenIMAppSecret"].ToString();
            }
            else 
            {
                throw new Exception("OpenIMAppKey或OpenIMAppSecret没有进行配置");
            }
            client = new DefaultTopClient(Url, AppKey, AppSecret);
        }
        OpenIMUserInfo IOpenIMUserInfo.GetUserInfo(string userID)
        {
            OpenIMUserInfo result = null;
            OpenimUsersGetRequest request = new OpenimUsersGetRequest();
            request.Userids = userID;
            try
            {
                OpenimUsersGetResponse response = client.Execute(request);
                if (response.IsError == true)
                {
                    throw new OpenIMException(new OpenIMExceptionMessage()
                    {
                        ClassName = ClassName,
                        NamespaceName = NamespaceName,
                        MethodName = MethodBase.GetCurrentMethod().Name,
                        ErrCode = response.ErrCode,
                        ErrMessage = response.ErrMsg,
                        SubErrCode = response.SubErrCode,
                        SubMessage = response.SubErrMsg
                    });
                    
                }
                else
                {
                    Userinfos userInfo = response.Userinfos[0];
                    result = new OpenIMUserInfo()
                    {
                        Address = userInfo.Address,
                        Age = (int)userInfo.Age,
                        UpdateTime = DateTime.Parse(userInfo.GmtModified),
                        Career = userInfo.Career,
                        Email = userInfo.Email,
                        Extra = userInfo.Extra,
                        Logo = userInfo.IconUrl,
                        Mobile = userInfo.Mobile,
                        NickName = userInfo.Nick,
                        UserID = userInfo.Userid,
                        QQ = userInfo.Qq,
                        RealName = userInfo.Name,
                        Remark = userInfo.Remark,
                        Sex = userInfo.Gender == "F" ? MiicSexSetting.Female : MiicSexSetting.Male,
                        Status = (OpenIMUserActivationSetting)(int)userInfo.Status,
                        TaobaoID = userInfo.Taobaoid,
                        Vip = userInfo.Vip,
                        Weibo = userInfo.Weibo,
                        Weixin = userInfo.Wechat
                    };
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
        bool IOpenIMUserInfo.Inserts(List<OpenIMUserInfo> userInfos)
        {
            bool result = false;
            OpenimUsersAddRequest request = new OpenimUsersAddRequest();
            List<Userinfos> list = new List<Userinfos>();
            foreach (var item in userInfos.AsEnumerable())
            {
                Userinfos user_item = new Userinfos()
                {
                    Address = item.Address,
                    Age = item.Age,
                    Career = item.Career,
                    Email = item.Email,
                    Extra = item.Extra,
                    Gender = (item.Sex == MiicSexSetting.Female ? "F" : "M"),
                    IconUrl = item.Logo,
                    Mobile = item.Mobile,
                    Name = item.RealName,
                    Nick = item.NickName,
                    Password = item.Password,
                    Qq = item.QQ,
                    Remark = item.Remark,
                    Status =(long)(int)item.Status,
                    Userid = item.UserID,
                    Taobaoid = item.TaobaoID,
                    Vip = item.Vip,
                    Wechat = item.Weixin,
                    Weibo = item.Weibo,
                    GmtModified = item.UpdateTime.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss")
                };
                Userinfos user = user_item;
                list.Add(user);
            }
            request.Userinfos_ = list;
            try
            {
                OpenimUsersAddResponse response = client.Execute(request);
                if (response.UidSucc.Count == userInfos.Count)
                {
                    result = true;
                }
                else
                {
                    ((IOpenIMUserInfo)this).Delete(response.UidSucc);
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
        bool IOpenIMUserInfo.Insert(OpenIMUserInfo userInfo)
        {
            List<OpenIMUserInfo> users = new List<OpenIMUserInfo>();
            users.Add(userInfo);
            return ((IOpenIMUserInfo)this).Inserts(users);
        }

        bool IOpenIMUserInfo.Update(OpenIMUserInfo userInfo)
        {
            bool result = false;
            OpenimUsersUpdateRequest request = new OpenimUsersUpdateRequest();

            List<Userinfos> list = new List<Userinfos>();
            Userinfos user_item = new Userinfos()
            {
                Address = userInfo.Address,
                Age = userInfo.Age,
                Career = userInfo.Career,
                Email = userInfo.Email,
                Extra = userInfo.Extra,
                Gender = (userInfo.Sex == MiicSexSetting.Female ? "F" : "M"),
                IconUrl = userInfo.Logo,
                Mobile = userInfo.Mobile,
                Name = userInfo.RealName,
                Nick = userInfo.NickName,
                Password = userInfo.Password,
                Qq = userInfo.QQ,
                Remark = userInfo.Remark,
                Status = (long)(int)userInfo.Status,
                Userid = userInfo.UserID,
                Taobaoid = userInfo.TaobaoID,
                Vip = userInfo.Vip,
                Wechat = userInfo.Weixin,
                Weibo = userInfo.Weibo,
                GmtModified = userInfo.UpdateTime.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss")
            };
            list.Add(user_item);
            request.Userinfos_ = list;
            try
            {
                OpenimUsersUpdateResponse response = client.Execute(request);
                if (response.UidSucc.Count == 1)
                {
                    result = true;
                }
                else
                {
                    throw new OpenIMException(new OpenIMExceptionMessage()
                    {
                        ClassName = ClassName,
                        NamespaceName = NamespaceName,
                        MethodName = MethodBase.GetCurrentMethod().Name,
                        ErrCode = response.ErrCode,
                        ErrMessage = response.ErrMsg,
                        SubErrCode = response.SubErrCode,
                        SubMessage = response.SubErrMsg
                    });
                    
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

        bool IOpenIMUserInfo.Delete(string userID)
        {
            List<string> userIDs = new List<string>();
            userIDs.Add(userID);
            return ((IOpenIMUserInfo)this).Delete(userIDs);
        }

        bool IOpenIMUserInfo.Delete(List<string> userIDs)
        {
            bool result = false;
            OpenimUsersDeleteRequest request = new OpenimUsersDeleteRequest();
            request.Userids = string.Join(",", userIDs.ToArray());
            try
            {
                OpenimUsersDeleteResponse response = client.Execute(request);
                if (response.IsError == false)
                {
                    result = true;
                }
                else
                {
                    throw new OpenIMException(new OpenIMExceptionMessage()
                    {
                        ClassName = ClassName,
                        NamespaceName = NamespaceName,
                        MethodName = MethodBase.GetCurrentMethod().Name,
                        ErrCode = response.ErrCode,
                        ErrMessage = response.ErrMsg,
                        SubErrCode = response.SubErrCode,
                        SubMessage = response.SubErrMsg
                    });
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

        internal bool Inserts(List<OpenIMUserInfo> openImUserInfoList)
        {
            bool result = false;
            OpenimUsersAddRequest request = new OpenimUsersAddRequest();
            List<Userinfos> list = new List<Userinfos>();
            foreach (var item in openImUserInfoList.AsEnumerable())
            {
                Userinfos user_item = new Userinfos()
                {
                    Address = item.Address,
                    Age = item.Age,
                    Career = item.Career,
                    Email = item.Email,
                    Extra = item.Extra,
                    Gender = (item.Sex == MiicSexSetting.Female ? "F" : "M"),
                    IconUrl = item.Logo,
                    Mobile = item.Mobile,
                    Name = item.RealName,
                    Nick = item.NickName,
                    Password = item.Password,
                    Qq = item.QQ,
                    Remark = item.Remark,
                    Status = (long)(int)item.Status,
                    Userid = item.UserID,
                    Taobaoid = item.TaobaoID,
                    Vip = item.Vip,
                    Wechat = item.Weixin,
                    Weibo = item.Weibo,
                    GmtModified = item.UpdateTime.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss")
                };
                Userinfos user = user_item;
                list.Add(user);
            }
            request.Userinfos_ = list;
            try
            {
                OpenimUsersAddResponse response = client.Execute(request);
                if (response.UidSucc.Count == openImUserInfoList.Count)
                {
                    result = true;
                }
                else
                {
                    for (var item = 0; item < response.FailMsg.Count;item++ )
                    {
                        if (response.FailMsg[item] == "data exist")
                        {
                            ((IOpenIMUserInfo)this).Delete(response.UidFail[item]);
                        }
                        else {
                            ((IOpenIMUserInfo)this).Delete(response.UidSucc[item]);
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
            return result;
        }

        internal bool Update(OpenIMUserInfo openIMUserInfo)
        {
            throw new NotImplementedException();
        }

        internal bool Delete(List<string> userIDs)
        {
            bool result = false;
            OpenimUsersDeleteRequest request = new OpenimUsersDeleteRequest();
            request.Userids = string.Join(",", userIDs.ToArray());
            try
            {
                OpenimUsersDeleteResponse response = client.Execute(request);
                if (response.IsError == false)
                {
                    result = true;
                }
                else
                {
                    throw new OpenIMException(new OpenIMExceptionMessage()
                    {
                        ClassName = ClassName,
                        NamespaceName = NamespaceName,
                        MethodName = MethodBase.GetCurrentMethod().Name,
                        ErrCode = response.ErrCode,
                        ErrMessage = response.ErrMsg,
                        SubErrCode = response.SubErrCode,
                        SubMessage = response.SubErrMsg
                    });
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
        internal bool Delete(string userID)
        {
            List<string> userIDs = new List<string>();
            userIDs.Add(userID);
            return ((IOpenIMUserInfo)this).Delete(userIDs);
        }
        internal bool Insert(OpenIMUserInfo openIMUserInfo)
        {
            
            List<OpenIMUserInfo> users = new List<OpenIMUserInfo>();
            users.Add(openIMUserInfo);
            return ((IOpenIMUserInfo)this).Inserts(users); 
        }
        internal OpenIMUserInfo GetUserInfo(string userID)
        {
            OpenIMUserInfo result = null;
            OpenimUsersGetRequest request = new OpenimUsersGetRequest();
            request.Userids = userID;
            try
            {
                OpenimUsersGetResponse response = client.Execute(request);
                if (response.IsError == true)
                {
                    throw new OpenIMException(new OpenIMExceptionMessage()
                    {
                        ClassName = ClassName,
                        NamespaceName = NamespaceName,
                        MethodName = MethodBase.GetCurrentMethod().Name,
                        ErrCode = response.ErrCode,
                        ErrMessage = response.ErrMsg,
                        SubErrCode = response.SubErrCode,
                        SubMessage = response.SubErrMsg
                    });

                }
                else
                {
                    Userinfos userInfo = response.Userinfos[0];
                    result = new OpenIMUserInfo()
                    {
                        Address = userInfo.Address,
                        Age = (int)userInfo.Age,
                        UpdateTime = DateTime.Parse(userInfo.GmtModified),
                        Career = userInfo.Career,
                        Email = userInfo.Email,
                        Extra = userInfo.Extra,
                        Logo = userInfo.IconUrl,
                        Mobile = userInfo.Mobile,
                        NickName = userInfo.Nick,
                        UserID = userInfo.Userid,
                        QQ = userInfo.Qq,
                        RealName = userInfo.Name,
                        Remark = userInfo.Remark,
                        Sex = userInfo.Gender == "F" ? MiicSexSetting.Female : MiicSexSetting.Male,
                        Status = (OpenIMUserActivationSetting)(int)userInfo.Status,
                        TaobaoID = userInfo.Taobaoid,
                        Vip = userInfo.Vip,
                        Weibo = userInfo.Weibo,
                        Weixin = userInfo.Wechat
                    };
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
