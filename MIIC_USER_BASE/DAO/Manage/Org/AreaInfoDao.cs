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
    public partial class AreaInfoDao : NoRelationCommon<AreaInfo>, IAreaInfo
    {
        private static readonly string ClassName = MethodBase.GetCurrentMethod().DeclaringType.Name;
        private static readonly string NamespaceName = MethodBase.GetCurrentMethod().DeclaringType.Namespace;
        public DataTable GetAreaInfosByPID(string pid)
        {
            DataTable result = new DataTable();
            string message = string.Empty;
            MiicConditionCollections conditions = new MiicConditionCollections(MiicDBLogicSetting.No);
            MiicCondition pidCondition;
            if (!string.IsNullOrEmpty(pid))
            {
                pidCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<AreaInfo, int?>(o => o.PID),
                      pid,
                     DbType.Int32,
                     MiicDBOperatorSetting.Equal);
            }
            else
            {
                pidCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<AreaInfo, int?>(o => o.PID),
                     null,
                    DbType.Int32,
                    MiicDBOperatorSetting.IsNull);
            }
            conditions.Add(new MiicConditionLeaf(MiicDBLogicSetting.No, pidCondition));
            MiicCondition validCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<AreaInfo, string>(o => o.Valid),
                ((int)MiicValidTypeSetting.Valid).ToString(),
                DbType.String,
                MiicDBOperatorSetting.Equal);
            conditions.Add(new MiicConditionLeaf(validCondition));
            try
            {
                result = dbService.GetInformations<AreaInfo>(null, conditions, out message);
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
        bool ICommon<AreaInfo>.Insert(AreaInfo areaInfo)
        {
            Contract.Requires<ArgumentNullException>(areaInfo != null, "参数areaInfo:不能为空");
            bool result = false;
            int count = 0;
            string message = string.Empty;
            areaInfo.BeginTime = DateTime.Now;
            areaInfo.EndTime = null;
            areaInfo.Valid = ((int)MiicValidTypeSetting.Valid).ToString();
            try
            {
                result = dbService.Insert(areaInfo, out count, out message);
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
                InsertCache(areaInfo);
            }
            return result;
        }

        bool ICommon<AreaInfo>.Update(AreaInfo areaInfo)
        {
            Contract.Requires<ArgumentNullException>(areaInfo != null, "参数areaInfo:不能为空");
            bool result = false;
            string message = string.Empty;
            string message1 = string.Empty;
            string message2 = string.Empty;
            List<string> sqls = new List<string>();
            AreaInfo temp = ((ICommon<AreaInfo>)this).GetInformation(areaInfo.ID.ToString());
            sqls.Add(DBService.UpdateSql(new AreaInfo()
            {
                ID = areaInfo.ID,
                EndTime = DateTime.Now,
                Valid = ((int)MiicValidTypeSetting.InValid).ToString()
            }, out message1));
            sqls.Add(DBService.InsertSql(new AreaInfo()
            {
                PID = areaInfo.PID != null ? areaInfo.PID : areaInfo.Type == ((int)MiicAreaCodeTypeSetting.Province).ToString() ? new Nullable<Int32>().GetValueOrDefault(Int32.MaxValue) : temp.PID,
                Code = !string.IsNullOrEmpty(areaInfo.Code) ? areaInfo.Code : temp.Code,
                Name = !string.IsNullOrEmpty(areaInfo.Name) ? areaInfo.Name : temp.Name,
                Type = !string.IsNullOrEmpty(areaInfo.Type) ? areaInfo.Type : temp.Type,
                FullName = !string.IsNullOrEmpty(areaInfo.FullName) ? areaInfo.FullName : temp.FullName,
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

                DeleteCache(o => o.ID == areaInfo.ID);
            }
            return result;
        }

        bool ICommon<AreaInfo>.Delete(string id)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(id), "参数id:不能为空");
            bool result = false;
            int count = 0;
            string message = string.Empty;
            try
            {
                result = dbService.Update(new AreaInfo()
                {
                    ID = int.Parse(id),
                    Valid = ((int)MiicValidTypeSetting.InValid).ToString(),
                    EndTime = DateTime.Now
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
                DeleteCache(o => o.ID == int.Parse(id));
            }
            return result;
        }

        AreaInfo ICommon<AreaInfo>.GetInformation(string id)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(id), "参数id:不能为空");
            AreaInfo result = null;
            string message = string.Empty;
            try
            {
                result = items.Find(o => o.ID == int.Parse(id));
                if (result == null)
                {
                    result = dbService.GetInformation(new AreaInfo
                    {
                        ID = int.Parse(id)
                    }, out message);
                    if (result != null)
                    {
                        InsertCache(result);
                    }
                }
                else
                {
                    string serializer = Config.Serializer.Serialize(result);
                    result = Config.Attribute.ConvertObjectWithDateTime(Config.Serializer.Deserialize<AreaInfo>(serializer));
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
