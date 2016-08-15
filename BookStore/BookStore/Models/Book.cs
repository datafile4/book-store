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
        [Required]
        public string Name { get; set; }
        [Required]
        public string Author { get; set; }
        [Url]
        [Required]
        public string ImageURL { get; set; }
        [Required]
        [Range(0,1000)]
        public decimal? Price { get; set; }
        [Required]
        public int? GenreID { get; set; }
        [Required]
        public int? LanguageID { get; set; }
        [Required]
        public int? UserID { get; set; }
        
    }
}