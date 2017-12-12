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


    public class WorkflowStepStatus
    {
        public static string working = "working";
        public static string done = "done";
        public static string pending = "pending";
    }

    public abstract class WorkflowStepInterface
    {
        public WorkflowStepInterface()
        {
            NodeID = "";
            ParentNodeID = "";
            ContentID = "";
            StepName = "";
            StepType = "";
            StepStatus = "";
            IsRoot = false;
            BGColor = "#A6A6A6";
            IsReady2Operate = false;
        }

        public string ContentID { get; set; }
        public string NodeID { get; set; }
        public string ParentNodeID { get; set; }
        public string StepName { get; set; }
        public string StepStatus { get; set; }
        public string StepType { get; set; }
        public bool IsRoot
        {
            get { return true; }
            set { }
        }
        public bool IsReady2Operate { get; set; }

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
                    if (string.Compare(StepStatus, WorkflowStepStatus.working) == 0)
                    {
                        return "#FF7400";
                    }
                    else if (string.Compare(StepStatus, WorkflowStepStatus.done) == 0)
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
    }


}
