using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            celestialObject.Satellites = (from celestials in _context.CelestialObjects
                                          where celestials.Id == id
                                          select celestials).ToList();
            if (celestialObject == null)
            {
                return NotFound();
            }
            return Ok(celestialObject);
        }

        [HttpGet("{name}", Name = "GetByName")]
        public IActionResult GetByName(string name)
        {
            var celestialObject = _context.CelestialObjects.Where(x => x.Name == name).FirstOrDefault();
            var celestialObjectId = celestialObject.Id;
            celestialObject.Satellites = (from celestials in _context.CelestialObjects
                                          where celestials.Id == celestialObjectId
                                          select celestials).ToList();
            if (celestialObject == null)
            {
                return NotFound();
            }
            return Ok(celestialObject);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            // account for streaming and non streaming deferred execution
            var celestialObjectGroups = (from celestialObj in _context.CelestialObjects
                                         orderby celestialObj.Id
                                         group celestialObj by celestialObj.Id
                                         into idGroups
                                         select idGroups);

            var celestialObjects = _context.CelestialObjects.ToList();

            // Linq code for doing this?
            foreach (var grp in celestialObjectGroups)
            {
                foreach (var obj in celestialObjects)
                {
                    if (obj.Id == grp.Key)
                    {
                        var groupObjectName = _context.CelestialObjects.Find(obj.Id).Name;
                        if (obj.Name != groupObjectName)
                        {
                            obj.Satellites.Add((CelestialObject)celestialObjects.Where(x => x.Id == grp.Key));
                        }
                    }
                }
            }

            if (celestialObjects == null)
            {
                return NotFound();
            }
            return Ok(celestialObjects);
        }
    }
}
