namespace Hackathon_BAC.Model
{
    public enum LockStatus
    {
        Locked,
        UnLocked
    }
    public class Door
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public LockStatus LockStatus { get; set; }
    }
}
