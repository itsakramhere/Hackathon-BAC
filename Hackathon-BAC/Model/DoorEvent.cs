using Hackathon_BAC.Model;

namespace Hackathon_BAC.Model
{
    public class DoorEvent
    {
        public DateTime TimeStamp { get; set; }
        public string DoorId { get; set; } = string.Empty;
        public string DoorName { get; set;} = string.Empty;
        public LockStatus NewStatus { get; set; }


        public string Message => $"{TimeStamp:yyyy-MM-dd HH:mm:ss} - {DoorName} was set to {NewStatus}";

    }
}

