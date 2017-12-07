using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Amazon.Models
{

    public class RegistedWorkFlowStep
    {
        public RegistedWorkFlowStep(string stepname, string steptype)
        {
            WorkflowStepName = stepname;
            WorkflowStepType = steptype;
        }
        public string WorkflowStepName { set; get; }
        public string WorkflowStepType { set; get; }

        public static List<RegistedWorkFlowStep> GetRegistedWorkflowStep()
        {
            var ret = new List<RegistedWorkFlowStep>();
            ret.Add(new RegistedWorkFlowStep("PlannerCreateFAJO", REGISTERWORKFLOWSTEPTYPE.COMMON));
            ret.Add(new RegistedWorkFlowStep("PlannerCreateReworkJO", REGISTERWORKFLOWSTEPTYPE.COMMON));
            ret.Add(new RegistedWorkFlowStep("PlannerCreateROSJO", REGISTERWORKFLOWSTEPTYPE.COMMON));
            ret.Add(new RegistedWorkFlowStep("PlannerSubmitSSD", REGISTERWORKFLOWSTEPTYPE.COMMON));
            ret.Add(new RegistedWorkFlowStep("CQEAddDispositionForEachSN", REGISTERWORKFLOWSTEPTYPE.RMA));
            ret.Add(new RegistedWorkFlowStep("CQEAssignFAEngineer", REGISTERWORKFLOWSTEPTYPE.RMA));
            ret.Add(new RegistedWorkFlowStep("CQEConfirmAssembly", REGISTERWORKFLOWSTEPTYPE.RMA));
            ret.Add(new RegistedWorkFlowStep("CQEIssueRework", REGISTERWORKFLOWSTEPTYPE.RMA));
            ret.Add(new RegistedWorkFlowStep("CQEPassModule2Clerk", REGISTERWORKFLOWSTEPTYPE.RMA));
            ret.Add(new RegistedWorkFlowStep("CQESubmit2ndFAR", REGISTERWORKFLOWSTEPTYPE.RMA));
            ret.Add(new RegistedWorkFlowStep("CQESubmitInitFAR", REGISTERWORKFLOWSTEPTYPE.RMA));
            ret.Add(new RegistedWorkFlowStep("CQMDecideLessonLearn", REGISTERWORKFLOWSTEPTYPE.RMA));
            ret.Add(new RegistedWorkFlowStep("FinalFARCQEReview", REGISTERWORKFLOWSTEPTYPE.RMA));
            ret.Add(new RegistedWorkFlowStep("IntialFARCQEReview", REGISTERWORKFLOWSTEPTYPE.RMA));
            ret.Add(new RegistedWorkFlowStep("PETESubmitFVRFAR", REGISTERWORKFLOWSTEPTYPE.RMA));
            ret.Add(new RegistedWorkFlowStep("PEUpdateFlow", REGISTERWORKFLOWSTEPTYPE.RMA));
            ret.Add(new RegistedWorkFlowStep("PQEApprove", REGISTERWORKFLOWSTEPTYPE.RMA));
            ret.Add(new RegistedWorkFlowStep("RMAAdminBookSO", REGISTERWORKFLOWSTEPTYPE.RMA));
            ret.Add(new RegistedWorkFlowStep("RMAAdminUpdateTrackingNum", REGISTERWORKFLOWSTEPTYPE.RMA));
            ret.Add(new RegistedWorkFlowStep("RMAAssitantConfirmCACompleted", REGISTERWORKFLOWSTEPTYPE.RMA));
            ret.Add(new RegistedWorkFlowStep("RMAClerkAssignCQE", REGISTERWORKFLOWSTEPTYPE.RMA));
            ret.Add(new RegistedWorkFlowStep("RMAClerkConfireWareHouseDeliver", REGISTERWORKFLOWSTEPTYPE.RMA));
            ret.Add(new RegistedWorkFlowStep("RMAClerkConfirmFADeliver", REGISTERWORKFLOWSTEPTYPE.RMA));
            ret.Add(new RegistedWorkFlowStep("RMACloseFAJO", REGISTERWORKFLOWSTEPTYPE.RMA));
            ret.Add(new RegistedWorkFlowStep("RMAFAKickoffModule", REGISTERWORKFLOWSTEPTYPE.RMA));
            ret.Add(new RegistedWorkFlowStep("SecondFANeed", REGISTERWORKFLOWSTEPTYPE.RMA));

            ret.Sort(delegate (RegistedWorkFlowStep step1, RegistedWorkFlowStep step2)
            {
                return string.Compare(step2.WorkflowStepName, step1.WorkflowStepName);
            });
            return ret;
        }
    }

}