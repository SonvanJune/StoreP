using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using StoreSp.Converters;
using StoreSp.Converters.request;
using StoreSp.Converters.response;
using StoreSp.Dtos.request;
using StoreSp.Dtos.response;
using StoreSp.Entities;

namespace StoreSp.Stores;

public class CategoryFireStore(FirestoreDb firestoreDb) : FirestoreService(firestoreDb)
{
    public static string _collectionCategory = "Categories";
    private IBaseConverter<Category, CategoryDto> categoryConverter = new CategoryConverter();
    private IBaseConverter<Category, CreateCategoryDto> createCategoryConverter = new CreateCategoryConverter();

    public int Add(CreateCategoryDto categoryDto)
    {
        var categoryDb = _firestoreDb.Collection(_collectionCategory);
        var db = base.GetSnapshots(_collectionCategory);
        var category = createCategoryConverter.ToEntity(categoryDto);
        var existCategory = db.Documents.Select(r => r.ConvertTo<Category>()).ToList().Find(r => r.Code == category.Code);
        if (existCategory != null)
        {
            return -1;

        }

        if (categoryDto.ParentCategoryCode != null)
        {
            var parent = db.Documents.Select(r => r.ConvertTo<Category>()).ToList().Find(r => r.Code == categoryDto.ParentCategoryCode);

            if (parent != null)
            {
                category.ParentCategoryId = parent.Id!;
                category.ParentCategory = parent;
                category.Level = parent.Level + 1;
            }
            else
            {
                return 0;
            }
        }


        categoryDb.AddAsync(category);
        return 1;
    }

    public List<CategoryDto> GetAllCategories()
    {
        var snapshot = base.GetSnapshots(_collectionCategory);
        var categories = snapshot.Documents.Select(s => s.ConvertTo<Category>()).ToList();
        List<CategoryDto> result = new List<CategoryDto>();

        var categoryDtos = new List<CategoryDto>();
        int high = 0 ;
        
        foreach (var category in categories)
        {
            if(high <= category.Level){
                high = category.Level;
            }
            categoryDtos.Add(categoryConverter.ToDto(category));
        }
        
        for (int i = 0; i < high; i++)
        {
            for (int j = 0; j < categoryDtos.Count; j++)
            {
                if(categoryDtos[j].ParentCategoryId == null && categoryDtos[j].Level == i ){
                    result.Add(categoryDtos[j]);
                    List<CategoryDto> arr = categoryDtos.FindAll(c => c.ParentCategoryId == categoryDtos[j].Id);
                    categoryDtos[j].Children = arr;
                    break;
                }
                
                if(categoryDtos[j].ParentCategoryId != null && categoryDtos[j].Level == i ){
                    List<CategoryDto> arr = categoryDtos.FindAll(c => c.ParentCategoryId == categoryDtos[j].Id);
                    categoryDtos[j].Children = arr;
                }
            }
        }
        return result;
    }

}
