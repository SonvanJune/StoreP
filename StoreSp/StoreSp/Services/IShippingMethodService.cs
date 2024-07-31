using StoreSp.Dtos.request;

namespace StoreSp.Services;

public interface IShippingMethodService
{
    public IResult AddShippingUnit(AddShippingMethodDto createShippingMethodDto);
    public IResult GetAllShippingMethods();
}
