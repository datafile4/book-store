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
        [Key]
        public int ID { get; set; }

        public string Name { get; set; }
        public string Author { get; set; }

        [Display(Name ="Genre")] /* */ 
        public string GenreID { get; set; }
        [ForeignKey("GenreID")]
        public virtual Genre Genres { get; set; } /*fetches genre's primary keys. Probably. */

        [Display(Name = "Language")]  
        public string LanguageID { get; set; }
        [ForeignKey("LanguageID")]
        public virtual Genre Languages { get; set; }
    }
}