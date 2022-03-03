using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;

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
    }
}
