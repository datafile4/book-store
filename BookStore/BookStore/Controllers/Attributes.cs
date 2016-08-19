using BookStore.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace BookStore.Attributes
{

    public enum Roles
    {
        User = 1,
        Moderator,
        Admin
    }

    public class RequiresRoleAttribute : ActionFilterAttribute
    {

        public const string LoginToken = "user-g";
        private int roleID;

        public RequiresRoleAttribute() { roleID = 1; }
        public RequiresRoleAttribute(Roles Role)
        {
            roleID = (int) Role;
        }


        private bool CheckDatabase(string guid)
        {
            using (var con = new SqlConnection(BookStoreController.conStr))
            {
                con.Open();

                using (var cmd = new SqlCommand("uspGetUserRole", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Guid", SqlDbType.NVarChar).Value = guid;
                    int role = (int)(cmd.ExecuteScalar() ?? 0);
                    if (role < roleID)
                        return false;
                }

                const string dtFormat = "yyyy-MM-dd HH:mm:ss.fffffff zzz";

                using (var cmd = new SqlCommand("uspUpdateLastLogin", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@LastLogin", SqlDbType.DateTimeOffset).Value = DateTimeOffset.Now.ToString(dtFormat);
                    cmd.Parameters.Add("@Guid", SqlDbType.NVarChar).Value = guid;

                    int affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows < 1)
                        return false;
                }

                return true;
            }
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var login = actionContext.Request.Headers.GetCookies(LoginToken).FirstOrDefault();

            if (login != null)
            {
                string guid = login[LoginToken].Value;
                if (CheckDatabase(guid))
                {

                    using (var con = new SqlConnection(BookStoreController.conStr))
                    {
                        con.Open();

                        int userID;
                        using (var cmd = new SqlCommand("uspGetUserID", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@Guid", SqlDbType.NVarChar).Value = guid;

                            userID = (int)cmd.ExecuteScalar();
                            (actionContext.ControllerContext.Controller as BookStoreController).CurrentUserID = userID;
                        }
                    }

                    return;
                }
            }

            var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            actionContext.Response = response;
        }
    }

    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.ModelState.IsValid == false)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, actionContext.ModelState);
            }
        }
    }
}
