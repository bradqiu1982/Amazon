﻿<%@ WebHandler Language="C#" Class="ImageUpload" %>

using System;
using System.Web;
using System.IO;
using Amazon.Models;

public class ImageUpload : IHttpHandler {

    public void ProcessRequest (HttpContext context) {
        try
        {
            HttpPostedFile uploads = context.Request.Files[0];
            string fn = System.IO.Path.GetFileName(uploads.FileName);
            string url = "";

            var bimage = true;

            if(string.Compare(Path.GetExtension(fn),".jpg",true) == 0
                ||string.Compare(Path.GetExtension(fn),".png",true) == 0
                ||string.Compare(Path.GetExtension(fn),".gif",true) == 0
                ||string.Compare(Path.GetExtension(fn),".jpeg",true) == 0)
            {
                bimage = true;

                string datestring = DateTime.Now.ToString("yyyyMMdd");
                string imgdir = context.Server.MapPath(".") + "\\images\\" + datestring + "\\";
                if (!Directory.Exists(imgdir))
                {
                    Directory.CreateDirectory(imgdir);
                }

                fn = Path.GetFileNameWithoutExtension(fn)+"-"+DateTime.Now.ToString("yyyyMMddHHmmss")+Path.GetExtension(fn);
                fn = fn.Replace(" ", "_").Replace("#", "").Replace("'", "")
                            .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");

                uploads.SaveAs(imgdir + fn);
                url = "/userfiles/images/" +datestring+"/"+ fn;
            }
            else
            {
                bimage = false;

                string datestring = DateTime.Now.ToString("yyyyMMdd");
                string imgdir = context.Server.MapPath(".") + "\\docs\\" + datestring + "\\";
                if (!Directory.Exists(imgdir))
                {
                    Directory.CreateDirectory(imgdir);
                }

                fn = Path.GetFileNameWithoutExtension(fn)+"-"+DateTime.Now.ToString("yyyyMMddHHmmss")+Path.GetExtension(fn);
                fn = fn.Replace(" ", "_").Replace("#", "").Replace("'", "")
                            .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");

                uploads.SaveAs(imgdir + fn);
                url = "/userfiles/docs/" +datestring+"/"+ fn;

                //var dict = CookieUtility.UnpackCookie(new HttpRequestWrapper(context.Request));
                //if (dict.ContainsKey("issuekey")
                //    && !string.IsNullOrEmpty(dict["issuekey"])
                //    && dict.ContainsKey("currentaction")
                //    && string.Compare(dict["currentaction"],"UpdateIssue") == 0)
                //{
                //    IssueViewModels.StoreIssueAttachment(dict["issuekey"], url);
                //}
            }

            if (bimage)
            {
                context.Response.Write("<p><img src='" + url + "'/></p>");
            }
            else
            {
                context.Response.Write("<p><a href='" + url + "' target='_blank'>"+fn+"</a></p>");
            }

            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
        catch (Exception ex)
        {
            try
            {

                string url = "Failed to upload file for: " + ex.ToString();
                context.Response.Write(url);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex1)
            { }
        }
    }

    public bool IsReusable {

        get {
            return false;
        }

    }



}
