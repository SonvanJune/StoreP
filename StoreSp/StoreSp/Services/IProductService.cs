using StoreSp.Dtos.request;

namespace StoreSp.Services;

public interface IProductService
{
    public IResult AddProduct(CreateProductDto createProductDto);
    public IResult GetProductsByCategory(string code);
    public IResult GetProductByCode(string code);
    public IResult LikeProduct(LikeProductDto likeProductDto);
}
