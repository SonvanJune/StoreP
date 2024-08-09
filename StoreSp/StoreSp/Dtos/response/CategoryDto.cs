
namespace StoreSp.Dtos.response;

public class CategoryDto
{
    public string? Id { get; set; }
    public required string Code{ get; set; }
    public required string Name { get; set; }
    public required string Image { get; set; }
    public required int Level { get; set; }
    public string? ParentCategoryId{ get; set; }
    public List<CategoryDto> Children{ get; set;} = new List<CategoryDto>();
}
