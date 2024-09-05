using Microsoft.EntityFrameworkCore;
using StoreSp.Context;
using StoreSp.Converters;
using StoreSp.Converters.request;
using StoreSp.Converters.response;
using StoreSp.Dtos.request;
using StoreSp.Dtos.response;
using StoreSp.Models;

namespace StoreSp.Stores;

public class ProductFireStore
{
    //properties
    private readonly AppDbContext? _appDbContext = null;

    public ProductFireStore()
    {
        _appDbContext = AppDbContext.GetInstance();
    }
    
    private readonly IBaseConverter<User, UserDto> userConverter = new UserConverter();
    private readonly IBaseConverter<Product, CreateProductDto> createProductConverter = new CreateProductConverter();
    private readonly IBaseConverter<ProductClassify, CreateProductClassifyDto> createProductClassifyConverter = new CreateProductClassifyConverter();
    private readonly IBaseConverter<Product, ProductDto> productConverter = new ProductConverter();
    private readonly IBaseConverter<ProductClassify, ProductClassifyDto> productClassifyConverter = new ProductClassifyConverter();
    public readonly LogFireStore logFireStore = new LogFireStore();
    public readonly NotificationFireStore notificationFireStore = new NotificationFireStore();

    //method chinh
    public async Task<Product> AddProduct(CreateProductDto createProductDto)
    {
        var product = createProductConverter.ToEntity(createProductDto);
        Random rnd = new Random();
        string randomCode = rnd.Next(1, 100000).ToString();
        while (_appDbContext!.Products.SingleOrDefault(r => r.Code == randomCode) != null)
        {
            randomCode = rnd.Next(1, 100000).ToString();
        }
        product.Code = randomCode;

        User user = null!;
        if (_appDbContext!.Users.SingleOrDefault(r => r.Email == createProductDto.Auth) == null)
        {
            user = _appDbContext!.Users.SingleOrDefault(r => r.Phone == createProductDto.Auth)!;
        }
        else
        {
            user = _appDbContext!.Users.SingleOrDefault(r => r.Email == createProductDto.Auth)!;
        }
        product.Author = user;
        product.AuthorId = user!.Id;

        product.Categories = FindCategoryProduct(createProductDto.CategoryCode);
        product.ProductClassifies = FindProductClassify(createProductDto.ClassiFies!, product);
        product.ProductImages = FindProductImage(createProductDto.Images!, product);

        await _appDbContext!.Products.AddAsync(product);
        await _appDbContext!.SaveChangesAsync();
        await logFireStore.AddLogForUser(user, "dang-san-pham");
        await notificationFireStore.AddNotificationForUser(user, "Bạn vừa đăng sản phẩm", 0);
        return product;
    }

    public List<ProductDto> GetProductsByCategory(string categoryCode, string username)
    {
        List<ProductDto> productsDto = new List<ProductDto>();

        // tim category bang category code 
        var category = _appDbContext!.Categories.Include(u => u.Products).SingleOrDefault(r => r.Code == categoryCode);
        if (category == null)
        {
            return null!;
        }

        //lay danh sach product id trong bang category_product
        var product_list = category.Products;
        foreach (var cp in product_list!)
        {
            var product = _appDbContext.Products
            .Include(u => u.ProductClassifies)
            .Include(u => u.ProductImages)
            .Include(u => u.Likes)
            .Include(u => u.Author)
            .SingleOrDefault(r => r.Id == cp.Id);
            if (product != null)
            {
                var user = product.Author;
                ProductDto dto = productConverter.ToDto(product!);
                if (user != null)
                {
                    dto.Author = userConverter.ToDto(user!);
                }
                dto.Classifies = GetProductClassifiesByProduct(product.ProductClassifies!);
                dto.Images = GetProductImage(product.ProductImages!);
                dto.Categories = new List<CategoryDto>();
                dto.Likes = GetLikeOfProduct(product.Likes!);
                dto.IsLiked = CheckIsLike(username, cp.Id);
                productsDto.Add(dto);
            }
        }

        return productsDto;
    }

