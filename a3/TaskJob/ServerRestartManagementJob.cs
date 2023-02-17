using a3.Config;
using a3.Entity;
using a3.Tools;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace a3.TaskJob
{
    public class ServerRestartManagementJob : IJob
    {


        public virtual Task Execute(IJobExecutionContext context)
        {
            CronEntity cron = (CronEntity)context.JobDetail.JobDataMap.Get("Arma3Config");
            string key = (string)context.JobDetail.JobDataMap.Get("TaskKey");
            if (cron.Status == 1 && DefaultConfig.ServerList.ContainsKey(cron.ServerUUID)) {
                StartManagementTools tools = new StartManagementTools(cron.ServerUUID);
                switch (cron.Action) {
                    case 0:
                        tools.Stop();
                        tools.Start();
                        break;
                   case 1:
                        tools.Start();
                        break;
                    case 2:
                        tools.Stop();
                        break;
                    case 3:
                        tools.DetectRestart();
                        break;
                }
            }
            return Task.CompletedTask;
        }
    }
}
