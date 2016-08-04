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
        public int ID { get; set; }              
        public string BookID { get; set; }
     
        //we keep track on the date, not the date or time
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString ="{0:yyyy-MM-dd}",ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
    }
}