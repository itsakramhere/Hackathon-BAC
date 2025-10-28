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
                var toggleService = _doorService.Toggle(id);
                var updateval = _doorService.Get(id);

                await _Hub.Clients.All.SendAsync("DoorUpdated", updateval);
                await _Hub.Clients.All.SendAsync("HistoryAdded", toggleService);

                return Ok(new { door = updateval, toggleService });

            }
            catch (Exception ex)
            {
                //
            }
            return NotFound();
        }
        [HttpGet]
        public IActionResult Get()
        {
            var doors = _doorService.GetAll();
            var history = _doorService.GetHistory();
            return Ok(new { doors, history });
        }
    }
}
