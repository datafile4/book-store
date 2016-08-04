using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BookStore.Models
{
    public class User
    {
        [Key]
        public int ID { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        [Display(Name = "Card")]
        public string CardID { get; set; }
        [ForeignKey("CardID")]
        public virtual Card Cards { get; set; } //fetches primary keys
    }
}