using Miic.Base.Setting;

namespace Miic.Common
{
    public partial class ValidView
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 有效性
        /// </summary>
        public MiicValidTypeSetting Valid { get; set; }
    }
}
