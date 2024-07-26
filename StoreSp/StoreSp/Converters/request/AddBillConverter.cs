using Google.Cloud.Firestore;
using StoreSp.Dtos.request;
using StoreSp.Entities;

namespace StoreSp.Converters.response;

public class AddBillConverter : IBaseConverter<Bill, CreateBillDto>
{
    CreateBillDto IBaseConverter<Bill, CreateBillDto>.ToDto(Bill entity)
    {
        throw new NotImplementedException();
    }

    Bill IBaseConverter<Bill, CreateBillDto>.ToEntity(CreateBillDto dto)
    {
        return new Bill{
            CreatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)),
            Status = 0,
            ShippingUnit = dto.ShippingUnit,
            PaymentMethod = dto.PaymentMethod,
            ShippingUnitPrice = dto.ShippingUnitPrice
        };
    }
}