    public List<ProductDto> GetProductsBySearch(string name, string username)
    {
        List<ProductDto> productsDto = new List<ProductDto>();
        string searchName = $"%{name}%";
        var products = _appDbContext!.Products
            .Include(u => u.ProductClassifies)
            .Include(u => u.ProductImages)
            .Include(u => u.Likes)
            .Include(u => u.Author).Where(r => EF.Functions.Like(r.Name, searchName)).ToList();
        foreach (var product in products)
        {
            var user = product.Author;
            ProductDto dto = productConverter.ToDto(product!);
            if (user != null)
            {
                dto.Author = userConverter.ToDto(user!);
            }
            dto.Classifies = GetProductClassifiesByProduct(product.ProductClassifies!);
            dto.Images = GetProductImage(product.ProductImages!);
            dto.Categories = new List<CategoryDto>();
            dto.Likes = GetLikeOfProduct(product.Likes!);
            dto.IsLiked = CheckIsLike(username, product.Id);
            productsDto.Add(dto);
        }
        return productsDto;
    }

    public ProductDto GetProductByProductCode(string productCode, string username)
    {
        var product = _appDbContext!.Products.SingleOrDefault(r => r.Code == productCode);
        if (product != null)
        {
            var user = product.Author;
            ProductDto dto = productConverter.ToDto(product!);
            if (user != null)
            {
                dto.Author = userConverter.ToDto(user!);
            }
            dto.Classifies = GetProductClassifiesByProduct(product.ProductClassifies!);
            dto.Images = GetProductImage(product.ProductImages!);
            dto.Categories = new List<CategoryDto>();
            dto.Likes = GetLikeOfProduct(product.Likes!);
            dto.IsLiked = CheckIsLike(username, product.Id);
            return dto;
        }
        return null!;
    }

    public async Task<string> LikeProduct(LikeProductDto likeProductDto)
    {
        //check user
        User user = null!;
        if (_appDbContext!.Users.SingleOrDefault(r => r.Email == likeProductDto.Username) == null)
        {
            user = _appDbContext!.Users.Include(r => r.Likes).SingleOrDefault(r => r.Phone == likeProductDto.Username)!;
        }
        else
        {
            user = _appDbContext!.Users.Include(r => r.Likes).SingleOrDefault(r => r.Email == likeProductDto.Username)!;
        }

        if (user == null)
        {
            return null!;
        }

        var product = _appDbContext.Products.SingleOrDefault(r => r.Code == likeProductDto.ProductCode);
        //neu like da ton tai
        var list = user.Likes!.ToList();
        if (list.Find(r => r.UserId == user.Id && r.ProductId == product!.Id) != null)
        {
            var like = list.Find(r => r.UserId == user.Id && r.ProductId == product!.Id);
            list.Remove(like!);
            user.Likes = list;
            _appDbContext.Users.Update(user);
            await _appDbContext.SaveChangesAsync();
        }
        else
        {
            Like like = new Like
            {
                UserId = user.Id,
                ProductId = product!.Id,
                CreatedAt = DateTime.Now
            };
            user.Likes!.Add(like);
            _appDbContext.Users.Update(user);
            await _appDbContext.SaveChangesAsync();
            var shop = _appDbContext.Users.SingleOrDefault(r => r.Id == product!.AuthorId)!;
            await notificationFireStore.AddNotificationForUser(shop, likeProductDto.Username + "vừa like sản phẩm của bạn", 0);
        }
        return "";
    }

    public List<ProductDto> GetProductsNew(GetNewProductDto getNewProductDto, string username)
    {
        List<ProductDto> productsDto = new List<ProductDto>();
        DateTime dateLimit = DateTime.Now.AddDays(-getNewProductDto.Day);

        var products = _appDbContext!.Products
                                   .Include(u => u.ProductClassifies)
                                   .Include(u => u.ProductImages)
                                   .Include(u => u.Likes)
                                   .Include(u => u.Author)
                                   .Where(p => p.CreatedAt >= dateLimit)
                                   .OrderBy(p => p.CreatedAt)
                                   .Skip((getNewProductDto.Page - 1) * getNewProductDto.ProductInPage)
                                   .Take(getNewProductDto.ProductInPage)
                                   .ToList();

        foreach (var product in products)
        {
            var user = product.Author;
            ProductDto dto = productConverter.ToDto(product!);
            if (user != null)
            {
                dto.Author = userConverter.ToDto(user!);
            }
            dto.Classifies = GetProductClassifiesByProduct(product.ProductClassifies!);
            dto.Images = GetProductImage(product.ProductImages!);
            dto.Categories = new List<CategoryDto>();
            dto.Likes = GetLikeOfProduct(product.Likes!);
            dto.IsLiked = CheckIsLike(username, product.Id);
            productsDto.Add(dto);
        }
        return productsDto;
    }

