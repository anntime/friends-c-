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
    public partial class TradeInfoDao : NoRelationCommon<TradeInfo>, ITradeInfo
    {
        private static readonly string ClassName = MethodBase.GetCurrentMethod().DeclaringType.Name;
        private static readonly string NamespaceName = MethodBase.GetCurrentMethod().DeclaringType.Namespace;

        public TradeInfoDao() { }

        bool ICommon<TradeInfo>.Insert(TradeInfo tradeInfo)
        {
            Contract.Requires<ArgumentNullException>(tradeInfo != null, "参数tradeInfo：不能为空！");
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(tradeInfo.ID), "参数tradeInfo.ID：不能为空！");
            bool result = false;
            string message1 = string.Empty;
            string message2 = string.Empty;
            string message = string.Empty;
            List<string> sqls = new List<string>();
            sqls.Add(DBService.InsertSql(tradeInfo, out message1));
            sqls.Add(DBService.InsertSql(new DimTradeInfo()
            {
                TradeID = tradeInfo.ID,
                Name = tradeInfo.Name,
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
                InsertCache(tradeInfo);
            }
            return result;
        }

        bool ICommon<TradeInfo>.Update(TradeInfo tradeInfo)
        {
            Contract.Requires<ArgumentNullException>(tradeInfo != null, "参数tradeInfo:不能为空！");
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(tradeInfo.ID), "参数tradeInfo.ID:不能为空，因为是主键");
            bool result = false;
            string message = string.Empty;
            string message1 = string.Empty;
            string message2 = string.Empty;
            string message3 = string.Empty;
            List<string> sqls = new List<string>();
            sqls.Add(DBService.UpdateSql(tradeInfo, out message1));
            TradeInfo temp = ((ICommon<TradeInfo>)this).GetInformation(tradeInfo.ID);

            if (!string.IsNullOrEmpty(tradeInfo.Name) && temp.Name != tradeInfo.Name)
            {
                MiicConditionCollections conditions = new MiicConditionCollections(MiicDBLogicSetting.No);
                MiicCondition tradeID = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DimTradeInfo, string>(o => o.TradeID),
                   tradeInfo.ID,
                   DbType.String,
                   MiicDBOperatorSetting.Equal);

                conditions.Add(new MiicConditionLeaf(MiicDBLogicSetting.No, tradeID));
                MiicCondition valid = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DimTradeInfo, string>(o => o.Valid),
                    ((int)MiicValidTypeSetting.Valid).ToString(),
                    DbType.String,
                    MiicDBOperatorSetting.Equal);
                conditions.Add(new MiicConditionLeaf(valid));
                sqls.Add(DBService.UpdateConditionsSql(new DimTradeInfo()
                {
                    EndTime = DateTime.Now,
                    Valid = ((int)MiicValidTypeSetting.InValid).ToString()
                }, conditions, out message2));

                sqls.Add(DBService.InsertSql(new DimTradeInfo()
                {
                    TradeID = tradeInfo.ID,
                    Name = (!string.IsNullOrEmpty(tradeInfo.Name) && temp.Name != tradeInfo.Name) ? tradeInfo.Name : temp.Name,
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
                DeleteCache(o => o.ID == tradeInfo.ID);
            }
            return result;
        }

        bool ICommon<TradeInfo>.Delete(string id)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(id), "参数id:不能为空");
            bool result = false;
            string message = string.Empty;
            string message1 = string.Empty;
            string message2 = string.Empty;
            string message3 = string.Empty;
            string message4 = string.Empty;
            List<string> sqls = new List<string>();
            MiicCondition idCondition = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<TradeSubInfo, string>(o => o.TradeID),
                id,
                DbType.String,
                MiicDBOperatorSetting.Equal);
            MiicConditionSingle condition = new MiicConditionSingle(idCondition);
            //级联删除子行业
            sqls.Add(DBService.DeleteConditionSql<TradeSubInfo>(condition, out message2));
            //删除行业
            sqls.Add(DBService.DeleteSql<TradeInfo>(new TradeInfo()
            {
                ID = id
            }, out message1));

            //更新维度表
            MiicConditionCollections tradeConditions = new MiicConditionCollections(MiicDBLogicSetting.No);
            MiicCondition tradeID = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DimTradeInfo, string>(o => o.TradeID),
                id,
                DbType.String,
                MiicDBOperatorSetting.Equal);

            tradeConditions.Add(new MiicConditionLeaf(MiicDBLogicSetting.No, tradeID));
            MiicCondition valid = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DimTradeInfo, string>(o => o.Valid),
                ((int)MiicValidTypeSetting.Valid).ToString(),
                DbType.String,
                MiicDBOperatorSetting.Equal);
            tradeConditions.Add(new MiicConditionLeaf(valid));
            sqls.Add(DBService.UpdateConditionsSql<DimTradeInfo>(new DimTradeInfo()
            {
                EndTime = DateTime.Now,
                Valid = ((int)MiicValidTypeSetting.InValid).ToString()
            }, tradeConditions, out message3));
            MiicConditionCollections subTradeConditions = new MiicConditionCollections(MiicDBLogicSetting.No);
            MiicCondition subTradeID = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DimTradeSubInfo, string>(o => o.TradeID),
              id,
              DbType.String,
              MiicDBOperatorSetting.Equal);

            subTradeConditions.Add(new MiicConditionLeaf(MiicDBLogicSetting.No, subTradeID));
            MiicCondition subTradeValid = new MiicCondition(Config.Attribute.GetSqlColumnNameByPropertyName<DimTradeSubInfo, string>(o => o.Valid),
                ((int)MiicValidTypeSetting.Valid).ToString(),
                DbType.String,
                MiicDBOperatorSetting.Equal);
            subTradeConditions.Add(new MiicConditionLeaf(subTradeValid));
            sqls.Add(DBService.UpdateConditionsSql<DimTradeSubInfo>(new DimTradeSubInfo()
            {
                EndTime = DateTime.Now,
                Valid = ((int)MiicValidTypeSetting.InValid).ToString()
            }, subTradeConditions, out message4));
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
                TradeSubInfoDao.DeleteCache(o=>o.TradeID==id);
            }
            return result;
        }

        TradeInfo ICommon<TradeInfo>.GetInformation(string id)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(id), "参数id:不能为空");
            TradeInfo result = null;
            string message = string.Empty;
            try
            {
                result = items.Find(o => o.ID == id);
                if (result == null)
                {
                    result = dbService.GetInformation(new TradeInfo
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
                    result =Config.Attribute.ConvertObjectWithDateTime(Config.Serializer.Deserialize<TradeInfo>(serializer));
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
        /// 获取规模集合列表
        /// </summary>
        /// <returns>规模集合列表</returns>
        public DataTable GetAllTradesInfos()
        {
            DataTable result = new DataTable();
            string message = string.Empty;
            List<MiicOrderBy> orders = new List<MiicOrderBy>();
            orders.Add(new MiicOrderBy()
            {
                Desc = false,
                PropertyName = Config.Attribute.GetSqlColumnNameByPropertyName<TradeInfo, int?>(o => o.SortNo)
            });
            try
            {
                result = dbService.GetInformations<TradeInfo>(null, orders, out message);
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
