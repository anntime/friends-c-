using Miic.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miic.Manage.User
{
     [MiicTable(MiicParentStorageName = "MIIC_SOCIAL_COMMON", MiicStorageName = "SIMPLE_ORG_USER_VIEW", Description = "简略所有用户（机构）信息视图")]
    public partial class SimpleOrgUserView : GeneralSimpleUserView
    {
    }
}
