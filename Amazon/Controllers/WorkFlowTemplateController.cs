using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Amazon.Models;

namespace Amazon.Controllers
{
    public class WorkFlowTemplateController : Controller
    {

        public ActionResult AllWorkFlowTemplate()
        {
            return View();
        }

        public ActionResult CreateWorkFlowTemplate()
        {
            return View();
        }


        public JsonResult RegistedWorkFlowTypeList()
        {
            var ret = new JsonResult();
            ret.Data = RegistedWorkFlowType.GetRegistedWorkflowType();
            return ret;
        }

        public JsonResult RegistedWorkFlowStepList()
        {
            var type = Request.Form["type"];
            var ret = new JsonResult();
            ret.Data = RegistedWorkFlowStep.GetRegistedWorkflowStep(type);
            return ret;
        }

        public JsonResult StortNewWorkFlow()
        {
            var ret = new JsonResult();

            var wfname = Request.Form["wf_name"].ToUpper();
            var allwftname = WorkFlowTemplateVM.RetrieveAllWorkFlowTemplateName();
            if (allwftname.ContainsKey(wfname))
            {
                ret.Data = new {
                    success = false,
                    msg = "work flow template name already exist!"
                };
                return ret;
            }

            var wfid = Request.Form["wf_id"];
            var wftype = Request.Form["wf_type"];
            var wfdata = Request.Form["data"];

            WorkFlowTemplateVM.StoreWFT(wfid, wfname, wftype, wfdata);

            ret.Data = new
            {
                success = true
            };
            return ret;
        }


    }
}