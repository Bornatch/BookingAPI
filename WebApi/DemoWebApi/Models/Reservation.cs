using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DemoWebApi.Models
{
    public class Reservation
    {
        [Key]
        public int IdReservation { get; set; }
        public int IdClient { get; set; }
        public Decimal TotalPrice { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string hotelName { get; set; }
    }
}