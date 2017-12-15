using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Amazon.Models
{
    public class UserVM
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public string LoginKey { set; get; }

        public static bool CheckUserExist(string username)
        {
            var dbret = RetrieveUser(username);
            if (dbret != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void RegistUser()
        {
            var sql = @"insert into UserTable(UserName,PassWD,UpdateDate) 
                values(@UserName,@PassWD,@UpdateDate)";
            var param = new Dictionary<string, string>();
                param.Add("@UserName", Email.ToUpper());
                param.Add("@PassWD", Password);
                param.Add("@UpdateDate", DateTime.Now.ToString());
            DBUtility.ExeLocalSqlNoRes(sql, param);
        }

        public static UserVM RetrieveUser(string username)
        {
            var sql = @"select PassWD from UserTable where UserName = @UserName";
            var param = new Dictionary<string, string>();
                param.Add("@UserName", username.ToUpper());
            var dbret = DBUtility.ExeLocalSqlWithRes(sql, null, param);
            if (dbret.Count > 0)
            {
                var ret = new UserVM();
                ret.Email = username.ToUpper();
                ret.Password = Convert.ToString(dbret[0][0]);
                return ret;
            }
            return null;
        }

        public static void RestPwd(string username, string pwd)
        {
            var sql = @"update UserTable set PassWD = @PassWD 
                    where UserName = @UserName";
            var param = new Dictionary<string, string>();
                param.Add("@PassWD", pwd);
                param.Add("@UserName", username.ToUpper());
            DBUtility.ExeLocalSqlNoRes(sql, param);
        }

        public static void UpdateLoginKey(string username, string loginkey)
        {
            var sql = @"update  UserTable set APVal1 = @APVal1 where UserName = @UserName";
            var param = new Dictionary<string, string>();
                param.Add("@APVal1", loginkey);
                param.Add("@UserName", username.ToUpper());
            DBUtility.ExeLocalSqlNoRes(sql, param);
        }

        public static string RetrieveUserByKey(string loginkey)
        {
            var ret = string.Empty;
            var sql = @"select UserName from UserTable where APVal1 = @APVal1";
            var param = new Dictionary<string, string>();
                param.Add("@APVal1", loginkey);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql, null, param);
            if (dbret.Count > 0)
            {
                ret = Convert.ToString(dbret[0][0]);
            }
            return ret;
        }
    }
}