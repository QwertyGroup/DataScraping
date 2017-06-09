using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigDataCore
{
    public static class Extensions
    {
        public static List<string> SplitBy(this string tString, string splitString)
        {
            return tString.Split(new string[] { splitString }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public static List<string> SplitByAndTrim(this string tString, string splitString)
        {
            return tString.Split(new string[] { splitString }, StringSplitOptions.RemoveEmptyEntries).
                  Select(spl => spl.Trim()).ToList();
        }
    }
}
