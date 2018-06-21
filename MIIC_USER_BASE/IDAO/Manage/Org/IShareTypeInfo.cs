using Miic.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miic.Manage.Org
{
    public interface IShareTypeInfo:ICommon<ShareTypeInfo>
    {
        /// <summary>
        /// 获取控股类型集合列表
        /// </summary>
        /// <returns>控股类型集合列表</returns>
        DataTable GetAllShareTypeInfos();
    }
}
