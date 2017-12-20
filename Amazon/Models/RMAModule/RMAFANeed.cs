using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Amazon.Models
{
    public class RMAFANeed:WorkflowStepInterface
    {
        public RMAFANeed()
        {
            StepName = "RMAFANeed";
            IsLogicNode = true;
        }
    }
}