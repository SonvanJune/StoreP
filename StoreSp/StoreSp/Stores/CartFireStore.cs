using Microsoft.EntityFrameworkCore;
using StoreSp.Context;
using StoreSp.Converters;
using StoreSp.Converters.request;
using StoreSp.Converters.response;
using StoreSp.Dtos.request;
using StoreSp.Dtos.response;
using StoreSp.Models;

namespace StoreSp.Stores;

public class CartFireStore
{
    private readonly AppDbContext? _appDbContext = null;

    public CartFireStore()
    {
        _appDbContext = AppDbContext.GetInstance();
    }

    //Properties
    private readonly IBaseConverter<CartItem, AddCartItemDto> addCartItemConverter = new AddCartItemConverter();
    public readonly IBaseConverter<User, UserDto> userConverter = new UserConverter();
    private readonly IBaseConverter<Product, ProductDto> productConverter = new ProductConverter();
    private readonly IBaseConverter<CartItem, CartItemDto> cartItemConverter = new CartItemConverter();

    //Method chinh
    public async Task<string> AddToCart(AddCartItemDto itemDto)
    {
        CartItem item = null!;

        User user = null!;
        if (_appDbContext!.Users.SingleOrDefault(r => r.Email == itemDto.Username) == null)
        {
            user = _appDbContext!.Users
            .Include(r => r.Cart!.Items)
            .SingleOrDefault(r => r.Phone == itemDto.Username)!;
        }
        else
        {
            user = _appDbContext!.Users
            .Include(r => r.Cart!.Items)
            .SingleOrDefault(r => r.Email == itemDto.Username)!;
        }

        if (user == null)
        {
            return null!;
        }

        var product = _appDbContext!.Products.SingleOrDefault(r => r.Code == itemDto.ProductCode);
        if (product == null)
        {
            return null!;
        }

        //check cart item of the classify exist if the classify only one
        int idCartItemExist = CheckExistCartItem(product, user.Cart!, itemDto.ProductClassifyCodes!);
        if (idCartItemExist != -1)
        {
            //Update cart item of the classify of product input
            item = _appDbContext.CartItems.SingleOrDefault(r => r.Id == idCartItemExist)!;
            int oldTotal = item.Total;
            int newQuantity = itemDto.Quantity + item.Quantity;
            int total = item.Price * newQuantity;
            item.Quantity = newQuantity;
            item.Total = total;

            _appDbContext.CartItems.Update(item);
            await _appDbContext.SaveChangesAsync();

            //change status when add again
            await UpdateWhenAddAgain(user.Cart!, item, oldTotal);
        }
        else
        {
            //Add to cart item of the classify of product input
            //set cart item
            item = addCartItemConverter.ToEntity(itemDto);

            item.ProductId = product.Id!;
            item.CartId = user!.Cart!.Id;
            item.Cart = user!.Cart;
            item.Product = product;

            //get the highets increase percent
            int highIncreasePercent = 0;
            foreach (var code in itemDto.ProductClassifyCodes!)
            {
                var productClassify = _appDbContext.ProductClassifies.SingleOrDefault(r => r.Code == code);
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
            while (_appDbContext.CartItems.SingleOrDefault(r => r.Code == randomCode) != null)
            {
                randomCode = rnd.Next(1, 100000).ToString();
            }
            item.Code = randomCode;
            item.ProductClassifies = AddCartItem_ProductClassifies(itemDto.ProductClassifyCodes);

            user.Cart.Items!.Add(item);
            _appDbContext.Users.Update(user);
            await _appDbContext.SaveChangesAsync();
        }

        return Convert.ToString(idCartItemExist);
    }

    public async Task<CartItem> CheckoutItemInCart(string code)
    {
        var cartItem = _appDbContext!.CartItems
        .Include(r => r.Cart!.User)
        .SingleOrDefault(r => r.Code == code);
        if (cartItem != null)
        {
            int status = cartItem.Status == 0 ? 1 : 0;
            cartItem.Status = status;
            int totalAfterAdd = cartItem.Total;

            // add total for cart
            if (status == 1)
            {
                totalAfterAdd += cartItem.Cart!.TotalPrice;
            }
            else
            {
                totalAfterAdd = cartItem.Cart!.TotalPrice - totalAfterAdd;
            }

            cartItem.Cart.TotalPrice = totalAfterAdd;
            _appDbContext!.CartItems.Update(cartItem);
            await _appDbContext!.SaveChangesAsync();
            return cartItem;
        }
        return null!;
    }

    public async Task<CartDto> GetCartByUser(string username)
    {
        CartDto cartDto = new CartDto();

        //find user
        User user = null!;
        if (_appDbContext!.Users.SingleOrDefault(r => r.Email == username) == null)
        {
            user = _appDbContext!.Users
            .Include(r => r.Cart!.Items)
            .SingleOrDefault(r => r.Phone == username)!;
        }
        else
        {
            user = _appDbContext!.Users
            .Include(r => r.Cart!.Items)
            .SingleOrDefault(r => r.Email == username)!;
        }

        if (user == null)
        {
            return null!;
        }

        cartDto.TotalPrice = user.Cart!.TotalPrice;

        //tao list cartitem dto
        List<CartItemDto> cartItemDtos = await GetCartItemDtos(user.Cart.Items!);
        cartDto.Items = cartItemDtos;
        cartDto.Quantity = cartItemDtos.Count;

        return cartDto;
    }

    public async Task<string> UpdateCartByUser(UpdateCartDto updateCartDto)
    {

        User user = null!;
        if (_appDbContext!.Users.SingleOrDefault(r => r.Email == updateCartDto.Username) == null)
        {
            user = _appDbContext!.Users.Include(r => r.Cart!.Items).SingleOrDefault(r => r.Phone == updateCartDto.Username)!;
        }
        else
        {
            user = _appDbContext!.Users.Include(r => r.Cart!.Items).SingleOrDefault(r => r.Email == updateCartDto.Username)!;
        }

        int totalAfterAdd = user.Cart!.TotalPrice;
        //update cart items
        foreach (var item in updateCartDto.UpdateCartItems!)
        {
            var cartItem = _appDbContext.CartItems.Include(r => r.ProductClassifies).SingleOrDefault(r => r.Code == item.ItemCode);
            if (Convert.ToInt32(item.Quantity) <= 0 && item.Quantity != null)
            {
                cartItem!.ProductClassifies = [];
                _appDbContext.CartItems.Update(cartItem);
                user.Cart.Items!.Remove(cartItem);
                continue;
            }
            if (cartItem != null)
            {
                if (item.Quantity != null)
                {
                    totalAfterAdd = totalAfterAdd - cartItem.Total;
                    int quantity = Convert.ToInt32(item.Quantity);
                    int total = Convert.ToInt32(item.Quantity) * cartItem.Price;
                    totalAfterAdd += total;
                    cartItem.Quantity = quantity;
                    cartItem.Total = total;
                    _appDbContext.CartItems.Update(cartItem);
                }

                //update cartItem Product_Classiffy
                if (item.ClassifyCodes != null)
                {
                    cartItem.ProductClassifies = [];
                    for (int i = 0; i < item.ClassifyCodes.Count; i++)
                    {
                        var productClassify = _appDbContext.ProductClassifies.SingleOrDefault(r => r.Code == item.ClassifyCodes[i])!;
                        cartItem.ProductClassifies.Add(productClassify);
                    }
                    _appDbContext.CartItems.Update(cartItem);
                }
            }
        }

        //update cart
        user.Cart.TotalPrice = totalAfterAdd;
        _appDbContext.Carts.Update(user.Cart);
        await _appDbContext.SaveChangesAsync();
        return null!;
    }

    //method ho tro
    private List<ProductClassify> AddCartItem_ProductClassifies(List<string> productClassifyCodes)
    {
        List<ProductClassify> productClassifies = new List<ProductClassify>();
        foreach (var str in productClassifyCodes)
        {
            var productClassify = _appDbContext!.ProductClassifies.SingleOrDefault(x => x.Code == str);
            if (productClassify != null)
            {
                productClassifies.Add(productClassify);
            }
        }
        return productClassifies;
    }

    private int CheckExistCartItem(Product product, Cart cart, List<string> productClassifyCodes)
    {
        int status = -1;
        var list = cart.Items!.ToList();
        List<CartItem> cartItem_ProductClassifies_list = new List<CartItem>();

        var existCartItems = list!.FindAll(r => r.ProductId == product.Id);
        if (existCartItems.Count != 0)
        {
            foreach (var it in existCartItems)
            {
                // List<CartItem_ProductClassify> cartItem_ProductClassifies =
                // cartItem_ProductClassifyDb.Documents.Select(r => r.ConvertTo<CartItem_ProductClassify>()).ToList().FindAll(r => r.CartItem_Id == it.Id);
                // cartItem_ProductClassifies_list.Add(cartItem_ProductClassifies);
                var cartItem_ProductClassifies = _appDbContext!.CartItems
                .Include(r => r.ProductClassifies)
                .Where(i => i.Id == it.Id).ToList();
                cartItem_ProductClassifies_list = cartItem_ProductClassifies;
            }

            for (int i = 0; i < cartItem_ProductClassifies_list.Count; i++)
            {
                List<string> temp = new List<string>();
                var arr = cartItem_ProductClassifies_list[i].ProductClassifies!.ToList();
                for (int j = 0; j < arr.Count; j++)
                {
                    temp.Add(arr[j].Code!);
                }
                if (CompareArray(temp, productClassifyCodes))
                {
                    status = cartItem_ProductClassifies_list[i].Id;
                    break;
                }
            }
        }
        return status;
    }

    private async Task<List<CartItemDto>> GetCartItemDtos(ICollection<CartItem> cartItems)
    {
        List<CartItemDto> cartItemDtos = new List<CartItemDto>();
        foreach (var cartItem in cartItems)
        {
            var item = _appDbContext!.CartItems
            .Include(r => r.ProductClassifies)
            .Include(r => r.Product!.Author)
            .Include(r => r.Product!.ProductClassifies)
            .SingleOrDefault(r => r.Id == cartItem.Id);
            if (item!.Quantity == 0)
            {
                _appDbContext!.CartItems.Remove(item!);
                await _appDbContext!.SaveChangesAsync();
            }
            else
            {
                CartItemDto cartItemDto = cartItemConverter.ToDto(cartItem);
                //get product for cart item
                cartItemDto.Product = productConverter.ToDto(item.Product!);

                //get author for cart item
                cartItemDto.Shop = userConverter.ToDto(item.Product!.Author!);

                //get string product classify for cart item
                cartItemDto.options = GetOptionClassifyDtos(item.Product!.ProductClassifies!);
                cartItemDto.CartItem_ProductClassifies = GetStringProductClassify(item.ProductClassifies!, false);
                cartItemDto.CartItem_ProductClassifyCodes = GetStringProductClassify(item.ProductClassifies!, true);
                //them vao mang
                cartItemDtos.Add(cartItemDto);
            }
        }
        return cartItemDtos;
    }

    private string GetStringProductClassify(ICollection<ProductClassify> productClassifies, bool GetCode)
    {
        string result = "";
        var list = productClassifies.ToList();
        for (int i = 0; i < list.Count; i++)
        {
            if (i == list.Count - 1)
            {
                if (GetCode == true)
                {
                    result += list[i].Code;
                }
                else
                {
                    result += list[i].Name;

                }
            }
            else
            {
                if (GetCode == true)
                {
                    result += list[i].Code + ",";
                }
                else
                {
                    result += list[i].Name + ",";
                }
            }
        }
        return result;
    }
    private List<OptionClassifyDto> GetOptionClassifyDtos(ICollection<ProductClassify> productClassifies)
    {
        List<OptionClassifyDto> optionClassifyDtos = new List<OptionClassifyDto>();
        foreach (var item in productClassifies)
        {
            OptionClassifyDto optionClassifyDto = new OptionClassifyDto
            {
                Code = item.Code!,
                Name = item.Name,
                GroupName = item.GroupName,
                Image = item.Image,
                Quantity = item.Quantity
            };
            optionClassifyDtos.Add(optionClassifyDto);
        }
        return optionClassifyDtos;
    }
    private bool CompareArray(List<string> a, List<string> b)
    {
        if (a.Count != b.Count)
            return false;
        return !a.Except(b).Any() && !b.Except(a).Any();
    }

    private async Task UpdateWhenAddAgain(Cart cart, CartItem cartItem, int oldTotal)
    {
        if (cartItem != null)
        {
            if (cartItem.Status == 1)
            {
                cartItem.Status = 0;
                int total = cart.TotalPrice - oldTotal;
                cart.TotalPrice = total;
                _appDbContext!.Carts.Update(cart);
                _appDbContext!.CartItems.Update(cartItem);
                await _appDbContext!.SaveChangesAsync();
            }
        }
    }
}
