﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Amazon.Models
{
    public class InitialFARCQEReview : WorkflowStepInterface
    {
        public InitialFARCQEReview()
        {
            StepName = "InitialFARCQEReview";
            IsLogicNode = true;
        }

    }
}