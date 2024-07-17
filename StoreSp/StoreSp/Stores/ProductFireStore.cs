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
    public static string _collectionProducts = "Products";
    public static string _collectionProductClassify = "Product_Classifies";
    private readonly IBaseConverter<User, UserDto> userConverter = new UserConverter();
    private readonly IBaseConverter<Product, CreateProductDto> createProductConverter = new CreateProductConverter();
    private readonly IBaseConverter<ProductClassify, CreateProductClassifyDto> createProductClassifyConverter = new CreateProductClassifyConverter();
    private readonly IBaseConverter<Product, ProductDto> productConverter = new ProductConverter();
    private readonly IBaseConverter<ProductClassify, ProductClassifyDto> productClassifyConverter = new ProductClassifyConverter();
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

        var user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == createProductDto.AuthEmail);
        product.Author = user;
        product.AuthorId = user!.Id;


        await db.AddAsync(product);
        await AddCategoryProduct(createProductDto.CategoryCode, randomCode);
        await AddProductClassify(createProductDto.ClassiFies!, randomCode);
        return product;
    }

    public async Task<CreateProductClassifyDto[]> AddProductClassify(CreateProductClassifyDto[] productClassifies, string productCode)
    {
        var db = _firestoreDb.Collection(_collectionProductClassify);
        var productDb = base.GetSnapshots(_collectionProducts);
        var product = productDb.Documents.Select(r => r.ConvertTo<Product>()).ToList().Find(r => r.Code == productCode);

        foreach (var pClassify in productClassifies)
        {
            var productClassify = createProductClassifyConverter.ToEntity(pClassify);
            productClassify.Product = product;
            productClassify.ProductId = product!.Id;
            await db.AddAsync(productClassify);
        }

        return productClassifies;
    }

    public async Task AddCategoryProduct(string categoryCode, string productCode)
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

    public List<ProductDto> GetProductsByCategory(string categoryCode)
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
            dto.Author = userConverter.ToDto(user!);
            dto.Classifies = GetProductClassifiesByProduct(cp.ProductId);
            dto.Categories = GetCategoriesByProduct(cp.ProductId);
            productsDto.Add(dto);
        }

        return productsDto;
    }

    public List<ProductClassifyDto> GetProductClassifiesByProduct(string productId)
    {
        var productDb = base.GetSnapshots(_collectionProducts);
        var product = productDb.Documents.Select(r => r.ConvertTo<Product>()).ToList().Find(r => r.Id == productId);
        var productClassifyDb = base.GetSnapshots(_collectionProductClassify);
        List<ProductClassifyDto> productsDto = new List<ProductClassifyDto>();
        List<ProductClassify> productClassifies = productClassifyDb.Documents.Select(r => r.ConvertTo<ProductClassify>()).ToList().FindAll(r => r.ProductId == productId);
        foreach (var pc in productClassifies)
        {
            ProductClassifyDto dto = productClassifyConverter.ToDto(pc);
            dto.PriceAfterIncreasePercent = pc.IncreasePercent == 0 ? product!.Price : product!.Price + (product!.Price * pc.IncreasePercent /100);
            productsDto.Add(dto);
        }
        return productsDto;
    }

    public List<CategoryDto> GetCategoriesByProduct(string productId){
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

        int high = 0 ;
        
        foreach (var category in categories)
        {
            if(high <= category.Level){
                high = category.Level;
            }
            categoryDtos.Add(CategoryFireStore.categoryConverter.ToDto(category));
        }

        for (int i = 0; i < high; i++)
        {
            for (int j = 0; j < categoryDtos.Count; j++)
            {
                if(categoryDtos[j].ParentCategoryId == null && categoryDtos[j].Level == i ){
                    result.Add(categoryDtos[j]);
                    List<CategoryDto> arr = categoryDtos.FindAll(c => c.ParentCategoryId == categoryDtos[j].Id);
                    categoryDtos[j].Children = arr;
                    break;
                }
                
                if(categoryDtos[j].ParentCategoryId != null && categoryDtos[j].Level == i ){
                    List<CategoryDto> arr = categoryDtos.FindAll(c => c.ParentCategoryId == categoryDtos[j].Id);
                    categoryDtos[j].Children = arr;
                }
            }
        }

        return result;

    }
}
