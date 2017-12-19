using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amazon.Models
{
    public class REGISTERWORKFLOWSTEPTYPE
    {
        public static string RMA = "RMA";
        public static string GENERAL = "GENERAL";
        public static string COMMON = "COMMON";
    }

    public class WORKFLOWSTEPSTATUS
    {
        public static string working = "working";
        public static string done = "done";
        public static string pending = "pending";
    }

    public class WORKFLOWNODEENABLE
    {
        public static string DISABLE = "0";
        public static string ENABLE = "1";
    }

    public abstract class WorkflowStepInterface
    {
        public WorkflowStepInterface()
        {
            NodeID = "";
            ParentNodeID = "";
            ContentID = "";
            WorkFlowID = "";
            StepName = "";
            StepStatus = "";
            IsRoot = false;
            BGColor = "#A6A6A6";

            IsLogicNode = false;

            ChildrenIDList = new List<string>();
            ChildrenNameList = new List<string>();

            InitUpdateTime = DateTime.Parse("1982-05-06 10:00:00");
            UpdateTime = DateTime.Parse("1982-05-06 10:00:00");
        }

        public string ContentID { get; set; }
        public string WorkFlowID { get; set; }
        public string NodeID { get; set; }
        public string ParentNodeID { get; set; }
        public string StepName { get; set; }
        public string StepStatus { get; set; }
        public bool IsRoot { get; set; }
        public bool IsLogicNode { get; set; }

        public List<string> ChildrenIDList { set; get; }
        public List<string> ChildrenNameList { set; get; }

        public DateTime InitUpdateTime { set; get; }
        public DateTime UpdateTime { set; get; }

        public void StoreStepBaseInfo()
        {
            var rootstr = IsRoot ? "TRUE" : "FALSE";
            var childidliststr = "";
            var childnameliststr = "";
            foreach (var item in ChildrenIDList)
            { childidliststr = childidliststr + item + ";"; }
            foreach (var item in ChildrenNameList)
            { childnameliststr = childnameliststr + item + ";"; }

            var sql = "insert into WorkflowStepBaseInfo(WorkFlowID,NodeID,ParentNodeID,ContentID,StepName,StepStatus,IsRoot,ChildrenIDList,ChildrenNameList,InitUpdateTime,UpdateTime) "
                + " values(@WorkFlowID,@NodeID,@ParentNodeID,@ContentID,@StepName,@StepStatus,@IsRoot,@ChildrenIDList,@ChildrenNameList,@InitUpdateTime,@UpdateTime) ";
            var param = new Dictionary<string, string>();
            param.Add("@WorkFlowID", WorkFlowID);
            param.Add("@NodeID", NodeID);
            param.Add("@ParentNodeID", ParentNodeID);
            param.Add("@ContentID", ContentID);
            param.Add("@StepName", StepName);
            param.Add("@StepStatus", StepStatus);
            param.Add("@IsRoot", rootstr);
            param.Add("@ChildrenIDList", childidliststr);
            param.Add("@ChildrenNameList", childnameliststr);
            param.Add("@InitUpdateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            param.Add("@UpdateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            DBUtility.ExeLocalSqlNoRes(sql, param);
        }

        public static List<WorkflowStepInterface> RetrieveWorkFlowStepByWorkFlowID(string workflowid, string nodeid = null)
        {
            var ret = new List<WorkflowStepInterface>();

            var param = new Dictionary<string, string>();
            List<List<object>> dbret = null;
            if (string.IsNullOrEmpty(nodeid))
            {
                var sql = "select WorkFlowID,NodeID,ParentNodeID,ContentID,StepName,StepStatus,IsRoot,ChildrenIDList,ChildrenNameList,InitUpdateTime,UpdateTime from WorkflowStepBaseInfo where WorkFlowID = @WorkFlowID";
                param.Add("@WorkFlowID", workflowid);
                dbret = DBUtility.ExeLocalSqlWithRes(sql,null,param);
            }
            else
            {
                var sql = "select WorkFlowID,NodeID,ParentNodeID,ContentID,StepName,StepStatus,IsRoot,ChildrenIDList,ChildrenNameList,InitUpdateTime,UpdateTime from WorkflowStepBaseInfo where WorkFlowID = @WorkFlowID and NodeID = @NodeID";
                param.Add("@WorkFlowID", workflowid);
                param.Add("@NodeID", nodeid);
                dbret = DBUtility.ExeLocalSqlWithRes(sql, null, param);
            }

            foreach (var line in dbret)
            {
                var stepname = Convert.ToString(line[4]);

                Type objType = Type.GetType("Amazon.Models." + stepname + ", Amazon, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
                object obj = Activator.CreateInstance(objType);
                var tempnode = (WorkflowStepInterface)obj;
                //if (tempnode != null)
                {
                    tempnode.WorkFlowID = Convert.ToString(line[0]);
                    tempnode.NodeID = Convert.ToString(line[1]);
                    tempnode.ParentNodeID = Convert.ToString(line[2]);
                    tempnode.ContentID = Convert.ToString(line[3]);
                    tempnode.StepName = Convert.ToString(line[4]);
                    tempnode.StepStatus = Convert.ToString(line[5]);

                    var rootstr = Convert.ToString(line[6]);
                    var childidliststr = Convert.ToString(line[7]);
                    var childnameliststr = Convert.ToString(line[8]);

                    tempnode.InitUpdateTime = Convert.ToDateTime(line[9]);
                    tempnode.UpdateTime = Convert.ToDateTime(line[10]);

                    tempnode.IsRoot = (string.Compare(rootstr, "TRUE") == 0) ? true : false;
                    if (!string.IsNullOrEmpty(childidliststr))
                    {
                        tempnode.ChildrenIDList.AddRange(childidliststr.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries));
                        tempnode.ChildrenNameList.AddRange(childnameliststr.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries));
                    }
                    
                    ret.Add(tempnode);
                }
            }

            return ret;
        }

        public static List<WorkflowStepInterface> RetrieveWorkFlowStepWithStatus(string workflowid, string status)
        {
            var ret = new List<WorkflowStepInterface>();

            var param = new Dictionary<string, string>();
            List<List<object>> dbret = null;

            var sql = "select WorkFlowID,NodeID,ParentNodeID,ContentID,StepName,StepStatus,IsRoot,ChildrenIDList,ChildrenNameList,InitUpdateTime,UpdateTime from WorkflowStepBaseInfo where WorkFlowID = @WorkFlowID and StepStatus = @StepStatus";
            param.Add("@WorkFlowID", workflowid);
            param.Add("@StepStatus", status);
            dbret = DBUtility.ExeLocalSqlWithRes(sql, null, param);

            foreach (var line in dbret)
            {
                var stepname = Convert.ToString(line[4]);

                Type objType = Type.GetType("Amazon.Models." + stepname + ", Amazon, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
                object obj = Activator.CreateInstance(objType);
                var tempnode = (WorkflowStepInterface)obj;
                //if (tempnode != null)
                {
                    tempnode.WorkFlowID = Convert.ToString(line[0]);
                    tempnode.NodeID = Convert.ToString(line[1]);
                    tempnode.ParentNodeID = Convert.ToString(line[2]);
                    tempnode.ContentID = Convert.ToString(line[3]);
                    tempnode.StepName = Convert.ToString(line[4]);
                    tempnode.StepStatus = Convert.ToString(line[5]);

                    var rootstr = Convert.ToString(line[6]);
                    var childidliststr = Convert.ToString(line[7]);
                    var childnameliststr = Convert.ToString(line[8]);

                    tempnode.InitUpdateTime = Convert.ToDateTime(line[9]);
                    tempnode.UpdateTime = Convert.ToDateTime(line[10]);

                    tempnode.IsRoot = (string.Compare(rootstr, "TRUE") == 0) ? true : false;
                    if (!string.IsNullOrEmpty(childidliststr))
                    {
                        tempnode.ChildrenIDList.AddRange(childidliststr.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries));
                        tempnode.ChildrenNameList.AddRange(childnameliststr.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries));
                    }

                    ret.Add(tempnode);
                }
            }

            return ret;
        }

        public static List<object> Convert2JSNodes(List<WorkflowStepInterface> nodes)
        {
            var ret = new List<object>();
            foreach (var node in nodes)
            {
                var linktopic = "";
                if (string.Compare(node.NodeEnable, WORKFLOWNODEENABLE.ENABLE) == 0)
                {
                    linktopic = "<a href='/WorkFlow/WorkFlowNodeDetail?wfid=" + node.WorkFlowID + "&nodeid=" + node.NodeID + "'>" + node.StepName + "</a>";
                }
                else
                {
                    linktopic = node.StepName;
                }

                ret.Add(new {
                    id = node.NodeID,
                    isroot = node.IsRoot,
                    parentid = node.ParentNodeID,
                    topic = linktopic,
                    line_color = node.LineColor,
                    background_color = node.BGColor,
                    status = node.NodeEnable
                });
            }
            return ret;
        }

        public static List<object> ConstructStatusTree(List<WorkflowStepInterface> nodes)
        {
            var ret = new List<object>();
            ret.Add(new
            {
                id = "1",
                isroot = true,
                parentid = "",
                topic = " ",
                background_color = "#0F4FA8",
                line_color = "#0F4FA8",
                status = WORKFLOWNODEENABLE.DISABLE
            });

            var idx = 2;
            foreach (var node in nodes)
            {
                ret.Add(new
                {
                    id = idx.ToString(),
                    isroot = false,
                    parentid = "1",
                    topic = node.StepName,
                    background_color = node.BGColor,
                    status = node.NodeEnable
                });
                idx = idx + 1;
            }

            return ret;
        }

        private string bgcolor;
        public string BGColor
        {
            get
            {
                if (string.IsNullOrEmpty(StepStatus))
                {
                    return bgcolor;
                }
                else
                {
                    if (string.Compare(StepStatus, WORKFLOWSTEPSTATUS.working) == 0)
                    {
                        return "#FF7400";
                    }
                    else if (string.Compare(StepStatus, WORKFLOWSTEPSTATUS.done) == 0)
                    {
                        return "#0F4FA8";
                    }
                    else
                    {
                        return "#A6A6A6";
                    }
                }
            }
            set
            {
                bgcolor = value;
            }
        }

        public string NodeEnable {
            get {
                if (string.IsNullOrEmpty(StepStatus))
                {
                    return WORKFLOWNODEENABLE.DISABLE;
                }
                else
                {
                    if (string.Compare(StepStatus, WORKFLOWSTEPSTATUS.working) == 0
                        || string.Compare(StepStatus, WORKFLOWSTEPSTATUS.done) == 0)
                    {
                        return WORKFLOWNODEENABLE.ENABLE;
                    }
                    else
                    {
                        return WORKFLOWNODEENABLE.DISABLE;
                    }
                }
            }
        }

        public string LineColor
        {
            get
            {
                if (string.IsNullOrEmpty(StepStatus))
                {
                    return "#A6A6A6";
                }
                else
                {
                    if (string.Compare(StepStatus, WORKFLOWSTEPSTATUS.working) == 0
                        || string.Compare(StepStatus, WORKFLOWSTEPSTATUS.done) == 0)
                    {
                        return "#0F4FA8";
                    }
                    else
                    {
                        return "#A6A6A6";
                    }
                }
            }
        }


    }


}
