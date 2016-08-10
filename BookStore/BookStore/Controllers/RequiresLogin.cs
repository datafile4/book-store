using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace BookStore.Controllers
{
    public class RequiresLoginAttribute : ActionFilterAttribute
    {

        public const string LoginToken = "user-g";
        
        private bool CheckDatabase(string guid)
        {
            using (var con = new SqlConnection(BookStoreController.conStr))
            {
                con.Open();
                string queryString =
                    $@"select id 
                        from Tokens
                        where GUID = '{guid}'";

                int TokenID;
                using (var cmd = new SqlCommand(queryString, con))
                {
                    TokenID = (int)(cmd.ExecuteScalar() ?? 0);
                    if (TokenID < 1)
                        return false;
                }

                const string dtFormat = "yyyy-MM-dd HH:mm:ss.fffffff zzz";
                queryString =
                    $@"Update Tokens
                        Set LastLogin = '{DateTimeOffset.Now.ToString(dtFormat)}'
                        where id = {TokenID}";

                using (var cmd = new SqlCommand(queryString, con))
                {
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
                        string queryString =
                            $@"select userid 
                              from UserLogins
                              where GUID = '{guid}'";

                        int userID;
                        using (var cmd = new SqlCommand(queryString, con))
                        {
                            userID = (int)(cmd.ExecuteScalar() ?? 0);
                            (actionContext.ControllerContext.Controller as BookStoreController).CurrentUserID = userID;
                        }
                    }

                    return;
                }
            }

            var response = new HttpResponseMessage(HttpStatusCode.Redirect);
            ///TODO: add return url into uri after successful log in.
            response.Headers.Location = new Uri($"http://{actionContext.Request.RequestUri.Authority}/login.html");
            actionContext.Response = response;
        }
    }
}
