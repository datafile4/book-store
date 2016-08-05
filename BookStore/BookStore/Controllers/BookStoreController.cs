using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BookStore.Controllers
{

    public class BookStoreController : ApiController
    {
        #region ConnectionString
        ///actually storing password in public repository 
        ///is dangerous, but anyway. Database does not hold 
        ///anything  important :)
        ///
        ///TODO: save conectionString in Web.config file
        const string conStr =
                        @"Data Source=amiraslan.database.windows.net;
                          Initial Catalog=Enumeration;
                          Integrated Security=False;
                          User ID=emiraslan;
                          
                          Password=Orxan12Aslan24;
                          
                          Connect Timeout=60;
                          Encrypt=True;
                          TrustServerCertificate=False;
                          ApplicationIntent=ReadWrite;
                          MultiSubnetFailover=False";
        #endregion



        [HttpPost]
        public IHttpActionResult Login(LoginModel model)
        {
            using (var con = new SqlConnection(conStr))
            {
                con.Open();

                string queryString =
                    $@"select id 
                        from Users
                        where username = '{model.Username.ToLower()}' 
                        or email ='{model.Username.ToLower()}' 
                        and password = '{model.Password}'";

                using (var cmd = new SqlCommand(queryString, con))
                {
                    var reader = (int)(cmd.ExecuteScalar() ?? 0);
                    if (reader < 1)
                        return Ok(new
                        {
                            success = false,
                            message = "Login failed. Check your username/password"
                        });

                }

                return Ok(new
                {
                    success = true,
                    message = "Sucessfully logged in"
                });
            }
        }
    }
}
