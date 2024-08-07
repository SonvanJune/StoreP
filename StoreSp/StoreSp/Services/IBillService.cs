using StoreSp.Dtos.request;

namespace StoreSp.Services;

public interface IBillService
{
    public IResult Checkout(CreateBillDto createBillDto);
    public IResult GetBillByUser(GetBillOfUserDto getBillOfUserDto);
    public IResult ReOrderProducts(string code);
}
