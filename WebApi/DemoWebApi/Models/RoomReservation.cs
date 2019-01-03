using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DemoWebApi.Models
{
    public class RoomReservation
    {
        [Key]
        public int IdRoomReservation { get; set; }
        public Room Room { get; set; }
        public Reservation Reservation { get; set; }
        public int Quantity { get; set; }
    }
}