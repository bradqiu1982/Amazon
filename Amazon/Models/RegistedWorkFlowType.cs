using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Amazon.Models
{
    public class RegistedWorkFlowType
    {
        public static List<string> GetRegistedWorkflowStepType()
        {
            var ret = new List<string>();
            ret.Add(REGISTERWORKFLOWSTEPTYPE.RMA);
            return ret;
        }

        public static List<string> GetRegistedWorkflowVMType()
        {
            var ret = new List<string>();
            ret.Add("RMAWorkFlowVM");
            return ret;
        }
    }
}