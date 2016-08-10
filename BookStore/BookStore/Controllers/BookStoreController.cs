using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Web.Http.Controllers;
using BookStore.Models;
using System.Text;
using Newtonsoft.Json;

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
        public const string conStr =
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
        /// <summary>
        /// Available only for methods with RequiresLoginAttribute
        /// </summary>
        public int CurrentUserID { get; set; }

        public IHttpActionResult Ok(bool scs, string msg)
        {
            return Ok(new
            {
                success = scs,
                message = msg
            });
        }
        [HttpPost]
        public IHttpActionResult Login(LoginModel model)
        {
            using (var con = new SqlConnection(conStr))
            {
                con.Open();
                string queryString =
                    $@"select id 
                        from Users
                        where (username = '{model.Username.ToLower()}' 
                        or email ='{model.Username.ToLower()}') 
                        and password = '{model.Password}'";


                int UserID;
                using (var cmd = new SqlCommand(queryString, con))
                {
                    UserID = (int)(cmd.ExecuteScalar() ?? 0);
                    if (UserID < 1)
                        return Ok(false, "Login failed. Check your username/password");
                }



                const string dtFormat = "yyyy-MM-dd HH:mm:ss.fffffff zzz";
                var now = DateTimeOffset.Now;
                var expireDate = now.AddMonths(3);
                string GuidStr = Guid.NewGuid().ToString().ToLower();

                queryString = $@"insert into UserLogins
                                             values(
                                             {UserID}, 
                                            '{GuidStr}',                            
                                            '{expireDate.ToString(dtFormat)}', 
                                            '{now.ToString(dtFormat)}')";

                using (var cmd = new SqlCommand(queryString, con))
                {
                    var affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows < 1)
                    {
                        return Ok(false, "Login Failed. Try again!");
                    }
                }

                var responseMsg = new HttpResponseMessage(HttpStatusCode.OK);
                var cookie = new CookieHeaderValue(RequiresLoginAttribute.LoginToken, GuidStr);
                cookie.Expires = expireDate;
                cookie.Domain = Request.RequestUri.Host;
                cookie.Path = "/";
                responseMsg.Headers.AddCookies(new[] { cookie });

                var successMsg = new { success = true, message = "Sucessfully logged in" };
                var param = JsonConvert.SerializeObject(successMsg);
                responseMsg.Content = new StringContent(param, Encoding.UTF8, "application/json");

                return ResponseMessage(responseMsg);
            }
        }

        [HttpPost]
        public IHttpActionResult Register(RegisterModel model)
        {
            using (var con = new SqlConnection(conStr))
            {
                con.Open();

                string queryString = $"select id from Users where username = '{model.Username.ToLower()}'";
                using (var cmd = new SqlCommand(queryString, con))
                {
                    var result = (int)(cmd.ExecuteScalar() ?? 0);
                    if (result > 0)
                        return Ok(false, "Username already exists!");
                }

                queryString = $"select id from Users where email = '{model.Email.ToLower()}'";
                using (var cmd = new SqlCommand(queryString, con))
                {
                    var result = (int)(cmd.ExecuteScalar() ?? 0);
                    if (result > 0)
                        return Ok(false, "This email has already been registered once!");
                }

                using (SqlCommand cmd = new SqlCommand("uspRegisterUser", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    //yes, we need to add parameters in sequence
                    cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar).Value = model.FirstName;
                    cmd.Parameters.Add("@LastName", SqlDbType.NVarChar).Value = model.LastName;
                    //we don't need to make the username lowercase. Stored procedure will do this
                    cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = model.Username;
                    cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = model.Password;
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = model.Email;

                    var affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows < 1)
                    {
                        return Ok(false, "Registration Failed. Try It Again!");
                    }
                }

                return Ok(true, "Sucessfully Registered!");
            }
        }

        [HttpPost]
        [RequiresLogin]
        public IHttpActionResult AddToCart(int bookID)
        {
            using (var con = new SqlConnection(conStr))
            {
                con.Open();
                string queryString = $@"select id from Carts
                                             where UserID = {CurrentUserID} 
                                             and
                                             BookID = {bookID}";


                using (var cmd = new SqlCommand(queryString, con))
                {
                    var result = (int)(cmd.ExecuteScalar() ?? 0);
                    if (result > 0)
                        return Ok(false, "Book have already added!");
                }

                queryString = $@"insert into Carts
                                             values({CurrentUserID}, 
                                             {bookID})";

                using (var cmd = new SqlCommand(queryString, con))
                {
                    var affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows < 1)
                    {
                        return Ok(false, "Adding book to cart failed. Try again!");
                    }
                }

                return Ok(true, "Book successfully added into cart!");
            }
        }





        [HttpGet]
        [RequiresLogin]
        public IHttpActionResult SetCookie()
        {
            var resp = new HttpResponseMessage();

            Debug.WriteLine(CurrentUserID);

            var cookie = new CookieHeaderValue("session-id", "12sd345");
            cookie.Expires = DateTimeOffset.Now.AddDays(1);
            cookie.Domain = Request.RequestUri.Host;
            cookie.Path = "/";

            resp.Headers.AddCookies(new[] { cookie });

            return ResponseMessage(resp);
        }
    }
}
