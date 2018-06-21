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
using System.Linq;
using System.Reflection;
using System.Text; 
using System.Threading.Tasks;

namespace Miic.Manage.Org
{
    public partial class ShareTypeInfoDao : NoRelationCommon<ShareTypeInfo>, IShareTypeInfo
    {
        private static readonly string ClassName = MethodBase.GetCurrentMethod().DeclaringType.Name;
        private static readonly string NamespaceName = MethodBase.GetCurrentMethod().DeclaringType.Namespace;
        bool ICommon<ShareTypeInfo>.Insert(ShareTypeInfo shareType)
        {
            Contract.Requires<ArgumentNullException>(shareType != null, "参数shareInfo:不能为空");
            bool result = false;
            string message1 = string.Empty;
            string message2 = string.Empty;
            string message = string.Empty;
            List<string> sqls = new List<string>();
            sqls.Add(DBService.InsertSql(shareType, out message1));
            sqls.Add(DBService.InsertSql(new DimShareType()
            {
                ShareTypeID=shareType.ID,
                Name =shareType.Name,
                BeginTime = DateTime.Now,
                Valid = ((int)MiicValidTypeSetting.Valid).ToString()
            }, out message2));
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
                InsertCache(shareType);
            }
            return result;
        }

        bool ICommon<ShareTypeInfo>.Update(ShareTypeInfo shareType)
        {
            Contract.Requires<ArgumentNullException>(shareType != null, "参数shareInfo:不能为空");
            ShareTypeInfo temp = ((ICommon<ShareTypeInfo>)this).GetInformation(shareType.ID);
            bool result = false;
            string message = string.Empty;
            string message1 = string.Empty;
            string message2 = string.Empty;
            string message3 = string.Empty;
            List<string> sqls = new List<string>();
            sqls.Add(DBService.UpdateSql(shareType, out message1));
            if (temp.Name != shareType.Name)
            {
                MiicConditionCollections conditions = new MiicConditionCollections(MiicDBLogicSetting.No);
                MiicCondition shareTypeID = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DimShareType, string>(o => o.ShareTypeID),
                   shareType.ID,
                   DbType.String,
                   MiicDBOperatorSetting.Equal);

                conditions.Add(new MiicConditionLeaf(MiicDBLogicSetting.No, shareTypeID));
                MiicCondition valid = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DimShareType, string>(o => o.Valid),
                    ((int)MiicValidTypeSetting.Valid).ToString(),
                    DbType.String,
                    MiicDBOperatorSetting.Equal);
                conditions.Add(new MiicConditionLeaf(valid));
                sqls.Add(DBService.UpdateConditionsSql(new DimShareType()
                {
                    EndTime = DateTime.Now,
                    Valid = ((int)MiicValidTypeSetting.InValid).ToString()
                }, conditions, out message2));
                sqls.Add(DBService.InsertSql(new DimShareType()
                {
                    ShareTypeID = shareType.ID,
                    Name = shareType.Name,
                    BeginTime = DateTime.Now,
                    Valid = ((int)MiicValidTypeSetting.Valid).ToString()
                }, out message3));
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
                DeleteCache(o => o.ID == shareType.ID);
            }
            return result;
        }

        bool ICommon<ShareTypeInfo>.Delete(string id)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(id), "参数id:不能为空");
            bool result = false;
            string message = string.Empty;
            string message1 = string.Empty;
            string message2 = string.Empty;
            List<string> sqls = new List<string>();
            sqls.Add(DBService.DeleteSql(new ShareTypeInfo()
            {
                ID = id
            }, out message1));
            MiicConditionCollections conditions = new MiicConditionCollections(MiicDBLogicSetting.No);
            MiicCondition shareTypeID = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DimShareType, string>(o => o.ShareTypeID),
                id,
                DbType.String,
                MiicDBOperatorSetting.Equal);

            conditions.Add(new MiicConditionLeaf(MiicDBLogicSetting.No, shareTypeID));
            MiicCondition valid = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DimShareType, string>(o => o.Valid),
                ((int)MiicValidTypeSetting.Valid).ToString(),
                DbType.String,
                MiicDBOperatorSetting.Equal);
            conditions.Add(new MiicConditionLeaf(valid));
            sqls.Add(DBService.UpdateConditionsSql<DimShareType>(new DimShareType()
            {
                EndTime = DateTime.Now,
                Valid = ((int)MiicValidTypeSetting.InValid).ToString()
            }, conditions, out message2));
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
                DeleteCache(o => o.ID == id);
            }
            return result;
        }

        ShareTypeInfo ICommon<ShareTypeInfo>.GetInformation(string id)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(id), "参数id:不能为空");
            ShareTypeInfo result = null;
            string message = string.Empty;
            try
            {
                result = items.Find(o => o.ID == id);
                if (result == null)
                {
                    result = dbService.GetInformation(new ShareTypeInfo
                    {
                        ID = id
                    }, out message);
                    if (result != null)
                    {
                        InsertCache(result);
                    }
                }
                else
                {
                    string serializer = Config.Serializer.Serialize(result);
                    result =Config.Attribute.ConvertObjectWithDateTime(Config.Serializer.Deserialize<ShareTypeInfo>(serializer));
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

        public DataTable GetAllShareTypeInfos()
        {
            DataTable result = new DataTable();
            string message = string.Empty;
            List<MiicOrderBy> orders = new List<MiicOrderBy>();
            orders.Add(new MiicOrderBy()
            {
                Desc = false,
                PropertyName = Config.Attribute.GetSqlColumnNameByPropertyName<ShareTypeInfo, int?>(o => o.SortNo)
            });
            try
            {
                result = dbService.GetInformations<ShareTypeInfo>(null, orders, out message);
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
