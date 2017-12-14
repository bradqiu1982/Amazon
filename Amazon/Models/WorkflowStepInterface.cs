using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amazon.Models
{
    public class REGISTERWORKFLOWSTEPTYPE
    {
        public static string RMA = "RMA";
        public static string GENERAL = "GENERAL";
        public static string COMMON = "COMMON";
    }

    public class WORKFLOWSTEPSTATUS
    {
        public static string working = "working";
        public static string done = "done";
        public static string pending = "pending";
    }

    public class WORKFLOWNODEENABLE
    {
        public static string DISABLE = "0";
        public static string ENABLE = "1";
    }

    public abstract class WorkflowStepInterface
    {
        public WorkflowStepInterface()
        {
            NodeID = "";
            ParentNodeID = "";
            ContentID = "";
            WorkFlowID = "";
            StepName = "";
            StepStatus = "";
            IsRoot = false;
            BGColor = "#A6A6A6";

            ChildrenIDList = new List<string>();
            ChildrenNameList = new List<string>();
        }

        public string ContentID { get; set; }
        public string WorkFlowID { get; set; }
        public string NodeID { get; set; }
        public string ParentNodeID { get; set; }
        public string StepName { get; set; }
        public string StepStatus { get; set; }
        public bool IsRoot { get; set; }

        public List<string> ChildrenIDList { set; get; }
        public List<string> ChildrenNameList { set; get; }

        public void StoreStepBaseInfo()
        {
            var rootstr = IsRoot ? "TRUE" : "FALSE";
            var childidliststr = "";
            var childnameliststr = "";
            foreach (var item in ChildrenIDList)
            { childidliststr = childidliststr + item + ";"; }
            foreach (var item in ChildrenNameList)
            { childnameliststr = childnameliststr + item + ";"; }

            var sql = "insert into WorkflowStepBaseInfo(WorkFlowID,NodeID,ParentNodeID,ContentID,StepName,StepStatus,IsRoot,ChildrenIDList,ChildrenNameList) "
                + " values(@WorkFlowID,@NodeID,@ParentNodeID,@ContentID,@StepName,@StepStatus,@IsRoot,@ChildrenIDList,@ChildrenNameList) ";
            var param = new Dictionary<string, string>();
            param.Add("@WorkFlowID", WorkFlowID);
            param.Add("@NodeID", NodeID);
            param.Add("@ParentNodeID", ParentNodeID);
            param.Add("@ContentID", ContentID);
            param.Add("@StepName", StepName);
            param.Add("@StepStatus", StepStatus);
            param.Add("@IsRoot", rootstr);
            param.Add("@ChildrenIDList", childidliststr);
            param.Add("@ChildrenNameList", childnameliststr);
            DBUtility.ExeLocalSqlNoRes(sql, param);
        }

        private string bgcolor;
        public string BGColor
        {
            get
            {
                if (string.IsNullOrEmpty(StepStatus))
                {
                    return bgcolor;
                }
                else
                {
                    if (string.Compare(StepStatus, WORKFLOWSTEPSTATUS.working) == 0)
                    {
                        return "#FF7400";
                    }
                    else if (string.Compare(StepStatus, WORKFLOWSTEPSTATUS.done) == 0)
                    {
                        return "#0F4FA8";
                    }
                    else
                    {
                        return "#A6A6A6";
                    }
                }
            }
            set
            {
                bgcolor = value;
            }
        }

        public string NodeEnable {
            get {
                if (string.IsNullOrEmpty(StepStatus))
                {
                    return WORKFLOWNODEENABLE.DISABLE;
                }
                else
                {
                    if (string.Compare(StepStatus, WORKFLOWSTEPSTATUS.working) == 0
                        || string.Compare(StepStatus, WORKFLOWSTEPSTATUS.done) == 0)
                    {
                        return WORKFLOWNODEENABLE.ENABLE;
                    }
                    else
                    {
                        return WORKFLOWNODEENABLE.DISABLE;
                    }
                }
            }
        }

        public string LineColor
        {
            get
            {
                if (string.IsNullOrEmpty(StepStatus))
                {
                    return "#A6A6A6";
                }
                else
                {
                    if (string.Compare(StepStatus, WORKFLOWSTEPSTATUS.working) == 0
                        || string.Compare(StepStatus, WORKFLOWSTEPSTATUS.done) == 0)
                    {
                        return "#0F4FA8";
                    }
                    else
                    {
                        return "#A6A6A6";
                    }
                }
            }
        }


    }


}
