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

                return Ok(true, "Book is successfully added into cart!");
            }
        }

        [HttpPost]
        [RequiresLogin]
        public IHttpActionResult RemoveFromCart(int bookID)
        {
            using (var con = new SqlConnection(conStr))
            {
                con.Open();
                string queryString = $@"Delete from Carts
                                             where UserID = {CurrentUserID} 
                                             and
                                             BookID = {bookID}";


                using (var cmd = new SqlCommand(queryString, con))
                {
                    var affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows < 1)
                    {
                        return Ok(false, "Failed to remove book. Try again!");
                    }
                }

                return Ok(true, "Book is successfully removed from cart!");
            }
        }

        [HttpPost]
        public UserInfoModel GetUserInfo(string username)
        {
            using (var con = new SqlConnection(conStr))
            {
                con.Open();
                username = username.ToLower();
                string queryString =
                       $@"select email, lastname, firstname 
                        from Users
                        where username = '{username}'";

                //null if not found
                UserInfoModel returnModel = null;
                using (var cmd = new SqlCommand(queryString, con))
                {
                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        returnModel = new UserInfoModel();
                        returnModel.Email = reader.GetString(0);
                        returnModel.LastName = reader.GetString(1);
                        returnModel.FirstName = reader.GetString(2);
                        returnModel.Username = username;
                    }

                    return returnModel;
                }
            }
        }

        [HttpPost]
        [RequiresLogin] // needed to get CurrentUserID
        public UserInfoModel GetCurrentUserInfo()
        {
            using (var con = new SqlConnection(conStr))
            {
                con.Open();
                return GetUserInfo(CurrentUserID, con);
            }
        }

        [HttpPost, RequiresLogin]
        public IEnumerable<BookModel> GetCartItems()
        {
            List<BookModel> returnModels = null;

            using (var con = new SqlConnection(conStr))
            {
                con.Open();
                string queryString =
                       $@"select BookID
                        from Carts
                        where UserID = {CurrentUserID}";

                using (var cmd = new SqlCommand(queryString, con))
                {
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        returnModels.Add(GetBookInfo(reader.GetInt32(0), con));
                    }

                    return returnModels;
                }
            }
        }

        private UserInfoModel GetUserInfo(int userID, SqlConnection con)
        {
            string queryString =
                   $@"select email, lastname, firstname, username
                        from Users
                        where id = {userID}";

            //null if not found
            UserInfoModel returnModel = null;
            using (var cmd = new SqlCommand(queryString, con))
            {
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    returnModel = new UserInfoModel();
                    returnModel.Email = reader.GetString(0);
                    returnModel.LastName = reader.GetString(1);
                    returnModel.FirstName = reader.GetString(2);
                    returnModel.Username = reader.GetString(3);
                }

                return returnModel;
            }
        }
        private BookModel GetBookInfo(int bookID, SqlConnection con)
        {
            BookModel returnModel = null;

            string queryString =
                       $@"select Name,Author,ImageURL,
                        Price,LangID,GenreID,UserID
                        from Books
                        where BookID = {bookID}";
            using (var cmd = new SqlCommand(queryString, con))
            {
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    returnModel = new BookModel();

                    returnModel.Name = reader.GetString(0);
                    returnModel.Author = reader.GetString(1);
                    returnModel.ImageURL = reader.GetString(2);
                    returnModel.Pirce = reader.GetDecimal(3);
                    int langID = reader.GetInt32(4);
                    returnModel.Language = GetName(langID, "Langs", con);
                    int GenreID = reader.GetInt32(5);
                    returnModel.Genre= GetName(GenreID, "Genres", con);
                    int userID = reader.GetInt32(6);
                    returnModel.Uploader = GetUserInfo(userID, con);
                }
            }
            return returnModel;
        }
        private string GetName(int ID, string tableName, SqlConnection con)
        {
            string returnString = null;
            string queryString =
                     $@"select Name
                        from {tableName}
                        where id = {ID}";
            using (var cmd = new SqlCommand(queryString, con))
            {
                returnString = cmd.ExecuteScalar() as string;
                return returnString;
            }
        }

    }
}
