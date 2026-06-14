namespace LLMFlexWin.Models;

public sealed class Profile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public string Provider { get; set; } = "";
    public string BaseUrl { get; set; } = "";
    public string ModelName { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