    public List<ProductDto> GetProductsLike(GetProductLikeDto getProductLikeDto)
    {
        List<ProductDto> productsDto = new List<ProductDto>();

        //find user
        User user = null!;
        if (_appDbContext!.Users.SingleOrDefault(r => r.Email == getProductLikeDto.Username) == null)
        {
            user = _appDbContext!.Users.Include(r => r.Likes).SingleOrDefault(r => r.Phone == getProductLikeDto.Username)!;
        }
        else
        {
            user = _appDbContext!.Users.Include(r => r.Likes).SingleOrDefault(r => r.Email == getProductLikeDto.Username)!;
        }

        if (user == null)
        {
            return null!;
        }

        //find product like by user
        foreach (var item in user.Likes!)
        {
            var product = _appDbContext!.Products.SingleOrDefault(r => r.Id == item.ProductId);
            if (product != null)
            {
                var shop = product.Author;
                ProductDto dto = productConverter.ToDto(product!);
                if (user != null)
                {
                    dto.Author = userConverter.ToDto(shop!);
                }
                dto.Classifies = GetProductClassifiesByProduct(product.ProductClassifies!);
                dto.Images = GetProductImage(product.ProductImages!);
                dto.Categories = new List<CategoryDto>();
                dto.Likes = GetLikeOfProduct(product.Likes!);
                dto.IsLiked = CheckIsLike(getProductLikeDto.Username, product.Id);
                productsDto.Add(dto);
            }
        }


        // foreach (var item in productResult)
        // {
        //     var shop = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Id == item!.AuthorId);
        //     ProductDto dto = productConverter.ToDto(item!);
        //     if (shop != null)
        //     {
        //         // dto.Author = userConverter.ToDto(shop!);

        //     }
        //     dto.Classifies = GetProductClassifiesByProduct(item.Id!);
        //     dto.Images = GetProductImage(item.Id!);
        //     dto.Likes = GetLikeOfProduct(item.Id!);
        //     dto.IsLiked = CheckIsLike(getProductLikeDto.Username, item.Id!);
        //     productsDto.Add(dto);
        // }
        return productsDto;
    }

    public List<ProductDto> GetProductsHot(GetProductHot getProductHot, string username)
    {
        List<ProductDto> productsDto = new List<ProductDto>();

        var products = _appDbContext!.Products
                                   .Include(u => u.ProductClassifies)
                                   .Include(u => u.ProductImages)
                                   .Include(u => u.Likes)
                                   .Include(u => u.Author)
                                   .OrderByDescending(p => p.QuantitySelled)
                                   .Skip((getProductHot.Page - 1) * getProductHot.ProductInPage)
                                   .Take(getProductHot.ProductInPage)
                                   .ToList();

        foreach (var product in products)
        {
            var user = product.Author;
            ProductDto dto = productConverter.ToDto(product!);
            if (user != null)
            {
                dto.Author = userConverter.ToDto(user!);
            }
            dto.Classifies = GetProductClassifiesByProduct(product.ProductClassifies!);
            dto.Images = GetProductImage(product.ProductImages!);
            dto.Categories = new List<CategoryDto>();
            dto.Likes = GetLikeOfProduct(product.Likes!);
            dto.IsLiked = CheckIsLike(username, product.Id);
            productsDto.Add(dto);
        }
        return productsDto;
    }

