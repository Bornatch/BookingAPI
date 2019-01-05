using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using DemoWebApi.Models;

namespace DemoWebApi.Controllers
{
    public class HotelsController : ApiController
    {
        static String baseUri = "http://localhost:3749/api/";
        private HotelContext db = new HotelContext();        

        // GET: api/Hotels/GetHotel/1
        [ResponseType(typeof(Hotel))]
        public async Task<IHttpActionResult> GetHotel(int id)
        {
            Hotel hotel = await db.Hotels.FindAsync(id);
            if (hotel == null)
            {
                return NotFound();
            }
            return Ok(hotel);
        }


        // GET: api/Hotels/5/Rooms
        [ResponseType(typeof(Room))]
        [Route("api/Hotels/{id}/rooms")]
        public async Task<IHttpActionResult> GetRoomsHotel(int id)
        {
            Hotel hotel = await db.Hotels.FindAsync(id);
            if (hotel == null)
            {
                return NotFound();
            }

            List<Room> rooms = new List<Room>();

            foreach (Room r in db.Rooms)
            {
                if (r.IdHotel == id)
                    rooms.Add(r);
            }       


            return Ok(rooms);
        }


        //GET : api/Hotels/GetAvailableHotels/1
        [ResponseType(typeof(Hotel))]
        public List<Hotel> GetAvailableHotels(string dateStart, string dateEnd, string location, int persons)

        {
            //,string dateEnd, string location, int persons
            //based on paramaters, this method will return a list of all possible rooms
            List<Hotel> results = new List<Hotel>();
            
            //the datestart is set at midnight, we had seconds in order to do accurate comparisions in the query
            DateTime dateStartdate = convertToDate(dateStart);
            DateTime dateEnddate = convertToDate(dateEnd);

            dateStartdate = dateStartdate.AddSeconds(86399);
            dateEnddate = dateEnddate.AddSeconds(86399);

            List<Hotel> all = db.Hotels.ToList();

            /* isolation of each select from SQL
             * SELECT DISTINCT Hotel.IdHotel, Name, Hotel.Description, 
                Location, Category, HasWifi, HasParking, Phone, Email, Website 
                FROM Hotel 
	                INNER JOIN Room ON Room.IdHotel = Hotel.IdHotel 
		                WHERE Room.IdHotel IN(
			                SELECT Room.IdHotel 
			                FROM Room 
			                WHERE IdRoom NOT IN( 
				                SELECT Room.IdRoom 
				                FROM Room 
				                INNER JOIN RoomReservation 
				                ON Room.IdRoom = RoomReservation.IdRoom 
				                INNER JOIN Reservation 
				                ON RoomReservation.IdReservation = Reservation.idReservation
				                INNER JOIN Hotel 
				                ON Room.IdHotel = Hotel.IdHotel 
				                WHERE(@DateStart <= Reservation.DateEnd) 
				                AND(@DateEnd >= Reservation.DateStart))
			                AND Hotel.IdHotel IN( 
				                SELECT IdHotel 
				                FROM Hotel 
				                WHERE Hotel.Location = @Location) 
			                GROUP BY Room.IdHotel, Room.IdRoom);
            */
            // check location
            // generates a list of hotels that are in the desired location
            var z = from h in db.Hotels
                    where h.Location.Equals(location)
                    select h.IdHotel
                    ;
            z.ToList();           
            // Z IS WORKING !

            // check busy rooms (reservations during the selected dates)
            // generates a list of rooms that are already booked
            var y = from r in db.Rooms
                    join rresa in db.RoomReservations
                    on r.IdRoom equals rresa.Room.IdRoom
                    join res in db.Reservations
                    on rresa.Reservation.IdReservation equals res.IdReservation
                    join h in db.Hotels
                    on r.IdHotel equals h.IdHotel
                    where (dateStartdate <= res.DateStart &&
                           dateEnddate >= res.DateEnd)
                    select r.IdRoom;

            y.ToList();

            // Y IS WORKING !
            var x = from r in db.Rooms
                    where !y.Contains(r.IdRoom) && z.Contains(r.IdHotel)
                    select r.IdRoom;
            x.ToList();

            // X IS WORKING !
            var q = from h in db.Hotels
                    join r in db.Rooms
                    on h.IdHotel equals r.IdHotel
                    where x.Contains(r.IdRoom)
                    select h;
            q.Distinct();
            q.ToList();
            foreach (Hotel h in q.Distinct())
            {
                results.Add(h);
            }
            return results;

        }

