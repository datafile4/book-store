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
        public int ID { get; set; }

        public string Name { get; set; }
        public string Author { get; set; }
        public string GenreID { get; set; }        
          
        public string LanguageID { get; set; }
        
    }
}