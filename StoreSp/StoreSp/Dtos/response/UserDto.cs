﻿namespace StoreSp.Dtos.response;

public class UserDto
{
    public required string CreatedAt { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set;}
    public required string Phone { get; set;}
    public required string Avatar { get; set;}
    public int Status { get; set; }
    public required string VerifiedAt { get; set; }
    public required string Address { get; set; }
}
