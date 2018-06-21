using Miic.Base;
using Miic.Base.Setting;
using Miic.DB;
using Miic.DB.Setting;
using Miic.DB.SqlObject;
using Miic.Log;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.IO;
using System.Reflection;
using System.Web;

namespace Miic.Manage.Org
{
    public class OrganizationInfoDao : NoRelationCommon<OrganizationInfo>, IOrganizationInfo
    {
        private static readonly string ClassName = MethodBase.GetCurrentMethod().DeclaringType.Name;
        private static readonly string NamespaceName = MethodBase.GetCurrentMethod().DeclaringType.Namespace;
        public bool IsCached { get; private set; }
        public OrganizationInfoDao() { }
        public OrganizationInfoDao(bool isCached) 
        {
            this.IsCached = isCached;
        }

        bool ICommon<OrganizationInfo>.Insert(OrganizationInfo organizationInfo)
        {
            Contract.Requires<ArgumentNullException>(organizationInfo != null, "参数organizationInfo：不能为空！");
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(organizationInfo.OrgID), "参数organizationInfo.OrgID：不能为空！");
            bool result = false;
            string message1 = string.Empty;
            string message2 = string.Empty;
            string message = string.Empty;
            List<string> sqls = new List<string>();
            sqls.Add(DBService.InsertSql(organizationInfo, out message1));
            DimOrganizationInfo dimOrg = new DimOrganizationInfo()
            {
                AreaCityID = organizationInfo.AreaCityID,
                AreaCityName = organizationInfo.AreaCityName,
                AreaCountryID = organizationInfo.AreaCountryID,
                AreaCountryName = organizationInfo.AreaCountryName,
                AreaProvinceID = organizationInfo.AreaProvinceID,
                AreaProvinceName = organizationInfo.AreaProvinceName,
                BeginTime = DateTime.Now,
                TradeID = organizationInfo.TradeID,                         //@Majichao 添加大行业维度 2016-07-15
                TradeName = organizationInfo.TradeName,
                TradeSubID = organizationInfo.TradeSubID,                   //@Majichao 添加小行业维度 2016-07-15
                TradeSubName = organizationInfo.TradeSubName,
                RegisterTypeID = organizationInfo.RegisterTypeID,
                RegisterTypeName = organizationInfo.RegisterTypeName,
                ScaleTypeID = organizationInfo.ScaleTypeID,
                ScaleTypeName = organizationInfo.ScaleTypeName,
                OrgID = organizationInfo.OrgID,
                ShareTypeID = organizationInfo.ShareTypeID,
                ShareTypeName = organizationInfo.ShareTypeName,
                Valid = ((int)MiicValidTypeSetting.Valid).ToString(),
            };
            sqls.Add(DBService.InsertSql(dimOrg, out message2));
            try
            {
                result = dbService.excuteSqls(sqls, out message);
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
            if (result == true)
            {
                if (this.IsCached == true)
                {
                    InsertCache(organizationInfo);
                }
            }
            return result;
        }
        bool ICommon<OrganizationInfo>.Update(OrganizationInfo organizationInfo)
        {
            Contract.Requires<ArgumentNullException>(organizationInfo != null, "参数organizationInfo:不能为空！");
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(organizationInfo.OrgID), "参数organizationInfo.OrgID:不能为空，因为是主键");
            string message = string.Empty;
            bool result = false;
            List<string> sqls = new List<string>();
            OrganizationInfo old = ((ICommon<OrganizationInfo>)this).GetInformation(organizationInfo.OrgID);
            if ((organizationInfo.TradeID != null && organizationInfo.TradeName != old.TradeName) ||           //@Majichao IndustryCode => TradeID 2016-7-15
                (organizationInfo.TradeSubID != null && organizationInfo.TradeSubName != old.TradeSubName) ||
                (organizationInfo.RegisterTypeID != null && organizationInfo.RegisterTypeID != old.RegisterTypeID) ||
                (organizationInfo.ShareTypeID != null && organizationInfo.ShareTypeID != old.ShareTypeID) ||
                (organizationInfo.ScaleTypeID != null && organizationInfo.ScaleTypeID != old.ScaleTypeID) ||
                (organizationInfo.AreaProvinceID!=null && organizationInfo.AreaProvinceID != old.AreaProvinceID) ||
                (organizationInfo.AreaCityID!=null && organizationInfo.AreaCityID != old.AreaCityID) ||
                (organizationInfo.AreaCountryID!=null && organizationInfo.AreaCountryID != old.AreaCountryID)
                )
            {
                string message1 = string.Empty;
                string message2 = string.Empty;
                string message3 = string.Empty;
                sqls.Add(DBService.UpdateSql(organizationInfo, out message1));
                MiicConditionCollections condition = new MiicConditionCollections(MiicDBLogicSetting.No);
                MiicCondition orgIDCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DimOrganizationInfo, string>(o => o.OrgID),
                    organizationInfo.OrgID,
                    DbType.String,
                    MiicDBOperatorSetting.Equal);
                condition.Add(new MiicConditionLeaf(MiicDBLogicSetting.No, orgIDCondition));
                MiicCondition validCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DimOrganizationInfo, string>(o => o.Valid),
                    ((int)MiicValidTypeSetting.Valid).ToString(),
                    DbType.String,
                    MiicDBOperatorSetting.Equal);
                condition.Add(new MiicConditionLeaf(validCondition));
                sqls.Add(DBService.UpdateConditionsSql<DimOrganizationInfo>(new DimOrganizationInfo()
                {
                    EndTime = DateTime.Now,
                    Valid = ((int)MiicValidTypeSetting.InValid).ToString()
                }, condition, out message3));
                DimOrganizationInfo dimOrg = new DimOrganizationInfo()
                {
                    AreaCityID = organizationInfo.AreaCityID,
                    AreaCityName = organizationInfo.AreaCityName,
                    AreaCountryID = organizationInfo.AreaCountryID,
                    AreaCountryName = organizationInfo.AreaCountryName,
                    AreaProvinceID = organizationInfo.AreaProvinceID,
                    AreaProvinceName = organizationInfo.AreaProvinceName,
                    BeginTime = DateTime.Now,
                    TradeID = organizationInfo.TradeID,                         //@Majichao 添加大行业维度 2016-07-15
                    TradeName = organizationInfo.TradeName,
                    TradeSubID = organizationInfo.TradeSubID,                   //@Majichao 添加小行业维度 2016-07-15
                    TradeSubName = organizationInfo.TradeSubName,
                    RegisterTypeID = organizationInfo.RegisterTypeID,
                    RegisterTypeName = organizationInfo.RegisterTypeName,
                    ScaleTypeID = organizationInfo.ScaleTypeID,
                    ScaleTypeName = organizationInfo.ScaleTypeName,
                    OrgID = organizationInfo.OrgID,
                    ShareTypeID = organizationInfo.ShareTypeID,
                    ShareTypeName = organizationInfo.ShareTypeName,
                    Valid = ((int)MiicValidTypeSetting.Valid).ToString()
                };
                sqls.Add(DBService.InsertSql(dimOrg, out message2));
            }
            else
            {
                string message1 = string.Empty;
                sqls.Add(DBService.UpdateSql(organizationInfo, out message1));
            }
            try
            {
                result = dbService.excuteSqls(sqls, out message);
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
            if (result == true)
            {
                if (this.IsCached == true)
                {
                    DeleteCache(o => o.OrgID == organizationInfo.OrgID);
                }
            }
            return result;
        }

        bool ICommon<OrganizationInfo>.Delete(string id)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(id), "参数id:不能为空");
            bool result = false;
            int count = 0;
            string message = string.Empty;
            try
            {
                result = dbService.Delete<OrganizationInfo>(new OrganizationInfo()
                {
                    OrgID = id
                }, out count, out message);
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
            if (result == true)
            {
                if (IsCached == true)
                {
                    DeleteCache(o => o.OrgID == id);
                }
            }
            return result;
        }

        OrganizationInfo ICommon<OrganizationInfo>.GetInformation(string id)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(id), "参数id:不能为空");
            OrganizationInfo result = null;
            string message = string.Empty;
            try
            {
                result = items.Find(o => o.OrgID == id);
                if (result == null)
                {
                    result = dbService.GetInformation(new OrganizationInfo
                    {
                        OrgID = id
                    }, out message);
                    if (result != null)
                    {
                        if (this.IsCached == true)
                        {
                            InsertCache(result);
                        }
                    }
                }
                else
                {
                    string serializer = Config.Serializer.Serialize(result);
                    result =Config.Attribute.ConvertObjectWithDateTime(Config.Serializer.Deserialize<OrganizationInfo>(serializer));
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

        /// <summary>
        /// 新增企业信息
        /// </summary>
        /// <param name="organizationInfo">企业信息</param>
        /// <param name="orgProductInfos">企业产品附件</param>
        /// <returns>Yes/No</returns>
        public bool Insert(OrganizationInfo organizationInfo, List<OrgProductInfo> orgProductInfos)
        {
            Contract.Requires<ArgumentNullException>(organizationInfo != null, "参数organizationInfo:不能为空");
            bool result = false;
            List<string> sqls = new List<string>();
            string message = string.Empty;
            string message1 = string.Empty;
            string message2 = string.Empty;
            string message3 = string.Empty;
            sqls.Add(DBService.InsertSql(organizationInfo, out message1));
            DimOrganizationInfo dimOrg = new DimOrganizationInfo()
            {
                AreaCityID = organizationInfo.AreaCityID,
                AreaCityName = organizationInfo.AreaCityName,
                AreaCountryID = organizationInfo.AreaCountryID,
                AreaCountryName = organizationInfo.AreaCountryName,
                AreaProvinceID = organizationInfo.AreaProvinceID,
                AreaProvinceName = organizationInfo.AreaProvinceName,
                BeginTime = DateTime.Now,
                TradeID = organizationInfo.TradeID,                         //@Majichao 添加大行业维度 2016-07-15
                TradeName = organizationInfo.TradeName,
                TradeSubID = organizationInfo.TradeSubID,                   //@Majichao 添加小行业维度 2016-07-15
                TradeSubName = organizationInfo.TradeSubName,
                RegisterTypeID = organizationInfo.RegisterTypeID,
                RegisterTypeName = organizationInfo.RegisterTypeName,
                ScaleTypeID = organizationInfo.ScaleTypeID,
                ScaleTypeName = organizationInfo.ScaleTypeName,
                OrgID = organizationInfo.OrgID,
                ShareTypeID = organizationInfo.ShareTypeID,
                ShareTypeName = organizationInfo.ShareTypeName,
                Valid = ((int)MiicValidTypeSetting.Valid).ToString(),
            };
            sqls.Add(DBService.InsertSql(dimOrg, out message2));
            if (orgProductInfos != null && orgProductInfos.Count != 0)
            {
                sqls.Add(DBService.InsertsSql(orgProductInfos, out message3));
            }

            bool fileResult = false;

            try
            {
                try
                {
                    if (orgProductInfos != null && orgProductInfos.Count != 0)
                    {
                        foreach (var item in orgProductInfos)
                        {
                            string dest = HttpContext.Current.Server.MapPath(item.FilePath);
                            string source = HttpContext.Current.Server.MapPath("/file/temp/OrgProduct/" + Path.GetFileName(dest));
                            File.Copy(source, dest, true);
                        }
                    }
                    fileResult = true;
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

                if (fileResult == true)
                {
                    result = dbService.excuteSqls(sqls, out message);
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
            if (result == true) 
            {
                if (IsCached == true) 
                {
                    InsertCache(organizationInfo);
                }
            }
            return result;
        }

        /// <summary>
        /// 更新企业信息
        /// </summary>
        /// <param name="organizationInfo">企业</param>
        /// <param name="removeSimpleOrgProductViews">删除企业产品附件</param>
        /// <returns></returns>
        public bool Update(OrganizationInfo organizationInfo, List<SimpleOrgProductView> removeSimpleOrgProductViews)
        {
            Contract.Requires<ArgumentNullException>(organizationInfo != null, "参数organizationInfo:不能为空");
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(organizationInfo.OrgID), "参数organizationInfo.OrgID:不能为空");
            Contract.Requires<ArgumentNullException>(removeSimpleOrgProductViews != null && removeSimpleOrgProductViews.Count != 0, "参数removeSimpleOrgProductViews:不能为空");
            bool result = false;
            List<string> sqls = new List<string>();
            string message = string.Empty;
            string message1 = string.Empty;
            string message2 = string.Empty;
            try
            {
                foreach (var item in removeSimpleOrgProductViews)
                {
                    sqls.Add(DBService.DeleteSql<OrgProductInfo>(new OrgProductInfo()
                    {
                        ID = item.ID
                    }, out message1));
                }

                sqls.Add(DBService.UpdateSql<OrganizationInfo>(organizationInfo, out message2));

                bool fileResult = false;
                try
                {
                    foreach (var item in removeSimpleOrgProductViews)
                    {
                        File.Delete(HttpContext.Current.Server.MapPath(item.FilePath));
                        File.Delete(HttpContext.Current.Server.MapPath("/file/temp/OrgProduct/" + Path.GetFileName(item.FilePath)));
                    }

                    fileResult = true;
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


                if (fileResult == true)
                {
                    result = dbService.excuteSqls(sqls, out message);
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
            if (result == true)
            {
                if (IsCached == true)
                {
                    DeleteCache(o => o.OrgID == organizationInfo.OrgID);
                }
            }
            return result;
        }

        /// <summary>
        /// 更新企业信息
        /// </summary>
        /// <param name="organizationInfo">企业</param>
        /// <param name="orgProductInfos">企业产品附件</param>
        /// <param name="removeSimpleOrgProductViews">删除企业产品附件</param>
        /// <returns></returns>
        public bool Update(OrganizationInfo organizationInfo, List<OrgProductInfo> orgProductInfos, List<SimpleOrgProductView> removeSimpleOrgProductViews)
        {
            Contract.Requires<ArgumentNullException>(organizationInfo != null, "参数organizationInfo:不能为空");
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(organizationInfo.OrgID), "参数organizationInfo.OrgID:不能为空");
            Contract.Requires<ArgumentNullException>(orgProductInfos != null && orgProductInfos.Count != 0, "参数orgProductInfos:不能为空");
            bool result = false;
            List<string> sqls = new List<string>();
            string message = string.Empty;
            string message1 = string.Empty;
            string message2 = string.Empty;
            string message3 = string.Empty;
            try
            {
                OrganizationInfo temOrg = ((ICommon<OrganizationInfo>)this).GetInformation(organizationInfo.OrgID);

                sqls.Add(DBService.UpdateSql<OrganizationInfo>(organizationInfo, out message1));

                foreach (var item in orgProductInfos)
                {
                    sqls.Add(DBService.InsertSql(item, out message2));
                }

                if (removeSimpleOrgProductViews != null && removeSimpleOrgProductViews.Count != 0)
                {
                    foreach (var item in removeSimpleOrgProductViews)
                    {
                        sqls.Add(DBService.DeleteSql<OrgProductInfo>(new OrgProductInfo()
                        {
                            ID = item.ID
                        }, out message3));
                    }
                }

                bool fileResult = false;
                try
                {
                    foreach (var item in orgProductInfos)
                    {
                        string dest = HttpContext.Current.Server.MapPath(item.FilePath);
                        string source = string.Empty;
                        source = HttpContext.Current.Server.MapPath("/file/temp/OrgProduct/" + Path.GetFileName(dest));
                        File.Copy(source, dest, true);
                    }

                    if (removeSimpleOrgProductViews != null && removeSimpleOrgProductViews.Count != 0)
                    {
                        foreach (var item in removeSimpleOrgProductViews)
                        {
                            File.Delete(HttpContext.Current.Server.MapPath(item.FilePath));
                            File.Delete(HttpContext.Current.Server.MapPath("/file/temp/OrgProduct/" + Path.GetFileName(item.FilePath)));
                        }
                    }

                    fileResult = true;
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

                if (fileResult == true)
                {
                    result = dbService.excuteSqls(sqls, out message);
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
            if (result == true)
            {
                if (IsCached == true)
                {
                    DeleteCache(o => o.OrgID == organizationInfo.OrgID);
                }
            }
            return result;
        }

        public DataTable GetOrganizationDetailInfo(string orgID)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(orgID), "参数orgID:不能为空");
            DataTable result = new DataTable();
            string message = string.Empty;
            MiicCondition orgIDCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyNameWithTable<OrganizationInfo, string>(o => o.OrgID),
                orgID,
                DbType.String,
                MiicDBOperatorSetting.Equal);
            MiicConditionSingle condition = new MiicConditionSingle(orgIDCondition);
            MiicRelation relation = new MiicRelation(Config.Attribute.GetSqlTableNameByClassName<OrganizationInfo>(),
                Config.Attribute.GetSqlColumnNameByPropertyName<OrganizationInfo, string>(o => o.OrgID),
                Config.Attribute.GetSqlTableNameByClassName<OrgProductInfo>(),
                Config.Attribute.GetSqlColumnNameByPropertyName<OrgProductInfo, string>(o => o.OrgID),
                MiicDBOperatorSetting.Equal,
                MiicDBRelationSetting.LeftJoin);
            MiicColumnCollections columns = new MiicColumnCollections();
            MiicColumn orgAllColumns = new MiicColumn(Config.Attribute.GetSqlTableNameByClassName<OrganizationInfo>());
            columns.Add(orgAllColumns);
            MiicColumn fileIDColumn = new MiicColumn(Config.Attribute.GetSqlTableNameByClassName<OrgProductInfo>(),
                Config.Attribute.GetSqlColumnNameByPropertyName<OrgProductInfo, string>(o => o.ID));
            columns.Add(fileIDColumn);
            MiicColumn fileNameColumn = new MiicColumn(Config.Attribute.GetSqlTableNameByClassName<OrgProductInfo>(),
                Config.Attribute.GetSqlColumnNameByPropertyName<OrgProductInfo, string>(o => o.FileName));
            columns.Add(fileNameColumn);
            MiicColumn filePathColumn = new MiicColumn(Config.Attribute.GetSqlTableNameByClassName<OrgProductInfo>(),
                Config.Attribute.GetSqlColumnNameByPropertyName<OrgProductInfo, string>(o => o.FilePath));
            columns.Add(filePathColumn);
            MiicColumn uploadTimeColumn = new MiicColumn(Config.Attribute.GetSqlTableNameByClassName<OrgProductInfo>(),
                Config.Attribute.GetSqlColumnNameByPropertyName<OrgProductInfo, DateTime?>(o => o.UploadTime));
            columns.Add(uploadTimeColumn);
            MiicColumn fileTypeColumn = new MiicColumn(Config.Attribute.GetSqlTableNameByClassName<OrgProductInfo>(),
                Config.Attribute.GetSqlColumnNameByPropertyName<OrgProductInfo, string>(o => o.FilePath));
            columns.Add(fileTypeColumn);
            try
            {
                result = dbService.GetInformations(columns, relation, condition, out message);
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
        /// 根据企业ID查询企业产品展示附件列表
        /// </summary>
        /// <param name="orgID">企业ID</param>
        /// <returns>产品展示附件列表</returns>
        public List<OrgProductInfo> GetOrgProductList(string orgID)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(orgID), "参数orgID:不能为空");
            List<OrgProductInfo> result = new List<OrgProductInfo>();
            MiicCondition orgIDCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<OrgProductInfo, string>(o => o.OrgID),
                orgID,
                DbType.String,
                MiicDBOperatorSetting.Equal);
            string message = string.Empty;
            MiicColumn allColumn = new MiicColumn(Config.Attribute.GetSqlTableNameByClassName<OrgProductInfo>());
            MiicColumnCollections columns = new MiicColumnCollections();
            columns.Add(allColumn);
            MiicConditionSingle condition = new MiicConditionSingle(orgIDCondition);
            try
            {
                DataTable dt = dbService.GetInformations<OrgProductInfo>(columns, condition, out message);
                if (dt.Rows.Count != 0)
                {
                    foreach (var item in dt.AsEnumerable())
                    {
                        result.Add(new OrgProductInfo()
                        {
                            ID = item[Config.Attribute.GetSqlColumnNameByPropertyName<OrgProductInfo, string>(o => o.ID)].ToString(),
                            OrgID = item[Config.Attribute.GetSqlColumnNameByPropertyName<OrgProductInfo, string>(o => o.OrgID)].ToString(),
                            FileName = item[Config.Attribute.GetSqlColumnNameByPropertyName<OrgProductInfo, string>(o => o.FileName)].ToString(),
                            FilePath = item[Config.Attribute.GetSqlColumnNameByPropertyName<OrgProductInfo, string>(o => o.FilePath)].ToString(),
                            UploadTime = (DateTime?)item[Config.Attribute.GetSqlColumnNameByPropertyName<OrgProductInfo, DateTime?>(o => o.UploadTime)]
                        });
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
    }
}
