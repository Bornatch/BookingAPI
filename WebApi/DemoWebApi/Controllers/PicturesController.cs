﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using DemoWebApi.Models;

namespace DemoWebApi.Controllers
{
    public class PicturesController : ApiController
    {


        private HotelContext db = new HotelContext();

        // GET: api/Pictures
        public IQueryable<Picture> GetPictures()
        {
            return db.Pictures;
        }

        // GET: api/Pictures/5
        [ResponseType(typeof(Picture))]
        public async Task<IHttpActionResult> GetPicture(int id)
        {
            Picture picture = await db.Pictures.FindAsync(id);
            if (picture == null)
            {
                return NotFound();
            }

            return Ok(picture);
        }

        [ResponseType(typeof(Picture))]
        //[Route("api/pictures/GetPicturesHotel/2")]
        public List<string> GetPicturesHotel(int id)
        {
            Hotel hotel = db.Hotels.Find(id);
            

            if (hotel == null)
            {
                return null;
            }

            List<Room> rooms = new List<Room>();
            List<Picture> pictures = new List<Picture>();
            

            foreach (Room r in db.Rooms)
            {
                if (r.IdHotel == id)
                {                 
                    foreach (Picture p in db.Pictures)
                    {
                        if (p.Room == r)
                            pictures.Add(p);
                    }
                }                    
            }
            List<string> results = new List<string>();

            foreach(Picture p in pictures)
            {
                results.Add(p.Url);
            }


            return results;
        }

        // GET: api/Hotels/5/Rooms
        [ResponseType(typeof(Picture))]
        //[Route("api/pictures/GetRoomsHotel/")]
        public async Task<IHttpActionResult> GetPicturesRoom(int id)
        {
            Room r = await db.Rooms.FindAsync(id);
            if (r == null)
            {
                return NotFound();
            }
            
            List<Picture> pictures = new List<Picture>();
            
            foreach (Picture p in db.Pictures)
            {
                if (p.Room == r)
                    pictures.Add(p);
            }               

            return Ok(pictures);
        }

        // PUT: api/Pictures/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutPicture(int id, Picture picture)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != picture.IdPicture)
            {
                return BadRequest();
            }

            db.Entry(picture).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PictureExists(id))
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

        // POST: api/Pictures
        [ResponseType(typeof(Picture))]
        public async Task<IHttpActionResult> PostPicture(Picture picture)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Pictures.Add(picture);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = picture.IdPicture }, picture);
        }

        // DELETE: api/Pictures/5
        [ResponseType(typeof(Picture))]
        public async Task<IHttpActionResult> DeletePicture(int id)
        {
            Picture picture = await db.Pictures.FindAsync(id);
            if (picture == null)
            {
                return NotFound();
            }

            db.Pictures.Remove(picture);
            await db.SaveChangesAsync();

            return Ok(picture);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PictureExists(int id)
        {
            return db.Pictures.Count(e => e.IdPicture == id) > 0;
        }
    }
}