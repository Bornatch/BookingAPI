using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoWebApi.Models
{
    public class Room
    {
        [Key]
        public int IdRoom { get; set; }
        public int IdHotel { get; set; }
        public int Number { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
        public Decimal Price { get; set; }
        public Boolean HasTV { get; set; }
        public Boolean HasHairDryer { get; set; }
    }
}