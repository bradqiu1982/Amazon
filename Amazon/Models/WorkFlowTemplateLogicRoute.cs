using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Amazon.Models
{
    public class jsroute
    {
        public string node_id { set; get; }
        public string node_name { set; get; }
        public string routelists { set; get; }
    }

    public class LOGICROUTETYPE
    {
        public static string ROLLBACK = "ROLLBACK";
        public static string JUMP = "JUMP";
    }

    public class WorkFlowTemplateLogicRoute
    {

        public WorkFlowTemplateLogicRoute()
        {
            WFTID = "";
            LogicNodeId = "";
            LogicNodeName = "";
            RouteNodeID = "";
            RouteNodeName = "";
            RouteType = "";
        }

        public WorkFlowTemplateLogicRoute(string wftid,string lid,string lname,string rid,string rname,string rtyep)
        {
            WFTID = wftid;
            LogicNodeId = lid;
            LogicNodeName = lname;
            RouteNodeID = rid;
            RouteNodeName = rname;
            RouteType = rtyep;
        }

        public string WFTID { set; get; }
        public string LogicNodeId { set; get; }
        public string LogicNodeName { set; get; }
        public string RouteNodeID { set; get; }
        public string RouteNodeName { set; get; }
        public string RouteType { set; get; }

        public void StoreLogicRoute()
        {

        }

        public static List<WorkFlowTemplateLogicRoute> RetrieveLogicRoute()
        {
            var ret = new List<WorkFlowTemplateLogicRoute>();

            return ret;
        }

        public static void ParseRoute(string WFTID,string WFTData, string WFRoute)
        {
            var nodes = (List<JSNode2C>)Newtonsoft.Json.JsonConvert.DeserializeObject(WFTData, (new List<JSNode2C>()).GetType());
            var logicwithroute = (List<jsroute>)Newtonsoft.Json.JsonConvert.DeserializeObject(WFRoute, (new List<jsroute>()).GetType());
            foreach (var logic in logicwithroute)
            {
                //try{
                    var pidlist = new Dictionary<string, bool>();
                    var sidlist = new Dictionary<string, bool>();
                    PickTreePath(logic.node_id,nodes,pidlist,sidlist);
                    var parenttree = PartlyTree(nodes,pidlist);
                    var sontree = PartlyTree(nodes, sidlist);

                    var routenames = logic.routelists.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var rname in routenames)
                    {
                        var fid = FindIDByName(parenttree, rname);
                        if (!string.IsNullOrEmpty(fid))
                        {
                            var route = new WorkFlowTemplateLogicRoute(WFTID, logic.node_id, logic.node_name, fid, rname, LOGICROUTETYPE.ROLLBACK);
                            route.StoreLogicRoute();
                        }
                        else
                        {
                            fid = FindIDByName(sontree, rname);
                            if (!string.IsNullOrEmpty(fid))
                            {
                                var route = new WorkFlowTemplateLogicRoute(WFTID, logic.node_id, logic.node_name, fid, rname, LOGICROUTETYPE.JUMP);
                                route.StoreLogicRoute();
                            }
                        }
                    }
                //}
                //catch (Exception ex) { }
            }
        }

        private static string FindIDByName(List<JSNode2C> tree, string name)
        {
            foreach (var item in tree)
            {
                if (string.Compare(name, item.topic) == 0)
                {
                    return item.id;
                }
            }
            return string.Empty;
        }

        private static List<JSNode2C> PartlyTree (List<JSNode2C> tree, Dictionary<string, bool> iddict)
        {
                var pttree = new List<JSNode2C>();
                foreach (var kv in iddict)
                {
                    foreach (var item in tree)
                    {
                        if (string.Compare(kv.Key, item.id) == 0)
                        {
                            pttree.Add(item);
                            break;
                        }
                    }
                }
            return pttree;
        }

        private static void ScanSonID(JSNode2C currentnode, List<JSNode2C> tree,Dictionary<string,bool> sidlist)
        {
            var multison = 0;

            var sonnode = new JSNode2C();
            foreach (var item in tree)
            {
                if (string.Compare(item.parentid, currentnode.id) == 0)
                {
                    sonnode.id = item.id;
                    sonnode.isroot = item.isroot;
                    sonnode.parentid = item.parentid;
                    sonnode.topic = item.topic;
                    multison = multison + 1;
                }
            }
            if (multison == 0 || multison > 1)
            {
                return;
            }

            sidlist.Add(sonnode.id,true);
            ScanSonID(sonnode, tree, sidlist);
        }

        private static void ScanParentID(JSNode2C currentnode, List<JSNode2C> tree,Dictionary<string,bool> pidlist)
        {
            if (string.IsNullOrEmpty(currentnode.parentid))
                return;

            var parentnode = new JSNode2C();
            foreach (var item in tree)
            {
                if (string.Compare(item.id, currentnode.parentid) == 0)
                {
                    parentnode.id = item.id;
                    parentnode.isroot = item.isroot;
                    parentnode.parentid = item.parentid;
                    parentnode.topic = item.topic;
                    pidlist.Add(item.id,true);
                    break;
                }
            }

            ScanParentID(parentnode, tree, pidlist);
        }

        public static void PickTreePath(string nodeid, List<JSNode2C> tree, Dictionary<string, bool> pidlist, Dictionary<string, bool> sidlist)
        {
            var currentnode = new JSNode2C();
            foreach (var item in tree)
            {
                if (string.Compare(item.id, nodeid) == 0)
                {
                    currentnode.id = item.id;
                    currentnode.isroot = item.isroot;
                    currentnode.parentid = item.parentid;
                    currentnode.topic = item.topic;
                    break;
                }
            }

            ScanParentID(currentnode, tree, pidlist);
            ScanSonID(currentnode, tree, sidlist);
        }



    }
}