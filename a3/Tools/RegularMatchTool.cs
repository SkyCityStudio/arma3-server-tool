using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace a3.Tools
{
    public class RegularMatchTool
    {
        public static Regex IsIntegerRegex = new Regex("^[0-9]{1,}$");

        public static int ToInteger(string value,int def) {
            return IsIntegerRegex.IsMatch(value)? Convert.ToInt32(value) : def;
        }

        public static long ToLong(string value, int def)
        {
            return IsIntegerRegex.IsMatch(value) ? Convert.ToInt64(value) : def;
        }


    }
}
