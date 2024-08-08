using StoreSp.Dtos.request;

namespace StoreSp.Services;

public interface IProductService
{
    public IResult AddProduct(CreateProductDto createProductDto);
    public IResult GetProductsByCategory(string code);
    public IResult GetProductByCode(string code);
    public IResult GetProductsBySearch(string name);
    public IResult GetProductsNew(GetNewProductDto getNewProductDto);
    public IResult GetProductsHot(GetProductHot getProductHot);
    public IResult LikeProduct(LikeProductDto likeProductDto);
}
