﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Amazon.Models
{
    public class WORKFLOWRUNNINGsTATUS
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
            var sql = "insert into WorkFlowVM(WorkFlowID,WFTName,WFTID,WorkFlowType,WorkFlowDesc,WorkFlowRunningStatus,CreateTime) value(@WorkFlowID,@WFTName,@WFTID,@WorkFlowType,@WorkFlowDesc,@WorkFlowRunningStatus,@CreateTime)";
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

        public static string GetUniqKey()
        {
            return Guid.NewGuid().ToString("N");
        }

        public virtual WorkFlowVM CreateNewWorkFlow(string wftid)
        {
            return null;
        }

        protected static WorkFlowVM CreateNewWorkFlow(string wftid, WorkFlowVM instance)
        {
            var workflowtemplate = WorkFlowTemplateVM.RetrieveWorkFlowTemplateByID(wftid);
            if (workflowtemplate.Count > 0)
            {
                var workflow = instance;
                instance.WorkFlowRunningStatus = WORKFLOWRUNNINGsTATUS.RUNNING;
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
                tempnode.NodeID = item.id;
                tempnode.ParentNodeID = item.parentid;
                tempnode.StepName = item.topic;
                tempnode.IsRoot = item.isroot;
                instance.WorkFlowStepList.Add(tempnode);
            }
        }



    }

}