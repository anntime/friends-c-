using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miic.Manage.User.Setting
{
    public enum UserLevelSetting
    {
        [Description("Level1级")]
        Level1 = 1,
        [Description("Level2级")]
        Level2 = 2,
        [Description("Level3级")]
        Level3 = 3,
        [Description("Level4级")]
        Level4 = 4,
        [Description("Level5级")]
        Level5 = 5,
        [Description("Level6级")]
        Level6 = 6,
        [Description("Level7级")]
        Level7 = 7,
        [Description("VIP")]
        LevelMax = 9

    }
}
