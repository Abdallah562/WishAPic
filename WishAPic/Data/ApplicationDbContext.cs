using WishAPic.Identity;
using WishAPic.Models;

namespace WishAPic.Data
{
    public class ApplicationDbContext: 
        IdentityDbContext<ApplicationUser,ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):
            base(options)
        { }
        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);

        //    builder.Entity<ImageData>()
        //        .HasKey(imageData => new { imageData.UserId, imageData.Image });
        //}

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ImageData> Images { get; set; }
        //public DbSet<Favorites> Favorites { get; set; }

    }
}
