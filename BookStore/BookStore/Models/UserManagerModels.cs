using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
    public class RegisterModel
    {

        [Required(ErrorMessage = "First name is required!")]
        public string FirstName { get; set; }


        [Required(ErrorMessage = "Last name is required!")]
        public string LastName { get; set; }

        [Required (ErrorMessage ="Username is required!")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Username must be at least 5 characters long.")]
        public string Username { get; set; }


        [Required(ErrorMessage = "The email address is required!")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage ="Password is required!")]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }
        public string Contact { get; set; }
    }

    public class UserInfoModel
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
    public class BookModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string ImageURL { get; set; }
        public string Genre { get; set; }
        public string Language { get; set; }
        public decimal Pirce { get; set; } 
        public UserInfoModel Uploader { get; set; }
    }
}