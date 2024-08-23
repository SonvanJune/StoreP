using Microsoft.EntityFrameworkCore;
using StoreSp.Models;
using StoreSp.Services.Impl;
using StoreSp.Stores;

namespace StoreSp.Context;

public class AppDbContext : DbContext
{
    public static AppDbContext? _dbcontext;
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Log> Logs { get; set; }
    public DbSet<ShippingMethod> ShippingMethods { get; set; }
    public DbSet<Bill> Bills { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Bill_Product> Bill_Product { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<ProductClassify> ProductClassifies { get; set; }
    public DbSet<Banner> Banners { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Boxchat> Boxchats { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Cart> Carts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //1 role many users
        modelBuilder.Entity<Role>()
        .HasMany(u => u.Users)
        .WithOne(r => r.Role)
        .HasForeignKey(r => r.RoleId);

        //1 user many notifications
        modelBuilder.Entity<User>()
        .HasMany(u => u.Notifications)
        .WithOne(r => r.User)
        .HasForeignKey(r => r.UserId);

        //1 user many logs
        modelBuilder.Entity<User>()
        .HasMany(u => u.Logs)
        .WithOne(r => r.User)
        .HasForeignKey(r => r.UserId);

        //many user many address
        modelBuilder.Entity<User>()
            .HasMany(u => u.Addresses)
            .WithMany(a => a.Users)
            .UsingEntity(j => j.ToTable("Address_User"));

        //1 user more bills
        modelBuilder.Entity<User>()
        .HasMany(u => u.Bills)
        .WithOne(r => r.User)
        .HasForeignKey(r => r.UserId);

        //1 address more bills
        modelBuilder.Entity<Address>()
       .HasMany(u => u.Bills)
       .WithOne(r => r.Address)
       .HasForeignKey(r => r.AddressId);

        //1 shipping method more bills
        modelBuilder.Entity<ShippingMethod>()
       .HasMany(u => u.Bills)
       .WithOne(r => r.ShippingMethod)
       .HasForeignKey(r => r.ShippingMethodId);

        //many bills many products
        modelBuilder.Entity<Bill_Product>()
             .HasKey(sc => new { sc.BillId, sc.ProductId });

        modelBuilder.Entity<Bill_Product>()
            .HasOne(sc => sc.Bill)
            .WithMany(s => s.Bill_Products)
            .HasForeignKey(sc => sc.BillId);

        modelBuilder.Entity<Bill_Product>()
            .HasOne(sc => sc.Product)
            .WithMany(c => c.Bill_Products)
            .HasForeignKey(sc => sc.ProductId);

        //many products many categories
        modelBuilder.Entity<Product>()
            .HasMany(u => u.Categories)
            .WithMany(a => a.Products)
            .UsingEntity(j => j.ToTable("Category_Product"));

        //1 category 1 category
        modelBuilder.Entity<Category>()
            .HasOne(c => c.ParentCategory)
            .WithMany(p => p.ChildrenCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        //1 product many image
        modelBuilder.Entity<User>()
            .HasMany(u => u.ProductSells)
            .WithOne(r => r.Author)
            .HasForeignKey(r => r.AuthorId);

        //1 user many product
        modelBuilder.Entity<Product>()
            .HasMany(u => u.ProductImages)
            .WithOne(r => r.Product)
            .HasForeignKey(r => r.ProductId);

        //1 product many classifies
        modelBuilder.Entity<Product>()
            .HasMany(u => u.ProductClassifies)
            .WithOne(r => r.Product)
            .HasForeignKey(r => r.ProductId);

        //1 user 1 cart
        modelBuilder.Entity<User>()
            .HasOne(u => u.Cart)
            .WithOne(c => c.User)
            .HasForeignKey<Cart>(c => c.UserId);

        //1 cart many cartitem
        modelBuilder.Entity<Cart>()
            .HasMany(u => u.Items)
            .WithOne(r => r.Cart)
            .HasForeignKey(r => r.CartId);

        //many cartitem many productClassify
        modelBuilder.Entity<CartItem>()
            .HasMany(u => u.ProductClassifies)
            .WithMany(a => a.Items)
            .UsingEntity(j => j.ToTable("CartItem_ProductClassify"));

        //many user many product (like)
        modelBuilder.Entity<Like>()
             .HasKey(sc => new { sc.UserId, sc.ProductId });

        modelBuilder.Entity<Like>()
            .HasOne(sc => sc.User)
            .WithMany(s => s.Likes)
            .HasForeignKey(sc => sc.UserId);

        modelBuilder.Entity<Like>()
            .HasOne(sc => sc.Product)
            .WithMany(c => c.Likes)
            .HasForeignKey(sc => sc.ProductId);

        //1 boxchat many messages
        modelBuilder.Entity<Boxchat>()
            .HasMany(u => u.Messages)
            .WithOne(r => r.Boxchat)
            .HasForeignKey(r => r.BoxchatId);

        //1 boxchat many users
        modelBuilder.Entity<Boxchat>()
            .HasMany(u => u.Users)
            .WithMany(a => a.Boxchats)
            .UsingEntity(j => j.ToTable("Boxchat_User"));

        //1 user many message sender
        modelBuilder.Entity<User>()
            .HasMany(u => u.SendMessages)
            .WithOne(r => r.Sender)
            .HasForeignKey(r => r.SenderId);

        //1 user many message receiver
        modelBuilder.Entity<User>()
            .HasMany(u => u.ReceivMessages)
            .WithOne(r => r.Receiver)
            .HasForeignKey(r => r.ReceiverId);
    }

    public static AppDbContext GetInstance()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseMySql("server=localhost;database=speed;User=root;Password=;",
            ServerVersion.AutoDetect("server=localhost;database=speed;User=root;Password=;"));

        _dbcontext = new AppDbContext(optionsBuilder.Options);
        return _dbcontext;
    }
}


