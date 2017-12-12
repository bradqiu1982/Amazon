using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Amazon.Models
{
    public class RegistedWorkFlowType
    {
        public static List<string> GetRegistedWorkflowType()
        {
            var ret = new List<string>();
            ret.Add(REGISTERWORKFLOWSTEPTYPE.RMA);
            return ret;
        }
    }
}