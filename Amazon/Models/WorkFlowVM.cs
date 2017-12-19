using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Amazon.Models
{
    public class WORKFLOWRUNNINGSTATUS
    {
        public static string RUNNING = "RUNNING";
        public static string COMPLETE = "COMPLETE";
        public static string HOLD = "HOLD";
        public static string REMOVED = "REMOVED";
    }

    public class WorkFlowVM
    {
        public WorkFlowVM()
        {
            WorkFlowID = "";
            WFTName = "";
            WFTID = "WFTID";
            WorkFlowType = "";
            WorkFlowDesc = "";
            WorkFlowRunningStatus = "";

            WorkFlowStepList = new List<WorkflowStepInterface>();
            WorkFlowStatusList = new List<WorkflowStepInterface>();
        }

        public string WorkFlowID { set; get; }
        public string WFTName { set; get; }
        public string WFTID { set; get; }
        public string WorkFlowType { set; get; }
        public string WorkFlowDesc { set; get; }
        public string WorkFlowRunningStatus { set; get; }
        public DateTime CreateTime { set; get; }

        public int SpendDays { get {
               return (DateTime.Now - CreateTime).Days;
            } }

        public List<WorkflowStepInterface> WorkFlowStepList { set; get; }
        public List<WorkflowStepInterface> WorkFlowStatusList { set; get; }

        public virtual void StoreWorkFlow()
        {
            var sql = "insert into WorkFlowVM(WorkFlowID,WFTName,WFTID,WorkFlowType,WorkFlowDesc,WorkFlowRunningStatus,CreateTime) values(@WorkFlowID,@WFTName,@WFTID,@WorkFlowType,@WorkFlowDesc,@WorkFlowRunningStatus,@CreateTime)";
            var param = new Dictionary<string, string>();
            param.Add("@WorkFlowID", WorkFlowID);
            param.Add("@WFTName", WFTName);
            param.Add("@WFTID", WFTID);
            param.Add("@WorkFlowType", WorkFlowType);
            param.Add("@WorkFlowDesc", WorkFlowDesc);
            param.Add("@WorkFlowRunningStatus", WorkFlowRunningStatus);
            param.Add("@CreateTime",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            DBUtility.ExeLocalSqlNoRes(sql, param);

            foreach (var item in WorkFlowStepList)
            {
                item.StoreStepBaseInfo();
            }

        }

        public static List<WorkFlowVM> RetrieveAllWorkFlow(string RunningStatus)
        {
            var ret = new List<WorkFlowVM>();

            var sql = "select WorkFlowID,WFTName,WFTID,WorkFlowType,WorkFlowDesc,WorkFlowRunningStatus,CreateTime from WorkFlowVM where WorkFlowRunningStatus = @WorkFlowRunningStatus";
            var param = new Dictionary<string, string>();
            param.Add("@WorkFlowRunningStatus", RunningStatus);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql, null, param);
            foreach (var line in dbret)
            {
                var WorkFlowID = Convert.ToString(line[0]);
                var WFTName = Convert.ToString(line[1]);
                var WFTID = Convert.ToString(line[2]);
                var WorkFlowType = Convert.ToString(line[3]);
                var WorkFlowDesc = Convert.ToString(line[4]);
                var WorkFlowRunningStatus = Convert.ToString(line[5]);
                var CreateTime = Convert.ToDateTime(line[6]);

                Type objType = Type.GetType("Amazon.Models." + WorkFlowType + ", Amazon, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
                object obj = Activator.CreateInstance(objType);
                var tempflow = (WorkFlowVM)obj;
                //if (tempflow != null)
                {
                    tempflow.WorkFlowID = WorkFlowID;
                    tempflow.WFTName = WFTName;
                    tempflow.WFTID = WFTID;
                    tempflow.WorkFlowType = WorkFlowType;
                    tempflow.WorkFlowDesc = WorkFlowDesc;
                    tempflow.WorkFlowRunningStatus = WorkFlowRunningStatus;
                    tempflow.CreateTime = CreateTime;
                    ret.Add(tempflow);
                }
            }

            return ret;
        }

        public static List<WorkFlowVM> RetrieveWorkFlowByID(string wfid)
        {
            var ret = new List<WorkFlowVM>();

            var sql = "select WorkFlowID,WFTName,WFTID,WorkFlowType,WorkFlowDesc,WorkFlowRunningStatus,CreateTime from WorkFlowVM where WorkFlowID = @WorkFlowID";
            var param = new Dictionary<string, string>();
            param.Add("@WorkFlowID", wfid);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql, null, param);
            foreach (var line in dbret)
            {
                var WorkFlowID = Convert.ToString(line[0]);
                var WFTName = Convert.ToString(line[1]);
                var WFTID = Convert.ToString(line[2]);
                var WorkFlowType = Convert.ToString(line[3]);
                var WorkFlowDesc = Convert.ToString(line[4]);
                var WorkFlowRunningStatus = Convert.ToString(line[5]);
                var CreateTime = Convert.ToDateTime(line[6]);

                Type objType = Type.GetType("Amazon.Models." + WorkFlowType + ", Amazon, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
                object obj = Activator.CreateInstance(objType);
                var tempflow = (WorkFlowVM)obj;
                //if (tempflow != null)
                {
                    tempflow.WorkFlowID = WorkFlowID;
                    tempflow.WFTName = WFTName;
                    tempflow.WFTID = WFTID;
                    tempflow.WorkFlowType = WorkFlowType;
                    tempflow.WorkFlowDesc = WorkFlowDesc;
                    tempflow.WorkFlowRunningStatus = WorkFlowRunningStatus;
                    tempflow.CreateTime = CreateTime;
                    ret.Add(tempflow);
                }
            }

            return ret;
        }

        public static void UpdateWorkFlowStatus(string wfid, string status)
        {
            var sql = "Update WorkFlowVM set WorkFlowRunningStatus = @WorkFlowRunningStatus  where WorkFlowID = @WorkFlowID";
            var param = new Dictionary<string, string>();
            param.Add("@WorkFlowID", wfid);
            param.Add("@WorkFlowRunningStatus", status);
            DBUtility.ExeLocalSqlNoRes(sql,param);
        }

        public static string GetUniqKey()
        {
            return Guid.NewGuid().ToString("N");
        }

        public virtual WorkFlowVM CreateNewWorkFlow(string wftid)
        {
            return null;
        }

        public virtual void RetireveSpecialInfo(string wftid)
        {

        }

        protected static WorkFlowVM CreateNewWorkFlow(string wftid, WorkFlowVM instance)
        {
            var workflowtemplate = WorkFlowTemplateVM.RetrieveWorkFlowTemplateByID(wftid);
            if (workflowtemplate.Count > 0)
            {
                var workflow = instance;
                instance.WorkFlowRunningStatus = WORKFLOWRUNNINGSTATUS.RUNNING;
                workflow.WorkFlowID = GetUniqKey();
                workflow.WFTName = workflowtemplate[0].WFTName;
                workflow.WFTID = workflowtemplate[0].WFTID;

                CreateFlowSequence(instance, workflowtemplate[0].WFTData);

                foreach (var item in instance.WorkFlowStepList)
                {
                    item.ContentID = GetUniqKey();
                    item.WorkFlowID = workflow.WorkFlowID;
                    if (item.IsRoot)
                    {
                        item.StepStatus = WORKFLOWSTEPSTATUS.working;
                    }
                    else
                    {
                        item.StepStatus = WORKFLOWSTEPSTATUS.pending;
                    }
                }

                ScanSons(instance);

                return instance;
            }
            return null;
        }

        private static void ScanSons(WorkFlowVM instance)
        {
            var nodesize = instance.WorkFlowStepList.Count;
            for (var idx = 0; idx < nodesize; idx++)
            {
                var currentnode = instance.WorkFlowStepList[idx];

                for (var jdx = 0; jdx < nodesize; jdx++)
                {
                    var scannode = instance.WorkFlowStepList[jdx];
                    if (string.Compare(currentnode.NodeID, scannode.ParentNodeID, true) == 0)
                    {
                        currentnode.ChildrenIDList.Add(scannode.NodeID);
                        currentnode.ChildrenNameList.Add(scannode.StepName);
                    }
                }//end for
            }//end for
        }

        private static void CreateFlowSequence(WorkFlowVM instance, string template)
        {
            var nodes = (List<JSNode2C>)Newtonsoft.Json.JsonConvert.DeserializeObject(template, (new List<JSNode2C>()).GetType());
            foreach (var item in nodes)
            {
                Type objType = Type.GetType("Amazon.Models."+ item.topic + ", Amazon, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
                object obj = Activator.CreateInstance(objType);
                var tempnode = (WorkflowStepInterface) obj;
                //if (tempnode != null)
                {
                    tempnode.NodeID = item.id;
                    tempnode.ParentNodeID = item.parentid;
                    tempnode.StepName = item.topic;
                    tempnode.IsRoot = item.isroot;
                    instance.WorkFlowStepList.Add(tempnode);
                }
            }
        }



    }

}