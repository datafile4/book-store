using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore.Models
{
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class RegisterModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserInfoModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }

    public class BookModel
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public string ImageURL { get; set; }
        public string Genre { get; set; }
        public string Language { get; set; }
        public decimal Pirce { get; set; } 
        public string Username { get; set; }
    }


}