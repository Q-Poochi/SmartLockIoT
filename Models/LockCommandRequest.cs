namespace SmartLockSystem.Models
{
    public class LockCommandRequest
    {
        public string DeviceId { get; set; }= string.Empty;
        public bool Unlock { get; set; }
    }
}
