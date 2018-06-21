using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.Configuration;
using Miic.BaseStruct;
/// <summary>
/// 基础服务
/// </summary>
[WebService(Namespace = "http://share.miic.com.cn/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[ScriptService]
public class BasicService : WebService {
    private static readonly string OpenIMAppKey = WebConfigurationManager.AppSettings["OpenIMAppKey"].ToString();
    private static readonly string OpenIMAppSecret = "000000";
    public BasicService () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod(Description="获取IMAppKey")]
    [ScriptMethod(ResponseFormat=ResponseFormat.Json)]
    [GenerateScriptType(typeof(MiicKeyValue))]
    public MiicKeyValue GetOpenIMAppKey() 
    {
        return new MiicKeyValue()
        {
            Name = OpenIMAppKey,
            Value=OpenIMAppSecret
        };
        
    }
    
}
