using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Enums;

namespace AvansMaaltijdreservering.Core.Domain.Tests.TestDataBuilders;

public static class PackageTestDataBuilder
{
    public static Package CreateRegularPackage(string name = "Regular Package")
    {
        return new Package
        {
            Id = 1,
            Name = name,
            City = City.BREDA,
            CanteenLocation = CanteenLocation.BREDA_LA_BUILDING,
            PickupTime = DateTime.Today.AddDays(1).AddHours(12), // Tomorrow at noon
            LatestPickupTime = DateTime.Today.AddDays(1).AddHours(14), // Tomorrow at 2 PM
            Price = 5.95m,
            MealType = MealType.Lunch,
            CanteenId = 1,
            Products = new List<Product>
            {
                ProductTestDataBuilder.CreateRegularProduct("Sandwich"),
                ProductTestDataBuilder.CreateRegularProduct("Orange Juice")
            }
        };
    }

    public static Package CreateAlcoholPackage(string name = "Wine Package")
    {
        return new Package
        {
            Id = 2,
            Name = name,
            City = City.BREDA,
            CanteenLocation = CanteenLocation.BREDA_LA_BUILDING,
            PickupTime = DateTime.Today.AddDays(1).AddHours(18), // Tomorrow at 6 PM
            LatestPickupTime = DateTime.Today.AddDays(1).AddHours(20), // Tomorrow at 8 PM
            Price = 15.95m,
            MealType = MealType.WarmEveningMeal,
            CanteenId = 1,
            Products = new List<Product>
            {
                ProductTestDataBuilder.CreateRegularProduct("Salmon"),
                ProductTestDataBuilder.CreateAlcoholProduct("Red Wine")
            }
        };
    }

    public static Package CreateReservedPackage(string name = "Reserved Package", int reservedByStudentId = 1)
    {
        var package = CreateRegularPackage(name);
        package.Id = 3;
        package.ReservedByStudentId = reservedByStudentId;
        return package;
    }

    public static Package CreateFuturePackage(int daysFromNow = 1, string name = "Future Package")
    {
        var package = CreateRegularPackage(name);
        package.Id = 4;
        package.PickupTime = DateTime.Today.AddDays(daysFromNow).AddHours(12);
        package.LatestPickupTime = DateTime.Today.AddDays(daysFromNow).AddHours(14);
        return package;
    }

    public static Package CreatePastPackage(string name = "Past Package")
    {
        var package = CreateRegularPackage(name);
        package.Id = 5;
        package.PickupTime = DateTime.Today.AddDays(-1).AddHours(12); // Yesterday
        package.LatestPickupTime = DateTime.Today.AddDays(-1).AddHours(14);
        return package;
    }

    public static Package CreateWarmMealPackage(CanteenLocation location = CanteenLocation.BREDA_LA_BUILDING)
    {
        return new Package
        {
            Id = 6,
            Name = "Warm Meal Package",
            City = City.BREDA,
            CanteenLocation = location,
            PickupTime = DateTime.Today.AddDays(1).AddHours(17),
            LatestPickupTime = DateTime.Today.AddDays(1).AddHours(19),
            Price = 8.50m,
            MealType = MealType.WarmEveningMeal,
            CanteenId = 1,
            Products = new List<Product>
            {
                ProductTestDataBuilder.CreateRegularProduct("Hot Pasta")
            }
        };
    }

    public static Package WithPickupTime(this Package package, DateTime pickupTime)
    {
        package.PickupTime = pickupTime;
        package.LatestPickupTime = pickupTime.AddHours(2);
        return package;
    }

    public static Package WithPrice(this Package package, decimal price)
    {
        package.Price = price;
        return package;
    }

    public static Package WithProducts(this Package package, params Product[] products)
    {
        package.Products = products.ToList();
        return package;
    }

    public static Package WithReservation(this Package package, int studentId)
    {
        package.ReservedByStudentId = studentId;
        return package;
    }
}