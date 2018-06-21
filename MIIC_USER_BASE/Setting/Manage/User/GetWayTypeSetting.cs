using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miic.Manage.User.Setting
{
    public enum GetWayTypeSetting
    {
        [Description("行为方式获得")]
        Behavior = 1,
        [Description("发布方式获得")]
        Publish = 2,
        [Description("评论方式获得")]
        Comment = 3
        //带扩充
    }
}
