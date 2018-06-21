using Miic.Base;
using System.Data;

namespace Miic.Manage.Org
{
    public interface ITradeInfo : ICommon<TradeInfo>
    {
        /// <summary>
        /// 获取规模集合列表
        /// </summary>
        /// <returns>规模集合列表</returns>
        DataTable GetAllTradesInfos();
    }
}
