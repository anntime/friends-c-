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
    public partial class TradeSubInfoDao : NoRelationCommon<TradeSubInfo>, ITradeSubInfo
    {
        private static readonly string ClassName = MethodBase.GetCurrentMethod().DeclaringType.Name;
        private static readonly string NamespaceName = MethodBase.GetCurrentMethod().DeclaringType.Namespace;
        public TradeSubInfoDao() { }

        /// <summary>
        /// 获取小行业信息集合
        /// </summary>
        /// <param name="pid">pid</param>
        /// <returns>小行业信息集合</returns>
        public DataTable GetTradeSubInfosByPID(string pid)
        {
            DataTable result = new DataTable();
            string message = string.Empty;
            MiicConditionCollections conditions = new MiicConditionCollections(MiicDBLogicSetting.No);
            MiicCondition pidCondition;
            if (!string.IsNullOrEmpty(pid))
            {
                pidCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<TradeSubInfo, string>(o => o.PID),
                    pid,
                    DbType.String,
                    MiicDBOperatorSetting.Equal);
            }
            else
            {
                pidCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<TradeSubInfo, string>(o => o.PID),
                    null,
                    DbType.String,
                    MiicDBOperatorSetting.IsNull);
            }
            conditions.Add(new MiicConditionLeaf(MiicDBLogicSetting.No, pidCondition));
            List<MiicOrderBy> orders = new List<MiicOrderBy>();
            orders.Add(new MiicOrderBy()
            {
                Desc = false,
                PropertyName = Config.Attribute.GetSqlColumnNameByPropertyName<TradeSubInfo, int?>(o => o.SortNo)
            });
            conditions.order = orders;
            try
            {
                result = dbService.GetInformations<TradeSubInfo>(null, conditions, out message);
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
        /// 根据大行业ID获取小行业信息集合
        /// </summary>
        /// <param name="tradeID">大行业ID</param>
        /// <returns>小行业信息集合</returns>
        public DataTable GetTradeSubInfosByTradeID(string tradeID)
        {
            DataTable result = new DataTable();
            string message = string.Empty;
            MiicConditionCollections conditions = new MiicConditionCollections(MiicDBLogicSetting.No);
            MiicCondition tradeIDCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<TradeSubInfo, string>(o => o.TradeID),
                    tradeID,
                    DbType.String,
                    MiicDBOperatorSetting.Equal);

            conditions.Add(new MiicConditionLeaf(MiicDBLogicSetting.No, tradeIDCondition));
            MiicCondition pidCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<TradeSubInfo, string>(o => o.PID),
                null,
                DbType.String,
                MiicDBOperatorSetting.IsNull);
            conditions.Add(new MiicConditionLeaf(pidCondition));
            List<MiicOrderBy> orders = new List<MiicOrderBy>();
            orders.Add(new MiicOrderBy()
            {
                Desc = false,
                PropertyName = Config.Attribute.GetSqlColumnNameByPropertyName<TradeSubInfo, int?>(o => o.SortNo)
            });
            conditions.order = orders;
            try
            {
                result = dbService.GetInformations<TradeSubInfo>(null, conditions, out message);
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

        bool ICommon<TradeSubInfo>.Insert(TradeSubInfo tradeSubInfo)
        {
            Contract.Requires<ArgumentNullException>(tradeSubInfo != null, "参数tradeSubInfo：不能为空！");
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(tradeSubInfo.ID), "参数tradeSubInfo.ID：不能为空！");
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(tradeSubInfo.TradeID), "参数tradeSubInfo.TradeID：不能为空！");
            bool result = false;
            string message1 = string.Empty;
            string message2 = string.Empty;
            string message = string.Empty;
            List<string> sqls = new List<string>();
            sqls.Add(DBService.InsertSql(tradeSubInfo, out message1));
            sqls.Add(DBService.InsertSql(new DimTradeSubInfo()
            {
                TradeSubID = tradeSubInfo.ID,
                Name = tradeSubInfo.Name,
                TradeID = tradeSubInfo.TradeID,
                PID = tradeSubInfo.PID,
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
                InsertCache(tradeSubInfo);
            }
            return result;
        }

        bool ICommon<TradeSubInfo>.Update(TradeSubInfo tradeSubInfo)
        {
            Contract.Requires<ArgumentNullException>(tradeSubInfo != null, "参数tradeSubInfo:不能为空！");
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(tradeSubInfo.ID), "参数tradeSubInfo.ID:不能为空，因为是主键");
            TradeSubInfo temp = ((ICommon<TradeSubInfo>)this).GetInformation(tradeSubInfo.ID);

            bool result = false;
            string message = string.Empty;
            string message1 = string.Empty;
            string message2 = string.Empty;
            string message3 = string.Empty;
            List<string> sqls = new List<string>();
            sqls.Add(DBService.UpdateSql(tradeSubInfo, out message1));
            if ((temp.Name != tradeSubInfo.Name && tradeSubInfo.Name != null) || (temp.PID != tradeSubInfo.PID && tradeSubInfo.PID != null))
            {
                MiicConditionCollections conditions = new MiicConditionCollections(MiicDBLogicSetting.No);
                MiicCondition tradeSubID = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DimTradeSubInfo, string>(o => o.TradeSubID),
                   tradeSubInfo.ID,
                   DbType.String,
                   MiicDBOperatorSetting.Equal);

                conditions.Add(new MiicConditionLeaf(MiicDBLogicSetting.No, tradeSubID));
                MiicCondition valid = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DimTradeSubInfo, string>(o => o.Valid),
                    ((int)MiicValidTypeSetting.Valid).ToString(),
                    DbType.String,
                    MiicDBOperatorSetting.Equal);
                conditions.Add(new MiicConditionLeaf(valid));
                sqls.Add(DBService.UpdateConditionsSql(new DimTradeSubInfo()
                {
                    EndTime = DateTime.Now,
                    Valid = ((int)MiicValidTypeSetting.InValid).ToString()
                }, conditions, out message2));
                sqls.Add(DBService.InsertSql(new DimTradeSubInfo()
                {
                    TradeSubID = tradeSubInfo.ID,
                    PID = (temp.PID != tradeSubInfo.PID && tradeSubInfo.PID != null) ? tradeSubInfo.PID : temp.PID,
                    Name = (temp.Name != tradeSubInfo.Name && tradeSubInfo.Name != null) ? tradeSubInfo.Name : temp.Name,
                    TradeID = (temp.TradeID != tradeSubInfo.TradeID && tradeSubInfo.TradeID != null) ? tradeSubInfo.TradeID : temp.TradeID,
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
                DeleteCache(o => o.ID == tradeSubInfo.ID);
            }
            return result;
        }

        bool ICommon<TradeSubInfo>.Delete(string id)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(id), "参数id:不能为空");
            bool result = false;
            string message = string.Empty;
            string message1 = string.Empty;
            string message2 = string.Empty;
            List<string> sqls = new List<string>();
            sqls.Add(DBService.DeleteSql(new TradeSubInfo()
            {
                ID = id
            }, out message1));
            MiicConditionCollections conditions = new MiicConditionCollections(MiicDBLogicSetting.No);
            MiicCondition tradeSubID = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DimTradeSubInfo, string>(o => o.TradeSubID),
                id,
                DbType.String,
                MiicDBOperatorSetting.Equal);

            conditions.Add(new MiicConditionLeaf(MiicDBLogicSetting.No, tradeSubID));
            MiicCondition valid = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DimTradeSubInfo, string>(o => o.Valid),
                ((int)MiicValidTypeSetting.Valid).ToString(),
                DbType.String,
                MiicDBOperatorSetting.Equal);
            conditions.Add(new MiicConditionLeaf(valid));
            sqls.Add(DBService.UpdateConditionsSql<DimTradeSubInfo>(new DimTradeSubInfo()
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

        TradeSubInfo ICommon<TradeSubInfo>.GetInformation(string id)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(id), "参数id:不能为空");
            TradeSubInfo result = null;
            string message = string.Empty;
            try
            {
                result = items.Find(o => o.ID == id);
                if (result == null)
                {
                    result = dbService.GetInformation(new TradeSubInfo
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
                    result =Config.Attribute.ConvertObjectWithDateTime(Config.Serializer.Deserialize<TradeSubInfo>(serializer));
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
        /// 根据小行业ID查询其所有父ID信息集合
        /// </summary>
        /// <param name="tradeSubID">小行业ID</param>
        /// <returns>所有父ID信息集合</returns>
        public List<MiicKeyValue> GetTradeParentsByTradeSubID(string tradeSubID)
        {
            List<MiicKeyValue> result = new List<MiicKeyValue>();
            string message = string.Empty;
            //此处调用表值函数 按ID正序排列 根节点第一位 子节点最后一位
            string sql = "select * from MIIC_SOCIAL_COMMON.dbo.GetTradeParentsByChildID('" + tradeSubID + "') order by ID asc";
            try
            {
                DataTable dt = dbService.querySql(sql, out message);
                if (dt != null && dt.Rows.Count != 0)
                {
                    foreach (var dr in dt.AsEnumerable())
                    {
                        result.Add(new MiicKeyValue()
                        {
                            Name = dr[Config.Attribute.GetSqlColumnNameByPropertyName<TradeSubInfo, string>(o => o.ID)].ToString(),
                            Value = dr[Config.Attribute.GetSqlColumnNameByPropertyName<TradeSubInfo, string>(o => o.Name)].ToString()
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
