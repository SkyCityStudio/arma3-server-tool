using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace a3.Tools
{
    public class MissionsTool
    {
        public static int difficultyNameToInt(string difficulty) {
            switch (difficulty) {
                case "新兵":
                    return 0;
                case "正常":
                    return 1;
                case "老兵":
                    return 2;
                case "自定义":
                    return 3;
                case "关闭":
                    return 4;
                default: 
                    return 3;
            }
        }

        public static string intToDifficulty(int difficulty)
        {
            switch (difficulty)
            {
                case 0:
                    return "Recruit";
                case 1:
                    return "Regular";
                case 2:
                    return "Veteran";
                case 3:
                    return "Custom";
                case 4:
                    return "none";
                default:
                    return "Custom";
            }
        }

        public static int DifficultyToInt(string difficulty)
        {
            switch (difficulty)
            {

                case "Recruit":
                    return 0;
                case "Regular":
                    return 1;
                case "Veteran":
                    return 2;
                case "Custom":
                    return 3;
                case "none":
                    return 4;
                default:
                    return 3;
            }
        }

        public static bool getBoolean(string s) {
            return s == "True";
        }
    }
}
