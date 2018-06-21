using Miic.Attribute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miic.Manage.User
{
    [MiicTable(MiicParentStorageName = "MIIC_SOCIAL_COMMON", MiicStorageName = "DOWNLOAD_ALL_USER_VIEW", Description = "下载用户搜索视图表")]
    public class DownloadAllUserView
    {
        [MiicField(MiicStorageName = "ID", MiicDbType = DbType.String, Description = "唯一码")]
        public string ID { get; set; }
        [MiicField(MiicStorageName = "MIIC_SOCIAL_CODE", MiicDbType = DbType.String, Description = "用户编码")]
        public string SocialCode { get; set; }
        [MiicField(MiicStorageName = "USER_LEVEL", MiicDbType = DbType.String, Description = "用户等级")]
        public string Level { get; set; }
        [MiicField(MiicStorageName = "MOBILE", MiicDbType = DbType.String, Description = "手机")]
        public string Mobile { get; set; }
        [MiicField(MiicStorageName = "EMAIL", MiicDbType = DbType.String, Description = "Email")]
        public string Email { get; set; }
        [MiicField(MiicStorageName = "USER_TYPE", MiicDbType = DbType.String, Description = "用户类型")]
        public string UserType { get; set; }
        [MiicField(MiicStorageName = "ACTIVATE_FLAG", MiicDbType = DbType.String, Description = "是否激活")]
        public string ActivateFlag { get; set; }
        [MiicField(MiicStorageName = "DISABLED_FLAG", MiicDbType = DbType.String, Description = "是否禁用")]
        public string DisabledFlag { get; set; }
        [MiicField(MiicStorageName = "VALID", MiicDbType = DbType.String, Description = "有效性")]
        public string Valid { get; set; }
        [MiicField(MiicStorageName = "REMARK", MiicDbType = DbType.String, Description = "备注")]
        public string Remark { get; set; }
        [MiicField(MiicStorageName = "SORT_NO", MiicDbType = DbType.Int32, IsIdentification = true, Description = "排序")]
        public int? SortNo { get; set; }
        [MiicField(MiicStorageName = "ORG_NAME", MiicDbType = DbType.String, Description = "企业名称")]
        public string OrgName { get; set; }
        [MiicField(MiicStorageName = "REAL_NAME", MiicDbType = DbType.String, Description = "用户名")]
        public string RealName { get; set; }
        [MiicField(MiicStorageName = "USER_NAME", MiicDbType = DbType.String, Description = "用户昵称")]
        public string UserName { get; set; }
        [MiicField(MiicStorageName = "SEX", MiicDbType = DbType.String, Description = "性别")]
        public string Sex { get; set; }
        [MiicField(MiicStorageName = "QQ", MiicDbType = DbType.String, Description = "QQ")]
        public string QQ { get; set; }
        [MiicField(MiicStorageName = "TEL", MiicDbType = DbType.String, Description = "电话")]
        public string Tel { get; set; }
        [MiicField(MiicStorageName = "FAX", MiicDbType = DbType.String, Description = "传真")]
        public string Fax { get; set; }
        [MiicField(MiicStorageName = "ADDRESS", MiicDbType = DbType.String, Description = "通信地址")]
        public string Address { get; set; }
        [MiicField(MiicStorageName = "POSTCODE", MiicDbType = DbType.String, Description = "邮政编码")]
        public string Postcode { get; set; }
        [MiicField(MiicStorageName = "LINKMAN", MiicDbType = DbType.String, Description = "联系人")]
        public string Linkman { get; set; }
        [MiicField(MiicStorageName = "AREA_PROV_NAME", MiicDbType = DbType.String, Description = "省")]
        public string AreaProName { get; set; }
        [MiicField(MiicStorageName = "AREA_CITY_NAME", MiicDbType = DbType.String, Description = "市")]
        public string AreaCityName { get; set; }
        [MiicField(MiicStorageName = "AREA_COUNTRY_NAME", MiicDbType = DbType.String, Description = "区、县")]
        public string AreaCountryName { get; set; }
        [MiicField(MiicStorageName = "JURISTIC_PERSON_CODE", MiicDbType = DbType.String, Description = "法人代码")]
        public string JuristicPersonCode { get; set; }
        [MiicField(MiicStorageName = "CODE_TYPE", MiicDbType = DbType.String, Description = "机构代码类型")]
        public string CodeType { get; set; }
        [MiicField(MiicStorageName = "LEGAL_PERSON", MiicDbType = DbType.String, Description = "法人")]
        public string LegalPerson { get; set; }
        [MiicField(MiicStorageName = "SCALE_TYPE_NAME", MiicDbType = DbType.String, Description = "企业规模名称")]
        public string ScaleTypeName { get; set; }
        [MiicField(MiicStorageName = "TRADE_NAME", MiicDbType = DbType.String, Description = "所属大行业")]
        public string TradeName { get; set; }
        [MiicField(MiicStorageName = "TRADE_SUB_NAME", MiicDbType = DbType.String, Description = "所属小行业")]
        public string TradeSubName { get; set; }
        [MiicField(MiicStorageName = "REGISTER_TYPE_NAME", MiicDbType = DbType.String, Description = "注册类型")]
        public string RegisterTypeName { get; set; }
        [MiicField(MiicStorageName = "SHARE_TYPE_NAME", MiicDbType = DbType.String, Description = "控股类型")]
        public string ShareTypeName { get; set; }
    }
}
