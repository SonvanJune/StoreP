namespace StoreSp.Models;

public class Category
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public required string Name { get; set; }

    public required string Code { get; set; }

    public int Level { get; set; }
    
    public string Avatar { get; set; } = null!;

    public ICollection<Product>? Products { get; set; }

    public int ParentCategoryId { get; set; }

    public Category? ParentCategory { get; set; }
    public ICollection<Category>? ChildrenCategories { get; set; }
}
