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
    /// <summary>
    /// Controlador API para gestionar elementos y dispositivos P2P.
    /// Proporciona endpoints REST para operaciones CRUD sobre dispositivos P2P.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class P2PController : ControllerBase
    {
        private readonly P2PContext _context;

        /// <summary>
        /// Inicializa una nueva instancia del controlador P2PController.
        /// </summary>
        /// <param name="context">El contexto de base de datos P2P inyectado por dependencia.</param>
        public P2PController(P2PContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene la lista completa de todos los elementos P2P registrados.
        /// </summary>
        /// <returns>Una lista de objetos <see cref="P2PItems"/> de la base de datos.</returns>
        /// <response code="200">Retorna la lista de elementos P2P.</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<P2PItems>>> GetP2PItems()
        {
            return await _context.P2PItems.ToListAsync();
        }

        
        /// <summary>
        /// Escanea y obtiene la lista de dispositivos disponibles en la red local.
        /// </summary>
        /// <returns>Una colección anónima de dispositivos con propiedades ip, name y osType.</returns>
        /// <response code="200">Retorna la lista de dispositivos encontrados en la red.</response>
        /// <remarks>
        /// Este endpoint utiliza el escáner universal de dispositivos para descubrir
        /// todos los hosts disponibles en la red local.
        /// </remarks>
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

        /// <summary>
        /// Obtiene un elemento P2P específico por su identificador.
        /// </summary>
        /// <param name="id">El identificador único del elemento P2P a recuperar.</param>
        /// <returns>El objeto <see cref="P2PItems"/> si existe; de lo contrario, NotFound.</returns>
        /// <response code="200">Retorna el elemento P2P encontrado.</response>
        /// <response code="404">Si el elemento P2P con el id especificado no existe.</response>
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

        /// <summary>
        /// Actualiza un elemento P2P existente.
        /// </summary>
        /// <param name="id">El identificador único del elemento P2P a actualizar.</param>
        /// <param name="p2PItems">El objeto <see cref="P2PItems"/> con los datos actualizados.</param>
        /// <returns>NoContent si la actualización es exitosa; BadRequest si los ids no coinciden; NotFound si el elemento no existe.</returns>
        /// <response code="204">Si la actualización fue exitosa.</response>
        /// <response code="400">Si el id de la URL no coincide con el id del objeto.</response>
        /// <response code="404">Si el elemento P2P con el id especificado no existe.</response>
        /// <remarks>
        /// Para proteger contra ataques de overposting, vea https://go.microsoft.com/fwlink/?linkid=2123754
        /// </remarks>
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

        /// <summary>
        /// Crea un nuevo elemento P2P en la base de datos.
        /// </summary>
        /// <param name="p2PItems">El objeto <see cref="P2PItems"/> a crear. Si no tiene Id, se genera automáticamente.</param>
        /// <returns>El objeto <see cref="P2PItemsDto"/> creado con código 201 (Created).</returns>
        /// <response code="201">Si el elemento P2P fue creado exitosamente.</response>
        /// <response code="409">Si ya existe un elemento con el mismo Id (Conflict).</response>
        /// <remarks>
        /// Si el elemento P2P no tiene un Id, se genera automáticamente con formato timestamp-GUID.
        /// Para proteger contra ataques de overposting, vea https://go.microsoft.com/fwlink/?linkid=2123754
        /// </remarks>
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

        /// <summary>
        /// Elimina un elemento P2P específico de la base de datos.
        /// </summary>
        /// <param name="id">El identificador único del elemento P2P a eliminar.</param>
        /// <returns>NoContent si la eliminación es exitosa; NotFound si el elemento no existe.</returns>
        /// <response code="204">Si la eliminación fue exitosa.</response>
        /// <response code="404">Si el elemento P2P con el id especificado no existe.</response>
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

        /// <summary>
        /// Verifica si existe un elemento P2P con el identificador especificado.
        /// </summary>
        /// <param name="id">El identificador único a verificar.</param>
        /// <returns>Verdadero si el elemento existe; falso en caso contrario.</returns>
        private bool P2PItemsExists(string id)
        {
            return _context.P2PItems.Any(e => e.Id == id);
        }
    }
}
