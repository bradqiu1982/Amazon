using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Amazon.Models
{
        public class CfgUtility
        {
            public static Dictionary<string, string> GetSysConfig(Controller ctrl)
            {
                var lines = System.IO.File.ReadAllLines(ctrl.Server.MapPath("~/Scripts/AmazonCfg.txt"));
                var ret = new Dictionary<string, string>();
                foreach (var line in lines)
                {
                    if (line.Contains("##"))
                    {
                        continue;
                    }

                    if (line.Contains(":::"))
                    {
                        var kvpair = line.Split(new string[] { ":::" }, StringSplitOptions.RemoveEmptyEntries);
                        if (!ret.ContainsKey(kvpair[0].Trim()))
                        {
                            ret.Add(kvpair[0].Trim(), kvpair[1].Trim());
                        }
                    }//end if
                }//end foreach
                return ret;
            }

            public static Dictionary<string, string> GetStandardPJList(Controller ctrl)
            {
                var lines = System.IO.File.ReadAllLines(ctrl.Server.MapPath("~/Scripts/StandardPJList.txt"));
                var ret = new Dictionary<string, string>();
                foreach (var line in lines)
                {
                    if (line.Contains("##"))
                    {
                        continue;
                    }

                    if (line.Contains(":::"))
                    {
                        var kvpair = line.Split(new string[] { ":::" }, StringSplitOptions.RemoveEmptyEntries);
                        if (!ret.ContainsKey(kvpair[0].Trim()))
                        {
                            ret.Add(kvpair[0].Trim(), kvpair[1].Trim());
                        }
                    }//end if
                }//end foreach
                return ret;
            }

            public static Dictionary<string, string> GetNPIMachine(Controller ctrl)
            {
                var lines = System.IO.File.ReadAllLines(ctrl.Server.MapPath("~/Scripts/npidepartmentmachine.txt"));
                var ret = new Dictionary<string, string>();
                foreach (var line in lines)
                {
                    if (line.Contains("##"))
                    {
                        continue;
                    }

                    if (line.Contains(":::"))
                    {
                        var kvpair = line.Split(new string[] { ":::" }, StringSplitOptions.RemoveEmptyEntries);
                        if (!ret.ContainsKey(kvpair[0].Trim()))
                        {
                            ret.Add(kvpair[0].Trim().ToUpper(), kvpair[1].Trim());
                        }
                    }//end if
                }//end foreach
                return ret;
            }

        }

        public class SeverHtmlDecode
        {
            public static string Decode(Controller ctrl, string src)
            {
                return ctrl.Server.HtmlDecode(src).Replace("border=\"0\"", "border=\"2\"");
            }
        }
    
}