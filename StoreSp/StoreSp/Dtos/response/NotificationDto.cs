namespace StoreSp.Dtos.response;

public class NotificationDto
{
    public required int Id { get; set; }
    public required int Type { get; set; }
    public required string CreatedAt { get; set; }
    public required string Message { get; set; }
    public required int Status { get; set; }
}
