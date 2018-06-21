using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Miic.Base;
using Miic.Collections;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading.Tasks;
using Miic.Friends.Notice;
using Miic.BaseStruct;
using System.Data;
using Miic.Log;
using System.Reflection;
using Miic.Friends.AddressBook;

[HubName("IMService")]
public class ShareHub : Hub
{
    private static readonly string ClassName = MethodBase.GetCurrentMethod().DeclaringType.Name;
    private static readonly string NamespaceName = MethodBase.GetCurrentMethod().DeclaringType.Namespace;
    private static  MiicSafeCollection<string> users = new MiicSafeCollection<string>();
    private static readonly INoticeInfo<NoticeInfo> InoticeInfo = new NoticeInfoDao();
    private static readonly IAddressBookInfo IaddressBookInfo = new AddressBookInfoDao();
    public ShareHub()
    {
        Console.WriteLine("IMService已启动");
        Config.IlogicLogService.Write(new LogicLog()
        {
            AppName = Config.AppName,
            ClassName = ClassName,
            NamespaceName = NamespaceName,
            MethodName = MethodBase.GetCurrentMethod().Name,
            Message = "IMService已启动,目前用户数为："+users.Count,
            Oper = "system"
        });
    }
    public override Task OnConnected()
    {
        if (users.Contains(Context.RequestCookies["MiicID"].Value) == false)
        {
            users.Add(Context.RequestCookies["MiicID"].Value);
            Config.IlogicLogService.Write(new LogicLog()
            {
                AppName = Config.AppName,
                ClassName = ClassName,
                NamespaceName = NamespaceName,
                MethodName = MethodBase.GetCurrentMethod().Name,
                Message ="目前用户数为："+users.Count+",加入："+ Context.RequestCookies["MiicID"].Value,
                Oper = "system"
            });
        }

        return base.OnConnected();
    }
   
    public override Task OnReconnected()
    {
        if (users.Contains(Context.RequestCookies["MiicID"].Value) == false)
        {
            users.Add(Context.RequestCookies["MiicID"].Value);
        }
        return base.OnReconnected();
    }

    public override Task OnDisconnected(bool stopCalled)
    {
        if (stopCalled)
        {
            Console.WriteLine(String.Format("Client {0} explicitly closed the connection.", Context.ConnectionId));
        }
        else
        {
            Console.WriteLine(String.Format("Client {0} timed out .", Context.ConnectionId));
        }
        users.Remove(Context.RequestCookies["MiicID"].Value);
        return base.OnDisconnected(stopCalled);
    }
     [HubMethodName("SendLine")]
    public async Task SendLine() 
    {
       
         DataTable dt = IaddressBookInfo.GetPersonAllAddressBookList(Context.RequestCookies["MiicID"].Value);
         List<string> temp = new List<string>();
         foreach(var dr in dt.AsEnumerable())
         {
             foreach(var user in users.AsEnumerable())
             {
                  if(dr[Config.Attribute.GetSqlColumnNameByPropertyName<AddressBookInfo, string>(o => o.AddresserID)].ToString()==user)
                  {
                      temp.Add(user);
                  }
             }
         }
        await  Clients.Users(temp).ReceiveLineNotice(Context.RequestCookies["MiicID"].Value);
        
    }
    [HubMethodName("SendOffline")]
     public async Task SendOffline() 
     {
         string user_id = Context.RequestCookies["MiicID"].Value;
         DataTable dt = IaddressBookInfo.GetPersonAllAddressBookList(user_id);

         List<string> temp = new List<string>();
         foreach (var dr in dt.AsEnumerable())
         {
             foreach (var user in users.AsEnumerable())
             {
                 if (dr[Config.Attribute.GetSqlColumnNameByPropertyName<AddressBookInfo, string>(o => o.AddresserID)].ToString() == user)
                 {
                     temp.Add(user);
                 }
             }
         }
         users.Remove(user_id);
         Config.IlogicLogService.Write(new LogicLog()
         {
             AppName = Config.AppName,
             ClassName = ClassName,
             NamespaceName = NamespaceName,
             MethodName = MethodBase.GetCurrentMethod().Name,
             Message = "目前用户数为：" + users.Count + ",删除：" + user_id,
             Oper = "system"
         });
         await Clients.Users(temp).ReceiveOfflineNotice(user_id);
     }
    [HubMethodName("SendMessage")]
    public async Task Send(string publishID, string title, NoticeUserView noticeUser)
    {
        MiicSafeCollection<string> noticerIDs = new MiicSafeCollection<string>();
        string userID = Context.RequestCookies["MiicID"].Value;
        string userName = Context.RequestCookies["MiicUserName"].Value;
        foreach (var userIDItem in noticeUser.Noticers)
        {
            foreach (var item in users.Where(o => o == userIDItem.UserID))
            {
               noticerIDs.Add(item);
            }
        }
        if (noticerIDs.Count > 0)
        {
            DataTable dt =((NoticeInfoDao) InoticeInfo).GetUnReadNoticeID(new GetUnreadNoticeView()
            {
                PublishID = publishID,
                Source = ((int)noticeUser.NoticeSource).ToString(),
                NoticeType = ((int)noticeUser.NoticeType).ToString(),
                PublisherID = userID,
                NoticerID =  noticerIDs.AsEnumerable().ToList()
            });
            Task[] list = new Task[noticerIDs.Count];
            int index = 0;
            Task taskResult = null;
            try
            {
                foreach (var item in dt.AsEnumerable())
                {
                    foreach (var noticer in noticerIDs)
                    {
                        if (item[Config.Attribute.GetSqlColumnNameByPropertyName<NoticeInfo, string>(o => o.NoticerID)].ToString() == noticer)
                        {
                            list[index] = Clients.User(noticer).Recieve(new { ID = item[Config.Attribute.GetSqlColumnNameByPropertyName<NoticeInfo, string>(o => o.ID)].ToString(), PublishID = publishID, Title = title, UserID = userID, UserName = userName });
                            index++;
                        }
                    }
                }
                await (taskResult= Task.WhenAll(list));
            }
            catch (Exception ex) 
            {
                Config.IlogicLogService.Write(new LogicLog()
                {
                    AppName = Config.AppName,
                    ClassName = ClassName,
                    NamespaceName = NamespaceName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    Message = "Hub Error:"+ex.Message,
                    Oper = Config.Oper
                });
                foreach (var extask in taskResult.Exception.InnerExceptions) 
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
        }

    }

}
