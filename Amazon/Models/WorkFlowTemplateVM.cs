using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Amazon.Models
{
    public class WorkFlowTemplateVM
    {
        public WorkFlowTemplateVM()
        {
            WFTID = "";
            WFTName = "";
            WFTType = "";
            WFTData = "";
            WFTRoute = "";
        }

        public string WFTID { set; get; }
        public string WFTName { set; get; }
        public string WFTType { set; get; }
        public string WFTData { set; get; }
        public string WFTRoute { set; get; }

        public static List<WorkFlowTemplateVM> RetrieveAllWorkFlowTemplate()
        {
            var ret = new List<WorkFlowTemplateVM>();
            var sql = "select WFTID,WFTName,WFTType,WFTData from WorkFlowTemplateVM where Removed <> 'TRUE' order by UpdateTime DESC";
            var dbret = DBUtility.ExeLocalSqlWithRes(sql, null);
            foreach (var line in dbret)
            {
                var temp = new WorkFlowTemplateVM();
                temp.WFTID = Convert.ToString(line[0]);
                temp.WFTName = Convert.ToString(line[1]);
                temp.WFTType = Convert.ToString(line[2]);
                temp.WFTData = Convert.ToString(line[3]);
                ret.Add(temp);
            }
            return ret;
        }

        public static List<WorkFlowTemplateVM> RetrieveWorkFlowTemplateByID(string wftid)
        {
            var ret = new List<WorkFlowTemplateVM>();
            var sql = "select WFTID,WFTName,WFTType,WFTData from WorkFlowTemplateVM where WFTID = @WFTID order by UpdateTime DESC";
            var param2 = new Dictionary<string, string>();
            param2.Add("@WFTID", wftid);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql, null,param2);
            foreach (var line in dbret)
            {
                var temp = new WorkFlowTemplateVM();
                temp.WFTID = Convert.ToString(line[0]);
                temp.WFTName = Convert.ToString(line[1]);
                temp.WFTType = Convert.ToString(line[2]);
                temp.WFTData = Convert.ToString(line[3]);
                ret.Add(temp);
            }
            return ret;
        }

        public static void RemoveWFT(string WFTID)
        {
            var sql = "update WorkFlowTemplateVM set Removed = 'TRUE' where WFTID = @WFTID";
            var param2 = new Dictionary<string, string>();
            param2.Add("@WFTID", WFTID);
            DBUtility.ExeLocalSqlNoRes(sql, param2);
        }

        public static Dictionary<string, bool> RetrieveAllWorkFlowTemplateName()
        {
            var ret = new Dictionary<string, bool>();
            var allflow = RetrieveAllWorkFlowTemplate();
            foreach (var item in allflow)
            {
                ret.Add(item.WFTName, true);
            }
            return ret;
        }

        public static void StoreWFT(string WFTID, string WFTName, string WFTType, string WFTData)
        {
            var sql = "insert into WorkFlowTemplateVM(WFTID,WFTName,WFTType,WFTData,UpdateTime) values(@WFTID,@WFTName,@WFTType,@WFTData,@UpdateTime)";
            var param2 = new Dictionary<string, string>();
            param2.Add("@WFTID", WFTID);
            param2.Add("@WFTName", WFTName);
            param2.Add("@WFTType", WFTType);
            param2.Add("@WFTData", WFTData);
            param2.Add("@UpdateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            DBUtility.ExeLocalSqlNoRes(sql, param2);
        }

        public static void CacheWFT(string UserName, string WFTName, string WFTType, string WFTData,string WFTRoute)
        {
            var sql = "delete from WorkFlowTemplateCache where UserName = @UserName";
            var param1 = new Dictionary<string, string>();
            param1.Add("@UserName", UserName);
            DBUtility.ExeLocalSqlNoRes(sql, param1);

            sql = "insert into WorkFlowTemplateCache(UserName,WFTName,WFTType,WFTData,WFTRoute) values(@UserName,@WFTName,@WFTType,@WFTData,@WFTRoute)";
            var param2 = new Dictionary<string, string>();
            param2.Add("@UserName", UserName);
            param2.Add("@WFTName", WFTName);
            param2.Add("@WFTType", WFTType);
            param2.Add("@WFTData", WFTData);
            param2.Add("@WFTRoute", WFTRoute);
            DBUtility.ExeLocalSqlNoRes(sql, param2);
        }

        public static List<WorkFlowTemplateVM> RetrieveCachedWFT(string UserName)
        {
            var ret = new List<WorkFlowTemplateVM>();
            var sql = "select UserName,WFTName,WFTType,WFTData,WFTRoute from WorkFlowTemplateCache where UserName = @UserName";
            var param1 = new Dictionary<string, string>();
            param1.Add("@UserName", UserName);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql, null, param1);
            foreach (var line in dbret)
            {
                var temp = new WorkFlowTemplateVM();
                temp.WFTID = Convert.ToString(line[0]);
                temp.WFTName = Convert.ToString(line[1]);
                temp.WFTType = Convert.ToString(line[2]);
                temp.WFTData = Convert.ToString(line[3]);
                temp.WFTRoute = Convert.ToString(line[4]);
                ret.Add(temp);
            }
            return ret;
        }


    }
}