using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Amazon.Models
{
    public class RMAWorkFlowVM : WorkFlowVM
    {
        public RMAWorkFlowVM()
        {
            WorkFlowType = REGISTERWORKFLOWSTEPTYPE.RMA;
        }

        public override WorkFlowVM CreateNewWorkFlow(string wftid)
        {
            var rmaflow = new RMAWorkFlowVM();
            var workflow = CreateNewWorkFlow(wftid, rmaflow) as RMAWorkFlowVM;
            return workflow;
        }

        public override void StoreWorkFlow()
        {
            base.StoreWorkFlow();
        }
    }
}