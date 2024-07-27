using Google.Cloud.Firestore;
using StoreSp.Converters;
using StoreSp.Converters.response;
using StoreSp.Dtos.request;
using StoreSp.Dtos.response;
using StoreSp.Entities;

namespace StoreSp.Stores;

public class BillFirestore(FirestoreDb firestoreDb) : FirestoreService(firestoreDb)
{
    public static string _collectionBill = "Bills";
    public static string _collectionBill_Product = "Bill_Product";

    public readonly IBaseConverter<Bill, CreateBillDto> AddBillConverter = new AddBillConverter();
    public readonly IBaseConverter<Bill, BillDto> BillConverter = new BillConverter();
    public readonly IBaseConverter<User, UserDto> userConverter = new UserConverter();
    private readonly IBaseConverter<Product, ProductDto> productConverter = new ProductConverter();

    public async Task<int> Checkout(CreateBillDto createBillDto)
    {
        var db = _firestoreDb.Collection(_collectionBill);
        var userDb = base.GetSnapshots(UserFireStore._collectionUser);
        var billDb = base.GetSnapshots(_collectionBill);

        //tao bill
        var bill = AddBillConverter.ToEntity(createBillDto);

        //set user cho bill
        User user = null!;
        if (userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == createBillDto.Username) == null)
        {
            user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Phone == createBillDto.Username)!;
        }
        else
        {
            user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == createBillDto.Username)!;
        }
        bill.UserId = user.Id;
        bill.User = user;

        //get cart cua user
        var cartDb = base.GetSnapshots(CartFireStore._collectionCart);
        var cart = cartDb.Documents.Select(r => r.ConvertTo<Cart>()).ToList().Find(r => r.UserId == user.Id);
        bill.TotalPrice = cart!.TotalPrice + bill.ShippingUnitPrice;

        if (user.Account < bill.TotalPrice)
        {
            return -1;
        }

        //get product cua cart cua user co status la 1
        var cartItemDb = base.GetSnapshots(CartFireStore._collectionCartItem);
        var cartItems = cartItemDb.Documents.Select(r => r.ConvertTo<CartItem>()).ToList().FindAll(r => r.CartId == cart.Id && r.Status == 1);

        if (cartItems.Count == 0)
        {
            return 0;
        }
        bill.Quantity = cartItems.Count;

        //gen code
        Random rnd = new Random();
        string randomCode = rnd.Next(1, 100000).ToString();
        while (billDb.Documents.Select(r => r.ConvertTo<Bill>()).ToList().Find(r => r.Code == randomCode) != null)
        {
            randomCode = rnd.Next(1, 100000).ToString();
        }
        bill.Code = randomCode;
        bill.TotalProductPrice = bill.TotalPrice - bill.ShippingUnitPrice;

        //get list productIds
        await db.AddAsync(bill);
        await AddBill_Product(cartItems, randomCode);
        await AfterCheckout(cart, cartItems, randomCode);
        return 1;
    }
    public List<BillDto> GetBillByUser(GetBillOfUserDto request)
    {
        var billDb = base.GetSnapshots(_collectionBill);
        var userDb = base.GetSnapshots(UserFireStore._collectionUser);
        List<BillDto> billDtos = new List<BillDto>();


        //tim user
        User user = null!;
        if (userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == request.Username) == null)
        {
            user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Phone == request.Username)!;
        }
        else
        {
            user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Email == request.Username)!;
        }

        if(user == null){
            return null!;
        }

        List<Bill> bills = new List<Bill>();

        if (request.Status == null)
        {
            bills = billDb.Documents.Select(r => r.ConvertTo<Bill>()).ToList().FindAll(r => r.UserId == user.Id);
        }
        else{
            bills = billDb.Documents.Select(r => r.ConvertTo<Bill>()).ToList().FindAll(r => r.UserId == user.Id && r.Status.ToString() == request.Status);
        }

        foreach (var bill in bills)
        {
            var billDto = BillConverter.ToDto(bill);
            billDto.User = userConverter.ToDto(user);
            var billItems = SetBillItem(bill.Id!);
            billDto.BillItems = billItems;
            billDtos.Add(billDto);
        }

        return billDtos;
    }
    public async Task AddBill_Product(List<CartItem> cartItems, string code)
    {
        var billDb = base.GetSnapshots(_collectionBill);
        var bill = billDb.Documents.Select(r => r.ConvertTo<Bill>()).ToList().Find(r => r.Code == code);
        var db = _firestoreDb.Collection(_collectionBill_Product);

        foreach (var item in cartItems)
        {
            var billProduct = new Bill_Product
            {
                BillId = bill!.Id!,
                ProductId = item.ProductId!,
                ProductClassifies = GetStringProductClassify(item.Id!)
            };
            await db.AddAsync(billProduct);
        }
    }
    private string GetStringProductClassify(string cartItemId)
    {
        string result = "";
        var cartItem_ProductClassifyDb = base.GetSnapshots(CartFireStore._collectionCartItem_ProductClassify);
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

    private async Task AfterCheckout(Cart c, List<CartItem> cartItems, string billCode)
    {
        //truong hop thanh toan thanh cong thi update lai tai khoan user va xoa gio hang
        var userDb = base.GetSnapshots(UserFireStore._collectionUser);
        var cartDb = base.GetSnapshots(CartFireStore._collectionCart);
        var billDb = base.GetSnapshots(_collectionBill);
        var productDb = base.GetSnapshots(ProductFireStore._collectionProducts);
        var bill = billDb.Documents.Select(r => r.ConvertTo<Bill>()).ToList().Find(r => r.Code == billCode); 
        var productClassifyDb = base.GetSnapshots(ProductFireStore._collectionProductClassify);
        var cartItem_ProductClassifyDb = base.GetSnapshots(CartFireStore._collectionCartItem_ProductClassify);

        //lay user tu gio hang
        var cart = cartDb.Documents.Select(r => r.ConvertTo<Cart>()).ToList().Find(r => r.Id == c.Id);
        var user = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Id == c.UserId);

        //update lai tai khoan user
        int account = user!.Account - bill!.TotalPrice;
        DocumentReference docref = _firestoreDb.Collection(UserFireStore._collectionUser).Document(user.Id);
        Dictionary<string, object> data = new Dictionary<string, object>{
            {"Account" , account}
        };
        DocumentSnapshot snapshot = await docref.GetSnapshotAsync();
        if (snapshot.Exists)
        {
            await docref.UpdateAsync(data);
        }

        //xoa gio hang
        foreach (var item in cartItems)
        {
            //update so luong da ban cua san pham
            var product = productDb.Documents.Select(r => r.ConvertTo<Product>()).ToList().Find(r => r.Id == item.ProductId);
            DocumentReference docrefProduct = _firestoreDb.Collection(ProductFireStore._collectionProducts).Document(product!.Id);
            Dictionary<string, object> dataProduct = new Dictionary<string, object>{
                {"QuantitySelled" , product.QuantitySelled + item.Quantity}
            };
            DocumentSnapshot snapshotProduct = await docrefProduct.GetSnapshotAsync();
            if (snapshotProduct.Exists)
            {
                await docrefProduct.UpdateAsync(dataProduct);
            }

            var cartItem_ProductClassifies = cartItem_ProductClassifyDb.Documents
            .Select(r => r.ConvertTo<CartItem_ProductClassify>())
            .ToList()
            .FindAll(r => r.CartItem_Id == item.Id);
            foreach (var it in cartItem_ProductClassifies)
            {
                //update so luong hang ton
                var productClassify = productClassifyDb.Documents.Select(r => r.ConvertTo<ProductClassify>()).ToList().Find(r => r.Id == it.ProductClassify_Id);
                var a = productClassify!.Quantity - item.Quantity;
                DocumentReference docrefProductClassify = _firestoreDb.Collection(ProductFireStore._collectionProductClassify).Document(productClassify!.Id);
                Dictionary<string, object> dataProductClassify = new Dictionary<string, object>{
                   {"Quantity" , a}
                };
                DocumentSnapshot snapshotProductClassify = await docrefProductClassify.GetSnapshotAsync();
                if (snapshotProductClassify.Exists)
                {
                    await docrefProductClassify.UpdateAsync(dataProductClassify);
                }

                //xoa phan loai cua san pham cua gio hang
                DocumentReference docrefIt = _firestoreDb.Collection(CartFireStore._collectionCartItem_ProductClassify).Document(it.Id);
                await docrefIt.DeleteAsync();
            }
            DocumentReference docrefCartItem = _firestoreDb.Collection(CartFireStore._collectionCartItem).Document(item.Id);
            await docrefCartItem.DeleteAsync();
        }

        //update total cho cart cua user
        DocumentReference docrefCart = _firestoreDb.Collection(CartFireStore._collectionCart).Document(cart!.Id);
        Dictionary<string, object> dataCart = new Dictionary<string, object>{
            {"TotalPrice" , 0}
        };
        DocumentSnapshot snapshotCart = await docrefCart.GetSnapshotAsync();
        if (snapshotCart.Exists)
        {
            await docrefCart.UpdateAsync(dataCart);
        }
    }
    
    private List<BillItemDto> SetBillItem(string billId){
        var productDb = base.GetSnapshots(ProductFireStore._collectionProducts);
        var userDb = base.GetSnapshots(UserFireStore._collectionUser);
        var bill_ProductDb = base.GetSnapshots(_collectionBill_Product);
        List<BillItemDto> billItemDtos = new List<BillItemDto>();
        var billItems = bill_ProductDb.Documents.Select(r => r.ConvertTo<Bill_Product>()).ToList().FindAll(r => r.BillId == billId);
        foreach (var item in billItems)
        {
            BillItemDto billItemDto = new BillItemDto();
            var product = productDb.Documents.Select(r => r.ConvertTo<Product>()).ToList().Find(r => r.Id == item.ProductId);
            var productDto = productConverter.ToDto(product!);
            var author = userDb.Documents.Select(r => r.ConvertTo<User>()).ToList().Find(r => r.Id == product!.AuthorId);
            productDto.Author = userConverter.ToDto(author!);
            billItemDto.Product = productDto;
            billItemDto.ProductClassifies = item.ProductClassifies;
            billItemDtos.Add(billItemDto);
        }
        return billItemDtos;
    }
}
