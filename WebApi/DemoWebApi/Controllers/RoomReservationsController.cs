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
using DemoWebApi.Controllers;

namespace DemoWebApi.Controllers
{
    public class RoomReservationsController : ApiController
    {
        private HotelContext db = new HotelContext();

        // GET: api/RoomReservations
        public IQueryable<RoomReservation> GetRoomReservations()
        {
            return db.RoomReservations;
        }

        // GET: api/RoomReservations/5
        [ResponseType(typeof(RoomReservation))]
        public async Task<IHttpActionResult> GetRoomReservation(int id)
        {
            RoomReservation roomReservation = await db.RoomReservations.FindAsync(id);
            if (roomReservation == null)
            {
                return NotFound();
            }

            return Ok(roomReservation);
        }

        // PUT: api/RoomReservations/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutRoomReservation(int id, RoomReservation roomReservation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != roomReservation.IdRoomReservation)
            {
                return BadRequest();
            }

            db.Entry(roomReservation).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomReservationExists(id))
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

        // POST: api/RoomReservations
        [ResponseType(typeof(RoomReservation))]
        public async Task<IHttpActionResult> PostRoomReservation(RoomReservation roomReservation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.RoomReservations.Add(roomReservation);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = roomReservation.IdRoomReservation }, roomReservation);
        }

        // DELETE: api/RoomReservations/5
        [ResponseType(typeof(RoomReservation))]
        public async Task<IHttpActionResult> DeleteRoomReservation(int id)
        {
            RoomReservation roomReservation = await db.RoomReservations.FindAsync(id);
            if (roomReservation == null)
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

        private bool RoomReservationExists(int id)
        {
            return db.RoomReservations.Count(e => e.IdRoomReservation == id) > 0;
        }
    }
}