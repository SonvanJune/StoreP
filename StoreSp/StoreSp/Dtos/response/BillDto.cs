﻿namespace StoreSp.Dtos.response;

public class BillDto{
    public UserDto? User{get; set;}
    public required string CreatedAt { get; set; }
    public required string Code { get; set; }
    public required string PaymentMethod { get; set; }
    public ShippingMethodDto? ShippingMethod { get; set; }
    public AddressDto? Address { get; set; }
    public required int Quantity { get; set; }
    public required int Status { get; set; }
    public required int TotalPrice { get; set; }
    public required int TotalProductPrice { get; set;}
    public List<BillItemDto>? BillItems { get; set; }
}