        //GET : api/Hotels/GetAvailableHotelsAdvanced/1
        [ResponseType(typeof(Hotel))]
        public List<Hotel> GetAvailableHotelsAdvanced(string dateStart, string dateEnd, string location, int persons,
            string hasWifiS, string hasParkingS, int category, string hasTvS, string hasHairDryerS)
        {
            //based on paramaters, this method will return a list of all possible rooms
            List<Hotel> results = new List<Hotel>();

            //the datestart is set at midnight, we had seconds in order to do accurate comparisions in the query
            DateTime dateStartdate = convertToDate(dateStart);
            DateTime dateEnddate = convertToDate(dateEnd);

            
            var hasWifi = new Boolean();
            var hasParking = new Boolean();
            var hasTv = new Boolean();
            var hasHairDryer = new Boolean();

            if (hasWifiS.Equals("true"))
                hasWifi = true;
            
            if (hasParkingS.Equals("true"))
                hasParking = true;

            if (hasTvS.Equals("true"))
                hasTv = true;

            if (hasHairDryerS.Equals("true"))
                hasHairDryer = true;

            
            dateStartdate = dateStartdate.AddSeconds(86399);
            dateEnddate = dateEnddate.AddSeconds(86399);

            List<Hotel> all = db.Hotels.ToList();

            /* isolation of each select from SQL
             * SELECT DISTINCT Hotel.IdHotel, Name, Hotel.Description, Location, Category, HasWifi, HasParking, Phone, Email, Website 
                FROM Hotel 
                INNER JOIN Room ON Room.IdHotel = Hotel.IdHotel 
                WHERE Room.IdHotel IN( 
                        SELECT Room.IdHotel 
                        FROM Room 
                        WHERE IdRoom NOT IN( 
                                SELECT Room.IdRoom 
                                FROM Room 
                                INNER JOIN RoomReservation ON Room.IdRoom = RoomReservation.IdRoom 
                                INNER JOIN Reservation ON RoomReservation.IdReservation = Reservation.idReservation 
                                INNER JOIN Hotel ON Room.IdHotel = Hotel.IdHotel 
                                WHERE(@DateStart <= Reservation.DateEnd) 
                                AND (@DateEnd >= Reservation.DateStart) 
                                AND Room.Type != @Persons "
            */

            //check HairDryer
            var hairDryerList = new List<int>();
            if (hasHairDryer == true)
            {
                var hairDryer = from r in db.Rooms
                                where r.HasHairDryer.Equals(hasHairDryer)
                                select r.IdHotel
                                ;
                hairDryerList = hairDryer.ToList();
            }
            else
            {
                var hairDryer = from h in db.Hotels
                                select h.IdHotel;
                hairDryerList = hairDryer.ToList();
            }

            //check Tv
            var tvList = new List<int>();
            if (hasTv == true)
            {
                var tv = from r in db.Rooms
                         where r.HasTV.Equals(hasTv)
                         select r.IdHotel
                         ;
                tvList = tv.ToList();
            }
            else
            {
                var tv = from h in db.Hotels
                         select h.IdHotel;
                tvList = tv.ToList();
            }

            //check Category
            var stars = from h in db.Hotels
                        where h.Category >= category
                        select h.IdHotel
                        ;

            //check Parking
            var parcList = new List<int>();
            if (hasParking == true)
            {
                var parc = from h in db.Hotels
                           where h.HasParking.Equals(hasParking)
                           select h.IdHotel
                           ;
                parcList = parc.ToList();
            }
            else
            {
                var parc = from h in db.Hotels
                           select h.IdHotel;
                parcList = parc.ToList();
            }

            //check Wifi
            var wifiList = new List<int>();
            if (hasWifi == true)
            {
                var wifi = from h in db.Hotels
                           where h.HasWifi.Equals(hasWifi)
                           select h.IdHotel
                           ;
                wifiList = wifiList.ToList();
            }
            else
            {
                var wifi = from h in db.Hotels
                           select h.IdHotel
                           ;
                wifiList = wifiList.ToList();
            }
                

            // check location
            // generates a list of hotels that are in the desired location
            var loc = from h in db.Hotels
                      where h.Location.Equals(location)
                      select h.IdHotel
                      ;
            loc.ToList();
            // Zloc IS WORKING !            

            // check busy rooms (reservations during the selected dates)
            // generates a list of rooms that are already booked
            var busy = from r in db.Rooms
                       join rresa in db.RoomReservations
                       on r.IdRoom equals rresa.Room.IdRoom
                       join res in db.Reservations
                       on rresa.Reservation.IdReservation equals res.IdReservation
                       join h in db.Hotels
                       on r.IdHotel equals h.IdHotel
                       where (dateStartdate <= res.DateStart &&
                       dateEnddate >= res.DateEnd)
                       select r.IdRoom;

            busy.ToList();

            // Ybusy IS WORKING !
            var roomsOk = from r in db.Rooms
                    where !busy.Contains(r.IdRoom) 
                    && loc.Contains(r.IdHotel)
                    && wifiList.Contains(r.IdHotel)
                    && parcList.Contains(r.IdHotel)
                    && stars.Contains(r.IdHotel)
                    && tvList.Contains(r.IdHotel)
                    && hairDryerList.Contains(r.IdHotel)
                          select r.IdRoom;
            roomsOk.ToList();
            // XroomsOK IS WORKING !
            var q = from h in db.Hotels
                    join r in db.Rooms
                    on h.IdHotel equals r.IdHotel
                    where roomsOk.Contains(r.IdRoom)
                    select h;
            q.Distinct();
            q.ToList();
            foreach (Hotel h in q.Distinct())
            {
                results.Add(h);
            }
            return results;

        }

