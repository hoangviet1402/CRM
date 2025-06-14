namespace Menu.DTOs;

public class MenuDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int Order { get; set; }
    public string Icon { get; set; } = string.Empty;
}
