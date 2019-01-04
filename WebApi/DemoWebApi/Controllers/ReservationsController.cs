using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using DemoWebApi.Models;

namespace DemoWebApi.Controllers
{
    public class ReservationsController : ApiController
    {
        private HotelContext db = new HotelContext();

        // GET: api/Reservations
        public IQueryable<Reservation> GetReservations()
        {
            return db.Reservations;
        }

        //id = id Client
        public List<Reservation> GetAllReservations(int id)
        {
            List<Reservation> results = null;

            var q = from res in db.Reservations
                    where res.IdClient.Equals(id)
                    select res
                    ;

            foreach (Reservation r in q)
            {
                results.Add(r);
            }
            return results;
        }

        public int GetLastIdReservation()
        {
            int id = 0;

            //"SELECT TOP 1 * FROM Reservations ORDER BY idReservation DESC";
            var q = from r in db.Reservations
                    orderby r.IdReservation
                    select r;

            id = q.First().IdReservation;

            return id;
        }

        // GET: api/Reservations/5
        [ResponseType(typeof(Reservation))]
        public async Task<IHttpActionResult> GetReservation(int id)
        {
            Reservation reservation = await db.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            return Ok(reservation);
        }

        // PUT: api/Reservations/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutReservation(int id, Reservation reservation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != reservation.IdReservation)
            {
                return BadRequest();
            }

            db.Entry(reservation).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(id))
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

        // POST: api/Reservations
        [ResponseType(typeof(Reservation))]
        public async Task<IHttpActionResult> AddNewReservation(int idClient, DateTime dateStart, DateTime dateEnd, decimal totalPrice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Reservation reservation = new Reservation();
            reservation.IdClient = idClient;
            reservation.DateStart = dateStart;
            reservation.DateEnd = dateEnd;
            reservation.TotalPrice = totalPrice;

            db.Reservations.Add(reservation);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = reservation.IdReservation }, reservation);
        }

        // DELETE: api/Reservations/5
        [ResponseType(typeof(Reservation))]
        public async Task<IHttpActionResult> DeleteReservation(int id)
        {
            Reservation reservation = await db.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            db.Reservations.Remove(reservation);
            await db.SaveChangesAsync();

            return Ok(reservation);
        }

        [ResponseType(typeof(RoomReservation))]
        public async Task<IHttpActionResult> DeleteRoomReservation(int id)
        {
            RoomReservation roomReservation = await db.RoomReservations.FindAsync(id);
            if(roomReservation == null)
            {
                return NotFound();
            }

            db.RoomReservations.Remove(roomReservation);
            await db.SaveChangesAsync();

            return Ok(roomReservation);

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ReservationExists(int id)
        {
            return db.Reservations.Count(e => e.IdReservation == id) > 0;
        }
    }
}