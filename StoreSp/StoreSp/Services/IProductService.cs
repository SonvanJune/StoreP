using StoreSp.Dtos.request;

namespace StoreSp.Services;

public interface IProductService
{
    public IResult AddProduct(CreateProductDto createProductDto);
    public IResult GetProductsByCategory(string code , string username);
    public IResult GetProductByCode(string code , string username);
    public IResult GetProductsBySearch(string name , string username);
    public IResult GetProductsNew(GetNewProductDto getNewProductDto , string username);
    public IResult GetProductsHot(GetProductHot getProductHot , string username);
    public IResult GetProductsLike(GetProductLikeDto getProductLikeDto);
    public IResult LikeProduct(LikeProductDto likeProductDto);
}
