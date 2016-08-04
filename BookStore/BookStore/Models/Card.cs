using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;

namespace BookStore.Models
{
    public class Card
    {
        [Key]        
        public int ID { get; set; }        

        [Display(Name = "Book")]
        public string BookID { get; set; }
        [ForeignKey("BookID")]
        public virtual Book Books { get; set; } //fetches primary keys


        //we keep track on the date, not the date or time
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString ="{0:yyyy-MM-dd}",ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
    }
}