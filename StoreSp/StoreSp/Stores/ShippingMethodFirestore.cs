using Google.Cloud.Firestore;
using StoreSp.Converters;
using StoreSp.Converters.request;
using StoreSp.Converters.response;
using StoreSp.Dtos.request;
using StoreSp.Dtos.response;
using StoreSp.Entities;

namespace StoreSp.Stores;

public class ShippingMethodFirestore(FirestoreDb firestoreDb) : FirestoreService(firestoreDb)
{
    public static string _collectionShippingMethod = "ShippingMethods";
    private readonly IBaseConverter<ShippingMethod, AddShippingMethodDto> createShippingMethod = new AddShippingMethodConverter();
    private readonly IBaseConverter<ShippingMethod, ShippingMethodDto> shippingMethodConverter = new ShippingMethodConverter();

    public Task Add(AddShippingMethodDto dto)
    {
        var db = _firestoreDb.Collection(_collectionShippingMethod);
        var shippingMethodDb = base.GetSnapshots(_collectionShippingMethod);
        var shippingMethod = createShippingMethod.ToEntity(dto);
        Random rnd = new Random();
        string randomCode = rnd.Next(1, 100000).ToString();
        while (shippingMethodDb.Documents.Select(r => r.ConvertTo<ShippingMethod>()).ToList().Find(r => r.Code == randomCode) != null)
        {
            randomCode = rnd.Next(1, 100000).ToString();
        }
        shippingMethod.Code = randomCode;
        return db.AddAsync(shippingMethod);
    }

    public Task<List<ShippingMethodDto>> GetAllShippingMethods()
    {
        var snapshot = base.GetSnapshots(_collectionShippingMethod);
        var shippingMethod = snapshot.Documents.Select(s => s.ConvertTo<ShippingMethod>()).ToList();
        return Task.FromResult(shippingMethod.Select(shippingMethodConverter.ToDto).ToList());
    }
}
