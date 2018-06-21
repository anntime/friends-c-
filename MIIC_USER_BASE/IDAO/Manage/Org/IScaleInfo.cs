using Miic.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miic.Manage.Org
{
    public interface IScaleInfo:ICommon<ScaleInfo>
    {
        /// <summary>
        /// 获取规模集合列表
        /// </summary>
        /// <returns>规模集合列表</returns>
        DataTable GetAllScaleInfos();
    }
}
