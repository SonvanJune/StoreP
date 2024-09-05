using Google.Cloud.Firestore;
using Microsoft.EntityFrameworkCore;
using StoreSp.Commonds;
using StoreSp.Context;
using StoreSp.Converters;
using StoreSp.Converters.response;
using StoreSp.Dtos.request;
using StoreSp.Dtos.response;
using StoreSp.Models;

namespace StoreSp.Stores;

public class BillFirestore
{
    private readonly AppDbContext? _appDbContext = null;

    public BillFirestore()
    {
        _appDbContext = AppDbContext.GetInstance();
    }

    public readonly IBaseConverter<Bill, CreateBillDto> AddBillConverter = new AddBillConverter();
    public readonly IBaseConverter<Bill, BillDto> BillConverter = new BillConverter();
    public readonly IBaseConverter<User, UserDto> userConverter = new UserConverter();
    private readonly IBaseConverter<Product, ProductDto> productConverter = new ProductConverter();
    public readonly IBaseConverter<Address, AddressDto> addressConverter = new AddressConverter();
    private readonly IBaseConverter<ShippingMethod, ShippingMethodDto> shippingMethodConverter = new ShippingMethodConverter();
    public readonly LogFireStore logFireStore = new LogFireStore();
    public readonly NotificationFireStore notificationFireStore = new NotificationFireStore();
    public readonly CartFireStore cartFirestore = new CartFireStore();
    public readonly ProductFireStore productFireStore = new ProductFireStore();

