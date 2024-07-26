namespace StoreSp.Dtos.response;

public class UserTokenDto
{
    public required string Token { get; set; }
    public required UserDto User { get; set; }
    public required string RoleCode { get; set; }

}
