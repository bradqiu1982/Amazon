﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Amazon.Models
{
    public class RMAAdminUpdateTrackingNum : WorkflowStepInterface
    {
        public RMAAdminUpdateTrackingNum()
        {
            StepName = "RMAAdminUpdateTrackingNum";
            IsLogicNode = true;
        }
    }
}