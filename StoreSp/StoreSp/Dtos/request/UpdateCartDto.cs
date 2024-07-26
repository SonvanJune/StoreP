namespace StoreSp.Dtos.request;

public record UpdateCartDto
(    
    string Username,
    List<UpdateCartItemDto>? UpdateCartItems
);
