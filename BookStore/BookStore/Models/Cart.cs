using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Cart
    {
        public int CartId { get; set; }
        public string ProductId { get; set; }
        //we keep track on the date, not the date or time
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString ="{0:yyyy-MM-dd}",ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
    }
}