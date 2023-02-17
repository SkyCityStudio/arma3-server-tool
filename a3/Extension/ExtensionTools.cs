using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace a3.Extension
{
    public class ExtensionTools
    {
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern int FindWindow(string lpClassName, string lpWindowName);



        public static int FindServerWindow(string lpClassName, string lpWindowName) {
           return FindWindow(lpClassName, lpWindowName);
        }
    }
}
