using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using log4net;
using Amazon.Models;
using System.Web.Mvc;
using System.Net;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace Amazon.Models
{
    public class LogHelper
    {
        public static void WriteLog(string logContent, Dictionary<string, string> cusProperties)
        {
            foreach(KeyValuePair<string, string> property in cusProperties)
            {
                log4net.LogicalThreadContext.Properties[property.Key] = property.Value;
            }
            WriteLog(null, logContent, Log4NetLevel.Error);
        }
        
        public static void WriteLog(string logContent, Log4NetLevel log4Level, Dictionary<string, string> cusProperties)
        {
            foreach (KeyValuePair<string, string> property in cusProperties)
            {
                log4net.LogicalThreadContext.Properties[property.Key] = property.Value;
            }
            WriteLog(null, logContent, log4Level);
        }
        
        public static void WriteLog(Type type, string logContent, Log4NetLevel log4Level)
        {
            ILog log = type == null ? LogManager.GetLogger("") : LogManager.GetLogger(type);

            switch (log4Level)
            {
                case Log4NetLevel.Warn:
                    log.Warn(logContent);
                    break;
                case Log4NetLevel.Debug:
                    log.Debug(logContent);
                    break;
                case Log4NetLevel.Info:
                    log.Info(logContent);
                    break;
                case Log4NetLevel.Fatal:
                    log.Fatal(logContent);
                    break;
                case Log4NetLevel.Error:
                    log.Error(logContent);
                    break;
            }
        }

    }

    public enum Log4NetLevel
    {
        [Description("Warning")]
        Warn = 1,
        [Description("Debug")]
        Debug = 2,
        [Description("Info")]
        Info = 3,
        [Description("Fatal")]
        Fatal = 4,
        [Description("Error")]
        Error = 5
    }
    
    public class LogVM
    {
        public string ID { set; get; }
        public string UserName { set; get; }
        public string Machine { set; get; }
        public string Url { set; get; }
        public int LogType { set; get; }
        public string LogLevel { set; get; }
        public string OperateModule { set; get; }
        public string Operate { set; get; }
        public string WFTID { set; get; }
        public string WFID { set; get; }
        public string NodeID { set; get; }
        public string Message { set; get; }
        public string Date { set; get; }

        public static void WriteLog(string uName, string machine, string url,
                int lType, Log4NetLevel lLevel, string oModule, string op, 
                string wftid, string wfid, string nid, string msg)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("uname", uName);
            dic.Add("machine", machine);
            dic.Add("url", url);
            dic.Add("ltype", lType.ToString());
            dic.Add("module", oModule);
            dic.Add("operate", op);
            dic.Add("wftid", wftid);
            dic.Add("wfid", wftid);
            dic.Add("nodeid", wftid);
            LogHelper.WriteLog(msg, lLevel, dic);
        }
        public static void WriteLogWithCtrl(Controller ctrl, int lType, Log4NetLevel lLevel, string oModule, string op,
            string wftid, string wfid, string nid, string msg)
        {
            var ckdict = CookieUtility.UnpackCookie(ctrl);
            var username = ckdict["logonuser"];

            var dic = new Dictionary<string, string>();
            dic.Add("uname", username);
            dic.Add("machine", DetermineCompName(ctrl.Request.UserHostName));
            dic.Add("url", ctrl.Request.Url.ToString());
            dic.Add("ltype", lType.ToString());
            dic.Add("module", oModule);
            dic.Add("operate", op);
            dic.Add("wftid", wftid);
            dic.Add("wfid", wfid);
            dic.Add("nodeid", nid);
            LogHelper.WriteLog(msg, lLevel, dic);
        }

        public static string DetermineCompName(string IP)
        {
            try
            {
                IPAddress myIP = IPAddress.Parse(IP);
                IPHostEntry GetIPHost = Dns.GetHostEntry(myIP);
                List<string> compName = GetIPHost.HostName.ToString().Split('.').ToList();
                return compName.First();
            }
            catch (Exception ex)
            { return string.Empty; }
        }

        public static int GetChangeDueDateLogCnt(string iKey, int lType)
        {
            string sql = @"select count(*) from Log 
                        where IssueKey = @IssueKey and LogType = @LogType ";
            var param = new Dictionary<string, string>();
                param.Add("@IssueKey", iKey);
                param.Add("@LogType", lType.ToString());
            var dbret = DBUtility.ExeLocalSqlWithRes(sql, null, param);

            return Convert.ToInt32(dbret[0][0]);
        }
    }

    public class LogType
    {
        public static int Default = 0;
        public static int WorkFlowTemplate = 1;
        public static int WorkFlow = 2;
    }
    

}
