using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.Where(x => x.Id == id).FirstOrDefault();

            if (celestialObject == null)
                return NotFound();

            celestialObject.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == id).ToList();
            return Ok(celestialObject);
        }

        [HttpGet("{name}", Name = "GetByName")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(x => x.Name == name).ToList();

            if (celestialObjects.Count == 0)
            {
                return NotFound();
            }

            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = celestialObjects.Where(x => x.Id == celestialObject.Id).ToList();
            }
            _context.SaveChanges();
            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();

            foreach (var celestialObject in celestialObjects)
            {
                var celestialObjectId = celestialObject.Id;
                celestialObject.Satellites = celestialObjects.Where(x => x.OrbitedObjectId == celestialObject.Id).ToList();
            };

            //// account for streaming and non streaming deferred execution
            //var celestialObjectGroups = (from celestialObj in _context.CelestialObjects
            //                             orderby celestialObj.Id
            //                             group celestialObj by celestialObj.Id
            //                             into idGroups
            //                             select idGroups);

            //var celestialObjects = _context.CelestialObjects.ToList();

            //// Linq code for doing this?
            //foreach (var grp in celestialObjectGroups)
            //{
            //    foreach (var obj in celestialObjects)
            //    {
            //        if (obj.Id == grp.Key)
            //        {
            //            var groupObjectName = _context.CelestialObjects.Find(obj.Id).Name;
            //            if (obj.Name != groupObjectName)
            //            {
            //                obj.Satellites.Add((CelestialObject)celestialObjects.Where(x => x.Id == grp.Key));
            //            }
            //        }
            //    }
            //}

            //if (celestialObjects == null)
            //{
            //    return NotFound();
            //}
            //_context.SaveChanges();
            return Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestial)
        {
            _context.CelestialObjects.Add(celestial);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { id = celestial.Id }, celestial);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObjectFromBody)
        {
            var celestialObject = _context.CelestialObjects.Find(id);
            if (celestialObject == null)
            {
                return NotFound();
            }

            celestialObject.Name = celestialObjectFromBody.Name;
            celestialObject.OrbitalPeriod = celestialObjectFromBody.OrbitalPeriod;
            celestialObject.OrbitedObjectId = celestialObjectFromBody.OrbitedObjectId;

            _context.CelestialObjects.Update(celestialObject);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestialObject = _context.CelestialObjects.Find(id);
            if (celestialObject == null)
            {
                return NotFound();
            }

            celestialObject.Name = name;
            _context.CelestialObjects.Update(celestialObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            List<CelestialObject> celestialObjects = _context.CelestialObjects.Where(x => x.Id == id).ToList();
            if (celestialObjects.Count == 0)
            {
                return NotFound();
            }
            _context.CelestialObjects.RemoveRange(celestialObjects);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
