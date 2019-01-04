﻿using System;
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

            foreach(var id in z)
            {
                Console.WriteLine(id);
            }

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

            foreach (var id in y)
            {
                Console.WriteLine(id);
            }

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

        // GET: api/Hotels
        public IQueryable<Hotel> GetHotels()
        {
            return db.Hotels;
        }

        public DateTime convertToDate(string dateText)
        {
            DateTime result = new DateTime(1000, 01, 01);
            //09-01-2019
            string day = dateText.Substring(0, 2);
            string month = dateText.Substring(3, 2);
            string year = dateText.Substring(6, 4);

            result.Day.Equals(day);
            result.Month.Equals(month);
            result.Year.Equals(year);

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
 