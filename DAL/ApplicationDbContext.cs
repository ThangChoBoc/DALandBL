using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ZelnyTrh.EF.DAL.Entities;

namespace ZelnyTrh.EF.DAL
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<ApplicationUser> User { get; set; }
        public DbSet<AttributeDefinition> AttributeDefinitions { get; set; }
        public DbSet<CropAttributes> CropAttributes { get; set; }
        public DbSet<CropCategories> CropCategories { get; set; }
        public DbSet<Crops> Crops { get; set; }
        public DbSet<Offers> Offers { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<Reviews> Reviews { get; set; }
        public DbSet<SelfPickups> SelfPickups { get; set; }
        public DbSet<OrderOffers> OrderOffers { get; set; }
        public DbSet<SelfPickupRegistrations> SelfPickupRegistrations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Identity Tables
            modelBuilder.Entity<ApplicationUser>(b =>
            {
                // User Claims
                b.HasMany(e => e.Claims)
                    .WithOne()
                    .HasForeignKey(uc => uc.UserId)
                    .IsRequired();

                // User Logins
                b.HasMany(e => e.Logins)
                    .WithOne()
                    .HasForeignKey(ul => ul.UserId)
                    .IsRequired();

                // User Tokens
                b.HasMany(e => e.Tokens)
                    .WithOne()
                    .HasForeignKey(ut => ut.UserId)
                    .IsRequired();

                // User Roles
                b.HasMany(e => e.UserRoles)
                    .WithOne()
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            // Configure Identity Role
            modelBuilder.Entity<IdentityRole>(b =>
            {
                b.HasMany<IdentityRoleClaim<string>>()
                    .WithOne()
                    .HasForeignKey(rc => rc.RoleId)
                    .IsRequired();
            });

            // Configure ApplicationUser
            modelBuilder.Entity<ApplicationUser>(b =>
            {
                // Include navigation properties for roles
                b.HasMany(e => e.UserRoles)
                    .WithOne()
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            // Relation USER->OFFER
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(user => user.Offer)
                .WithOne(offer => offer.User)
                .HasForeignKey(offer => offer.UserId)
                .IsRequired();

            // Relation USER->ORDER
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(user => user.Orders)
                .WithOne(order => order.User)
                .HasForeignKey(order => order.UserId)
                .IsRequired();

            // Relation USER->REVIEW
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(user => user.Review)
                .WithOne(review => review.User)
                .HasForeignKey(review => review.UserId)
                .IsRequired();

            // Relation USER->SelfPickupRegistration
            modelBuilder.Entity<SelfPickups>()
                .HasOne(sp => sp.Offer)
                .WithMany(o => o.SelfPickups)
                .HasForeignKey(sp => sp.OfferId)
                .IsRequired();

            // Configure SelfPickupRegistrations (Many-to-Many)
            modelBuilder.Entity<SelfPickupRegistrations>()
                .HasKey(spr => spr.Id);

            modelBuilder.Entity<SelfPickupRegistrations>()
                .HasOne(spr => spr.SelfPickup)
                .WithMany(sp => sp.Registrations)
                .HasForeignKey(spr => spr.SelfPickupId)
                .IsRequired();

            modelBuilder.Entity<SelfPickupRegistrations>()
                .HasOne(spr => spr.User)
                .WithMany(u => u.SelfPickupRegistrations)
                .HasForeignKey(spr => spr.UserId)
                .IsRequired();

            // Relation OFFER->CROP
            modelBuilder.Entity<Offers>()
                .HasOne(o => o.Crop)
                .WithMany(c => c.Offers)
                .HasForeignKey(o => o.CropId)
                .IsRequired();

            modelBuilder.Entity<Offers>()
                .HasIndex(o => new { o.UserId, o.CropId })
                .IsUnique();

            // ENUM as string
            modelBuilder.Entity<Offers>()
                .Property(o => o.OfferType)
                .HasConversion<string>();

            modelBuilder.Entity<Orders>()
                .Property(o => o.PaymentType)
                .HasConversion<string>();

            modelBuilder.Entity<Orders>()
                .Property(o => o.OrderStatus)
                .HasConversion<string>();

            modelBuilder.Entity<CropCategories>()
                .Property(o => o.CropCategoryStatus)
                .HasConversion<string>();

            // InterJoin Relation ORDER->OFFER
            modelBuilder.Entity<OrderOffers>()
                .HasKey(oo => oo.Id);

            // Configure relationships
            modelBuilder.Entity<OrderOffers>()
                .HasOne(oo => oo.Order)
                .WithMany(o => o.OrderOffers)
                .HasForeignKey(oo => oo.OrderId)
                .IsRequired();

            modelBuilder.Entity<OrderOffers>()
                .HasOne(oo => oo.Offer)
                .WithMany(o => o.OrderOffers)
                .HasForeignKey(oo => oo.OfferId)
                .IsRequired();

            // Relation CROP->CROPCATEGORY
            modelBuilder.Entity<Crops>()
                .HasOne(c => c.Category)
                .WithMany(cc => cc.Crops)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CropCategories>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.ChildCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relation CROPCATEGORY->ATTRIBUTEDEFINITION
            modelBuilder.Entity<AttributeDefinition>()
                .HasOne(a => a.Category)
                .WithMany(c => c.AttributeDefinitions)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relation CROP->ATTRIBUTE
            modelBuilder.Entity<CropAttributes>()
                .HasOne(ca => ca.AttributeDefinition)
                .WithMany(ad => ad.CropAttributes)
                .HasForeignKey(ca => ca.AttributeDefinitionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CropAttributes>()
                .HasOne(ca => ca.Crop)
                .WithMany(c => c.CropAttributes)
                .HasForeignKey(ca => ca.CropId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            modelBuilder.Entity<AttributeDefinition>()
                .HasIndex(a => new { a.CategoryId, a.Name })
                .IsUnique();

            modelBuilder.Entity<CropAttributes>()
                .HasIndex(ca => new { ca.CropId, ca.AttributeDefinitionId })
                .IsUnique();
        }
    }
}