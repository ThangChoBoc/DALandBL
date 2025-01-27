using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ZelnyTrh.EF.DAL.Entities;
using ZelnyTrh.EF.DAL.Enums;

namespace ZelnyTrh.EF.DAL.Seeders;

public class DatabaseSeederLogger { }

public static class DatabaseSeeder
{
    public static async Task SeedAllAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DatabaseSeederLogger>>();

        try
        {
            logger.LogInformation("Starting database initialization and seeding...");

            // Begin transaction
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                // Seed roles and admin
                logger.LogInformation("Seeding roles and admin user...");
                await RoleSeeder.SeedRolesAndAdminAsync(serviceProvider);

                // Seed categories
                logger.LogInformation("Seeding crop categories...");
                var categories = await SeedCropCategoriesAsync(context, logger);
                await context.SaveChangesAsync();

                // Seed attributes
                logger.LogInformation("Seeding attribute definitions...");
                var attributes = await SeedAttributeDefinitionsAsync(context, categories, logger);
                await context.SaveChangesAsync();

                // Seed crops and their attributes
                logger.LogInformation("Seeding crops and attributes...");
                await SeedCropsAsync(context, categories, attributes, logger);
                await context.SaveChangesAsync();

                // Seed test offer
                logger.LogInformation("Seeding test offer...");
                await SeedOffersAsync(context, categories, attributes, logger);
                await context.SaveChangesAsync();

                // Commit transaction
                await transaction.CommitAsync();
                logger.LogInformation("Database seeding completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during seeding. Rolling back transaction.");
                await transaction.RollbackAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during database initialization.");
            throw;
        }
    }

    private static async Task<Dictionary<string, CropCategories>> SeedCropCategoriesAsync(
        ApplicationDbContext context,
        ILogger logger)
    {
        var categoryDict = new Dictionary<string, CropCategories>();

        logger.LogInformation("Creating main categories...");

        // Create main categories first
        var vegetables = new CropCategories
        {
            Id = "cat-vegetables",
            Name = "Vegetables",
            CropCategoryStatus = CropCategoryStatus.Approved,
            ParentCategoryId = null,
            AttributeDefinitions = [],
            ChildCategories = [],
            Crops = []
        };

        var fruits = new CropCategories
        {
            Id = "cat-fruits",
            Name = "Fruits",
            CropCategoryStatus = CropCategoryStatus.Approved,
            ParentCategoryId = null,
            AttributeDefinitions = [],
            ChildCategories = [],
            Crops = []
        };

        var herbs = new CropCategories
        {
            Id = "cat-herbs",
            Name = "Herbs",
            CropCategoryStatus = CropCategoryStatus.Approved,
            ParentCategoryId = null,
            AttributeDefinitions = [],
            ChildCategories = [],
            Crops = []
        };

        context.CropCategories.AddRange(vegetables, fruits, herbs);
        await context.SaveChangesAsync();

        categoryDict.Add(vegetables.Id, vegetables);
        categoryDict.Add(fruits.Id, fruits);
        categoryDict.Add(herbs.Id, herbs);

        logger.LogInformation("Creating subcategories...");

        // Create subcategories
        var rootVegetables = new CropCategories
        {
            Id = "cat-root-vegetables",
            Name = "Root Vegetables",
            CropCategoryStatus = CropCategoryStatus.Approved,
            ParentCategoryId = vegetables.Id,
            ParentCategory = vegetables,
            AttributeDefinitions = [],
            ChildCategories = [],
            Crops = []
        };

        var leafyGreens = new CropCategories
        {
            Id = "cat-leafy-greens",
            Name = "Leafy Greens",
            CropCategoryStatus = CropCategoryStatus.Approved,
            ParentCategoryId = vegetables.Id,
            ParentCategory = vegetables,
            AttributeDefinitions = [],
            ChildCategories = [],
            Crops = []
        };

        var berries = new CropCategories
        {
            Id = "cat-berries",
            Name = "Berries",
            CropCategoryStatus = CropCategoryStatus.Approved,
            ParentCategoryId = fruits.Id,
            ParentCategory = fruits,
            AttributeDefinitions = [],
            ChildCategories = [],
            Crops = []
        };

        var citrus = new CropCategories
        {
            Id = "cat-citrus",
            Name = "Citrus",
            CropCategoryStatus = CropCategoryStatus.Approved,
            ParentCategoryId = fruits.Id,
            ParentCategory = fruits,
            AttributeDefinitions = [],
            ChildCategories = [],
            Crops = []
        };

        var culinary = new CropCategories
        {
            Id = "cat-culinary",
            Name = "Culinary",
            CropCategoryStatus = CropCategoryStatus.Approved,
            ParentCategoryId = herbs.Id,
            ParentCategory = herbs,
            AttributeDefinitions = [],
            ChildCategories = [],
            Crops = []
        };

        context.CropCategories.AddRange(rootVegetables, leafyGreens, berries, citrus, culinary);
        await context.SaveChangesAsync();

        categoryDict.Add(rootVegetables.Id, rootVegetables);
        categoryDict.Add(leafyGreens.Id, leafyGreens);
        categoryDict.Add(berries.Id, berries);
        categoryDict.Add(citrus.Id, citrus);
        categoryDict.Add(culinary.Id, culinary);

        logger.LogInformation("Categories created successfully: {Categories}",
            string.Join(", ", categoryDict.Values.Select(c => c.Name)));

        return categoryDict;
    }

    private static async Task<Dictionary<string, AttributeDefinition>> SeedAttributeDefinitionsAsync(
        ApplicationDbContext context,
        Dictionary<string, CropCategories> categories,
        ILogger logger)
    {
        var attribDict = new Dictionary<string, AttributeDefinition>();
        var vegetables = categories["cat-vegetables"];

        logger.LogInformation("Creating attribute definitions...");

        var attributes = new List<AttributeDefinition>
        {
            new()
            {
                Id = "attr-growing-season",
                Name = "Growing Season",
                DataType = "string",
                IsRequired = true,
                CategoryId = vegetables.Id,
                Category = vegetables,
                ValidationRule = null,
                Unit = null,
                CropAttributes = []
            },
            new()
            {
                Id = "attr-water-needs",
                Name = "Water Needs",
                DataType = "string",
                IsRequired = true,
                CategoryId = vegetables.Id,
                Category = vegetables,
                ValidationRule = "Low|Medium|High",
                Unit = null,
                CropAttributes = []
            },
            new()
            {
                Id = "attr-days-to-harvest",
                Name = "Days to Harvest",
                DataType = "number",
                IsRequired = true,
                CategoryId = vegetables.Id,
                Category = vegetables,
                ValidationRule = "range:30-365",
                Unit = "days",
                CropAttributes = []
            }
        };

        foreach (var attr in attributes)
        {
            attribDict.Add(attr.Id, attr);
            vegetables.AttributeDefinitions.Add(attr);
        }

        context.AttributeDefinitions.AddRange(attributes);
        await context.SaveChangesAsync();

        logger.LogInformation("Attribute definitions created successfully: {Attributes}",
            string.Join(", ", attribDict.Values.Select(a => a.Name)));

        return attribDict;
    }

    private static async Task SeedCropsAsync(
        ApplicationDbContext context,
        Dictionary<string, CropCategories> categories,
        Dictionary<string, AttributeDefinition> attributeDefinitions,
        ILogger logger)
    {
        logger.LogInformation("Creating crops...");

        var rootVegetables = categories["cat-root-vegetables"];
        var leafyGreens = categories["cat-leafy-greens"];

        // Create crops with their categories
        var crops = new List<Crops>
        {
            new()
            {
                Id = "crop-carrot",
                Name = "Carrot",
                CategoryId = rootVegetables.Id,
                Category = rootVegetables,
                CropAttributes = [],
                Offers = []
            },
            new()
            {
                Id = "crop-lettuce",
                Name = "Lettuce",
                CategoryId = leafyGreens.Id,
                Category = leafyGreens,
                CropAttributes = [],
                Offers = []
            }
        };

        context.Crops.AddRange(crops);
        await context.SaveChangesAsync();

        logger.LogInformation("Creating crop attributes...");

        // Create crop attributes for carrot
        var carrot = crops[0];
        var carrotAttributes = new List<CropAttributes>
        {
            new()
            {
                Id = "attr-carrot-season",
                CropId = carrot.Id,
                Crop = carrot,
                AttributeDefinitionId = attributeDefinitions["attr-growing-season"].Id,
                AttributeDefinition = attributeDefinitions["attr-growing-season"],
                Value = "Spring, Fall"
            },
            new()
            {
                Id = "attr-carrot-water",
                CropId = carrot.Id,
                Crop = carrot,
                AttributeDefinitionId = attributeDefinitions["attr-water-needs"].Id,
                AttributeDefinition = attributeDefinitions["attr-water-needs"],
                Value = "Medium"
            },
            new()
            {
                Id = "attr-carrot-harvest",
                CropId = carrot.Id,
                Crop = carrot,
                AttributeDefinitionId = attributeDefinitions["attr-days-to-harvest"].Id,
                AttributeDefinition = attributeDefinitions["attr-days-to-harvest"],
                Value = "70"
            }
        };

        carrot.CropAttributes = carrotAttributes;
        context.CropAttributes.AddRange(carrotAttributes);
        await context.SaveChangesAsync();

        logger.LogInformation("Crops and attributes created successfully.");
    }

    private static async Task SeedOffersAsync(ApplicationDbContext context,
        Dictionary<string, CropCategories> categories,
        Dictionary<string, AttributeDefinition> attributeDefinitions,
        ILogger logger)
    {
        logger.LogInformation("Creating offers...");

        var user = await context.Users.FirstOrDefaultAsync();
        var lettuce = await context.Crops.FindAsync("crop-lettuce");
        var carrot = await context.Crops.FindAsync("crop-carrot");

        if (user == null || lettuce == null || carrot == null)
        {
            logger.LogWarning("User and/or crop not found");
            return;
        }


        var offers = new List<Offers>
        {
            new()
            {
                Id = "lettuce-offer-1",
                Name = "Salát 1",
                Price = 45,
                Currency = "Kč",
                Amount = 1,
                UnitsAvailable = 10,
                Origin = "Země",
                UserId = user.Id,
                User = user,
                OfferType = OfferType.Online,
                CropId = lettuce.Id,
                Crop = lettuce
            },
            new()
            {
                Id = "carrot-offer-2",
                Name = "Mrkev 2",
                Price = 35,
                Currency = "Kč",
                Amount = 10,
                UnitsAvailable = 70,
                Origin = "Země opravdu",
                UserId = user.Id,
                User = user,
                OfferType = OfferType.Online,
                CropId = carrot.Id,
                Crop = carrot
            }
        };

        // Check if offers already exist to avoid duplicates
        foreach (var offer in offers)
        {
            if (!await context.Offers.AnyAsync(o => o.Id == offer.Id))
            {
                await context.Offers.AddAsync(offer);
            }
        }

        await context.SaveChangesAsync();
        logger.LogInformation("Offers created successfully.");
    }
}

// Extension method for easy integration
public static class DatabaseSeederExtensions
{
    public static async Task SeedDatabaseAsync(this IServiceProvider serviceProvider)
    {
        await DatabaseSeeder.SeedAllAsync(serviceProvider);
    }
}