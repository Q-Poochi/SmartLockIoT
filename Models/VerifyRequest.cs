namespace SmartLockSystem.Models;

public class VerifyRequest
{
    public string AccessCode { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}
