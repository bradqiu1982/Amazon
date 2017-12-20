using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Amazon.Models
{
    public class FinalFARCQEReview : WorkflowStepInterface
    {
        public FinalFARCQEReview()
        {
            StepName = "FinalFARCQEReview";
            IsLogicNode = true;
        }
    }
}