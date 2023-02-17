using a3.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace a3.Tools
{
    public class StartManagementTools
    {
        public string serverUUID;

        public StartManagementTools(string serverUUID)
        {
            this.serverUUID = serverUUID;
        }

        public Process Execute(string path,string args) {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    Arguments = args,
                    FileName = path
                };
                return Process.Start(startInfo);
            }
            catch  {
                return null;
            }
        }

        public bool KillProcess(int pid) {
            try
            {
                Process.GetProcessById(pid).Kill();
                return true;
            }
            catch {
                return false;
            }
        }

        public void StartHeadlessClient() {
            string args = CfgTool.GetHeadlessClientCommandLine(serverUUID);
            Console.WriteLine(args);
            Execute(DefaultConfig.ServerList[serverUUID].ServerDir + (DefaultConfig.ServerList[serverUUID].x64 ? @"\arma3server_x64.exe" : @"\arma3server.exe"), args);
        }


        public void Start()
        {
            CfgTool.SaveStartCommandLine(serverUUID);
            Process p = Execute(DefaultConfig.ServerList[serverUUID].ServerDir + (DefaultConfig.ServerList[serverUUID].x64 ? @"\arma3server_x64.exe" : @"\arma3server.exe"), DefaultConfig.ServerList[serverUUID].StartCommandLine);
            if (p != null)
            {
                DefaultConfig.ServerList[serverUUID].ServerTaskManagement.ProcessById = p.Id;
            }
            else
            {

            }
        }

        public void Stop()
        {
            if (DefaultConfig.ServerList[serverUUID].ServerTaskManagement.ProcessById != 0)
            {
                bool isOk = KillProcess(DefaultConfig.ServerList[serverUUID].ServerTaskManagement.ProcessById);
                if (isOk)
                {

                }
                else
                {

                }
            }
        }


        public void DetectRestart()
        {
            try
            {
                Process.GetProcessById(DefaultConfig.ServerList[serverUUID].ServerTaskManagement.ProcessById);
            }
            catch
            {
                Start();
            }
        }

    }
}
