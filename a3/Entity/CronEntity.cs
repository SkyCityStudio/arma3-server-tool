using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace a3.Entity
{

    public class CronEntity
    {
        public string TaskId { get; set; }
        public string ServerUUID { get; set; }
        public string ServerName { get; set; }
        public string Cron { get; set; }
        public string Remark { get; set; }
        public int Status { get; set; }
        public int Action { get; set; }
        public string ActionText { get; set; }

        public CronEntity(string taskId, string serverUUID, string serverName, string cron, string remark, int status, int action,string actionText)
        {
            TaskId = taskId;
            ServerUUID = serverUUID;
            ServerName = serverName;
            Cron = cron;
            Remark = remark;
            Status = status;
            Action = action;
            ActionText = actionText;
        }

        public CronEntity()
        {
        }

        public string String()
        {
            return "任务ID:"+ TaskId+"\r\n"+"服务器UUID:" + ServerUUID + "\r\n" + "服务器昵称:"+ServerName + "\r\n" + "CRON表达式:" + Cron+"\r\n" + "备注:" +Remark+ "\r\n" + "需要执行的操作:"+ActionText + "\r\n" ;
        }
    }
}
