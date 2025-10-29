using Hackathon_BAC.Hub;
using Hackathon_BAC.Service.IServiceRepo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Hackathon_BAC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoorsController : ControllerBase
    {
        private readonly IDoorService _doorService;

        private readonly IHubContext<DoorHub> _Hub;

        public DoorsController(IDoorService doorService, IHubContext<DoorHub> hubContext)
        {
            _doorService = doorService;
            _Hub = hubContext;
        }
        [HttpPost("{id}/toggle")]
        public async Task<IActionResult> Toggle(string id)
        {
            try
            {
                var toggleEvent = _doorService.Toggle(id);
                var updatedDoor = _doorService.Get(id);

                if (updatedDoor == null)
                    return NotFound();

                // Shape door DTO where lockStatus is string ("Locked" or "UnLocked")
                var doorDto = new
                {
                    id = updatedDoor.Id,
                    name = updatedDoor.Name,
                    location = updatedDoor.Location,
                    lockStatus = updatedDoor.LockStatus.ToString() // <-- string
                };

                // Shape event DTO with a human readable message
                var eventDto = new
                {
                    message = toggleEvent.Message,
                    timeStamp = toggleEvent.TimeStamp,
                    doorId = toggleEvent.DoorId,
                    doorName = toggleEvent.DoorName,
                    newStatus = toggleEvent.NewStatus.ToString()
                };

                // Notify clients
                await _Hub.Clients.All.SendAsync("DoorUpdated", doorDto);
                await _Hub.Clients.All.SendAsync("HistoryAdded", eventDto);

                return Ok(new { door = doorDto, toggleEvent = eventDto });
            }
            catch (Exception ex)
            {
                // Return meaningful error to caller
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet]
        public IActionResult Get()
        {
            var doors = _doorService.GetAll().Select(d => new
            {
                id = d.Id,
                name = d.Name,
                location = d.Location,
                lockStatus = d.LockStatus.ToString()
            });

            var history = _doorService.GetHistory().Select(ev => new
            {
                message = $"{ev.DoorName} is now {(ev.NewStatus == Hackathon_BAC.Model.LockStatus.Locked ? "Locked" : "Unlocked")}",
                timeStamp = ev.TimeStamp,
                doorId = ev.DoorId,
                doorName = ev.DoorName,
                newStatus = ev.NewStatus.ToString()
            });

            return Ok(new { doors, history });
        }
    }
}
