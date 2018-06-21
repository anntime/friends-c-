using Miic.Base;
using Miic.Base.Setting;
using Miic.Cryptographic;
using Miic.Cryptographic.SM2;
using Miic.DB.Setting;
using Miic.DB.SqlObject;
using Miic.Log;
using System;
using System.Data;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Miic.Manage.User
{
    public class LoginRequestView
    {
        private static readonly string ClassName = MethodBase.GetCurrentMethod().DeclaringType.Name;
        private static readonly string NamespaceName = MethodBase.GetCurrentMethod().DeclaringType.Namespace;
        private string rsaPassword;
        /// <summary>
        /// RSA握手私钥
        /// </summary>
        public string RSAPassword
        {
            get
            {
                return rsaPassword;
            }
            set
            {
                Contract.Requires(value != null, "Password不能为空！");
                rsaPassword = value;
            }
        }
        private string socialCode;
        /// <summary>
        /// 用户登录名
        /// </summary>
        public string SocialCode
        {
            get
            {
                return socialCode;
            }
            set
            {
                Contract.Requires(value != null, "SocialCode不能为空！");
                socialCode = value;
            }
        }
        /// <summary>
        /// 握手检测
        /// </summary>
        /// <param name="rsaPassword">私钥</param>
        /// <returns>是/否</returns>
        private string getMD5byRSA(string rsaPassword)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(HttpContext.Current.Session["PrivateKey"].ToString());
            byte[] tempResult = rsa.Decrypt(Config.Convert.HexStringToBytes(rsaPassword), false);
            ASCIIEncoding enc = new ASCIIEncoding();
            string strPwdMD5 = enc.GetString(tempResult);
            return strPwdMD5;
        }
        private string getMD5bySM2(string sm2Password) 
        {
            string result = string.Empty;
            try
            {
                IAsymmetricEncryptionService IasymmetricEncryptionService = Sm2Service.Instance;
                result = IasymmetricEncryptionService.Decrypt(HttpContext.Current.Session["PrivateKey"].ToString(), sm2Password);
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
        /// 登录访问器
        /// </summary>
        /// <param name="miicSocialUserDao">登录DAO</param>
        /// <returns>登录访问生成条件</returns>
        public MiicConditionCollections visitor(MiicSocialUserDao miicSocialUserDao)
        {
            MiicConditionCollections result = new MiicConditionCollections(MiicDBLogicSetting.No);
            MiicCondition passwordCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.SM3Password),
                                                                  getMD5bySM2(rsaPassword),
                                                                  DbType.String,
                                                                  MiicDBOperatorSetting.Equal);
            result.Add(new MiicConditionLeaf(MiicDBLogicSetting.No, passwordCondition));
            MiicConditionCollections userCodeCondition = new MiicConditionCollections(MiicDBLogicSetting.And);

            MiicCondition socialCodeCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.SocialCode),
                                                                  socialCode,
                                                                  DbType.String,
                                                                  MiicDBOperatorSetting.Equal);
            userCodeCondition.Add(new MiicConditionLeaf(MiicDBLogicSetting.No, socialCodeCondition));
            MiicCondition mobileCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.Mobile),
                                                                  socialCode,
                                                                  DbType.String,
                                                                  MiicDBOperatorSetting.Equal);
            userCodeCondition.Add(new MiicConditionLeaf(MiicDBLogicSetting.Or, mobileCondition));
            result.Add(userCodeCondition);
            MiicCondition validCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.Valid),
                                                                  ((int)MiicValidTypeSetting.Valid).ToString(),
                                                                  DbType.String,
                                                                  MiicDBOperatorSetting.Equal);
            result.Add(new MiicConditionLeaf(validCondition));
            MiicCondition activateFlagCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.ActivateFlag),
                ((int)(MiicSimpleApproveStatusSetting.Agree)).ToString(),
                DbType.String,
                MiicDBOperatorSetting.Equal);
            result.Add(new MiicConditionLeaf(activateFlagCondition));

            MiicCondition disabledFlagCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<MiicSocialUserInfo, string>(o => o.DisabledFlag),
               ((int)(MiicYesNoSetting.No)).ToString(),
               DbType.String,
               MiicDBOperatorSetting.Equal);
            result.Add(new MiicConditionLeaf(disabledFlagCondition));
            return result;
        }
    }
}