    //method chinh
    public async Task<int> Checkout(CreateBillDto createBillDto)
    {
        //tao bill
        var bill = AddBillConverter.ToEntity(createBillDto);

        if (createBillDto.PaymentMethod.ToLower().Contains("Tha"))
        {
            bill.Status = 1;
        }
        else
        {
            bill.Status = 0;
        }

        //set user cho bill
        User user = null!;
        if (_appDbContext!.Users.SingleOrDefault(r => r.Email == createBillDto.Username) == null)
        {
            user = _appDbContext!.Users.Include(r => r.Cart!.Items).SingleOrDefault(r => r.Phone == createBillDto.Username)!;
        }
        else
        {
            user = _appDbContext!.Users.Include(r => r.Cart!.Items).SingleOrDefault(r => r.Email == createBillDto.Username)!;
        }
        bill.UserId = user.Id;
        bill.User = user;

        var address = _appDbContext.Addresses.SingleOrDefault(r => r.Code == createBillDto.AddressCode);
        bill.AddressId = address!.Id;
        var shippingMethod = _appDbContext.ShippingMethods.SingleOrDefault(r => r.Code == createBillDto.ShippingCode);
        bill.ShippingMethodId = shippingMethod!.Id;
        var shippingCost = shippingMethod!.Price + (createBillDto.Kilometers * VariableConfig<double>.Application["price-of-kilometer"]);
        bill.TotalPrice = Convert.ToInt32(user.Cart!.TotalPrice + shippingCost);

        //get product cua cart cua user co status la 1
        var cartItems = _appDbContext.CartItems.Where(r => r.CartId == user.Cart.Id && r.Status == 1).ToList();

        if (cartItems.Count == 0)
        {
            return 0;
        }
        int quantity = 0;
        foreach (var cartItem in cartItems)
        {
            quantity += cartItem.Quantity;
        }
        bill.Quantity = quantity;

        //gen code
        Random rnd = new Random();
        string randomCode = rnd.Next(1, 100000).ToString();
        while (_appDbContext.Bills.SingleOrDefault(r => r.Code == randomCode) != null)
        {
            randomCode = rnd.Next(1, 100000).ToString();
        }
        bill.Code = randomCode;
        bill.TotalProductPrice = Convert.ToInt32(bill.TotalPrice - shippingCost);
        bill.Bill_Products = AddBill_Product(cartItems, bill);
        _appDbContext.Bills.Add(bill);

        //sau khi checkout
        foreach (var item in user.Cart.Items!)
        {
            var cartItem = _appDbContext.CartItems.Include(i => i.Product).Include(i => i.ProductClassifies).SingleOrDefault(i => i.Id == item.Id);
            if (cartItem!.Status == 1)
            {
                cartItem!.Product!.QuantitySelled += cartItem.Quantity;
                foreach (var i in cartItem.ProductClassifies!)
                {
                    i.Quantity -= cartItem.Quantity;
                }
                cartItem.ProductClassifies = [];
                user.Cart.Items.Remove(cartItem);
                _appDbContext.Users.Update(user);
                await _appDbContext.SaveChangesAsync();
            }
        }
        user.Cart.TotalPrice = 0;
        _appDbContext.Users.Update(user);
        await _appDbContext.SaveChangesAsync();
        await logFireStore.AddLogForUser(user, "thanh-toan");
        await notificationFireStore.AddNotificationForUser(user, "Bạn vừa than toán đơn hàng", 0);
        if (user.DeviceToken != null && user.DeviceToken != "")
        {
            try
            {
                await FirestoreService._fmcService.SendNotificationAsync(user.DeviceToken!, "Thanh toán thành công", "This is a test notification");

            }
            catch
            {
                Console.WriteLine("Send notification fail");
            }
        }
        return 1;
    }
    public List<BillDto> GetBillByUser(GetBillOfUserDto request)
    {
        List<BillDto> billDtos = new List<BillDto>();

        // tim user
        User user = null!;
        if (_appDbContext!.Users.SingleOrDefault(r => r.Email == request.Username) == null)
        {
            user = _appDbContext!.Users.Include(r => r.Bills).SingleOrDefault(r => r.Phone == request.Username)!;
        }
        else
        {
            user = _appDbContext!.Users.Include(r => r.Bills).SingleOrDefault(r => r.Email == request.Username)!;
        }

        if (user == null)
        {
            return null!;
        }

        List<Bill> bills = new List<Bill>();
        if (request.Status == null)
        {
            bills = user.Bills!.ToList().FindAll(r => r.UserId == user.Id);
        }
        else
        {
            bills = user.Bills!.ToList().FindAll(r => r.UserId == user.Id && r.Status.ToString() == request.Status);
        }

        foreach (var item in bills)
        {
            var bill = _appDbContext.Bills.Include(r => r.Bill_Products).FirstOrDefault(r => r.Id == item.Id);
            if (bill != null)
            {
                var billDto = BillConverter.ToDto(bill);
                var shippingMethod = _appDbContext.ShippingMethods.SingleOrDefault(r => r.Id == bill.ShippingMethodId);
                billDto.ShippingMethod = shippingMethodConverter.ToDto(shippingMethod!);
                var address = _appDbContext.Addresses.SingleOrDefault(r => r.Id == bill.AddressId);
                billDto.Address = addressConverter.ToDto(address!);
                billDto.User = userConverter.ToDto(user);
                var billItems = SetBillItem(bill.Bill_Products!);
                billDto.BillItems = billItems;
                billDtos.Add(billDto);
            }
        }

        return billDtos;
    }
    public List<BillDto> GetBills()
    {
        List<BillDto> billDtos = new List<BillDto>();

        var bills = _appDbContext!.Bills.ToList();

        foreach (var bill in bills)
        {
            var billDto = BillConverter.ToDto(bill);
            var shippingMethod = _appDbContext.ShippingMethods.SingleOrDefault(r => r.Id == bill.ShippingMethodId);
            billDto.ShippingMethod = shippingMethodConverter.ToDto(shippingMethod!);
            var address = _appDbContext.Addresses.SingleOrDefault(r => r.Id == bill.AddressId);
            billDto.Address = addressConverter.ToDto(address!);
            var user = _appDbContext.Users.SingleOrDefault(r => r.Id == bill.UserId);
            billDto.User = userConverter.ToDto(user!);
            var billItems = SetBillItem(bill.Bill_Products!);
            billDto.BillItems = billItems;
            billDtos.Add(billDto);
        }

        return billDtos;
    }
    public async Task<string> ReOrderProducts(string code)
    {
        var bill = _appDbContext!.Bills
        .Include(r => r.User)
        .Include(r => r.Bill_Products)
        .SingleOrDefault(r => r.Code == code);
        if (bill!.Bill_Products != null)
        {
            foreach (var billProduct in bill.Bill_Products)
            {
                List<string> productClassifyCodes = new List<string>();
                var product = _appDbContext.Products.SingleOrDefault(r => r.Id == billProduct.ProductId);
                string[] productClassifyNames = billProduct.ProductClassifies!.Split(',');
                foreach (var item in productClassifyNames)
                {
                    var productClassify = _appDbContext.ProductClassifies.SingleOrDefault(r => r.Name == item && r.ProductId == product!.Id);
                    productClassifyCodes.Add(productClassify!.Code!);
                }

                var addCartDto = new AddCartItemDto
                {
                    Status = "1",
                    ProductCode = product!.Code!,
                    Quantity = billProduct.Quantity,
                    Username = bill.User!.Email != null ? bill.User!.Email : bill.User!.Phone!,
                    ProductClassifyCodes = productClassifyCodes
                };
                await cartFirestore.AddToCart(addCartDto);
            }
        }
        return "success";
    }
    public async Task<string> UpdateStatusBill(UpdateBillDto updateBillDto)
    {
        var bill = _appDbContext!.Bills.SingleOrDefault(r => r.Code == updateBillDto.Code);
        if (bill == null)
        {
            return null!;
        }
        bill.Status = updateBillDto.Status;
        _appDbContext.Bills.Update(bill);
        await _appDbContext.SaveChangesAsync();
        return "success";
    }
    //method ho tro
    public ICollection<Bill_Product> AddBill_Product(List<CartItem> cartItems, Bill bill)
    {
        ICollection<Bill_Product> result = new List<Bill_Product>();
        foreach (var item in cartItems)
        {
            var cartItem = _appDbContext!.CartItems.Include(r => r.ProductClassifies).SingleOrDefault(c => c.Id == item.Id);
            var billProduct = new Bill_Product
            {
                BillId = bill!.Id!,
                ProductId = cartItem!.ProductId!,
                ProductClassifies = GetStringProductClassify(cartItem),
                Quantity = cartItem.Quantity
            };
            result.Add(billProduct);
        }
        return result;
    }
    private string GetStringProductClassify(CartItem cartItem)
    {
        string result = "";
        var list = cartItem.ProductClassifies!.ToList();
        for (int i = 0; i < list.Count; i++)
        {
            if (i == list.Count - 1)
            {
                result += list[i].Name;
            }
            else
            {
                result += list[i].Name + ",";
            }
        }
        return result;
    }
    private List<BillItemDto> SetBillItem(ICollection<Bill_Product> billItems)
    {
        List<BillItemDto> billItemDtos = new List<BillItemDto>();
        foreach (var item in billItems)
        {
            BillItemDto billItemDto = new BillItemDto();
            var product = _appDbContext!.Products.Include(r => r.ProductImages).SingleOrDefault(r => r.Id == item.ProductId);
            var productDto = productConverter.ToDto(product!);
            var author = _appDbContext.Users.SingleOrDefault(r => r.Id == product!.AuthorId);
            productDto.Author = userConverter.ToDto(author!);
            billItemDto.Product = productDto;
            billItemDto.Product.Images = productFireStore.GetProductImage(product!.ProductImages!);
            billItemDto.ProductClassifies = item.ProductClassifies;
            billItemDto.Quantity = item.Quantity;
            billItemDtos.Add(billItemDto);
        }
        return billItemDtos;
    }
}
