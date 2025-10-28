using Hackathon_BAC.Model;
using Hackathon_BAC.Service.IServiceRepo;
using System.Collections.Concurrent;

namespace Hackathon_BAC.Service.ServiceRepository
{
    public class DoorServiceRepository : IDoorService
    {
        private readonly ConcurrentDictionary<string, Door> _doors = new();
        private readonly ConcurrentQueue<DoorEvent> _events = new();

        public DoorServiceRepository()
        {
            var DataBase = new[]
              {
                new Door { Id = "door1", Name = "B1F1_FireExitDoor", Location = "/building1/floor1", LockStatus = LockStatus.UnLocked },
                new Door { Id = "door2", Name = "B1F2_FireExitDoor", Location = "/building1/floor2", LockStatus = LockStatus.Locked },
                new Door { Id = "door3", Name = "B1F3_FireExitDoor", Location = "/building1/floor3", LockStatus = LockStatus.UnLocked }
              };

            foreach (var d in DataBase)
            {
                _doors[d.Id] = d;
            }
        }

        public Door? Get(string id)
        {
            return _doors.TryGetValue(id, out var d) ? d : null; 
        }

        public IEnumerable<Door> GetAll()
        {
            return _doors.Values.OrderBy(d => d.Name);
        }

        public IEnumerable<DoorEvent> GetHistory()
        {
            return _events.ToArray().OrderByDescending(e => e.TimeStamp);
        }

        public DoorEvent Toggle(string id)
        {
            if (!_doors.TryGetValue(id, out var door))
                throw new KeyNotFoundException($"Door not found: {id}");

            door.LockStatus = door.LockStatus == LockStatus.Locked ? LockStatus.UnLocked : LockStatus.Locked;

            var ev = new DoorEvent
            {
                TimeStamp = DateTime.UtcNow,
                DoorId = door.Id,
                DoorName = door.Name,
                NewStatus = door.LockStatus
            };

            _events.Enqueue(ev);

            while (_events.Count > 1000) _events.TryDequeue(out _);

            return ev;
        }
    }
}