    public List<ProductDto> GetProductsByShop(string username)
    {
        List<ProductDto> productsDto = new List<ProductDto>();

        User shop = null!;
        if (_appDbContext!.Users.SingleOrDefault(r => r.Email == username) == null)
        {
            shop = _appDbContext!.Users.Include(r => r.ProductSells).SingleOrDefault(r => r.Phone == username)!;
        }
        else
        {
            shop = _appDbContext!.Users.Include(r => r.ProductSells).SingleOrDefault(r => r.Email == username)!;
        }

        if (shop == null)
        {
            return null!;
        }

        foreach (var item in shop.ProductSells!)
        {
            var product = _appDbContext!.Products
            .Include(r => r.ProductClassifies)
            .Include(r => r.ProductImages)
            .Include(r => r.Likes)
            .SingleOrDefault(r => r.Id == item.Id);
            ProductDto dto = productConverter.ToDto(product!);
            dto.Classifies = GetProductClassifiesByProduct(product!.ProductClassifies!);
            dto.Images = GetProductImage(product.ProductImages!);
            dto.Categories = new List<CategoryDto>();
            dto.Likes = GetLikeOfProduct(product.Likes!);
            dto.IsLiked = CheckIsLike(username, product.Id);
            productsDto.Add(dto);
        }
        return productsDto;
    }
    
    public List<ProductDto> GetProductsByShopName(string username)
    {
        List<ProductDto> productsDto = new List<ProductDto>();

        User shop = null!;
        if (_appDbContext!.Users.SingleOrDefault(r => r.Email == username) == null)
        {
            shop = _appDbContext!.Users.Include(r => r.ProductSells).SingleOrDefault(r => r.Phone == username)!;
        }
        else
        {
            shop = _appDbContext!.Users.Include(r => r.ProductSells).SingleOrDefault(r => r.Email == username)!;
        }

        if (shop == null)
        {
            return null!;
        }

        foreach (var item in shop.ProductSells!)
        {
            var product = _appDbContext!.Products
            .Include(r => r.ProductClassifies)
            .Include(r => r.ProductImages)
            .Include(r => r.Likes)
            .SingleOrDefault(r => r.Id == item.Id);
            ProductDto dto = productConverter.ToDto(product!);
            dto.Author = userConverter.ToDto(shop!);
            dto.Classifies = GetProductClassifiesByProduct(product!.ProductClassifies!);
            dto.Images = GetProductImage(product.ProductImages!);
            dto.Categories = new List<CategoryDto>();
            dto.Likes = GetLikeOfProduct(product.Likes!);
            dto.IsLiked = CheckIsLike(username, product.Id);
            productsDto.Add(dto);
        }
        return productsDto;
    }

    //method ho tro
    private List<ProductClassify> FindProductClassify(CreateProductClassifyDto[] productClassifies, Product product)
    {
        List<ProductClassify> result = new List<ProductClassify>();
        foreach (var pClassify in productClassifies)
        {
            var productClassify = createProductClassifyConverter.ToEntity(pClassify);
            productClassify.Product = product;
            productClassify.ProductId = product!.Id;
            Random rnd = new Random();
            string randomCode = rnd.Next(1, 100000).ToString();
            while (_appDbContext!.ProductClassifies.SingleOrDefault(r => r.Code == randomCode) != null)
            {
                randomCode = rnd.Next(1, 100000).ToString();
            }
            productClassify.Code = randomCode;
            result.Add(productClassify);
        }

        return result;
    }

    private List<ProductImage> FindProductImage(List<string> productImages, Product product)
    {
        List<ProductImage> result = new List<ProductImage>();
        foreach (var pImage in productImages)
        {
            var productImage = new ProductImage
            {
                Image = pImage,
                Product = product,
                ProductId = product!.Id
            };
            result.Add(productImage);
        }
        return result;
    }

    private List<Category> FindCategoryProduct(string categoryCode)
    {
        var categories = _appDbContext!.Categories.ToList();
        List<int> categoryIds = new List<int>();
        List<Category> result = new List<Category>();

        var category = _appDbContext.Categories.SingleOrDefault(r => r.Code == categoryCode);
        categoryIds.Add(category!.Id!);
        int temp = category.ParentCategoryId;

        for (int i = category!.Level - 1; i >= 0; i--)
        {
            for (int j = 0; j < categories.Count; j++)
            {
                if (temp == categories[j].Id && categories[j].Level == i && categories[j].ParentCategoryId == -1)
                {
                    categoryIds.Add(categories[j].Id!);
                    temp = categories[j].ParentCategoryId!;
                }
                if (temp == categories[j].Id && categories[j].Level == i)
                {
                    categoryIds.Add(categories[j].Id!);
                    temp = categories[j].ParentCategoryId!;
                }
            }
        }

        foreach (var categoryId in categoryIds)
        {
            var cate = _appDbContext.Categories.SingleOrDefault(c => c.Id == categoryId);
            result.Add(cate!);
        }
        return result;
    }

