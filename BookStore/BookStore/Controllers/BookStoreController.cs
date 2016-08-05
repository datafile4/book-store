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
       @"Data Source=superbookstore.database.windows.net;
        Initial Catalog = BookStore;
        Integrated Security = False;
        User ID = emiraslan;
        Password=Orxan12Aslan24;
        Connect Timeout = 15;
        Encrypt=False;
        TrustServerCertificate=True;
        ApplicationIntent=ReadWrite;
        MultiSubnetFailover=False";
        #endregion



        [HttpPost]
        public IHttpActionResult Login(LoginModel model)
        {
            IHttpActionResult requestResult;
            using (var con = new SqlConnection(conStr))
            {
                con.Open();

                string queryString =
                    $@"select id 
                        from Users
                        where username = '{model.Username}' 
                        or email ='{model.Username}' 
                        and password = '{model.Password}'";

                using (var cmd = new SqlCommand(queryString, con))
                {
                    var reader = (int)cmd.ExecuteScalar();
                    if (reader < 1)
                    {
                        requestResult = BadRequest("Login failed. Check your username/password");
                    }
                    else
                    {
                        requestResult = Ok("Sucessfully logged in");
                    }
                }
                con.Close();
                return requestResult;
            }


        }
    }
}
