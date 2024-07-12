using Google.Cloud.Firestore;
using StoreSp.Converters;
using StoreSp.Converters.request;
using StoreSp.Converters.response;
using StoreSp.Dtos.request;
using StoreSp.Dtos.response;
using StoreSp.Entities;

namespace StoreSp.Stores;

public class CategoryFireStore(FirestoreDb firestoreDb) : FirestoreService(firestoreDb)
{
    private const string _collectionCategory = "Categories";
    private IBaseConverter<Category,CategoryDto> categoryConverter = new CategoryConverter();
    private IBaseConverter<Category,CreateCategoryDto> createCategoryConverter =  new CreateCategoryConverter();

    public Task Add(CreateCategoryDto categoryDto)
    {
        var categoryDb = _firestoreDb.Collection(_collectionCategory);
        var category = createCategoryConverter.ToEntity(categoryDto);
        return categoryDb.AddAsync(category);
    }

    public Task<List<CategoryDto>> GetAllCategories()
    {
        var snapshot = base.GetSnapshots(_collectionCategory);
        var category = snapshot.Documents.Select(s => s.ConvertTo<Category>()).ToList();
        return Task.FromResult(category.Select(categoryConverter.ToDto).ToList());
    }
}
