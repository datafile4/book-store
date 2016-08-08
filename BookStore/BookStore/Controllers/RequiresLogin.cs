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

                using (var cmd = new SqlCommand(queryString, con))
                {
                    var result = (int)(cmd.ExecuteScalar() ?? 0);
                    if (result < 1)
                        return false;
                }

                return true;
            }
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var login = actionContext.Request.Headers.GetCookies(LoginToken).FirstOrDefault();

            if ( login != null)
            {

                if (CheckDatabase(login[LoginToken].Value))
                {
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