        // GET: api/Hotels
        public List<Hotel> GetHotels()
        {
            return db.Hotels.ToList();
        }

        public DateTime convertToDate(string dateText)
        {
            string day = dateText.Substring(0, 2);
            string month = dateText.Substring(3, 2);
            string year = dateText.Substring(6, 4);

            DateTime result = new DateTime(Convert.ToInt32(year),
                                            Convert.ToInt32(month), 
                                            Convert.ToInt32(day));
            return result;
        }

        // PUT: api/Hotels/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutHotel(int id, Hotel hotel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != hotel.IdHotel)
            {
                return BadRequest();
            }

            db.Entry(hotel).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HotelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Hotels
        [ResponseType(typeof(Hotel))]
        public async Task<IHttpActionResult> PostHotel(Hotel hotel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Hotels.Add(hotel);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = hotel.IdHotel }, hotel);
        }

        // DELETE: api/Hotels/5
        [ResponseType(typeof(Hotel))]
        public async Task<IHttpActionResult> DeleteHotel(int id)
        {
            Hotel hotel = await db.Hotels.FindAsync(id);
            if (hotel == null)
            {
                return NotFound();
            }

            db.Hotels.Remove(hotel);
            await db.SaveChangesAsync();

            return Ok(hotel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool HotelExists(int id)
        {
            return db.Hotels.Count(e => e.IdHotel == id) > 0;
        }
    }
}
 