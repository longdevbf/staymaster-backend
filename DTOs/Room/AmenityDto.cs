namespace HotelManagement.API.DTOs.Room;

public class AmenityDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Group { get; set; }
    public string? Icon { get; set; }
}
