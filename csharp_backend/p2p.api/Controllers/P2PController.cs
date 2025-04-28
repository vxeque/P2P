using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using p2p.models;
using p2p.services;

namespace p2p.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class P2PController : ControllerBase
    {
        private readonly P2PContext _context;


        public P2PController(P2PContext context)
        {
            _context = context;
        }

        // GET: api/P2P
        [HttpGet]
        public async Task<ActionResult<IEnumerable<P2PItems>>> GetP2PItems()
        {
            return await _context.P2PItems.ToListAsync();
        }

        
        [HttpGet("devices")]
        public async Task<IActionResult> GetDevices()
        {
            // var lanIpScanner = new LanIpScanner();
            // var localIp = lanIpScanner.GetIpLocal();

            // var devices = lanIpScanner.GetType(); 
            // return devices.ToString();

            var descriptionDevices = new services.UniversalDeviceScanner();
            var devices = await descriptionDevices.DescriptionDevices(); 
            Console.WriteLine($"Devices: {devices}");
            
            var devicesObject = devices.Select(d => new { d.ip, d.name, d.osType });
            Console.WriteLine($"DevicesObject: {devicesObject}");

            return Ok(devicesObject);
        }
        // public async Task<ActionResult<IEnumerable<(string ip, string name, string osType)>>> GetP2PDtoDevices()
        // {

        //    var devices = _p2PService.DescriptionDevices();
        //    return Ok(devices);
        //    //return await _context.P2PItems.ToListAsync();
        // }

        // GET: api/P2P/5
        [HttpGet("{id}")]
        public async Task<ActionResult<P2PItems>> GetP2PItems(string id)
        {
            var p2PItems = await _context.P2PItems.FindAsync(id);

            if (p2PItems == null)
            {
                return NotFound();
            }

            return p2PItems;
        }

        // PUT: api/P2P/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutP2PItems(string id, P2PItems p2PItems)
        {
            if (id != p2PItems.Id)
            {
                return BadRequest();
            }

            _context.Entry(p2PItems).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!P2PItemsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/P2P
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        //public async Task<ActionResult<P2PItems>> PostP2PItems(P2PItems p2PItems)
        public async Task<ActionResult<P2PItemsDto>> PostP2PItems(P2PItems p2PItems)
        {
            // en caso de que no tenga un id, lo generamos 
            if (string.IsNullOrWhiteSpace(p2PItems.Id))
            {
                p2PItems.Id = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid()}";
            }

            _context.P2PItems.Add(p2PItems);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (P2PItemsExists(p2PItems.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            // mapear un DTO
            var dto = new P2PItemsDto()
            {
                
                DeviceName = p2PItems.DeviceName,
                DeviceType = p2PItems.DeviceType,
                DeviceIp = p2PItems.DeviceIp
            };

            return CreatedAtAction("GetP2PItems", new { id = p2PItems.Id }, dto);
        }

        // DELETE: api/P2P/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteP2PItems(string id)
        {
            var p2PItems = await _context.P2PItems.FindAsync(id);
            if (p2PItems == null)
            {
                return NotFound();
            }

            _context.P2PItems.Remove(p2PItems);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool P2PItemsExists(string id)
        {
            return _context.P2PItems.Any(e => e.Id == id);
        }
    }
}
