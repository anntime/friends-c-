using Miic.Attribute;
using System;
using System.Data;

namespace Miic.Manage.Org
{
    [MiicTable(MiicParentStorageName = "MIIC_SOCIAL_COMMON", MiicStorageName = "ORG_INFO", Description = "企业信息表")]
    public partial class OrganizationInfo
    {
        public OrganizationInfo() { }
        [MiicField(MiicStorageName = "ORG_ID", IsPrimaryKey = true, IsNotNull = true, MiicDbType = DbType.String, Description = "唯一码")]
        public string OrgID { get; set; }
       
        [MiicField(MiicStorageName = "ORG_NAME", IsNotNull = true, MiicDbType = DbType.String, Description = "企业名称")]
        public string OrgName { get; set; }
        [MiicField(MiicStorageName = "ORG_SHORT_NAME", MiicDbType = DbType.String, Description = "企业名称(简称)")]
        public string OrgShortName { get; set; }
        [MiicField(MiicStorageName = "JURISTIC_PERSON_CODE", IsNotNull = true, MiicDbType = DbType.String, Description = "法人代码或者统一社会信用代码")]
        public string JuristicPersonCode { get; set; }
        [MiicField(MiicStorageName = "CODE_TYPE", IsNotNull = true, MiicDbType = DbType.String, Description = "机构代码类型")]
        public string CodeType { get; set; }
        [MiicField(MiicStorageName = "LEGAL_PERSON", MiicDbType = DbType.String, Description = "法人")]
        public string LegalPerson { get; set; }
        [MiicField(MiicStorageName = "TRADE_ID", MiicDbType = DbType.String, Description = "所属大行业ID")]
        public string TradeID { get; set; }
        [MiicField(MiicStorageName = "TRADE_NAME", MiicDbType = DbType.String, Description = "所属大行业名称")]
        public string TradeName { get; set; }
        [MiicField(MiicStorageName = "TRADE_SUB_ID", MiicDbType = DbType.String, Description = "所属小行业ID（最底层）")]
        public string TradeSubID { get; set; }
        [MiicField(MiicStorageName = "TRADE_SUB_NAME", MiicDbType = DbType.String, Description = "所属小行业名称（最底层）")]
        public string TradeSubName { get; set; }
        [MiicField(MiicStorageName = "REGISTER_TYPE_ID", MiicDbType = DbType.String, Description = "注册类型ID")]
        public string RegisterTypeID { get; set; }
        [MiicField(MiicStorageName = "REGISTER_TYPE_NAME",  MiicDbType = DbType.String, Description = "注册类型名称")]
        public string RegisterTypeName { get; set; }
        [MiicField(MiicStorageName = "SHARE_TYPE_ID", MiicDbType = DbType.String, Description = "控股类型ID")]
        public string ShareTypeID { get; set; }
        [MiicField(MiicStorageName = "SHARE_TYPE_NAME", MiicDbType = DbType.String, Description = "控股类型名称")]
        public string ShareTypeName { get; set; }
        [MiicField(MiicStorageName = "SCALE_TYPE_ID", MiicDbType = DbType.String, Description = "企业规模ID")]
        public string ScaleTypeID { get; set; }
        [MiicField(MiicStorageName = "SCALE_TYPE_NAME", MiicDbType = DbType.String, Description = "企业规模名称")]
        public string ScaleTypeName { get; set; }
        [MiicField(MiicStorageName = "AREA_PROV_ID", MiicDbType = DbType.Int32, Description = "省ID")]
        public int? AreaProvinceID { get; set; }
        [MiicField(MiicStorageName = "AREA_PROV_NAME", MiicDbType = DbType.String, Description = "省名")]
        public string AreaProvinceName { get; set; }
        [MiicField(MiicStorageName = "AREA_CITY_ID", MiicDbType = DbType.Int32, Description = "市ID")]
        public int? AreaCityID { get; set; }
        [MiicField(MiicStorageName = "AREA_CITY_NAME", MiicDbType = DbType.String, Description = "市名")]
        public string AreaCityName { get; set; }
        [MiicField(MiicStorageName = "AREA_COUNTRY_ID", MiicDbType = DbType.Int32, Description = "区、县ID")]
        public int? AreaCountryID { get; set; }
        [MiicField(MiicStorageName = "AREA_COUNTRY_NAME", MiicDbType = DbType.String, Description = "区、县名")]
        public string AreaCountryName { get; set; }
        [MiicField(MiicStorageName = "ADDRESS", MiicDbType = DbType.String, Description = "通信地址")]
        public string Address { get; set; }
        [MiicField(MiicStorageName = "POSTCODE", MiicDbType = DbType.String, Description = "邮政编码")]
        public string PostCode { get; set; }
        [MiicField(MiicStorageName = "WEBSITE", MiicDbType = DbType.String, Description = "网站")]
        public string WebSite { get; set; }
        [MiicField(MiicStorageName = "LINKMAN", MiicDbType = DbType.String, Description = "联系人")]
        public string LinkMan { get; set; }
        [MiicField(MiicStorageName = "TEL", MiicDbType = DbType.String, Description = "电话")]
        public string Tel { get; set; }
        [MiicField(MiicStorageName = "FAX", MiicDbType = DbType.String, Description = "传真")]
        public string Fax { get; set; }
        [MiicField(MiicStorageName = "EMAIL", IsNotNull = true, MiicDbType = DbType.String, Description = "联系人电子邮箱")]
        public string Email { get; set; }
        [MiicField(MiicStorageName = "MOBILE", MiicDbType = DbType.String, Description = "联系人手机")]
        public string Mobile { get; set; }
        [MiicField(MiicStorageName = "REMARK", MiicDbType = DbType.String, Description = "企业备注")]
        public string Remark { get; set; }
        [MiicField(MiicStorageName = "OPENED_TIME", MiicDbType = DbType.Date, Description = "开业时间")]
        public DateTime? OpenTime { get; set; }
        [MiicField(MiicStorageName = "IS_LISTED", IsNotNull = true, MiicDbType = DbType.String, Description = "是否上市")]
        public string IsListed { get; set; }
        [MiicField(MiicStorageName = "STOCKCODE", MiicDbType = DbType.String, Description = "股票代码")]
        public string StockCode { get; set; }
    }
}
