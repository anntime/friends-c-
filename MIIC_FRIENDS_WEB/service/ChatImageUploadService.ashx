<%@ WebHandler Language="C#" Class="ChatImageUploadService" %>

using System;
using System.Web;
using System.IO;
using System.Reflection;
using System.Web.Configuration;
using Miic.Base;
using Miic.Friends.Common.Setting;
public class ChatImageUploadService : IHttpHandler {
    
    private static readonly string ClassName = MethodBase.GetCurrentMethod().DeclaringType.Name;
    private static readonly string NamespaceName = MethodBase.GetCurrentMethod().DeclaringType.Namespace;
    public static readonly string ChatLogoPath = WebConfigurationManager.AppSettings["ChatLogoPath"].ToString();
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        string filePath = string.Empty;
        string ChatPath = string.Empty;
        if (context.Request["Type"] != null)
        {
           string type = context.Request["Type"].ToString();
           if (type == ((int)BusinessTypeSetting.Moments).ToString()) 
           {
               ChatPath = ChatLogoPath + BusinessTypeSetting.Moments.ToString();
           }
           else if (type == ((int)BusinessTypeSetting.Group).ToString()) 
           {
               ChatPath = ChatLogoPath + BusinessTypeSetting.Group.ToString();
           }
           else if (type == ((int)BusinessTypeSetting.Community).ToString()) 
           {
               ChatPath = ChatLogoPath + BusinessTypeSetting.Community.ToString();
               //暂不支持
           }
           ChatPath += "/";
        }
        foreach (string item in context.Request.Files.AllKeys)
        {
            HttpPostedFile file = context.Request.Files[item];
            string fileExt = Path.GetExtension(file.FileName).TrimStart('.').ToLower();

            if (fileExt.ToLower() == "jpg" ||fileExt.ToLower()=="jpeg" || fileExt.ToLower() == "gif" || fileExt.ToLower() == "png")
            {
                filePath = HttpContext.Current.Server.MapPath(ChatPath);
            }
            else
            {
                context.Response.Write(Config.Serializer.Serialize(new { result = false, message = "请选择合法的上传文件！" }));
                context.Response.End();
                return;
            }

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            string newName = Guid.NewGuid().ToString();
            string newFileName = newName + "." + fileExt;
            string id = newName;
            try
            {
                file.SaveAs(filePath + newFileName);
                context.Response.Write(Config.Serializer.Serialize(new { result = true, url = CommonService.GetBaseFullUrl(ChatPath + newFileName) }));
            }
            catch (Exception ex)
            {
                Config.IlogicLogService.Write(new Miic.Log.LogicLog()
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
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}