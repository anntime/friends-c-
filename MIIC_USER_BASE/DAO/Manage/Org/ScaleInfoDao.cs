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
    public class ScaleInfoDao : NoRelationCommon<ScaleInfo>, IScaleInfo
    {
        private static readonly string ClassName = MethodBase.GetCurrentMethod().DeclaringType.Name;
        private static readonly string NamespaceName = MethodBase.GetCurrentMethod().DeclaringType.Namespace;
        bool ICommon<ScaleInfo>.Insert(ScaleInfo scaleInfo)
        {
            Contract.Requires<ArgumentNullException>(scaleInfo != null, "参数scaleInfo:不能为空");
            bool result = false;
            string message1 = string.Empty;
            string message2 = string.Empty;
            string message = string.Empty;
            List<string> sqls = new List<string>();
            sqls.Add(DBService.InsertSql(scaleInfo, out message1));
            sqls.Add(DBService.InsertSql(new DimScaleInfo()
            {
                ScaleID = scaleInfo.ID,
                Name = scaleInfo.Name,
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
                InsertCache(scaleInfo);
            }
            return result;
        }

        bool ICommon<ScaleInfo>.Update(ScaleInfo scaleInfo)
        {
            Contract.Requires<ArgumentNullException>(scaleInfo != null, "参数scaleInfo:不能为空");
            ScaleInfo temp = ((ICommon<ScaleInfo>)this).GetInformation(scaleInfo.ID);
          
            bool result = false;
            string message = string.Empty;
            string message1 = string.Empty;
            string message2 = string.Empty;
            string message3 = string.Empty;
            List<string> sqls = new List<string>();
            sqls.Add(DBService.UpdateSql(scaleInfo, out message1));
            if (temp.Name != scaleInfo.Name)
            {
                MiicConditionCollections conditions = new MiicConditionCollections(MiicDBLogicSetting.No);
                MiicCondition scaleID = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DimScaleInfo, string>(o => o.ScaleID),
                   scaleInfo.ID,
                   DbType.String,
                   MiicDBOperatorSetting.Equal);

                conditions.Add(new MiicConditionLeaf(MiicDBLogicSetting.No, scaleID));
                MiicCondition valid = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DimScaleInfo, string>(o => o.Valid),
                    ((int)MiicValidTypeSetting.Valid).ToString(),
                    DbType.String,
                    MiicDBOperatorSetting.Equal);
                conditions.Add(new MiicConditionLeaf(valid));
                sqls.Add(DBService.UpdateConditionsSql(new DimScaleInfo()
                {
                    EndTime = DateTime.Now,
                    Valid = ((int)MiicValidTypeSetting.InValid).ToString()
                }, conditions, out message2));
                sqls.Add(DBService.InsertSql(new DimScaleInfo()
                {
                    ScaleID = scaleInfo.ID,
                    Name = scaleInfo.Name,
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
                DeleteCache(o => o.ID == scaleInfo.ID);
            }
            return result;
        }

       bool ICommon<ScaleInfo>.Delete(string id)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(id), "参数id:不能为空");
            bool result = false;
            string message = string.Empty;
            string message1 = string.Empty;
            string message2 = string.Empty;
            List<string> sqls = new List<string>();
            sqls.Add(DBService.DeleteSql(new ScaleInfo()
            {
                ID = id
            }, out message1));
            MiicConditionCollections conditions = new MiicConditionCollections(MiicDBLogicSetting.No);
            MiicCondition scaleID = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DimScaleInfo, string>(o => o.ScaleID),
                id,
                DbType.String,
                MiicDBOperatorSetting.Equal);

            conditions.Add(new MiicConditionLeaf(MiicDBLogicSetting.No, scaleID));
            MiicCondition valid = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DimScaleInfo, string>(o => o.Valid),
                ((int)MiicValidTypeSetting.Valid).ToString(),
                DbType.String,
                MiicDBOperatorSetting.Equal);
            conditions.Add(new MiicConditionLeaf(valid));
            sqls.Add(DBService.UpdateConditionsSql<DimScaleInfo>(new DimScaleInfo()
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

       ScaleInfo ICommon<ScaleInfo>.GetInformation(string id)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(id), "参数id:不能为空");
            ScaleInfo result = null;
            string message = string.Empty;
            try
            {
                result = items.Find(o => o.ID == id);
                if (result == null)
                {
                    result = dbService.GetInformation(new ScaleInfo
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
                    result =Config.Attribute.ConvertObjectWithDateTime( Config.Serializer.Deserialize<ScaleInfo>(serializer));
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

       public DataTable GetAllScaleInfos()
       {
           DataTable result = new DataTable();
           string message = string.Empty;
           List<MiicOrderBy> orders = new List<MiicOrderBy>();
           orders.Add(new MiicOrderBy()
           {
               Desc=false,
               PropertyName=Config.Attribute.GetSqlColumnNameByPropertyName<ScaleInfo,int?>(o=>o.SortNo)
           });
           try
           {
               result = dbService.GetInformations<ScaleInfo>(null, orders, out message);
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
