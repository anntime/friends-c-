using Miic.Base;
using Miic.Base.Setting;
using Miic.BaseStruct;
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
    public partial class RegisterTypeInfoDao:NoRelationCommon<RegisterTypeInfo>, IRegisterTypeInfo
    {
        private static readonly string ClassName = MethodBase.GetCurrentMethod().DeclaringType.Name;
        private static readonly string NamespaceName = MethodBase.GetCurrentMethod().DeclaringType.Namespace;
        public DataTable GetRegisterTypeInfosByPID(string pid)
        {
            DataTable result = new DataTable();
            string message = string.Empty;
            MiicConditionCollections conditions = new MiicConditionCollections(MiicDBLogicSetting.No);
            MiicCondition pidCondition;
            if (!string.IsNullOrEmpty(pid))
            {
                pidCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<RegisterTypeInfo, string>(o => o.PID),
                    pid,
                    DbType.String,
                    MiicDBOperatorSetting.Equal);
            }
            else 
            {
                pidCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<RegisterTypeInfo, string>(o => o.PID),
                    null,
                    DbType.String,
                    MiicDBOperatorSetting.IsNull);
            }
            conditions.Add(new MiicConditionLeaf(MiicDBLogicSetting.No, pidCondition));
            List<MiicOrderBy> orders = new List<MiicOrderBy>();
            orders.Add(new MiicOrderBy()
            {
                 Desc=false,
                 PropertyName=Config.Attribute.GetSqlColumnNameByPropertyName<RegisterTypeInfo,int?>(o=>o.SortNo)
            });
            conditions.order = orders;
            try
            {
                result = dbService.GetInformations<RegisterTypeInfo>(null, conditions, out message);
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

        bool ICommon<RegisterTypeInfo>.Insert(RegisterTypeInfo registerType)
        {
            Contract.Requires<ArgumentNullException>(registerType != null, "参数registerType:不能为空");
            bool result = false;
            string message1 = string.Empty;
            string message2 = string.Empty;
            string message = string.Empty;
            List<string> sqls = new List<string>();
            sqls.Add(DBService.InsertSql(registerType, out message1));
            sqls.Add(DBService.InsertSql(new DimRegisterType()
            {
                RegisterTypeID=registerType.ID,
                Name = registerType.Name,
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
                InsertCache(registerType);
            }
            return result;
        }

        bool ICommon<RegisterTypeInfo>.Update(RegisterTypeInfo registerType)
        {
            Contract.Requires<ArgumentNullException>(registerType != null, "参数registerType:不能为空");
            RegisterTypeInfo temp = ((ICommon<RegisterTypeInfo>)this).GetInformation(registerType.ID);

            bool result = false;
            string message = string.Empty;
            string message1 = string.Empty;
            string message2 = string.Empty;
            string message3 = string.Empty;
            List<string> sqls = new List<string>();
            sqls.Add(DBService.UpdateSql(registerType, out message1));
            if ((temp.Name != registerType.Name&&registerType.Name!=null)||(temp.PID!=registerType.PID&&registerType.PID!=null))
            {
                MiicConditionCollections conditions = new MiicConditionCollections(MiicDBLogicSetting.No);
                MiicCondition registerID = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DimRegisterType, string>(o => o.RegisterTypeID),
                   registerType.ID,
                   DbType.String,
                   MiicDBOperatorSetting.Equal);

                conditions.Add(new MiicConditionLeaf(MiicDBLogicSetting.No, registerID));
                MiicCondition valid = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DimRegisterType, string>(o => o.Valid),
                    ((int)MiicValidTypeSetting.Valid).ToString(),
                    DbType.String,
                    MiicDBOperatorSetting.Equal);
                conditions.Add(new MiicConditionLeaf(valid));
                sqls.Add(DBService.UpdateConditionsSql(new DimRegisterType()
                {
                    EndTime = DateTime.Now,
                    Valid = ((int)MiicValidTypeSetting.InValid).ToString()
                }, conditions, out message2));
                sqls.Add(DBService.InsertSql(new DimRegisterType()
                {
                    RegisterTypeID = registerType.ID,
                    PID=(temp.PID!=registerType.PID&&registerType.PID!=null)?registerType.PID:temp.PID,
                    Name = (temp.Name != registerType.Name&&registerType.Name!=null)?registerType.Name:temp.Name,
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
                DeleteCache(o => o.ID == registerType.ID);
            }
            return result;
        }

        bool ICommon<RegisterTypeInfo>.Delete(string id)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(id), "参数id:不能为空");
            bool result = false;
            string message = string.Empty;
            string message1 = string.Empty;
            string message2 = string.Empty;
            List<string> sqls = new List<string>();
            sqls.Add(DBService.DeleteSql(new RegisterTypeInfo()
            {
                ID = id
            }, out message1));
            MiicConditionCollections conditions = new MiicConditionCollections(MiicDBLogicSetting.No);
            MiicCondition registerTypeID = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DimRegisterType, string>(o => o.RegisterTypeID),
                id,
                DbType.String,
                MiicDBOperatorSetting.Equal);

            conditions.Add(new MiicConditionLeaf(MiicDBLogicSetting.No, registerTypeID));
            MiicCondition valid = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DimRegisterType, string>(o => o.Valid),
                ((int)MiicValidTypeSetting.Valid).ToString(),
                DbType.String,
                MiicDBOperatorSetting.Equal);
            conditions.Add(new MiicConditionLeaf(valid));
            sqls.Add(DBService.UpdateConditionsSql<DimRegisterType>(new DimRegisterType()
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

        RegisterTypeInfo ICommon<RegisterTypeInfo>.GetInformation(string id)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(id), "参数id:不能为空");
            RegisterTypeInfo result = null;
            string message = string.Empty;
            try
            {
                result = items.Find(o => o.ID == id);
                if (result == null)
                {
                    result = dbService.GetInformation(new RegisterTypeInfo
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
                    result =Config.Attribute.ConvertObjectWithDateTime(Config.Serializer.Deserialize<RegisterTypeInfo>(serializer));
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
        /// 根据注册类别子ID查询器所有父ID 键值对队列
        /// </summary>
        /// <param name="registerTypeID">注册类别ID</param>
        /// <returns>所有父ID 键值对队列</returns>
        public List<MiicKeyValue> GetRegisterTypeParentsByID(string registerTypeID)
        {
            List<MiicKeyValue> result = new List<MiicKeyValue>();
            string message = string.Empty;
            //此处调用表值函数 按ID正序排列出树形结构
            string sql = "select * from MIIC_SOCIAL_COMMON.dbo.GetRegisterTypeParentsByChildID('" + registerTypeID + "') order by ID asc";
            try
            {
                DataTable dt = dbService.querySql(sql, out message);
                if (dt != null && dt.Rows.Count != 0)
                {
                    foreach (var dr in dt.AsEnumerable())
                    {
                        result.Add(new MiicKeyValue()
                        {
                            Name = dr[Config.Attribute.GetSqlColumnNameByPropertyName<RegisterTypeInfo, string>(o => o.ID)].ToString(),
                            Value = dr[Config.Attribute.GetSqlColumnNameByPropertyName<RegisterTypeInfo, string>(o => o.Name)].ToString()
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
