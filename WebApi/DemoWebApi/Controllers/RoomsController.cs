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
    public class RoomsController : ApiController
    {
        private HotelContext db = new HotelContext();

        public List<Room> GetAllRoomsByListId(List<int> idRooms)
        {
            List<Room> ListOfRooms = new List<Room>();

            for (int i = 0; i < idRooms.Count; i++)
            {
                ListOfRooms.Add(GetRoomById(idRooms[i]));
            }

            return ListOfRooms;
        }

        public Room GetRoomById(int id)
        //id = idRoom
        {
            Room result = db.Rooms.Find(id);

            return result;
        }


        //api/Rooms/GetRooms/1/01-01-2019/01-02-2019
        public List<Room> GetRooms(int idHotel, string dateStart, string dateEnd)
        {
            DateTime dateStartdate = convertToDate(dateStart);
            DateTime dateEnddate = convertToDate(dateEnd);

            // (1) Get list of IdRooms from unavailable rooms
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

            // (2) Get all available rooms from desired hotel
            var x = from r in db.Rooms
                    where r.IdHotel.Equals(idHotel) && !y.Contains(r.IdRoom)
                    select r;

            

            List<Room> results = new List<Room>();
            int totalNumberOfAvailableRooms = x.Count();
            int totalNumberRooms = CountAllRooms(idHotel);

            foreach (Room r in x.ToList())
            {
                // (3) Modify price if criteria is met before adding to list of results
                
                //count number of nights
                //source : https://stackoverflow.com/questions/33345344/calculate-number-of-nights-between-2-datetimes

                var frm = dateStartdate < dateEnddate ? dateStartdate : dateEnddate;
                var to = dateStartdate < dateEnddate ? dateEnddate : dateStartdate;
                int totalDays = (int)(to - frm).TotalDays;


                //if booked over 70%, increase price by 20%
                if (totalNumberOfAvailableRooms <= totalNumberRooms * 30 / 100)
                {
                    r.Price = (decimal)r.Price * 120 / 100 * totalDays;
                }
                else
                {
                    r.Price = (decimal)r.Price * totalDays;
                }

                results.Add(r);
            }

            return results;
        }

        public int CountAllRooms(int id)
            //id = idHotel
        {
            int result = 0;
            var x = from r in db.Rooms
                    where r.IdHotel.Equals(id)
                    select r;
            result = x.Count();

            return result;
        }

        // GET: api/Rooms/5
        [ResponseType(typeof(Room))]
        public async Task<IHttpActionResult> GetRoom(int id)
        {
            Room room = await db.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            return Ok(room);
        }

        // PUT: api/Rooms/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutRoom(int id, Room room)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != room.IdRoom)
            {
                return BadRequest();
            }

            db.Entry(room).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomExists(id))
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

        // POST: api/Rooms
        [ResponseType(typeof(Room))]
        public async Task<IHttpActionResult> PostRoom(Room room)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Rooms.Add(room);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = room.IdRoom }, room);
        }

        // DELETE: api/Rooms/5
        [ResponseType(typeof(Room))]
        public async Task<IHttpActionResult> DeleteRoom(int id)
        {
            Room room = await db.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            db.Rooms.Remove(room);
            await db.SaveChangesAsync();

            return Ok(room);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RoomExists(int id)
        {
            return db.Rooms.Count(e => e.IdRoom == id) > 0;
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
    }
}