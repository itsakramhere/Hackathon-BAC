using System.Threading.Tasks;
using Hackathon_BAC.Controllers;
using Hackathon_BAC.Service.IServiceRepo;
using Hackathon_BAC.Hub;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DoorControllerTest
{
    public class DoorsControllerTests
    {
        private readonly Mock<IDoorService> _doorServiceMock;
        private readonly Mock<IHubContext<DoorHub>> _hubContextMock;
        private readonly Mock<IHubClients> _hubClientsMock;
        private readonly Mock<IClientProxy> _clientProxyMock;
        private readonly DoorsController _controller;

        public DoorsControllerTests()
        {
            _doorServiceMock = new Mock<IDoorService>();
            _hubContextMock = new Mock<IHubContext<DoorHub>>();
            _hubClientsMock = new Mock<IHubClients>();
            _clientProxyMock = new Mock<IClientProxy>();

            // Setup hub context to return mocked clients
            _hubContextMock.Setup(h => h.Clients).Returns(_hubClientsMock.Object);
            _hubClientsMock.Setup(c => c.All).Returns(_clientProxyMock.Object);

            _controller = new DoorsController(_doorServiceMock.Object, _hubContextMock.Object);
        }

        [Fact]
        public async Task Toggle_ReturnsOkResult_WhenDoorExists()
        {
            // Arrange
            var doorId = "door1";
            var toggleResult = new { Status = "Toggled" };
            var doorResult = new { Id = doorId, IsOpen = true };

            _doorServiceMock.Setup(s => s.Toggle(doorId)).Returns(toggleResult);
            _doorServiceMock.Setup(s => s.Get(doorId)).Returns(doorResult);
            _clientProxyMock.Setup(c => c.SendAsync("DoorUpdated", doorResult, default)).Returns(Task.CompletedTask);
            _clientProxyMock.Setup(c => c.SendAsync("HistoryAdded", toggleResult, default)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Toggle(doorId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value;
            Assert.Equal(doorResult, value.door);
            Assert.Equal(toggleResult, value.toggleService);
        }

        [Fact]
        public void Get_ReturnsOkResult_WithDoorsAndHistory()
        {
            // Arrange
            var doors = new[] { new { Id = "door1", IsOpen = true } };
            var history = new[] { new { DoorId = "door1", Action = "Opened" } };

            _doorServiceMock.Setup(s => s.GetAll()).Returns(doors);
            _doorServiceMock.Setup(s => s.GetHistory()).Returns(history);

            // Act
            var result = _controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value;
            Assert.Equal(doors, value.doors);
            Assert.Equal(history, value.history);
        }
    }
}