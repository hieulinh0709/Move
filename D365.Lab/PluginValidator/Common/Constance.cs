using System.Collections.Generic;

namespace PluginValidator.Common
{
    public class Constance
    {
        public const int CountryPrefix = 100000001;

        public const int VinaPrefix = 100000001;
        public const int MobiPrefix = 100000002;
        public const int ViettelPrefix = 100000003;

        public Dictionary<int, string> EmailSuffix = new Dictionary<int, string>(){
            {100000001, "@aia.com"},
            {100000002, "@gmail.com"},
            {100000003, "@yahoo.com"}
        };
    }
}