    private List<ProductClassifyDto> GetProductClassifiesByProduct(ICollection<ProductClassify> productClassifies)
    {
        var list = productClassifies.ToList();
        var product = _appDbContext!.Products.SingleOrDefault(p => p.Id == list[0].ProductId);
        List<ProductClassifyDto> productsDto = new List<ProductClassifyDto>();
        foreach (var pc in productClassifies)
        {
            ProductClassifyDto dto = productClassifyConverter.ToDto(pc);
            dto.PriceAfterIncreasePercent = pc.IncreasePercent == 0 ? product!.Price : product!.Price + (product!.Price * pc.IncreasePercent / 100);
            productsDto.Add(dto);
        }
        return productsDto;
    }

    public List<string> GetProductImage(ICollection<ProductImage> productImages)
    {
        List<string> images = new List<string>();
        foreach (var pc in productImages)
        {
            images.Add(pc.Image);
        }
        return images;
    }

    private List<CategoryDto> GetCategoriesByProduct(string productId)
    {
        // var productDb = base.GetSnapshots(_collectionProducts);
        // var categoryDb = base.GetSnapshots(CategoryFireStore._collectionCategory);
        // var categoryProductDb = base.GetSnapshots(Category_ProductFireStore._collectionCategoryProduct);
        // var categoryDtos = new List<CategoryDto>();
        // var result = new List<CategoryDto>();

        // var product = productDb.Documents.Select(r => r.ConvertTo<Product>()).ToList().Find(r => r.Id == productId);
        // List<Category_Product> category_product_list = categoryProductDb.Documents.Select(r => r.ConvertTo<Category_Product>()).ToList().FindAll(r => r.ProductId == productId);

        // var categories = new List<Category>();
        // foreach (var pc in category_product_list)
        // {
        //     var category = categoryDb.Documents.Select(r => r.ConvertTo<Category>()).ToList().Find(r => r.Id == pc.CategoryId);
        //     categories.Add(category!);
        // }

        // int high = 0;

        // foreach (var category in categories)
        // {
        //     if (high <= category.Level)
        //     {
        //         high = category.Level;
        //     }
        //     categoryDtos.Add(CategoryFireStore.categoryConverter.ToDto(category));
        // }

        // for (int i = 0; i < high; i++)
        // {
        //     for (int j = 0; j < categoryDtos.Count; j++)
        //     {
        //         if (categoryDtos[j].ParentCategoryId == null && categoryDtos[j].Level == i)
        //         {
        //             result.Add(categoryDtos[j]);
        //             List<CategoryDto> arr = categoryDtos.FindAll(c => c.ParentCategoryId == categoryDtos[j].Id);
        //             categoryDtos[j].Children = arr;
        //             break;
        //         }

        //         if (categoryDtos[j].ParentCategoryId != null && categoryDtos[j].Level == i)
        //         {
        //             List<CategoryDto> arr = categoryDtos.FindAll(c => c.ParentCategoryId == categoryDtos[j].Id);
        //             categoryDtos[j].Children = arr;
        //         }
        //     }
        // }

        // return result;
        return null!;
    }

    private int GetLikeOfProduct(ICollection<Like> productLikes)
    {
        return productLikes.Count;
    }

    private bool CheckIsLike(string username, int productId)
    {
        User user = null!;
        if (_appDbContext!.Users.SingleOrDefault(r => r.Email == username) == null)
        {
            user = _appDbContext!.Users.SingleOrDefault(r => r.Phone == username)!;
        }
        else
        {
            user = _appDbContext!.Users.SingleOrDefault(r => r.Email == username)!;
        }

        if (user != null)
        {
            return _appDbContext.Likes.SingleOrDefault(r => r.UserId == user.Id && r.ProductId == productId) != null;
        }
        else
        {
            return false;
        }
    }
}
