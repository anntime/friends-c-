using Miic.Attribute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miic.Manage.Org
{
    [MiicTable(MiicParentStorageName = "MIIC_SOCIAL_COMMON", MiicStorageName = "DIM_ORG", Description = "企业维度表")]
    public  class DimOrganizationInfo
    {
        [MiicField(MiicStorageName = "ORG_ID", IsNotNull = true, MiicDbType = DbType.String, Description = "唯一码")]
        public string OrgID { get; set; }

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
        [MiicField(MiicStorageName = "REGISTER_TYPE_NAME", MiicDbType = DbType.String, Description = "注册类型名称")]
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
        [MiicField(MiicStorageName = "AREA_COUNTY_ID", MiicDbType = DbType.Int32, Description = "区、县ID")]
        public int? AreaCountryID { get; set; }
        [MiicField(MiicStorageName = "AREA_COUNTY_NAME", MiicDbType = DbType.String, Description = "区、县名")]
        public string AreaCountryName { get; set; }
        [MiicField(MiicStorageName = "BEGIN_TIME", IsNotNull = true, MiicDbType = DbType.DateTime, Description = "创建时间")]
        public DateTime? BeginTime { get; set; }
        [MiicField(MiicStorageName = "END_TIME", MiicDbType = DbType.DateTime, Description = "失效时间")]
        public DateTime? EndTime { get; set; }
        [MiicField(MiicStorageName = "VALID", IsNotNull = true, MiicDbType = DbType.String, Description = "有效性")]
        public string Valid { get; set; }
        [MiicField(MiicStorageName = "SORT_NO", IsPrimaryKey=true, IsIdentification = true, MiicDbType = DbType.Int32, Description = "排序")]
        public int? SortNo { get; set; }
    }
}
