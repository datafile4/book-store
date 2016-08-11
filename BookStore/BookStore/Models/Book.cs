using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Models
{
    public class Book
    {        
        public string Name { get; set; }
        public string Author { get; set; }
        public string ImageURL { get; set; }
        public  decimal Price { get; set; }
        public int GenreID { get; set; }        
        public int LanguageID { get; set; }
        public int UserID { get; set; }
        
    }
}