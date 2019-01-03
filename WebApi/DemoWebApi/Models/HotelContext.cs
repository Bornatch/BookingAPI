using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DemoWebApi.Models
{
    public class HotelContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public HotelContext() : base("name=HotelContext")
        {
        }

        public System.Data.Entity.DbSet<DemoWebApi.Models.Client> Clients { get; set; }

        public System.Data.Entity.DbSet<DemoWebApi.Models.Hotel> Hotels { get; set; }

        public System.Data.Entity.DbSet<DemoWebApi.Models.Picture> Pictures { get; set; }

        public System.Data.Entity.DbSet<DemoWebApi.Models.Reservation> Reservations { get; set; }

        public System.Data.Entity.DbSet<DemoWebApi.Models.Room> Rooms { get; set; }

        public System.Data.Entity.DbSet<DemoWebApi.Models.RoomReservation> RoomReservations { get; set; }
    }
}
