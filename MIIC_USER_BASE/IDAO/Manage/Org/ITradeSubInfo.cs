using Miic.Base;
using Miic.BaseStruct;
using System.Collections.Generic;
using System.Data;

namespace Miic.Manage.Org
{
    public interface ITradeSubInfo : ICommon<TradeSubInfo>
    {
        /// <summary>
        /// 获取小行业信息集合
        /// </summary>
        /// <param name="pid">pid</param>
        /// <returns>小行业信息集合</returns>
        DataTable GetTradeSubInfosByPID(string pid);
        /// <summary>
        /// 根据大行业ID获取小行业信息集合
        /// </summary>
        /// <param name="tradeID">大行业ID</param>
        /// <returns>小行业信息集合</returns>
        DataTable GetTradeSubInfosByTradeID(string tradeID);
        /// <summary>
        /// 根据小行业ID查询其所有父ID 键值对
        /// </summary>
        /// <param name="tradeSubID">小行业ID</param>
        /// <returns>所有父ID信息 键值对</returns>
        List<MiicKeyValue> GetTradeParentsByTradeSubID(string tradeSubID);
    }
}
