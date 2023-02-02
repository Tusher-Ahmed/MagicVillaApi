using MagicVilla_VillaApi.Data;
using MagicVilla_VillaApi.Logging;
using MagicVilla_VillaApi.Models;
using MagicVilla_VillaApi.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaApiController : ControllerBase
    {
        //private readonly ILogging _logger;

        //public VillaApiController(ILogging logger)
        //{
        //    _logger = logger;
        //}
        private readonly ApplicationDbContext _db;
        public VillaApiController(ApplicationDbContext db)
        {
            _db= db;
        }
        [HttpGet]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            //_logger.Log("Get All Villas","");
            return Ok( _db.Villas.ToList());
        }
        [HttpGet("{id:int}",Name ="GetVilla")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<VillaDTO> GetVilla(int id)
        {
            if(id == 0)
            {
                //_logger.Log("Get Villa Error With id :" + id,"error");
                return BadRequest();
            }
            var villa = _db.Villas.FirstOrDefault(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            return Ok(villa);
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDTO> CreateVilla([FromBody]VillaDTO villa) {
            //if(!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
           if(_db.Villas.FirstOrDefault(u=>u.Name.ToLower()== villa.Name.ToLower()) != null)
            {
                ModelState.AddModelError("", "Villa already exists");
                return BadRequest(ModelState);
            }
           if(villa==null)
            {
                return BadRequest();
            }
           if(villa.Id > 0) { 
             return StatusCode(StatusCodes.Status500InternalServerError);
            }
            Villa model = new Villa()
            {
                Amenity = villa.Amenity,
                Details = villa.Details,
                Id = villa.Id,
                ImageUrl = villa.ImageUrl,
                Name = villa.Name,
                Occupancy = villa.Occupancy,
                Rate = villa.Rate,
                Sqft = villa.Sqft,
            };
            _db.Villas.Add(model);
            _db.SaveChanges();
            return CreatedAtRoute("GetVilla", new {id=villa.Id},villa);
            
        }

        [HttpDelete("{id:int}",Name ="DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult DeleteVilla(int id)
        {
            if(id==0)
            {
                return BadRequest();
            }
            var villa= _db.Villas.FirstOrDefault(v => v.Id==id);
            if(villa==null) { 
                return NotFound();
            }
            _db.Villas.Remove(villa);
            _db.SaveChanges();
            return NoContent();
        }
        [HttpPut("{id:int}",Name ="UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult UpdateVilla(int id, [FromBody]VillaDTO villa) {
          if(id!=villa.Id || villa == null)
            {
                return BadRequest();
            }
            //var villa= _db.Villas.FirstOrDefault(u=>u.Id==id);
            //  villa.Name=villaDTO.Name;
            //  villa.Occupancy=villaDTO.Occupancy;
            //  villa.Sqft=villaDTO.Sqft;
            Villa model = new Villa()
            {
                Amenity = villa.Amenity,
                Details = villa.Details,
                Id = villa.Id,
                ImageUrl = villa.ImageUrl,
                Name = villa.Name,
                Occupancy = villa.Occupancy,
                Rate = villa.Rate,
                Sqft = villa.Sqft,
            };
            _db.Villas.Update(model);
            _db.SaveChanges();
            return NoContent();
        }
        [HttpPatch("{id:int}",Name ="UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        //visit: jsonpatch.com
        public IActionResult UpdatePartialVilla(int id,JsonPatchDocument<VillaDTO> patchDTO)
        {
            if(patchDTO==null || id==0)
            {
                return BadRequest();
            }
            var villa= _db.Villas.AsNoTracking().FirstOrDefault(u=>u.Id==id);
            VillaDTO villaDTO = new()
            {
                Amenity = villa.Amenity,
                Details = villa.Details,
                Id = villa.Id,
                ImageUrl = villa.ImageUrl,
                Name = villa.Name,
                Occupancy = villa.Occupancy,
                Rate = villa.Rate,
                Sqft = villa.Sqft,
            };
            if(villa==null)
            {
                return BadRequest();
            }

            patchDTO.ApplyTo(villaDTO, ModelState);
            Villa model = new Villa()
            {
                Amenity = villaDTO.Amenity,
                Details = villaDTO.Details,
                Id = villaDTO.Id,
                ImageUrl = villaDTO.ImageUrl,
                Name = villaDTO.Name,
                Occupancy = villaDTO.Occupancy,
                Rate = villaDTO.Rate,
                Sqft = villaDTO.Sqft,
            };
            _db.Villas.Update(model);
            _db.SaveChanges();
            return NoContent();
        }

    }
}
