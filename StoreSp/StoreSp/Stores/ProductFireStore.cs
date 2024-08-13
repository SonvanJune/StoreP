using Google.Cloud.Firestore;
using StoreSp.Converters;
using StoreSp.Converters.request;
using StoreSp.Converters.response;
using StoreSp.Dtos.request;
using StoreSp.Dtos.response;
using StoreSp.Entities;

namespace StoreSp.Stores;

public class ProductFireStore(FirestoreDb firestoreDb) : FirestoreService(firestoreDb)
{
    //properties
    public static string _collectionProducts = "Products";
    public static string _collectionProduct_Like = "Product_Like";
    public static string _collectionProductClassify = "Product_Classifies";
    public static string _collectionProductImage = "Product_Images";
    private readonly IBaseConverter<User, UserDto> userConverter = new UserConverter();
    private readonly IBaseConverter<Product, CreateProductDto> createProductConverter = new CreateProductConverter();
    private readonly IBaseConverter<ProductClassify, CreateProductClassifyDto> createProductClassifyConverter = new CreateProductClassifyConverter();
    private readonly IBaseConverter<Product, ProductDto> productConverter = new ProductConverter();
    private readonly IBaseConverter<ProductClassify, ProductClassifyDto> productClassifyConverter = new ProductClassifyConverter();
    public readonly LogFireStore logFireStore = new LogFireStore(firestoreDb);
    public readonly NotificationFireStore notificationFireStore = new NotificationFireStore(firestoreDb);

