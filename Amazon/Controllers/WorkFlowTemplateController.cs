﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Amazon.Models;

namespace Amazon.Controllers
{
    public class WorkFlowTemplateController : Controller
    {

        public ActionResult AllWorkFlowTemplate(string templatetype)
        {
            ViewBag.WorkFlowTypeList = RegistedWorkFlowType.GetRegistedWorkflowType();
            var vm = WorkFlowTemplateVM.RetrieveAllWorkFlowTemplate();
            if (string.IsNullOrEmpty(templatetype)
                || string.Compare("ALL", templatetype.ToUpper()) == 0)
            {
                return View(vm);
            }
            else
            {
                var filtervm = new List<WorkFlowTemplateVM>();
                foreach (var item in vm)
                {
                    if (string.Compare(item.WFTType.ToUpper(), templatetype.ToUpper()) == 0)
                    {
                        filtervm.Add(item);
                    }
                }
                return View(filtervm);
            }
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
            var edittype = Request.Form["edit_type"];
            if (string.Compare(edittype, "cache", true) == 0)
            {
                var ckdict = CookieUtility.UnpackCookie(this);
                if (ckdict.ContainsKey("updater"))
                {
                    var username = ckdict["updater"];
                    var wfname = Request.Form["wf_name"].ToUpper();
                    var wftype = Request.Form["wf_type"];
                    var wfdata = Request.Form["data"];
                    WorkFlowTemplateVM.CacheWFT(username, wfname, wftype, wfdata);
                }

                var ret = new JsonResult();
                ret.Data = new
                {
                    success = true
                };
                return ret;
            }
            else
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

        public JsonResult WorkFlowTemplateByID()
        {
            var id = Request.Form["wf_id"];
            var workflowlist =  WorkFlowTemplateVM.RetrieveWorkFlowTemplateByID(id);
            if (workflowlist.Count > 0)
            {
                var ret = new JsonResult();
                ret.Data = new {
                    sucess = true,
                    data = workflowlist[0].WFTData
                };
                return ret;
            }
            else
            {
                var ret = new JsonResult();
                ret.Data = new {
                    sucess = false
                };
                return ret;
            }
        }

        public JsonResult RemoveWorkFlowTemplateByID()
        {
            var id = Request.Form["nwf_id"];
            WorkFlowTemplateVM.RemoveWFT(id);
            var ret = new JsonResult();
            ret.Data = new
            {
                sucess = true,
            };
            return ret;
        }

        public JsonResult WorkFlowTemplateKeyValueArray()
        {
            var allworkflowtemplate = WorkFlowTemplateVM.RetrieveAllWorkFlowTemplate();
            var tempkeyvaluelist = new List<List<string>>();
            foreach (var item in allworkflowtemplate)
            {
                var templist = new List<string>();
                templist.Add(item.WFTID);
                templist.Add(item.WFTName);
                tempkeyvaluelist.Add(templist);
            }

            var ret = new JsonResult();
            ret.Data = tempkeyvaluelist;
            return ret;
        }

    }
}