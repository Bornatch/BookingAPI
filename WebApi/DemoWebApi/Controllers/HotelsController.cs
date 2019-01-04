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

        // GET: api/Hotels
        public IQueryable<Hotel> GetHotels()
        {
            return db.Hotels;
        }

        // GET: api/Hotels/5
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

        
        //GET : api/Hotels/
        //[HttpGet]
        //[Route("api/Hotels/id/dateStart/dateEnd/location/{persons:int}")]
        [ResponseType(typeof(List<Hotel>))]
        [Route("api/Hotels/dateStart/dateEnd/location/{persons:int}")]
        public async Task<IHttpActionResult> GetAvailableHotels(string dateStartText, string dateEndText, string location, int persons)
        {
            //based on paramaters, this method will return a list of all possible rooms
            List<Hotel> results = new List<Hotel>();

            //the datestart is set at midnight, we had seconds in order to do accurate comparisions in the query
            DateTime dateStart = convertToDate(dateStartText);
            DateTime dateEnd = convertToDate(dateEndText);

            dateStart = dateStart.AddSeconds(86399);
            dateEnd = dateEnd.AddSeconds(86399);

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
            //check location
            var z = from h in db.Hotels
                    where h.Location.Equals(location)
                    select h.IdHotel
                    ;
            z.ToList();

            // check busy rooms (reservations during the selected dates)
            var y = from r in db.Rooms
                    join rresa in db.RoomReservations
                    on r.IdRoom equals rresa.Room.IdRoom
                    join res in db.Reservations
                    on rresa.Reservation.IdReservation equals res.IdReservation
                    join h in db.Hotels
                    on r.IdHotel equals h.IdHotel
                    where (dateStart <= res.DateStart &&
                           dateEnd >= res.DateEnd)
                    select r.IdRoom;
                    ;
            y.ToList();

            var x = from r in db.Rooms
                    where !y.Contains(r.IdRoom) && z.Contains(r.IdHotel)
                    select r.IdHotel    ;


            var q = from h in db.Hotels
                    join r in db.Rooms
                    on h.IdHotel equals r.IdHotel
                    where x.Contains(r.IdHotel)
                    select h;
            q.Distinct();

            /*
            foreach (Hotel h in q)
            {
                results.Add(h);
            }
            */
            Hotel newTest = new Hotel
            {
                Name = "Test hotel",
                Description = "Test for API"
            };

            results.Add(newTest);
            //.Distinct

            return Ok(results);

        }

        public DateTime convertToDate(string dateText)
        {
            DateTime result = new DateTime(1000, 01, 01);

            result.Day.Equals(dateText.Substring(0, 2));
            result.Month.Equals(dateText.Substring(3, 5));
            result.Year.Equals(dateText.Substring(6, 8));

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
 