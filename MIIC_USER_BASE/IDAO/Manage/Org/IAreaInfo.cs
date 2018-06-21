using Miic.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miic.Manage.Org
{
    public interface IAreaInfo : ICommon<AreaInfo>
    {
        /// <summary>
        /// 根据PID获取子区域信息集合
        /// </summary>
        /// <param name="pid">父ID</param>
        /// <returns>子区域信息集合</returns>
        DataTable GetAreaInfosByPID(string pid);
    }
}