    //method chinh
    public async Task<Product> AddProduct(CreateProductDto createProductDto)
    {
        var db = _firestoreDb.Collection(_collectionProducts);
        var categoryDb = base.GetSnapshots(CategoryFireStore._collectionCategory);
        var productDb = base.GetSnapshots(_collectionProducts);
        var userDb = base.GetSnapshots(UserFireStore._collectionUser);

        var product = createProductConverter.ToEntity(createProductDto);
        Random rnd = new Random();
        string randomCode = rnd.Next(1, 100000).ToString();
        while (productDb.Documents.Select(r => r.ConvertTo<Product>()).ToList().Find(r => r.Code == randomCode) != null)
        {
            randomCode = rnd.Next(1, 100000).ToString();
        }
        product.Code = randomCode;

        User user = null!;
        if (userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == createProductDto.Auth) == null)
        {
            user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Phone == createProductDto.Auth)!;
        }
        else
        {
            user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == createProductDto.Auth)!;
        }
        product.Author = user;
        product.AuthorId = user!.Id;


        await db.AddAsync(product);
        await AddCategoryProduct(createProductDto.CategoryCode, randomCode);
        await AddProductClassify(createProductDto.ClassiFies!, randomCode);
        await AddImage(createProductDto.Images!, randomCode);
        await logFireStore.AddLogForUser(user, "dang-san-pham");
        await notificationFireStore.AddNotificationForUser(user, "Bạn vừa đăng sản phẩm", 0);
        return product;
    }

    public List<ProductDto> GetProductsByCategory(string categoryCode, string username)
    {
        var productDb = base.GetSnapshots(_collectionProducts);
        var categoryProductDb = base.GetSnapshots(Category_ProductFireStore._collectionCategoryProduct);
        var userDb = base.GetSnapshots(UserFireStore._collectionUser);
        var categoryDb = base.GetSnapshots(CategoryFireStore._collectionCategory);
        List<ProductDto> productsDto = new List<ProductDto>();

        //tim category bang category code 
        var category = categoryDb.Documents.Select(r => r.ConvertTo<Category>()).ToList().Find(r => r.Code == categoryCode);
        if (category == null)
        {
            return null!;
        }

        //lay danh sach product id trong bang category_product
        var category_product_list = categoryProductDb.Documents.Select(r => r.ConvertTo<Category_Product>()).ToList().FindAll(r => r.CategoryId == category!.Id);
        foreach (var cp in category_product_list)
        {
            var product = productDb.Documents.Select(r => r.ConvertTo<Product>()).ToList().Find(r => r.Id == cp.ProductId);
            var user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Id == product!.AuthorId);
            ProductDto dto = productConverter.ToDto(product!);
            if (user != null)
            {
                dto.Author = userConverter.ToDto(user!);

            }
            dto.Classifies = GetProductClassifiesByProduct(cp.ProductId);
            dto.Images = GetProductImage(cp.ProductId);
            dto.Categories = GetCategoriesByProduct(cp.ProductId);
            dto.Likes = GetLikeOfProduct(cp.ProductId);
            dto.IsLiked = CheckIsLike(username, cp.ProductId);
            productsDto.Add(dto);
        }

        return productsDto;
    }

    public List<ProductDto> GetProductsBySearch(string name, string username)
    {
        var productDb = base.GetSnapshots(_collectionProducts);
        var userDb = base.GetSnapshots(UserFireStore._collectionUser);
        List<ProductDto> productsDto = new List<ProductDto>();

        var products = productDb.Documents.Select(r => r.ConvertTo<Product>()).ToList().FindAll(r => r.Name.Contains(name));
        foreach (var item in products)
        {
            var user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Id == item!.AuthorId);
            ProductDto dto = productConverter.ToDto(item!);
            if (user != null)
            {
                dto.Author = userConverter.ToDto(user!);

            }
            dto.Classifies = GetProductClassifiesByProduct(item.Id!);
            dto.Likes = GetLikeOfProduct(item.Id!);
            dto.IsLiked = CheckIsLike(username, item.Id!);
            dto.Images = GetProductImage(item.Id!);
            productsDto.Add(dto);
        }
        return productsDto;
    }

    public ProductDto GetProductByProductCode(string productCode, string username)
    {
        var productDb = base.GetSnapshots(_collectionProducts);
        var userDb = base.GetSnapshots(UserFireStore._collectionUser);
        var product = productDb.Documents.Select(r => r.ConvertTo<Product>()).ToList().Find(r => r.Code == productCode);
        if (product != null)
        {
            var user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Id == product!.AuthorId);
            ProductDto dto = productConverter.ToDto(product!);
            dto.Author = userConverter.ToDto(user!);
            dto.Classifies = GetProductClassifiesByProduct(product.Id!);
            dto.Likes = GetLikeOfProduct(product.Id!);
            dto.IsLiked = CheckIsLike(username, product.Id!);
            dto.Images = GetProductImage(product.Id!);
            dto.Categories = GetCategoriesByProduct(product.Id!);
            return dto;
        }
        return null!;
    }

    public async Task<string> LikeProduct(LikeProductDto likeProductDto)
    {
        var db = _firestoreDb.Collection(_collectionProduct_Like);
        var productDb = base.GetSnapshots(_collectionProducts);
        var likeDb = base.GetSnapshots(_collectionProduct_Like);
        var userDb = base.GetSnapshots(UserFireStore._collectionUser);
        //check user
        User user = null!;
        if (userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == likeProductDto.Username) == null)
        {
            user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Phone == likeProductDto.Username)!;
        }
        else
        {
            user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == likeProductDto.Username)!;
        }
        var product = productDb.Documents.Select(r => r.ConvertTo<Product>()).ToList().Find(r => r.Code == likeProductDto.ProductCode);
        //neu like da ton tai
        if (likeDb.Documents.Select(r => r.ConvertTo<Like>()).ToList().Find(r => r.UserId == user.Id && r.ProductId == product!.Id) != null)
        {
            var like = likeDb.Documents.Select(r => r.ConvertTo<Like>()).ToList().Find(r => r.UserId == user.Id && r.ProductId == product!.Id);
            DocumentReference docref = _firestoreDb.Collection(_collectionProduct_Like).Document(like!.Id);
            await docref.DeleteAsync();
        }
        else
        {
            Like like = new Like
            {
                UserId = user.Id,
                ProductId = product!.Id,
                CreatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc))
            };
            await db.AddAsync(like);
        }

        var shop = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Id == product!.AuthorId)!;
        await logFireStore.AddLogForUser(shop, "dang-san-pham");
        await notificationFireStore.AddNotificationForUser(shop, "Bạn vừa đăng sản phẩm", 0);
        return "";
    }

    public List<ProductDto> GetProductsNew(GetNewProductDto getNewProductDto, string username)
    {
        var productDb = base.GetSnapshots(_collectionProducts);
        var userDb = base.GetSnapshots(UserFireStore._collectionUser);
        List<ProductDto> productsDto = new List<ProductDto>();
        DateTime dateLimit = DateTime.Now.AddDays(-getNewProductDto.Day);
        var startIndex = getNewProductDto.ProductInPage * (getNewProductDto.Page - 1);
        var lastIndex = startIndex + getNewProductDto.ProductInPage;

        var productResult = new List<Product>();

        var products = productDb.Documents.Select(r => r.ConvertTo<Product>()).ToList().FindAll(r => r.CreatedAt.ToDateTime() >= dateLimit);
        for (int i = startIndex; i < lastIndex; i++)
        {
            if (i < products.Count)
            {
                if (products[i] != null)
                {
                    productResult.Add(products[i]);
                }
            }
            else
            {
                break;
            }
        }

        foreach (var item in productResult)
        {
            var user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Id == item!.AuthorId);
            ProductDto dto = productConverter.ToDto(item!);
            if (user != null)
            {
                dto.Author = userConverter.ToDto(user!);

            }
            dto.Classifies = GetProductClassifiesByProduct(item.Id!);
            dto.Likes = GetLikeOfProduct(item.Id!);
            dto.IsLiked = CheckIsLike(username, item.Id!);
            dto.Images = GetProductImage(item.Id!);
            productsDto.Add(dto);
        }
        return productsDto;
    }

    public List<ProductDto> GetProductsLike(GetProductLikeDto getProductLikeDto)
    {
        var productDb = base.GetSnapshots(_collectionProducts);
        var productlikeDb = base.GetSnapshots(_collectionProduct_Like);
        var userDb = base.GetSnapshots(UserFireStore._collectionUser);
        List<ProductDto> productsDto = new List<ProductDto>();

        var productResult = new List<Product>();

        //find user
        User user = null!;
        if (userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == getProductLikeDto.Username) == null)
        {
            user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Phone == getProductLikeDto.Username)!;
        }
        else
        {
            user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == getProductLikeDto.Username)!;
        }

        if (user == null)
        {
            return null!;
        }

        //find product like by user
        var productLikes = productlikeDb.Documents.Select(r => r.ConvertTo<Like>()).ToList().FindAll(r => r.UserId == user.Id);

        foreach (var item in productLikes)
        {
            var p = productDb.Documents.Select(r => r.ConvertTo<Product>()).ToList().Find(r => r.Id == item.ProductId);
            productResult.Add(p!);
        }


        foreach (var item in productResult)
        {
            var shop = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Id == item!.AuthorId);
            ProductDto dto = productConverter.ToDto(item!);
            if (shop != null)
            {
                dto.Author = userConverter.ToDto(shop!);

            }
            dto.Classifies = GetProductClassifiesByProduct(item.Id!);
            dto.Images = GetProductImage(item.Id!);
            dto.Likes = GetLikeOfProduct(item.Id!);
            dto.IsLiked = CheckIsLike(getProductLikeDto.Username, item.Id!);
            productsDto.Add(dto);
        }
        return productsDto;
    }

    public List<ProductDto> GetProductsHot(GetProductHot getProductHot, string username)
    {
        var productDb = base.GetSnapshots(_collectionProducts);
        var userDb = base.GetSnapshots(UserFireStore._collectionUser);
        List<ProductDto> productsDto = new List<ProductDto>();
        var startIndex = getProductHot.ProductInPage * (getProductHot.Page - 1);
        var lastIndex = startIndex + getProductHot.ProductInPage;

        var productResult = new List<Product>();
        var products = productDb.Documents.Select(r => r.ConvertTo<Product>()).OrderByDescending(p => p.QuantitySelled).ToList();
        for (int i = startIndex; i < lastIndex; i++)
        {
            if (i < products.Count)
            {
                if (products[i] != null)
                {
                    productResult.Add(products[i]);
                }
            }
            else
            {
                break;
            }
        }

        foreach (var item in productResult)
        {
            var user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Id == item!.AuthorId);
            ProductDto dto = productConverter.ToDto(item!);
            if (user != null)
            {
                dto.Author = userConverter.ToDto(user!);

            }
            dto.Classifies = GetProductClassifiesByProduct(item.Id!);
            dto.Images = GetProductImage(item.Id!);
            dto.Likes = GetLikeOfProduct(item.Id!);
            dto.IsLiked = CheckIsLike(username, item.Id!);
            productsDto.Add(dto);
        }
        return productsDto;
    }

    //method ho tro
    private async Task<CreateProductClassifyDto[]> AddProductClassify(CreateProductClassifyDto[] productClassifies, string productCode)
    {
        var db = _firestoreDb.Collection(_collectionProductClassify);
        var productDb = base.GetSnapshots(_collectionProducts);
        var productClassifyDb = base.GetSnapshots(_collectionProductClassify);
        var product = productDb.Documents.Select(r => r.ConvertTo<Product>()).ToList().Find(r => r.Code == productCode);

        foreach (var pClassify in productClassifies)
        {
            var productClassify = createProductClassifyConverter.ToEntity(pClassify);
            productClassify.Product = product;
            productClassify.ProductId = product!.Id;
            Random rnd = new Random();
            string randomCode = rnd.Next(1, 100000).ToString();
            while (productClassifyDb.Documents.Select(r => r.ConvertTo<ProductClassify>()).ToList().Find(r => r.Code == randomCode) != null)
            {
                randomCode = rnd.Next(1, 100000).ToString();
            }
            productClassify.Code = randomCode;
            await db.AddAsync(productClassify);
        }

        return productClassifies;
    }

    private async Task<List<string>> AddImage(List<string> productImages, string productCode)
    {
        var db = _firestoreDb.Collection(_collectionProductImage);
        var productDb = base.GetSnapshots(_collectionProducts);
        var product = productDb.Documents.Select(r => r.ConvertTo<Product>()).ToList().Find(r => r.Code == productCode);

        foreach (var pImage in productImages)
        {
            var productImage = new ProductImage
            {
                Image = pImage,
                Product = product,
                ProductId = product!.Id
            };
            await db.AddAsync(productImage);
        }

        return productImages;
    }

    private async Task AddCategoryProduct(string categoryCode, string productCode)
    {
        var db = _firestoreDb.Collection(Category_ProductFireStore._collectionCategoryProduct);
        var productDb = base.GetSnapshots(_collectionProducts);
        var product = productDb.Documents.Select(r => r.ConvertTo<Product>()).ToList().Find(r => r.Code == productCode);

        var categoryDb = base.GetSnapshots(CategoryFireStore._collectionCategory);
        var categories = categoryDb.Documents.Select(s => s.ConvertTo<Category>()).ToList();
        List<string> categoryIds = new List<string>();

        var category = categoryDb.Documents.Select(r => r.ConvertTo<Category>()).ToList().Find(r => r.Code == categoryCode);
        categoryIds.Add(category!.Id!);
        string temp = category.ParentCategoryId;

        for (int i = category!.Level - 1; i >= 0; i--)
        {
            for (int j = 0; j < categories.Count; j++)
            {
                if (temp == categories[j].Id && categories[j].Level == i && categories[j].ParentCategoryId == null)
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
            await db.AddAsync(new Category_Product { CategoryId = categoryId, ProductId = product!.Id! });
        }
    }

    private List<ProductClassifyDto> GetProductClassifiesByProduct(string productId)
    {
        var productDb = base.GetSnapshots(_collectionProducts);
        var product = productDb.Documents.Select(r => r.ConvertTo<Product>()).ToList().Find(r => r.Id == productId);
        var productClassifyDb = base.GetSnapshots(_collectionProductClassify);
        List<ProductClassifyDto> productsDto = new List<ProductClassifyDto>();
        List<ProductClassify> productClassifies = productClassifyDb.Documents.Select(r => r.ConvertTo<ProductClassify>()).ToList().FindAll(r => r.ProductId == productId);
        foreach (var pc in productClassifies)
        {
            ProductClassifyDto dto = productClassifyConverter.ToDto(pc);
            dto.PriceAfterIncreasePercent = pc.IncreasePercent == 0 ? product!.Price : product!.Price + (product!.Price * pc.IncreasePercent / 100);
            productsDto.Add(dto);
        }
        return productsDto;
    }

    public List<string> GetProductImage(string productId)
    {
        var productDb = base.GetSnapshots(_collectionProducts);
        var product = productDb.Documents.Select(r => r.ConvertTo<Product>()).ToList().Find(r => r.Id == productId);
        var productImagesDb = base.GetSnapshots(_collectionProductImage);
        List<string> images = new List<string>();
        List<ProductImage> productImages = productImagesDb.Documents.Select(r => r.ConvertTo<ProductImage>()).ToList().FindAll(r => r.ProductId == productId);
        foreach (var pc in productImages)
        {
            images.Add(pc.Image);
        }
        return images;
    }

    private List<CategoryDto> GetCategoriesByProduct(string productId)
    {
        var productDb = base.GetSnapshots(_collectionProducts);
        var categoryDb = base.GetSnapshots(CategoryFireStore._collectionCategory);
        var categoryProductDb = base.GetSnapshots(Category_ProductFireStore._collectionCategoryProduct);
        var categoryDtos = new List<CategoryDto>();
        var result = new List<CategoryDto>();

        var product = productDb.Documents.Select(r => r.ConvertTo<Product>()).ToList().Find(r => r.Id == productId);
        List<Category_Product> category_product_list = categoryProductDb.Documents.Select(r => r.ConvertTo<Category_Product>()).ToList().FindAll(r => r.ProductId == productId);

        var categories = new List<Category>();
        foreach (var pc in category_product_list)
        {
            var category = categoryDb.Documents.Select(r => r.ConvertTo<Category>()).ToList().Find(r => r.Id == pc.CategoryId);
            categories.Add(category!);
        }

        int high = 0;

        foreach (var category in categories)
        {
            if (high <= category.Level)
            {
                high = category.Level;
            }
            categoryDtos.Add(CategoryFireStore.categoryConverter.ToDto(category));
        }

        for (int i = 0; i < high; i++)
        {
            for (int j = 0; j < categoryDtos.Count; j++)
            {
                if (categoryDtos[j].ParentCategoryId == null && categoryDtos[j].Level == i)
                {
                    result.Add(categoryDtos[j]);
                    List<CategoryDto> arr = categoryDtos.FindAll(c => c.ParentCategoryId == categoryDtos[j].Id);
                    categoryDtos[j].Children = arr;
                    break;
                }

                if (categoryDtos[j].ParentCategoryId != null && categoryDtos[j].Level == i)
                {
                    List<CategoryDto> arr = categoryDtos.FindAll(c => c.ParentCategoryId == categoryDtos[j].Id);
                    categoryDtos[j].Children = arr;
                }
            }
        }

        return result;

    }

    private int GetLikeOfProduct(string productId)
    {
        var productLikeDb = base.GetSnapshots(_collectionProduct_Like);
        var productLikes = productLikeDb.Documents.Select(r => r.ConvertTo<Like>()).ToList().FindAll(r => r.ProductId == productId);
        return productLikes.Count;
    }

    private bool CheckIsLike(string username, string productId)
    {
        var userDb = base.GetSnapshots(UserFireStore._collectionUser);

        User user = null!;
        if (userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == username) == null)
        {
            user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Phone == username)!;
        }
        else
        {
            user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == username)!;
        }

        if (user != null)
        {
            var productLikeDb = base.GetSnapshots(_collectionProduct_Like);
            var productLike = productLikeDb.Documents.Select(r => r.ConvertTo<Like>()).ToList().Find(r => r.UserId == user.Id && r.ProductId == productId);
            return productLike != null;
        }
        else
        {
            return false;
        }
    }
}
