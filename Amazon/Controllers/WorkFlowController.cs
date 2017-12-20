using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Amazon.Models;
using System.Net;

namespace Amazon.Controllers
{
    public class WorkFlowController : Controller
    {
        private void UserAuth()
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (ckdict.ContainsKey("logonuser"))
            {
                ViewBag.IsLogin = true;
                ViewBag.EmailAddr = ckdict["logonuser"];
                ViewBag.UserName = ckdict["logonuser"].Replace("@FINISAR.COM", "");
            }
            else
            {
                ViewBag.IsLogin = false;
            }
        }

        // GET: WorkFlow
        public ActionResult AllWorkFlow()
        {
            UserAuth();

            if (!ViewBag.IsLogin)
            {
                return RedirectToAction("Index", "Login");
            }

            var ckdict = CookieUtility.UnpackCookie(this);
            if (ckdict.ContainsKey("logonredirectctrl")
                && ckdict.ContainsKey("logonredirectact")
                && !string.IsNullOrEmpty(ckdict["logonredirectact"]) 
                && !string.IsNullOrEmpty(ckdict["logonredirectctrl"]))
            {
                var logonredirectact = ckdict["logonredirectact"];
                var logonredirectctrl = ckdict["logonredirectctrl"];
                var ck = new Dictionary<string, string>();
                ck.Add("logonredirectact", "");
                ck.Add("logonredirectctrl", "");
                CookieUtility.SetCookie(this, ck);
                return RedirectToAction(logonredirectact, logonredirectctrl);
            }

            var allworkflow = WorkFlowVM.RetrieveAllWorkFlow(WORKFLOWRUNNINGSTATUS.RUNNING);
            var doneworkflow = WorkFlowVM.RetrieveAllWorkFlow(WORKFLOWRUNNINGSTATUS.COMPLETE);
            allworkflow.AddRange(doneworkflow);
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
            var jsnodes = new List<object>();
            if (workflownodes.Count > 0)
            {
                jsnodes = WorkflowStepInterface.ConstructStatusTree(workflownodes);
            }
            else
            {
                var workflow = WorkFlowVM.RetrieveWorkFlowByID(wfe_id);
                if (string.Compare(workflow[0].WorkFlowRunningStatus, WORKFLOWRUNNINGSTATUS.COMPLETE) == 0)
                {
                    var tempnode = new WorkFlowComplete();
                    tempnode.StepStatus = WORKFLOWSTEPSTATUS.done;
                    var templist = new List<WorkflowStepInterface>();
                    templist.Add(tempnode);
                    jsnodes = WorkflowStepInterface.ConstructStatusTree(templist, false);
                }
                else
                {
                    workflownodes = WorkflowStepInterface.RetrieveWorkFlowStepWithStatus(wfe_id, WORKFLOWSTEPSTATUS.done);
                    if (workflownodes.Count > 0)
                    {
                        var currentnodedict = new Dictionary<string, bool>();
                        foreach (var item in workflownodes)
                        {
                            currentnodedict.Add(item.NodeID,true);
                        }//end foreach

                        var templist = new List<WorkflowStepInterface>();
                        foreach (var item in workflownodes)
                        {
                            if (item.ChildrenIDList.Count == 0)
                            {
                                templist.Add(item);
                            }
                            else
                            {
                                bool allnotmatch = true;
                                foreach (var cid in item.ChildrenIDList)
                                {
                                    if (currentnodedict.ContainsKey(cid))
                                    {
                                        allnotmatch = false;
                                    }
                                }//end foreach
                                if (allnotmatch)
                                {
                                    templist.Add(item);
                                }
                            }
                        }//end foreach
                        jsnodes = WorkflowStepInterface.ConstructStatusTree(templist);
                    }
                    else
                    {
                        jsnodes = WorkflowStepInterface.ConstructStatusTree(new List<WorkflowStepInterface>(), false);
                    }
                }//end else
            }//end else

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

                    LogVM.WriteLogWithCtrl(this, LogType.WorkFlow, Log4NetLevel.Info, 
                                    "WorkFlow", "CreateNewWorkFlow", id, tempflow.WorkFlowID, "", "CreateNewWorkFlow");

                    var ret1 = new JsonResult();
                    ret1.Data = new { success = true};
                    return ret1;
                }
            }

            var ret = new JsonResult();
            ret.Data = new { success = false,msg ="Fail To Create WorkFlow From WorkFlow Template" };
            return ret;
        }

        public JsonResult RemoveWorkFlow()
        {
            var wfid = Request.Form["wfe_id"];
            WorkFlowVM.UpdateWorkFlowStatus(wfid, WORKFLOWRUNNINGSTATUS.REMOVED);
            var ret1 = new JsonResult();
            ret1.Data = new { success = true };
            return ret1;
        }

        public ActionResult WorkFlowNodeDetail(string wfid, string nodeid)
        {
            var workflowvm = WorkFlowVM.RetrieveWorkFlowByID(wfid);

            workflowvm[0].RetireveSpecialInfo(wfid);

            var currentnode = WorkflowStepInterface.RetrieveWorkFlowStepByWorkFlowID(wfid,nodeid);

            ViewBag.CurrentWFID = wfid;
            ViewBag.CurrentNodeID = nodeid;
            ViewBag.CurrentStepName =  currentnode[0].StepName;

            if (string.Compare(currentnode[0].StepStatus,WORKFLOWSTEPSTATUS.done) == 0)
            {
                ViewBag.CurrentChildNameList = currentnode[0].ChildrenNameList;
                ViewBag.CurrentChildIDList = currentnode[0].ChildrenIDList;
            }
            return View(workflowvm);
        }

        public JsonResult WorkFlowMoveNext()
        {
            var ret = new JsonResult();
            var wfid = Request.Form["wfid"];
            var nodeid = Request.Form["nodeid"];
            var currentnode = WorkflowStepInterface.RetrieveWorkFlowStepByWorkFlowID(wfid, nodeid);
            WorkflowStepInterface.UpdateWorkFlowNodeStatus(wfid, currentnode[0].NodeID, WORKFLOWSTEPSTATUS.done);

            if (currentnode[0].IsLogicNode)
            {
                ret.Data = new {
                    nodeid = currentnode[0].NodeID
                };
                return ret;
            }
            else
            {
                if (currentnode[0].ChildrenNameList.Count > 1)
                {
                    ret.Data = new
                    {
                        nodeid = currentnode[0].NodeID
                    };
                    return ret;
                }
                else if (currentnode[0].ChildrenNameList.Count == 1)
                {
                    var childnode = WorkflowStepInterface.RetrieveWorkFlowStepByWorkFlowID(wfid, currentnode[0].ChildrenIDList[0]);
                    if (string.Compare(childnode[0].StepStatus, WORKFLOWSTEPSTATUS.pending) == 0)
                    {
                        WorkflowStepInterface.UpdateWorkFlowNodeStatus(wfid, currentnode[0].ChildrenIDList[0], WORKFLOWSTEPSTATUS.working);
                    }
                    ret.Data = new
                    {
                        nodeid = currentnode[0].ChildrenIDList[0]
                    };
                    return ret;
                }
                else
                {
                    var workingnodes = WorkflowStepInterface.RetrieveWorkFlowStepWithStatus(wfid, WORKFLOWSTEPSTATUS.working);
                    if (workingnodes.Count == 0)
                    {
                        WorkFlowVM.UpdateWorkFlowStatus(wfid, WORKFLOWRUNNINGSTATUS.COMPLETE);
                    }
                    ret.Data = new
                    {
                        nodeid = currentnode[0].NodeID
                    };
                    return ret;
                }
            }//end else
        }

    }

}