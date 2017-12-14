using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Amazon.Models;

namespace Amazon.Controllers
{
    public class WorkFlowController : Controller
    {
        // GET: WorkFlow
        public ActionResult AllWorkFlow()
        {
            var allworkflow = WorkFlowVM.RetrieveAllWorkFlow(WORKFLOWRUNNINGSTATUS.RUNNING);
            return View(allworkflow);
        }



        public JsonResult WorkFlowByID()
        {
            var wfe_id = Request.Form["wfe_id"];
            var workflownodes = WorkflowStepInterface.RetrieveWorkFlowStepByWorkFlowID(wfe_id);
            var jsnodes = WorkflowStepInterface.Convert2JSNodes(workflownodes);
            var ret = new JsonResult();
            ret.Data = jsnodes;
            return ret;
        }

        public JsonResult WorkFlowStatusByID()
        {
            var wfe_id = Request.Form["wfe_id"];
            var workflownodes = WorkflowStepInterface.RetrieveWorkFlowStepWithStatus(wfe_id, WORKFLOWSTEPSTATUS.working);
            var jsnodes = WorkflowStepInterface.ConstructStatusTree(workflownodes);
            var ret = new JsonResult();
            ret.Data = jsnodes;
            return ret;
        }

        public JsonResult CreateNewWorkFlow()
        {
            var id = Request.Form["wf_id"];
            var desc = Request.Form["wfe_name"];

            var template = WorkFlowTemplateVM.RetrieveWorkFlowTemplateByID(id);
            if (template.Count > 0)
            {
                WorkFlowVM tempflow = null; 

                if (string.Compare(template[0].WFTType, REGISTERWORKFLOWSTEPTYPE.RMA) == 0)
                {
                    tempflow = (new RMAWorkFlowVM()).CreateNewWorkFlow(id);
                }

                if (tempflow != null)
                {
                    tempflow.WorkFlowDesc = desc;
                    tempflow.StoreWorkFlow();
                    var ret1 = new JsonResult();
                    ret1.Data = new { success = true};
                    return ret1;
                }
            }

            var ret = new JsonResult();
            ret.Data = new { success = false,msg ="Fail To Create WorkFlow From WorkFlow Template" };
            return ret;
        }


    }

}