using Miic.Base;
using Miic.Base.Setting;
using Miic.DB.SqlObject;
using Miic.Friends.Group;
using Miic.Friends.SimpleGroup;
using Miic.IM.Addressbook.Message;
using Miic.IM.Tribe.Group;
using Miic.IM.Tribe.Message;
using Miic.Log;
using Miic.Manage.User;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using Top.Api;
using Top.Api.Domain;
using Top.Api.Response;

/// <summary>
/// 讨论组服务
/// </summary>
[WebService(Namespace = "http://share.miic.com.cn/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[ScriptService]
public partial class GroupService : WebService
{
    private static readonly string ClassName = MethodBase.GetCurrentMethod().DeclaringType.Name;
    private static readonly string NamespaceName = MethodBase.GetCurrentMethod().DeclaringType.Namespace;
    private static readonly IGroupInfo IgroupInfo = new GroupInfoDao();
    //private static readonly IOpenIMGroupInfo IopenIMGroupInfo = new OpenGroupInfoService();
    public string UserID { get; private set; }
    public string UserName { get; private set; }
    public GroupService()
    {
        string message = string.Empty;
        Cookie cookie = new Cookie();
        this.UserID = cookie.GetCookie("MiicID", out message);
        this.UserName = HttpUtility.UrlDecode(cookie.GetCookie("MiicUserName", out message));
    }

    [WebMethod(Description = "添加讨论组", BufferResponse = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    [GenerateScriptType(typeof(GroupInfo))]
    [GenerateScriptType(typeof(GroupMember))]
    public bool Add(GroupInfo groupInfo, List<GroupMember> members)
    {
        bool result = false;
        groupInfo.CreaterID = this.UserID;
        groupInfo.CreaterName = this.UserName;
        groupInfo.CreateTime = DateTime.Now;
        groupInfo.Valid = ((int)MiicValidTypeSetting.Valid).ToString();
        if (members != null)
        {
            groupInfo.MemberCount = members.Count + 1;
        }
        else
        {
            groupInfo.MemberCount = 1;
        }
        if (members != null)
        {
            foreach (GroupMember item in members)
            {
                item.JoinTime = DateTime.Now;
            }
            result = IgroupInfo.Insert(groupInfo, members);
        }
        else
        {
            result = IgroupInfo.Insert(groupInfo);
        }
        return result;
    }
    [WebMethod(Description = "移除讨论组", BufferResponse = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public bool Remove(string groupID)
    {
        return ((ICommon<GroupInfo>)IgroupInfo).Delete(groupID);
    }


    [WebMethod(Description = "搜索我的讨论组", BufferResponse = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    [GenerateScriptType(typeof(MySimpleGroupSearchView))]
    [GenerateScriptType(typeof(MiicPage))]
    public string Search(MySimpleGroupSearchView searchView, MiicPage page)
    {
        string result = CommonService.InitialJsonList;
        DataTable dt = IgroupInfo.Search(searchView, page);
        if (dt.Rows.Count > 0)
        {
            var temp = from dr in dt.AsEnumerable()
                       select new
                       {
                           ID = dr[Config.Attribute.GetSqlColumnNameByPropertyName<GroupInfo, string>(o => o.ID)],
                           Name = dr[Config.Attribute.GetSqlColumnNameByPropertyName<GroupInfo, string>(o => o.Name)],
                           LogoUrl = dr[Config.Attribute.GetSqlColumnNameByPropertyName<GroupInfo, string>(o => o.LogoUrl)],
                           MemberCount = dr[Config.Attribute.GetSqlColumnNameByPropertyName<GroupInfo, int?>(o => o.MemberCount)],
                           CreaterID = dr[Config.Attribute.GetSqlColumnNameByPropertyName<GroupInfo, string>(o => o.CreaterID)],
                           CreaterName = dr[Config.Attribute.GetSqlColumnNameByPropertyName<GroupInfo, string>(o => o.CreaterName)],
                           CreateTime = dr[Config.Attribute.GetSqlColumnNameByPropertyName<GroupInfo, DateTime?>(o => o.CreateTime)],
                           GroupMemberID = dr["GroupMemberID"],
                           Remark = dr[Config.Attribute.GetSqlColumnNameByPropertyName<GroupMember, string>(o => o.Remark)].ToString()
                       };
            result = Config.Serializer.Serialize(temp);
        }
        return result;
    }
    [WebMethod(Description = "搜索我的讨论组数", BufferResponse = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    [GenerateScriptType(typeof(MySimpleGroupSearchView))]
    public int GetSearchCount(MySimpleGroupSearchView searchView)
    {
        return IgroupInfo.GetSearchCount(searchView);
    }
    [WebMethod(Description = "展示我的讨论组列表（右侧展示）", BufferResponse = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string ShowMyGroupInfoList(int top)
    {
        string result = CommonService.InitialJsonList;
        DataTable dt = IgroupInfo.GetGroupInfoList(this.UserID, top);
        if (dt.Rows.Count > 0)
        {
            var temp = from dr in dt.AsEnumerable()
                       select new
                       {
                           ID = dr[Config.Attribute.GetSqlColumnNameByPropertyName<GroupInfo, string>(o => o.ID)],
                           Name = dr[Config.Attribute.GetSqlColumnNameByPropertyName<GroupInfo, string>(o => o.Name)],
                           LogoUrl = dr[Config.Attribute.GetSqlColumnNameByPropertyName<GroupInfo, string>(o => o.LogoUrl)],
                           Remark = dr[Config.Attribute.GetSqlColumnNameByPropertyName<GroupMember, string>(o => o.Remark)].ToString()
                       };
            result = Config.Serializer.Serialize(temp);
        }
        return result;
    }
    [WebMethod(Description = "搜索我的最新动态讨论组", BufferResponse = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    [GenerateScriptType(typeof(MySimpleGroupSearchView))]
    [GenerateScriptType(typeof(MiicPage))]
    public string TrendsSearch(MySimpleGroupSearchView searchView, MiicPage page)
    {
        string result = CommonService.InitialJsonList;
        DataTable dt = IgroupInfo.TrendsSearch(searchView, page);
        if (dt.Rows.Count > 0)
        {
            var temp = from dr in dt.AsEnumerable()
                       group dr by new
                       {
                           ID = dr[Config.Attribute.GetSqlColumnNameByPropertyName<GroupInfo, string>(o => o.ID)],
                           LogoUrl = dr[Config.Attribute.GetSqlColumnNameByPropertyName<GroupInfo, string>(o => o.LogoUrl)],
                           Name = dr[Config.Attribute.GetSqlColumnNameByPropertyName<GroupInfo, string>(o => o.Name)],
                           MemberCount = dr[Config.Attribute.GetSqlColumnNameByPropertyName<GroupInfo, int?>(o => o.MemberCount)],
                           Manager = dr[Config.Attribute.GetSqlColumnNameByPropertyName<GroupInfo, string>(o => o.CreaterID)]
                       } into g
                       select new
                       {
                           ID = g.Key.ID,
                           LogoUrl = g.Key.LogoUrl,
                           Name = g.Key.Name,
                           MemberCount = g.Key.MemberCount,
                           Manager = g.Key.Manager,
                           TopicInfo = (from item in g.AsEnumerable()
                                        select new
                                        {
                                            Content = item[Config.Attribute.GetSqlColumnNameByPropertyName<TopicShowInfo, string>(o => o.TopicContent)],
                                            CreaterID = item["TopicCreaterID"],
                                            CreaterName = item["TopicCreaterName"],
                                            RealName = item["REAL_NAME"],
                                            IsFriend = item["IS_FRIEND"],
                                            CreaterUrl = CommonService.GetManageFullUrl(item[Config.Attribute.GetSqlColumnNameByPropertyName<TopicShowInfo, string>(o => o.CreaterUrl)].ToString()),
                                            CreateTime = item["TopicCreateTime"],
                                            MessageCount = item[Config.Attribute.GetSqlColumnNameByPropertyName<TopicShowInfo, int?>(o => o.MessageCount)]
                                        }).Take(10)

                       };
            result = Config.Serializer.Serialize(temp);
        }
        return result;
    }
    [WebMethod(Description = "搜索我的最新动态讨论组数", BufferResponse = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    [GenerateScriptType(typeof(MySimpleGroupSearchView))]
    public int GetTrendsSearchCount(MySimpleGroupSearchView searchView)
    {
        return IgroupInfo.GetTrendsSearchCount(searchView);
    }
    [WebMethod(Description = "是否为创建者", BufferResponse = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public bool IsCreater(string userID, string groupID)
    {
        return IgroupInfo.IsCreater(userID, groupID);
    }

    [WebMethod(BufferResponse = true, Description = "获取详细信息")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetGroupInfoByTopicID(string topicID)
    {
        string result = CommonService.InitialJsonList;
        DataTable dt = IgroupInfo.GetDetailGroupByTopicID(topicID);
        var temp = from dr in dt.AsEnumerable()
                   select new
                    {
                        ID = dr[Config.Attribute.GetSqlColumnNameByPropertyName<GroupInfo, string>(o => o.ID)],
                        LogoUrl = dr[Config.Attribute.GetSqlColumnNameByPropertyName<GroupInfo, string>(o => o.LogoUrl)],
                        Name = dr[Config.Attribute.GetSqlColumnNameByPropertyName<GroupInfo, string>(o => o.Name)],
                        MemberCount = dr[Config.Attribute.GetSqlColumnNameByPropertyName<GroupInfo, int?>(o => o.MemberCount)]
                    };

        result = Config.Serializer.Serialize(temp);
        return result;
    }
    [WebMethod(BufferResponse = true, Description = "获取我的所有讨论组列表信息")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetMyAllGroupList()
    {
        if (string.IsNullOrEmpty(this.UserID))
        {
            throw new ArgumentNullException("UserID", "UserID不能为空");
        }
        string result = CommonService.InitialJsonList;
        DataTable dt = IgroupInfo.GetPersonAllGroupList(this.UserID);
        if (dt.Rows.Count > 0)
        {
            var temp = from dr in dt.AsEnumerable()
                       select new
                       {
                           ID = dr[Config.Attribute.GetSqlColumnNameByPropertyName<GroupInfo, string>(o => o.ID)],
                           Name = dr[Config.Attribute.GetSqlColumnNameByPropertyName<GroupInfo, string>(o => o.Name)],
                           LogoUrl = dr[Config.Attribute.GetSqlColumnNameByPropertyName<GroupInfo, string>(o => o.LogoUrl)],
                           Remark = dr[Config.Attribute.GetSqlColumnNameByPropertyName<GroupMember, string>(o => o.Remark)].ToString(),
                           TribeID = dr[Config.Attribute.GetSqlColumnNameByPropertyName<GroupInfo, string>(o => o.TribeID)]
                       };
            result = Config.Serializer.Serialize(temp);
        }
        return result;
    }
    [WebMethod(Description = "获取讨论组的聊天记录", BufferResponse = true)]
    [GenerateScriptType(typeof(OpenIMTribeChatLogsResult))]
    [GenerateScriptType(typeof(OpenIMChatLogsTribeRequestInfo))]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public OpenIMTribeChatLogsResult GetChatLogs(OpenIMChatLogsTribeRequestInfo chatLogsRequestInfo)
    {
        chatLogsRequestInfo = Config.Attribute.ConvertObjectWithDateTime<OpenIMChatLogsTribeRequestInfo>(chatLogsRequestInfo);
        chatLogsRequestInfo.EndTime = chatLogsRequestInfo.EndTime.Value.AddHours(1);
        //OpenimTribelogsGetResponse.TribeMessageResultDomain chatLogs = IopenIMGroupInfo.GetChatLogs(chatLogsRequestInfo);
        //chatLogs.Messages = chatLogs.Messages.OrderBy(o => o.Time).ToList<OpenimTribelogsGetResponse.TribeMessageDomain>();
        OpenIMTribeChatLogsResult result = new OpenIMTribeChatLogsResult();
        //result.NextKey = chatLogs.NextKey;
        result.Messages = new List<TribeChatLogsMessageItem>();
        List<string> fromids = new List<string>();
        //foreach (var item in chatLogs.Messages)
        //{
           
        //    fromids.Add(item.FromId.Uid);
        //    result.Messages.Add(new TribeChatLogsMessageItem()
        //    {
        //        Content = item.Content,
        //        FromUserID = item.FromId.Uid,
        //        Time = item.Time,
        //        Type = item.Type,
        //        Uuid = item.Uuid
        //    });
        //}

        IUserInfo IuserInfo = new UserInfoDao(false);
        List<SimplePersonUserView> persons = IuserInfo.GetSimplePersonUserInfos(fromids);
       
        foreach (var item in result.Messages)
        {
            foreach (SimplePersonUserView sub in persons.AsEnumerable())
            {
                if (item.FromUserID.ToUpper() == sub.UserID.ToUpper())
                {
                    item.FromUserName = sub.UserName;
                    item.FromUserType = sub.UserType;
                    item.FromUserUrl = CommonService.GetManageFullUrl(sub.UserUrl);
                    item.FromUserLevel = sub.UserLevel;
                }
            }
        }
        return result;
    }
    [WebMethod(Description = "将小组成员依次导入阿里服务器", BufferResponse = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)] 
    public bool CopyGroupUserInfoToAliService(string groupID)
    {
        bool result = IgroupInfo.CopyGroupUserInfoToAliService(groupID); ;
        return result;
    } 
     
}
