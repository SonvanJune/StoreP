using Google.Cloud.Firestore;
using StoreSp.Converters;
using StoreSp.Converters.request;
using StoreSp.Dtos.request;
using StoreSp.Entities;

namespace StoreSp.Stores;

public class CartFireStore(FirestoreDb firestoreDb) : FirestoreService(firestoreDb)
{
    //Properties
    public static string _collectionCart = "Carts";
    public static string _collectionCartItem = "CartItems";
    public static string _collectionCartItem_ProductClassify = "CartItem_ProductClassifies";
    private readonly IBaseConverter<CartItem, AddCartItemDto> addCartItemConverter = new AddCartItemConverter();

    //Method chinh
    public async Task<CartItem> AddToCart(AddCartItemDto itemDto)
    {
        var db = _firestoreDb.Collection(_collectionCartItem);
        var productDb = base.GetSnapshots(ProductFireStore._collectionProducts);
        var productClassifyDb = base.GetSnapshots(ProductFireStore._collectionProductClassify);
        var userDb = base.GetSnapshots(UserFireStore._collectionUser);
        var cartDb = base.GetSnapshots(_collectionCart);
        List<string> productClassifyIds = new List<string>();

        var user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Id == itemDto.UserId);
        if (user == null)
        {
            return null!;
        }

        var cart = cartDb.Documents.Select(r => r.ConvertTo<Cart>()).ToList().Find(r => r.UserId == itemDto.UserId);

        var product = productDb.Documents.Select(r => r.ConvertTo<Product>()).ToList().Find(r => r.Code == itemDto.ProductCode);
        if (product == null)
        {
            return null!;
        }

        var item = addCartItemConverter.ToEntity(itemDto);

        item.ProductId = product.Id!;
        item.CartId = cart!.Id;
        item.Cart = cart;
        item.Product = product;

        int highIncreasePercent = 0;
        foreach (var code in itemDto.ProductClassifyCodes!)
        {
            var productClassify = productClassifyDb.Documents.Select(r => r.ConvertTo<ProductClassify>()).ToList().Find(r => r.Code == code);
            if (productClassify!.IncreasePercent > highIncreasePercent)
            {
                highIncreasePercent = productClassify!.IncreasePercent;
            }
            productClassifyIds.Add(productClassify!.Id!);
        }

        int price = highIncreasePercent == 0 ? product.PriceSaleOff : product.PriceSaleOff + (product.Price * highIncreasePercent / 100);
        int total = price * itemDto.Quantity;
        item.Price = price;
        item.Total = total;


        await db.AddAsync(item);
        await AddCartItem_ProductClassifies(productClassifyIds, item);
        return item;
    }


    //method ho tro
    public async Task AddCartItem_ProductClassifies(List<string> productClassifyIds, CartItem item)
    {
        var db = _firestoreDb.Collection(_collectionCartItem_ProductClassify);
        var cartItemDb = base.GetSnapshots(_collectionCartItem);
        var cartItem = cartItemDb.Documents.Select(r => r.ConvertTo<CartItem>()).ToList().Find(r => r.ProductId == item.ProductId && r.CartId == item.CartId);

        foreach (var str in productClassifyIds)
        {
            var a = new CartItem_ProductClassify
            {
                CartItem_Id = cartItem!.Id!,
                ProductClassify_Id = str
            };
            await db.AddAsync(a);
        }
    }
}
