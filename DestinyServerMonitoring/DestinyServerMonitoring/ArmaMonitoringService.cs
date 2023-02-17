using RGiesecke.DllExport;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

/**
* by 七龙
* 这个东西非常坑爹，对中国人用非常不友好，我的坑
* 首先必须把系统换成英文的,然后就是需要用Viewdll.exe查看是否有托管导出的函数
* 最后需要BuildTools_Full.exe编译错误依赖 https://www.microsoft.com/zh-CN/download/confirmation.aspx?id=48159
*

*/
namespace DestinyServerMonitoring
{

    public class ArmaMonitoringService
    {


        public static ExtensionCallback callback;
        public delegate int ExtensionCallback([MarshalAs(UnmanagedType.LPStr)] string name, [MarshalAs(UnmanagedType.LPStr)] string function, [MarshalAs(UnmanagedType.LPStr)] string data);


        //"DestinyServerMonitoring" callExtension ["ObjectManipulationNum", ["1"]];

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(
        int hWnd, // handle to destination window    
        int Msg, // message    
        int wParam, // first message parameter    
        ref COPYDATASTRUCT lParam // second message parameter    
        );

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern int FindWindow(string lpClassName, string lpWindowName);

        public struct COPYDATASTRUCT
        {

            public IntPtr dwData;

            public int cbData;

            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;

        }




#if WIN64
        [DllExport("RVExtensionRegisterCallback", CallingConvention = CallingConvention.Winapi)]
#else
        [DllExport("_RVExtensionRegisterCallback@4", CallingConvention = CallingConvention.Winapi)]
#endif
        public static void RVExtensionRegisterCallback([MarshalAs(UnmanagedType.FunctionPtr)] ExtensionCallback func)
        {
            callback = func;
        }




        /// <summary> 
        ///当arma启动并加载所有扩展时被调用。
        ///最好在单独的线程中加载静态对象，以使扩展不需要任何单独的初始化
        /// </ summary> 
        /// <param name =“ output”>包含以下内容的字符串生成器对象：函数的结果</ param> 
        /// <param name =“ outputSize”>可以返回的最大字节数</ param> 
#if WIN64
        [DllExport("RVExtensionVersion", CallingConvention = CallingConvention.Winapi)]
#else
        [DllExport("_RVExtensionVersion@8", CallingConvention = CallingConvention.Winapi)]
#endif
        public static void RvExtensionVersion(StringBuilder output, int outputSize)
        {
            string version = "MonitoringService v1.0 dev ok.";
            outputSize = version.Length;
            output.Append(version);
        }



        /// <summary> 
        ///默认callExtension命令的入口点。
        /// </ summary> 
        /// <param name =“ output”>包含函数结果的字符串生成器对象</ param> 
        /// <param name =“ outputSize”>可以返回</ param> 
        /// <param name =“ function”>与callExtension一起使用的字符串参数</ param> 
#if WIN64
        [DllExport("RVExtension", CallingConvention = CallingConvention.Winapi)]
#else
        [DllExport("_RVExtension@12", CallingConvention = CallingConvention.Winapi)]
#endif
        public static void RvExtension(StringBuilder output, int outputSize,
            [MarshalAs(UnmanagedType.LPStr)] string function)
        {

            output.Append(function);
        }

        const int WM_COPYDATA = 0x004A;


        /// <summary> 
        /// callExtensionArgs命令的入口点。
        /// </ summary> 
        /// <param name =“ output”>包含函数结果的字符串生成器对象</ param> 
        /// <param name =“ outputSize”>可以返回</ param> 
        /// <param name =“ function”>与callExtension一起使用的字符串参数</ param> 
        /// <param name =“ args”>作为字符串传递给callExtension的args array </ param> 
        /// <param name =“ argsCount”>字符串数组args的大小</ param> 
        /// <returns>结果代码</ returns> 
#if WIN64
        [DllExport("RVExtensionArgs", CallingConvention = CallingConvention.Winapi)]
#else
        [DllExport("_RVExtensionArgs@20", CallingConvention = CallingConvention.Winapi)]
#endif
        public static int RvExtensionArgs(StringBuilder output, int outputSize,
            [MarshalAs(UnmanagedType.LPStr)] string function,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 4)] string[] args, int argCount)
        {
            for (int i = 0; i < args.Length; i++)
            {
               args[i] = args[i].Trim().Replace("\"", "");
                output.Append(args[i]);
            }
              Task.Run( () => {
                  int WINDOW_HANDLER = FindWindow(null, "A3-DestinyStudio-ProcessCommunicationModule");
                  if (WINDOW_HANDLER != 0)
                  {
                      byte[] sarr = Encoding.Default.GetBytes(args[0]);
                      int len = sarr.Length;
                      COPYDATASTRUCT cds;
                      cds.dwData = (IntPtr)100;
                      cds.lpData = args[0];
                      cds.cbData = len + 1;
                      SendMessage(WINDOW_HANDLER, WM_COPYDATA, 0, ref cds);
                  }
              });
              output.Append("ok");
              return 200;
        }

    }
}
