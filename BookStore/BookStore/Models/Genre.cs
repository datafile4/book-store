using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Genre
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
    }
}