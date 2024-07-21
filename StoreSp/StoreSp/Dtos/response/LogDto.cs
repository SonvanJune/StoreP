namespace StoreSp.Dtos.response;

public class LogDto
{
    public required string Code { get; set; }
    public required string Message { get; set; }
    public required int Status { get; set; }
    public UserDto? User { get; set; }
}
