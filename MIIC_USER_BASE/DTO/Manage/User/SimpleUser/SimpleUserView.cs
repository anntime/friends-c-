using Miic.Attribute;
using System;
using System.Data;

namespace Miic.Manage.User
{
    [MiicTable(MiicParentStorageName = "MIIC_SOCIAL_COMMON", MiicStorageName = "SIMPLE_USER_VIEW", Description = "简略所有用户信息视图")]
    public class SimpleUserView:GeneralSimpleUserView
    {
        
    }
}
