using Hackathon_BAC.Model;

namespace Hackathon_BAC.Service.IServiceRepo
{
    public interface IDoorService
    {
        IEnumerable<Door> GetAll();
        Door? Get(string id);
        DoorEvent Toggle(string id);
        IEnumerable<DoorEvent> GetHistory();
    }
}
