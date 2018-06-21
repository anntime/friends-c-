using Miic.Attribute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miic.Manage.User
{
    [MiicTable(MiicParentStorageName = "MIIC_SOCIAL_COMMON", MiicStorageName = "SEARCH_USER_VIEW", Description = "用户搜索视图表")]
    public partial class SearchUserView:GeneralSearchUserView
    {
        

    }
}
