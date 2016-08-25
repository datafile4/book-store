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
                using (var cmd = new SqlCommand("uspLogin", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = model.Username.ToLower();
                    cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = model.Password.ToLower();

                    UserID = (int)(cmd.ExecuteScalar() ?? 0);
                    if (UserID < 1)
                        return Ok(false, "Login failed. Check your username/password");
                }


                const string dtFormat = "yyyy-MM-dd HH:mm:ss.fffffff zzz";
                var now = DateTimeOffset.Now;
                var expireDate = now.AddMonths(3);
                string GuidStr = Guid.NewGuid().ToString().ToLower();

                using (var cmd = new SqlCommand("uspInsertUserLogin", con))
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;
                    cmd.Parameters.Add("@GuidStr", SqlDbType.NVarChar).Value = GuidStr;
                    cmd.Parameters.Add("@expireDate", SqlDbType.NVarChar).Value = expireDate.ToString(dtFormat);
                    cmd.Parameters.Add("@now", SqlDbType.DateTimeOffset).Value = now.ToString(dtFormat);

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
                model.Username = model.Username.ToLower();
                model.Email = model.Email.ToLower();

                using (var cmd = new SqlCommand("uspCheckUsername", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = model.Username;


                    var result = (int)(cmd.ExecuteScalar() ?? 0);
                    if (result > 0)
                        return Ok(false, "Username already exists!");
                }

                using (var cmd = new SqlCommand("uspCheckEmail", con))
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = model.Email;

                    var result = (int)(cmd.ExecuteScalar() ?? 0);
                    if (result > 0)
                        return Ok(false, "This email has already been registered once!");
                }

                using (SqlCommand cmd = new SqlCommand("uspRegister", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar).Value = model.FirstName;
                    cmd.Parameters.Add("@LastName", SqlDbType.NVarChar).Value = model.LastName;
                    cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = model.Username;
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
        public IHttpActionResult AddToCart(int ID)
        {
            using (var con = new SqlConnection(conStr))
            {
                con.Open();

                using (var cmd = new SqlCommand("uspCheckBookInCart", con))
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = CurrentUserID;
                    cmd.Parameters.Add("@BookID", SqlDbType.Int).Value = ID;

                    var result = (int)(cmd.ExecuteScalar() ?? 0);
                    if (result > 0)
                        return Ok(false, "Book have already added!");
                }

                using (var cmd = new SqlCommand("uspAddToCart", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = CurrentUserID;
                    cmd.Parameters.Add("@BookID", SqlDbType.Int).Value = ID;

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
        public IHttpActionResult RemoveFromCart(int ID)
        {
            using (var con = new SqlConnection(conStr))
            {
                con.Open();

                using (var cmd = new SqlCommand("uspRemoveFromCart", con))
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = CurrentUserID;
                    cmd.Parameters.Add("@BookID", SqlDbType.Int).Value = ID;


                    var affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows < 1)
                    {
                        return Ok(false, "Failed to remove book. Try again!");
                    }
                }

                return Ok(true, "Book is successfully removed from cart!");
            }
        }

        [HttpGet]
        public UserModel GetUserInfo(int ID)
        {
            using (var con = new SqlConnection(conStr))
            {
                con.Open();

                using (var cmd = new SqlCommand("uspGetUserInfo", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = ID;

                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        return CreateUser(reader);
                    }
                    return null;
                }
            }
        }

        [HttpGet]
        [RequiresRole]
        public UserModel GetCurrentUserInfo()
        {
            return GetUserInfo(CurrentUserID);
        }

        [HttpGet, RequiresRole]
        public IEnumerable<BookModel> GetCartItems()
        {
            List<BookModel> returnModel = new List<BookModel>();

            using (var con = new SqlConnection(conStr))
            {
                con.Open();

                using (var cmd = new SqlCommand("uspGetCartItems", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = CurrentUserID;

                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        returnModel.Add(CreateBook(reader, 0));
                    }
                    return returnModel;
                }
            }
        }

        [HttpPost, RequiresRole, ValidateModel]
        public IHttpActionResult UploadBook(Book model)
        {
            using (var con = new SqlConnection(conStr))
            {
                con.Open();

                using (var cmd = new SqlCommand("uspUploadBook", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = model.Name;
                    cmd.Parameters.Add("@author", SqlDbType.NVarChar).Value = model.Author;
                    cmd.Parameters.Add("@ImageURL", SqlDbType.NVarChar).Value = model.ImageURL;
                    cmd.Parameters.Add("@price", SqlDbType.Decimal).Value = model.Price;
                    cmd.Parameters.Add("@langID", SqlDbType.Int).Value = model.LanguageID;
                    cmd.Parameters.Add("@genreID", SqlDbType.Int).Value = model.GenreID;
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = CurrentUserID;

                    var affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows < 1)
                        return Ok(false, "Could not upload book, try again!");
                }

                return Ok(true, "The book is successfully uploaded!");
            }
        }

        [HttpPost]
        public IEnumerable<BookModel> GetBooks(PaginationModel pagination)
        {
            List<BookModel> returnModel = new List<BookModel>();

            using (var con = new SqlConnection(conStr))
            {
                con.Open();

                using (var cmd = new SqlCommand("uspGetBooks", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@PageNumber", SqlDbType.Int).Value = pagination.PageNumber;
                    cmd.Parameters.Add("@PageLength", SqlDbType.Int).Value = pagination.PageLength;

                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        returnModel.Add(CreateBook(reader));
                    }
                    return returnModel;
                }
            }
        }

        [HttpGet]
        public IEnumerable<BookModel> GetRandomBooks(int pageLength)
        {
            List<BookModel> returnModel = new List<BookModel>();

            using (var con = new SqlConnection(conStr))
            {
                con.Open();

                using (var cmd = new SqlCommand("uspGetBooks", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@PageLength", SqlDbType.Int).Value = pageLength;

                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        returnModel.Add(CreateBook(reader));
                    }
                    return returnModel;
                }
            }
        }

        [HttpGet]
        public BookModel GetBookInfo(int ID)
        {
            using (var con = new SqlConnection(conStr))
            {
                con.Open();

                using (var cmd = new SqlCommand("uspGetBookInfo", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@BookID", SqlDbType.Int).Value = ID;
                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        return CreateBook(reader);
                    }
                    return null;
                }
            }
        }

        [HttpPost, RequiresRole(Roles.Moderator)]
        public IHttpActionResult ConfirmBook(int ID)
        {
            using (var con = new SqlConnection(conStr))
            {
                con.Open();
                using (var cmd = new SqlCommand("uspConfirmBook", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@BookID", SqlDbType.Int).Value = ID;
                    var affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows < 1)
                    {
                        return Ok(false, "Error! Try again");
                    }

                    return Ok(true, "Successful");
                }

            }

        }


        [HttpPost, RequiresRole(Roles.Admin)]
        public IHttpActionResult SetRole(string username, int roleID)
        {
            using (var con = new SqlConnection(conStr))
            {
                con.Open();
                using (var cmd = new SqlCommand("uspSetRole", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = username;
                    cmd.Parameters.Add("@Role", SqlDbType.Int).Value = roleID;

                    var affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows < 1)
                    {
                        return Ok(false, "Error! Try again");
                    }

                    return Ok(true, "Successful");
                }

            }
        }

        [HttpGet, RequiresRole(Roles.Moderator)]
        public IEnumerable<BookModel> GetUnconfirmedBooks()
        {
            List<BookModel> returnModel = new List<BookModel>();

            using (var con = new SqlConnection(conStr))
            {
                con.Open();

                using (var cmd = new SqlCommand("uspGetUnconfirmedBooks", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        returnModel.Add(CreateBook(reader));
                    }
                    return returnModel;
                }
            }

        }

        [HttpGet]
        public IEnumerable<NameIDModel> GetRoles()
        {
            List<NameIDModel> returnModel = new List<NameIDModel>();

            using (var con = new SqlConnection(conStr))
            {
                con.Open();
                using (var cmd = new SqlCommand("uspGetAllRoles", con))
                {
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        returnModel.Add(new NameIDModel
                        {
                            ID = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }

                    return returnModel;
                }

            }
        }

        [HttpGet]
        public IEnumerable<NameIDModel> GetGenres()
        {
            List<NameIDModel> returnModel = new List<NameIDModel>();

            using (var con = new SqlConnection(conStr))
            {
                con.Open();
                using (var cmd = new SqlCommand("uspGetAllGenres", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    var result = cmd.ExecuteReader();


                    while (result.Read())
                    {
                        var genre = new NameIDModel
                        {
                            ID = result.GetInt32(0),
                            Name = result.GetString(1)
                        };

                        returnModel.Add(genre);
                    }

                    return returnModel;
                }
            }
        }

        [HttpGet]
        public IEnumerable<NameIDModel> GetLanguages()
        {
            List<NameIDModel> getlangs = new List<NameIDModel>();

            using (var con = new SqlConnection(conStr))
            {
                con.Open();
                using (var cmd = new SqlCommand("uspGetAllLanguages", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    var result = cmd.ExecuteReader();


                    while (result.Read())
                    {
                        var lang = new NameIDModel
                        {
                            ID = result.GetInt32(0),
                            Name = result.GetString(1)
                        };

                        getlangs.Add(lang);
                    }

                    return getlangs;
                }
            }
        }

        [HttpPost, RequiresRole(Roles.Moderator)]
        public IHttpActionResult ConfirmBooks(IEnumerable<int> IDs)
        {
            using (var con = new SqlConnection(conStr))
            {
                con.Open();

                using (var cmd = new SqlCommand("uspConfirmBooks", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter Param = cmd.Parameters.AddWithValue("@BookIDs", CreateItemTable(IDs));

                    Param.SqlDbType = SqlDbType.Structured;
                    Param.TypeName = "IntListType";

                    int affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows < 1)
                        return Ok(false, "Some Not Confirmed !!! Try again ... ");
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

        [HttpPost, RequiresRole(Roles.Moderator)]
        public IHttpActionResult DeleteBooks(IEnumerable<int> IDs)
        {
            using (var con = new SqlConnection(conStr))
            {
                con.Open();

                using (var cmd = new SqlCommand("uspDeleteBooks", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter Param = cmd.Parameters.AddWithValue("@BookIDs", CreateItemTable(IDs));

                    Param.SqlDbType = SqlDbType.Structured;
                    Param.TypeName = "IntListType";

                    var affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows < 1)
                        return Ok(false, "Not deleted !!! Try again ... ");
                }

                return Ok(true, "The books are successfully deleted !!!");
            }
        }

        [HttpGet]
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
            return "0";
        }

        [HttpGet, RequiresRole]
        public IHttpActionResult RateUser(int ID, int rate)
        {
            if (rate < 0 || rate > 5) return Ok(false, "Rate can be only between 0 and 5");

            using (var con = new SqlConnection(conStr))
            {
                con.Open();
                using (var cmd = new SqlCommand("uspRateUser", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = ID;
                    cmd.Parameters.Add("@GiverID", SqlDbType.Int).Value = CurrentUserID;
                    cmd.Parameters.Add("@StarID", SqlDbType.Int).Value = rate;

                    var affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows < 1)
                        return Ok(false, "Something bad happened!");
                }

                return Ok(true, "User is sucessfully rated!");
            }
        }

        [HttpGet, RequiresRole]
        public string GetRatedStar(int ID)
        {
            using (var con = new SqlConnection(conStr))
            {
                con.Open();
                using (var cmd = new SqlCommand("uspGetRatedStar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = ID;
                    cmd.Parameters.Add("@GiverID", SqlDbType.Int).Value = CurrentUserID;

                    return (cmd.ExecuteScalar() ?? 0).ToString();
                }
            }
        }

        [HttpPost]
        public IEnumerable<BookModel> GetFilteredBooks(FilterModel filter)
        {
            var returnModel = new List<BookModel>();
            SqlParameter Param;
            using (var con = new SqlConnection(conStr))
            {
                con.Open();
                using (var cmd = new SqlCommand("uspGetFilteredBooks", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    Param = cmd.Parameters.AddWithValue("@BookIDs", CreateItemTable(filter.GenreIDs));
                    Param.SqlDbType = SqlDbType.Structured;
                    Param.TypeName = "IntListType";

                    Param = cmd.Parameters.AddWithValue("@LangIDs", CreateItemTable(filter.LangIDs));
                    Param.SqlDbType = SqlDbType.Structured;
                    Param.TypeName = "IntListType";

                    Param = cmd.Parameters.AddWithValue("@SearchTerms", CreateItemTable(filter.SearchTerms));
                    Param.SqlDbType = SqlDbType.Structured;
                    Param.TypeName = "StringListType";

                    cmd.Parameters.Add("@LowPrice", SqlDbType.Decimal).Value = filter.LowPrice;
                    cmd.Parameters.Add("@HighPrice", SqlDbType.Decimal).Value = filter.HighPrice;

                    cmd.Parameters.Add("@PageNumber", SqlDbType.Int).Value = filter.Pagination.PageNumber;
                    cmd.Parameters.Add("@PageLength", SqlDbType.Int).Value = filter.Pagination.PageLength;


                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        returnModel.Add(CreateBook(reader));
                    }

                }
            }

            return returnModel;
        }

        private static DataTable CreateItemTable<T>(IEnumerable<T> items)
        {
            DataTable table = new DataTable();
            table.Columns.Add("item", typeof(T));
            foreach (T item in items)
            {
                table.Rows.Add(item);
            }
            return table;
        }

        private static UserModel CreateUser(SqlDataReader reader, int startFrom = 0)
        {
            return new UserModel
            {
                ID = reader.GetInt32(startFrom++),
                FirstName = reader.GetString(startFrom++),
                LastName = reader.GetString(startFrom++),
                Username = reader.GetString(startFrom++),
                Email = reader.GetString(startFrom++),
                ImageUrl = reader.GetString(startFrom++),
                RoleID = reader.GetInt32(startFrom++),
                Ratings = new RatingModel
                {
                    Star1 = reader.GetInt32(startFrom++),
                    Star2 = reader.GetInt32(startFrom++),
                    Star3 = reader.GetInt32(startFrom++),
                    Star4 = reader.GetInt32(startFrom++),
                    Star5 = reader.GetInt32(startFrom++)
                }
            };

        }

        private static BookModel CreateBook(SqlDataReader reader, int startFrom = 0)
        {
            return new BookModel
            {
                ID = reader.GetInt32(startFrom++),
                Name = reader.GetString(startFrom++),
                Author = reader.GetString(startFrom++),
                Price = reader.GetDecimal(startFrom++),
                ImageURL = reader.GetString(startFrom++),
                Language = reader.GetString(startFrom++),
                Genre = reader.GetString(startFrom++),
                Uploader = CreateUser(reader, startFrom)
            };
        }
    }
}
