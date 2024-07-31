using StoreSp.Dtos.response;
using StoreSp.Entities;

namespace StoreSp.Converters.response;

public class BillConverter : IBaseConverter<Bill, BillDto>
{
    BillDto IBaseConverter<Bill, BillDto>.ToDto(Bill entity)
    {
        return new BillDto
        {
            CreatedAt = entity.CreatedAt.ToDateTime().ToShortDateString(),
            Code = entity.Code!,
            Status = entity.Status,
            PaymentMethod = entity.PaymentMethod,
            TotalPrice = entity.TotalPrice,
            TotalProductPrice = entity.TotalProductPrice,
            Quantity = entity.Quantity
        };
    }

    Bill IBaseConverter<Bill, BillDto>.ToEntity(BillDto dto)
    {
        throw new NotImplementedException();
    }
}
