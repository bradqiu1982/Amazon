using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Amazon.Models
{
    public class WorkFlowVM
    {
        public WorkFlowVM()
        {
            WorkFlowID = "";
            WFTName = "";
            WorkFlowType = "";
            WorkFlowDesc = "";
            WorkFlowRunningStatus = "";

            WorkFlowStepList = new List<WorkflowStepInterface>();
            WorkFlowStatusList = new List<WorkflowStepInterface>();
        }

        public string WorkFlowID { set; get; }
        public string WFTName { set; get; }
        public string WorkFlowType { set; get; }
        public string WorkFlowDesc { set; get; }
        public string WorkFlowRunningStatus { set; get; }

        public List<WorkflowStepInterface> WorkFlowStepList { set; get; }
        public List<WorkflowStepInterface> WorkFlowStatusList { set; get; }

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
                workflow.WorkFlowID = GetUniqKey();
                workflow.WFTName = workflowtemplate[0].WFTName;
                CreateFlowSequence(instance, workflowtemplate[0].WFTData);
            }
            return null;
        }

        private static void CreateFlowSequence(WorkFlowVM instance, string template)
        {
            var nodes = (List<JSNode2C>)Newtonsoft.Json.JsonConvert.DeserializeObject(template, (new List<JSNode2C>()).GetType());
            foreach (var item in nodes)
            {
                object obj = Activator.CreateInstance("Amazon.Models", item.topic);
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