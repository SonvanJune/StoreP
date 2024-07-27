using Google.Cloud.Firestore;
using StoreSp.Converters;
using StoreSp.Converters.request;
using StoreSp.Converters.response;
using StoreSp.Dtos.request;
using StoreSp.Dtos.response;
using StoreSp.Entities;

namespace StoreSp.Stores;

public class CartFireStore(FirestoreDb firestoreDb) : FirestoreService(firestoreDb)
{
    //Properties
    public static string _collectionCart = "Carts";
    public static string _collectionCartItem = "CartItems";
    public static string _collectionCartItem_ProductClassify = "CartItem_ProductClassifies";
    private readonly IBaseConverter<CartItem, AddCartItemDto> addCartItemConverter = new AddCartItemConverter();
    public readonly IBaseConverter<User, UserDto> userConverter = new UserConverter();
    private readonly IBaseConverter<Product, ProductDto> productConverter = new ProductConverter();
    private readonly IBaseConverter<CartItem, CartItemDto> cartItemConverter = new CartItemConverter();

    //Method chinh
    public async Task<string> AddToCart(AddCartItemDto itemDto)
    {
        var db = _firestoreDb.Collection(_collectionCartItem);
        var productDb = base.GetSnapshots(ProductFireStore._collectionProducts);
        var productClassifyDb = base.GetSnapshots(ProductFireStore._collectionProductClassify);
        var userDb = base.GetSnapshots(UserFireStore._collectionUser);
        var cartDb = base.GetSnapshots(_collectionCart);
        var cartItemDb = base.GetSnapshots(_collectionCartItem);
        CartItem item = null!;
        List<string> productClassifyIds = new List<string>();

        User user = null!;
        if (userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == itemDto.Username) == null)
        {
            user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Phone == itemDto.Username)!;
        }
        else
        {
            user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == itemDto.Username)!;
        }

        if (user == null)
        {
            return null!;
        }

        var cart = cartDb.Documents.Select(r => r.ConvertTo<Cart>()).ToList().Find(r => r.UserId == user.Id);

        var product = productDb.Documents.Select(r => r.ConvertTo<Product>()).ToList().Find(r => r.Code == itemDto.ProductCode);
        if (product == null)
        {
            return null!;
        }

        //get list ids of classify of product input
        foreach (var code in itemDto.ProductClassifyCodes!)
        {
            var productClassify = productClassifyDb.Documents.Select(r => r.ConvertTo<ProductClassify>()).ToList().Find(r => r.Code == code);
            productClassifyIds.Add(productClassify!.Id!);
        }

        //check cart item of the classify exist if the classify only one
        string idCartItemExist = CheckExistCartItem(product, cart!, productClassifyIds);
        if (idCartItemExist != "")
        {
            //Update cart item of the classify of product input
            item = cartItemDb.Documents.Select(r => r.ConvertTo<CartItem>()).ToList().Find(r => r.Id == idCartItemExist)!;
            int oldTotal = item.Total;
            int newQuantity = itemDto.Quantity + item.Quantity;
            int total = item.Price * newQuantity;
            DocumentReference docref = _firestoreDb.Collection(_collectionCartItem).Document(idCartItemExist);
            Dictionary<string, object> data = new Dictionary<string, object>{
               {"Quantity" , newQuantity},
               {"Total" , total}
            };

            DocumentSnapshot snapshot = await docref.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                await docref.UpdateAsync(data);
            }

            //change status when add again
            await UpdateWhenAddAgain(item.Code!, oldTotal);
        }
        else
        {
            //Add to cart item of the classify of product input
            //set cart item
            item = addCartItemConverter.ToEntity(itemDto);

            item.ProductId = product.Id!;
            item.CartId = cart!.Id;
            item.Cart = cart;
            item.Product = product;

            //get the highets increase percent
            int highIncreasePercent = 0;
            foreach (var code in itemDto.ProductClassifyCodes!)
            {
                var productClassify = productClassifyDb.Documents.Select(r => r.ConvertTo<ProductClassify>()).ToList().Find(r => r.Code == code);
                if (productClassify!.IncreasePercent > highIncreasePercent)
                {
                    highIncreasePercent = productClassify!.IncreasePercent;
                }
            }

            int price = highIncreasePercent == 0 ? product.PriceSaleOff : product.PriceSaleOff + (product.Price * highIncreasePercent / 100);
            int total = price * itemDto.Quantity;
            item.Price = price;
            item.Total = total;

            //gen code
            Random rnd = new Random();
            string randomCode = rnd.Next(1, 100000).ToString();
            while (cartItemDb.Documents.Select(r => r.ConvertTo<CartItem>()).ToList().Find(r => r.Code == randomCode) != null)
            {
                randomCode = rnd.Next(1, 100000).ToString();
            }
            item.Code = randomCode;

            await db.AddAsync(item);
            await AddCartItem_ProductClassifies(productClassifyIds, randomCode);
        }

        return idCartItemExist;
    }

    public async Task<CartItem> CheckoutItemInCart(string code)
    {
        var cartItemDb = base.GetSnapshots(_collectionCartItem);
        var cartDb = base.GetSnapshots(_collectionCart);
        var cartItem = cartItemDb.Documents.Select(r => r.ConvertTo<CartItem>()).ToList().Find(r => r.Code == code);
        if (cartItem != null)
        {
            int status = cartItem.Status == 0 ? 1 : 0;
            DocumentReference docref = _firestoreDb.Collection(_collectionCartItem).Document(cartItem.Id);
            Dictionary<string, object> data = new Dictionary<string, object>{
               {"Status" , status}
            };
            DocumentSnapshot snapshot = await docref.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                await docref.UpdateAsync(data);
            }
            int totalAfterAdd = cartItem.Total;


            var cart = cartDb.Documents.Select(r => r.ConvertTo<Cart>()).ToList().Find(r => r.Id == cartItem.CartId);
            // add total for cart
            if (status == 1)
            {
                totalAfterAdd += cart!.TotalPrice;
            }
            else
            {
                totalAfterAdd = cart!.TotalPrice - totalAfterAdd;
            }

            DocumentReference docrefCart = _firestoreDb.Collection(_collectionCart).Document(cart.Id);
            Dictionary<string, object> dataCart = new Dictionary<string, object>{
            {"TotalPrice" , totalAfterAdd}
            };

            DocumentSnapshot snapshotCart = await docrefCart.GetSnapshotAsync();
            if (snapshotCart.Exists)
            {
                await docrefCart.UpdateAsync(dataCart);
            }
            return cartItem;
        }
        return null!;
    }

    public CartDto GetCartByUser(string username)
    {
        var userDb = base.GetSnapshots(UserFireStore._collectionUser);
        var cartDb = base.GetSnapshots(_collectionCart);
        CartDto cartDto = new CartDto();

        //find user
        User user = null!;
        if (userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == username) == null)
        {
            user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Phone == username)!;
        }
        else
        {
            user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == username)!;
        }

        if (user == null)
        {
            return null!;
        }

        var userDto = userConverter.ToDto(user);

        //lay cart tu database
        var cart = cartDb.Documents.Select(r => r.ConvertTo<Cart>()).ToList().Find(r => r.UserId == user.Id);
        cartDto.TotalPrice = cart!.TotalPrice;

        //tao list cartitem dto
        List<CartItemDto> cartItemDtos = GetCartItemDtos(cart.Id!);

        cartDto.Items = cartItemDtos;
        cartDto.Quantity = cartItemDtos.Count;
        return cartDto;
    }

    public async Task<string> UpdateCartByUser(UpdateCartDto updateCartDto)
    {
        var cartUpdatedb = _firestoreDb.Collection(_collectionCart);
        var cartItemUpdatedb = _firestoreDb.Collection(_collectionCartItem);
        var cartDb = base.GetSnapshots(_collectionCart);
        var cartItemDb = base.GetSnapshots(_collectionCartItem);
        var userDb = base.GetSnapshots(UserFireStore._collectionUser);

        User user = null!;
        if (userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == updateCartDto.Username) == null)
        {
            user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Phone == updateCartDto.Username)!;
        }
        else
        {
            user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == updateCartDto.Username)!;
        }

        var cart = cartDb.Documents.Select(r => r.ConvertTo<Cart>()).ToList().Find(r => r.UserId == user.Id);
        
        int totalAfterAdd = cart!.TotalPrice;
        //update cart items
        foreach (var item in updateCartDto.UpdateCartItems!)
        {
            var cartItem = cartItemDb.Documents.Select(r => r.ConvertTo<CartItem>()).ToList().Find(r => r.Code == item.ItemCode);
            if (cartItem != null)
            {
                totalAfterAdd = totalAfterAdd - cartItem.Total;
                int quantity = item.Quantity;
                int total = item.Quantity * cartItem.Price;
                totalAfterAdd += total;
                DocumentReference docref = _firestoreDb.Collection(_collectionCartItem).Document(cartItem.Id);
                Dictionary<string, object> data = new Dictionary<string, object>{
                    {"Quantity" , quantity},
                    {"Total" , total}
                };
                DocumentSnapshot snapshot = await docref.GetSnapshotAsync();
                if (snapshot.Exists)
                {
                    await docref.UpdateAsync(data);
                }
            }
        }

        //update cart
        DocumentReference docrefCart = _firestoreDb.Collection(_collectionCart).Document(cart!.Id);
        Dictionary<string, object> dataCart = new Dictionary<string, object>{
            {"TotalPrice" , totalAfterAdd}
        };
        DocumentSnapshot snapshotCart = await docrefCart.GetSnapshotAsync();
        if (snapshotCart.Exists)
        {
            await docrefCart.UpdateAsync(dataCart);
        }
        return "Updated";
    }
    //method ho tro
    private async Task AddCartItem_ProductClassifies(List<string> productClassifyIds, string code)
    {
        var db = _firestoreDb.Collection(_collectionCartItem_ProductClassify);
        var cartItemDb = base.GetSnapshots(_collectionCartItem);
        var cartItem = cartItemDb.Documents.Select(r => r.ConvertTo<CartItem>()).ToList().Find(r => r.Code == code);

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

    private string CheckExistCartItem(Product product, Cart cart, List<string> productClassifyIds)
    {
        var cartItemDb = base.GetSnapshots(_collectionCartItem);
        var cartItem_ProductClassifyDb = base.GetSnapshots(_collectionCartItem_ProductClassify);
        string status = "";
        List<List<CartItem_ProductClassify>> cartItem_ProductClassifies_list = new List<List<CartItem_ProductClassify>>();

        var existCartItems = cartItemDb.Documents.Select(r => r.ConvertTo<CartItem>()).ToList().FindAll(r => r.ProductId == product.Id && r.CartId == cart!.Id);
        if (existCartItems.Count != 0)
        {

            foreach (var it in existCartItems)
            {
                List<CartItem_ProductClassify> cartItem_ProductClassifies =
                cartItem_ProductClassifyDb.Documents.Select(r => r.ConvertTo<CartItem_ProductClassify>()).ToList().FindAll(r => r.CartItem_Id == it.Id);
                cartItem_ProductClassifies_list.Add(cartItem_ProductClassifies);
            }

            for (int i = 0; i < cartItem_ProductClassifies_list.Count; i++)
            {
                List<string> temp = new List<string>();
                for (int j = 0; j < cartItem_ProductClassifies_list[i].Count; j++)
                {
                    temp.Add(cartItem_ProductClassifies_list[i][j].ProductClassify_Id);
                }
                if (CompareArray(temp, productClassifyIds))
                {
                    status = cartItem_ProductClassifies_list[i][0].CartItem_Id;
                    break;
                }
            }
        }
        return status;
    }

    private List<CartItemDto> GetCartItemDtos(string cartId)
    {
        List<CartItemDto> cartItemDtos = new List<CartItemDto>();
        var cartItemDb = base.GetSnapshots(_collectionCartItem);
        var userDb = base.GetSnapshots(UserFireStore._collectionUser);
        var cartItems = cartItemDb.Documents.Select(r => r.ConvertTo<CartItem>()).ToList().FindAll(r => r.CartId == cartId);
        var productDb = base.GetSnapshots(ProductFireStore._collectionProducts);
        foreach (var cartItem in cartItems)
        {
            CartItemDto cartItemDto = cartItemConverter.ToDto(cartItem);
            //get product for cart item
            var product = productDb.Documents.Select(r => r.ConvertTo<Product>()).ToList().Find(r => r.Id == cartItem.ProductId);
            cartItemDto.Product = productConverter.ToDto(product!);

            //get author for cart item
            var user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Id == product!.AuthorId);
            cartItemDto.Shop = userConverter.ToDto(user!);

            //get string product classify for cart item
            cartItemDto.CartItem_ProductClassifies = GetStringProductClassify(cartItem.Id!);
            //them vao mang
            cartItemDtos.Add(cartItemDto);
        }
        return cartItemDtos;
    }

    private string GetStringProductClassify(string cartItemId)
    {
        string result = "";
        var cartItem_ProductClassifyDb = base.GetSnapshots(_collectionCartItem_ProductClassify);
        var productClassifyDb = base.GetSnapshots(ProductFireStore._collectionProductClassify);
        var cartItem_ProductClassifies = cartItem_ProductClassifyDb.Documents
        .Select(r => r.ConvertTo<CartItem_ProductClassify>())
        .ToList()
        .FindAll(r => r.CartItem_Id == cartItemId);

        for (int i = 0; i < cartItem_ProductClassifies.Count; i++)
        {
            var productClassify = productClassifyDb.Documents.Select(r => r.ConvertTo<ProductClassify>()).ToList().Find(r => r.Id == cartItem_ProductClassifies[i].ProductClassify_Id);
            if (i == cartItem_ProductClassifies.Count - 1)
            {
                result += productClassify!.Name;
            }
            else
            {
                result += productClassify!.Name + ",";
            }
        }
        return result;
    }

    private bool CompareArray(List<string> a, List<string> b)
    {
        if (a.Count != b.Count)
            return false;
        return !a.Except(b).Any() && !b.Except(a).Any();
    }

    private async Task UpdateWhenAddAgain(string code, int oldTotal)
    {
        var cartItemDb = base.GetSnapshots(_collectionCartItem);
        var cartDb = base.GetSnapshots(_collectionCart);
        var cartItem = cartItemDb.Documents.Select(r => r.ConvertTo<CartItem>()).ToList().Find(r => r.Code == code);
        if (cartItem != null)
        {
            if (cartItem.Status == 1)
            {
                DocumentReference docref = _firestoreDb.Collection(_collectionCartItem).Document(cartItem.Id);
                Dictionary<string, object> data = new Dictionary<string, object>{
                {"Status" , 0}
                };
                DocumentSnapshot snapshot = await docref.GetSnapshotAsync();
                if (snapshot.Exists)
                {
                    await docref.UpdateAsync(data);
                }

                var cart = cartDb.Documents.Select(r => r.ConvertTo<Cart>()).ToList().Find(r => r.Id == cartItem.CartId);
                DocumentReference docrefCart = _firestoreDb.Collection(_collectionCart).Document(cart!.Id);
                Dictionary<string, object> dataCart = new Dictionary<string, object>{
                {"TotalPrice" , cart.TotalPrice-oldTotal}
                };

                DocumentSnapshot snapshotCart = await docrefCart.GetSnapshotAsync();
                if (snapshotCart.Exists)
                {
                    await docrefCart.UpdateAsync(dataCart);
                }
            }
        }
    }
}
