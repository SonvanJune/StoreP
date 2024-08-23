using Google.Cloud.Firestore;
using StoreSp.Dtos.request;
using StoreSp.Models;

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
            CreatedAt = DateTime.Now,
            Status = 0,
            PaymentMethod = dto.PaymentMethod
        };
    }
}
