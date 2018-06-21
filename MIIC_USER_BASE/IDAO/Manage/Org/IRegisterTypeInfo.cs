using Miic.Base;
using Miic.BaseStruct;
using System.Collections.Generic;
using System.Data;

namespace Miic.Manage.Org
{
    public interface IRegisterTypeInfo : ICommon<RegisterTypeInfo>
    {
        /// <summary>
        /// 获取注册类型信息集合
        /// </summary>
        /// <param name="pid">pid</param>
        /// <returns>注册类型信息集合</returns>
        DataTable GetRegisterTypeInfosByPID(string pid);
        /// <summary>
        /// 根据注册类别子ID查询器所有父ID 键值对队列
        /// </summary>
        /// <param name="registerTypeID">注册类别ID</param>
        /// <returns>所有父ID 键值对队列</returns>
        List<MiicKeyValue> GetRegisterTypeParentsByID(string registerTypeID);
    }
}
