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
using BookStore.Attributes;

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
        public const string conStr = //@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=BookStore;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
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

        [HttpPost, ValidateModel]
        public IHttpActionResult Login(LoginModel model)
        {
            using (var con = new SqlConnection(conStr))
            {
                con.Open();

                int UserID;
                using (var cmd = new SqlCommand("uspLoginProc", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = model.Username;
                    cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = model.Password;

                    UserID = (int)(cmd.ExecuteScalar() ?? 0);
                    if (UserID < 1)
                        return Ok(false, "Login failed. Check your username/password");
                }


                const string dtFormat = "yyyy-MM-dd HH:mm:ss.fffffff zzz";
                var now = DateTimeOffset.Now;
                var expireDate = now.AddMonths(3);
                string GuidStr = Guid.NewGuid().ToString().ToLower();

                using (var cmd = new SqlCommand("uspInsertIntoUserLogins", con))
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;
                    cmd.Parameters.Add("@GuidStr", SqlDbType.NVarChar).Value = GuidStr;
                    cmd.Parameters.Add("@expireDate", SqlDbType.NVarChar).Value = expireDate.ToString(dtFormat);
                    cmd.Parameters.Add("@now", SqlDbType.NVarChar).Value = now.ToString(dtFormat);

                    var affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows < 1)
                    {
                        return Ok(false, "Login Failed. Try again!");
                    }
                }

                var responseMsg = new HttpResponseMessage(HttpStatusCode.OK);
                var cookie = new CookieHeaderValue(RequiresRoleAttribute.LoginToken, GuidStr);
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

        [HttpPost, ValidateModel]
        public IHttpActionResult Register(RegisterModel model)
        {
            using (var con = new SqlConnection(conStr))
            {
                con.Open();

                using (var cmd = new SqlCommand("uspCheckUsername", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = model.Username.ToLower();


                    var result = (int)(cmd.ExecuteScalar() ?? 0);
                    if (result > 0)
                        return Ok(false, "Username already exists!");
                }

                using (var cmd = new SqlCommand("uspCheckEmail", con))
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = model.Email.ToLower();

                    var result = (int)(cmd.ExecuteScalar() ?? 0);
                    if (result > 0)
                        return Ok(false, "This email has already been registered once!");
                }

                using (SqlCommand cmd = new SqlCommand("uspRegister", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar).Value = model.FirstName;
                    cmd.Parameters.Add("@LastName", SqlDbType.NVarChar).Value = model.LastName;
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
        [RequiresRole]
        public IHttpActionResult AddToCart(int bookID)
        {
            using (var con = new SqlConnection(conStr))
            {
                con.Open();

                using (var cmd = new SqlCommand("uspAddToCartCheck", con))
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = CurrentUserID;
                    cmd.Parameters.Add("@BookID", SqlDbType.Int).Value = bookID;

                    var result = (int)(cmd.ExecuteScalar() ?? 0);
                    if (result > 0)
                        return Ok(false, "Book have already added!");
                }

                using (var cmd = new SqlCommand("uspAddToCart", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = CurrentUserID;
                    cmd.Parameters.Add("@BookID", SqlDbType.Int).Value = bookID;

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
        [RequiresRole]
        public IHttpActionResult RemoveFromCart(int bookID)
        {
            using (var con = new SqlConnection(conStr))
            {
                con.Open();

                using (var cmd = new SqlCommand("uspRemoveFromCart", con))
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = CurrentUserID;
                    cmd.Parameters.Add("@BookID", SqlDbType.Int).Value = bookID;


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

                //null if not found
                UserInfoModel returnModel = null;
                using (var cmd = new SqlCommand("uspGetUserInfo", con))
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
        [RequiresRole]
        public UserInfoModel GetCurrentUserInfo()
        {
            using (var con = new SqlConnection(conStr))
            {
                con.Open();
                UserInfoModel returnModel = null;
                using (var cmd = new SqlCommand("uspGetUserInfo", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = CurrentUserID;
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
        }

        [HttpPost, RequiresRole]
        public IEnumerable<BookModel> GetCartItems()
        {
            List<BookModel> returnModels = new List<BookModel>();

            using (var con = new SqlConnection(conStr))
            {
                con.Open();

                using (var cmd = new SqlCommand("uspGetBookInfo", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = CurrentUserID;

                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var book = new BookModel
                        {
                            Name = reader.GetString(0),
                            Author = reader.GetString(1),
                            ImageURL = reader.GetString(2),
                            Pirce = reader.GetDecimal(3),
                            Language = reader.GetString(4),
                            Genre = reader.GetString(5),
                            Uploader = new UserInfoModel
                            {
                                FirstName = reader.GetString(6),
                                LastName = reader.GetString(7),
                                Username = reader.GetString(8),
                                Email = reader.GetString(9)
                            }
                        };

                        returnModels.Add(book);
                    }
                    return returnModels;
                }
            }
        }

        [HttpPost, RequiresRole, ValidateModel]
        public IHttpActionResult UploadBook(Book book)
        {
            using (var con = new SqlConnection(conStr))
            {
                con.Open();

                using (var cmd = new SqlCommand("uspUploadBook", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = book.Name;
                    cmd.Parameters.Add("@author", SqlDbType.NVarChar).Value = book.Author;
                    cmd.Parameters.Add("@ImageURL", SqlDbType.NVarChar).Value = book.ImageURL;
                    cmd.Parameters.Add("@langID", SqlDbType.Int).Value = book.LanguageID;
                    cmd.Parameters.Add("@genreID", SqlDbType.Int).Value = book.GenreID;
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = CurrentUserID;
                    cmd.Parameters.Add("@price", SqlDbType.Decimal).Value = book.Price;

                    var affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows < 1)
                        return Ok(false, "Could not upload book, try again!");
                }

                return Ok(true, "The book is successfully uploaded!");
            }
        }

        [HttpGet]
        public IEnumerable<BookModel> GetAllBooks()
        {
            List<BookModel> allBookData = new List<BookModel>();

            using (var con = new SqlConnection(conStr))
            {
                con.Open();

                using (var cmd = new SqlCommand("spGetAllBooks", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var book = new BookModel
                        {
                            Name = reader.GetString(0),
                            Author = reader.GetString(1),
                            ImageURL = reader.GetString(2),
                            Pirce = reader.GetDecimal(3),
                            Language = reader.GetString(4),
                            Genre = reader.GetString(5),
                            Uploader = new UserInfoModel
                            {
                                FirstName = reader.GetString(6),
                                LastName = reader.GetString(7),
                                Username = reader.GetString(8),
                                Email = reader.GetString(9)
                            }
                        };

                        allBookData.Add(book);
                    }
                    return allBookData;
                }
            }
        }

        [HttpPost]
        public IEnumerable<BookModel> GetBookInfo(int bookid)
        {
            List<BookModel> allBookData = new List<BookModel>();

            using (var con = new SqlConnection(conStr))
            {
                con.Open();

                using (var cmd = new SqlCommand("uspGetBookInfo", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@BookID", SqlDbType.Int).Value = bookid;
                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        var book = new BookModel
                        {
                            Name = reader.GetString(0),
                            Author = reader.GetString(1),
                            ImageURL = reader.GetString(2),
                            Pirce = reader.GetDecimal(3),
                            Language = reader.GetString(4),
                            Genre = reader.GetString(5),
                            Uploader = new UserInfoModel
                            {
                                FirstName = reader.GetString(6),
                                LastName = reader.GetString(7),
                                Username = reader.GetString(8),
                                Email = reader.GetString(9)
                            }
                        };

                        allBookData.Add(book);
                    }
                    return allBookData;
                }
            }
        }

        [HttpPost, RequiresRole(Roles.Moderator)]
        public IHttpActionResult ConfirmBook(int bookID)
        {
            using (var con = new SqlConnection(conStr))
            {
                con.Open();
                using (var cmd = new SqlCommand("uspConfirmBook", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@BookID", SqlDbType.Int).Value = bookID;
                    var affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows < 1)
                    {
                        return Ok(false, "Error! Try again");
                    }

                    return Ok(true, "Successful");
                }

            }

        }


        [RequiresRole(Roles.Admin)]
        public IHttpActionResult SetRole(int UserID, int RoleID)
        {
            using (var con = new SqlConnection(conStr))
            {
                con.Open();
                using (var cmd = new SqlCommand("uspSetRole", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;
                    cmd.Parameters.Add("@Role", SqlDbType.Int).Value = RoleID;

                    var affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows < 1)
                    {
                        return Ok(false, "Error! Try again");
                    }

                    return Ok(true, "Successful");
                }

            }
        }

        [HttpPost, RequiresRole(Roles.User)]
        public IHttpActionResult NumberOfSoldBooks()
        {
            using (var con = new SqlConnection(conStr))
            {
                con.Open();
                using (var cmd = new SqlCommand("uspNumberOfSold", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = CurrentUserID;
                    var result = cmd.ExecuteReader();
                    int count = 0;
                    if (result.Read())
                    {
                        count = result.GetInt32(0);
                    }

                    return Ok(new { value = count });
                }

            }
        }

        [HttpPost, RequiresRole(Roles.User)]
        public IHttpActionResult NumberOfAllBooks()
        {
            using (var con = new SqlConnection(conStr))
            {
                con.Open();
                using (var cmd = new SqlCommand("uspNumberOfBooks", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = CurrentUserID;
                    var result = cmd.ExecuteReader();
                    int count = 0;
                    if (result.Read())
                    {
                        count = result.GetInt32(0);
                    }

                    return Ok(new { value = count });
                }
            }
        }

        [HttpPost, RequiresRole(Roles.User)]
        public IHttpActionResult NumberOfUnSold()
        {
            using (var con = new SqlConnection(conStr))
            {
                con.Open();
                using (var cmd = new SqlCommand("uspNumberOfUnSoldBooks", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = CurrentUserID;
                    var result = cmd.ExecuteReader();
                    int count = 0;
                    if (result.Read())
                    {
                        count = result.GetInt32(0);
                    }

                    return Ok(new { value = count });
                }
            }
        }

        [HttpGet, RequiresRole(Roles.Moderator)]
        public IEnumerable<BookModel> GetBooksForConfirmation()
        {
            List<BookModel> allBookData = new List<BookModel>();

            using (var con = new SqlConnection(conStr))
            {
                con.Open();

                using (var cmd = new SqlCommand("spUserPageForAdmin", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var book = new BookModel
                        {
                            ID = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Author = reader.GetString(2),
                            ImageURL = reader.GetString(3),
                            Pirce = reader.GetDecimal(4),
                            Language = reader.GetString(5),
                            Genre = reader.GetString(6),
                            Uploader = new UserInfoModel
                            {
                                FirstName = reader.GetString(7),
                                LastName = reader.GetString(8),
                                Username = reader.GetString(9),
                                Email = reader.GetString(10)
                            }
                        };

                        allBookData.Add(book);
                    }
                    return allBookData;
                }
            }

        }

        [HttpPost, RequiresRole(Roles.Moderator)]
        public IHttpActionResult ConfirmBook(List<int> BooksID)
        {
            using (var con = new SqlConnection(conStr))
            {
                con.Open();

                using (var cmd = new SqlCommand("uspConfirmBook ", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    var affectedRows = 0;
                    foreach (var bookID in BooksID)
                    {
                        cmd.Parameters.Add("@bookID", SqlDbType.Int).Value = bookID;
                        affectedRows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        if (affectedRows < 1)
                            return Ok(false, "Not Confirmed !!! Try again ... ");
                    }

                }

                return Ok(true, "The book is successfully Confirmed!!!");
            }
        }

        [HttpPost, RequiresRole(Roles.Moderator)]
        public IHttpActionResult DeleteBook(int ID)
        {
            using (var con = new SqlConnection(conStr))
            {
                con.Open();

                using (var cmd = new SqlCommand("uspDeleteBook", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@bookID", SqlDbType.Int).Value = ID;

                    var affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows < 1)
                        return Ok(false, "Not deleted !!! Try again ... ");
                }

                return Ok(true, "The book is successfully deleted !!!");
            }
        }

        public string GetUserRole()
        {
            var login = Request.Headers.GetCookies(RequiresRoleAttribute.LoginToken).FirstOrDefault();

            if (login != null)
            {
                string guid = login[RequiresRoleAttribute.LoginToken].Value;
                using (var con = new SqlConnection(conStr))
                {
                    con.Open();

                    using (var cmd = new SqlCommand("uspGetUserRole", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Guid", SqlDbType.NVarChar).Value = guid;
                        return (cmd.ExecuteScalar() ?? 0).ToString();
                    }
                }
            }
            return 0.ToString();
        }
    }
}
