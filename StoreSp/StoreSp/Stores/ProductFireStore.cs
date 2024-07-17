using Google.Cloud.Firestore;
using StoreSp.Converters;
using StoreSp.Converters.request;
using StoreSp.Dtos.request;
using StoreSp.Dtos.response;
using StoreSp.Entities;

namespace StoreSp.Stores;

public class ProductFireStore(FirestoreDb firestoreDb) : FirestoreService(firestoreDb)
{
    public static string _collectionProducts = "Products";
    public static string _collectionProductClassify = "Product_Classifies";
    private readonly IBaseConverter<Product, CreateProductDto> createProductConverter = new CreateProductConverter();
    private readonly IBaseConverter<ProductClassify, CreateProductClassifyDto> createProductClassifyConverter = new CreateProductClassifyConverter();

    public async Task<Product> AddProduct(CreateProductDto createProductDto){
        var db = _firestoreDb.Collection(_collectionProducts);
        var categoryDb = base.GetSnapshots(CategoryFireStore._collectionCategory);
        var productDb = base.GetSnapshots(_collectionProducts);
        var userDb = base.GetSnapshots(UserFireStore._collectionUser);

        var product = createProductConverter.ToEntity(createProductDto);
        Random rnd = new Random();
        string randomCode =  rnd.Next(1,100000).ToString();
        while(productDb.Documents.Select(r => r.ConvertTo<Product>()).ToList().Find(r => r.Code == randomCode) != null){
            randomCode = rnd.Next(1,100000).ToString();
        }
        product.Code = randomCode;

        var user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == createProductDto.AuthEmail);
        product.Author = user;
        product.AuthorId = user!.Id;

        
        await db.AddAsync(product);
        await AddCategoryProduct(createProductDto.CategoryCode , randomCode);
        await AddProductClassify(createProductDto.ClassiFies! , randomCode);
        return product;
    }
    
    public async Task<CreateProductClassifyDto[]> AddProductClassify(CreateProductClassifyDto[] productClassifies , string productCode){
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

    public async Task AddCategoryProduct(string categoryCode , string productCode){
        var db = _firestoreDb.Collection(Category_ProductFireStore._collectionCategoryProduct);
        var productDb = base.GetSnapshots(_collectionProducts);
        var product = productDb.Documents.Select(r => r.ConvertTo<Product>()).ToList().Find(r => r.Code == productCode);

        var categoryDb = base.GetSnapshots(CategoryFireStore._collectionCategory);
        var categories = categoryDb.Documents.Select(s => s.ConvertTo<Category>()).ToList();
        List<string> categoryIds = new List<string>();

        var category = categoryDb.Documents.Select(r => r.ConvertTo<Category>()).ToList().Find(r => r.Code == categoryCode);
        categoryIds.Add(category!.Id!);
        string temp = category.ParentCategoryId;

        for (int i = category!.Level - 1; i >= 0 ; i--)
        {
            for (int j = 0; j < categories.Count; j++){
                if(temp == categories[j].Id && categories[j].Level == i && categories[j].ParentCategoryId == null){
                    categoryIds.Add(categories[j].Id!);
                    temp = categories[j].ParentCategoryId!;
                }
                if(temp == categories[j].Id && categories[j].Level == i){
                    categoryIds.Add(categories[j].Id!);
                    temp = categories[j].ParentCategoryId!;
                }
            }
        }

        foreach (var categoryId in categoryIds)
        {
            await db.AddAsync(new Category_Product{ CategoryId = categoryId, ProductId = product!.Id! });
        }
    }

    
}
