using AvansMaaltijdreservering.Core.Domain.Entities;

namespace AvansMaaltijdreservering.Core.Domain.Tests.TestDataBuilders;

public static class ProductTestDataBuilder
{
    public static Product CreateRegularProduct(string name = "Regular Product")
    {
        return new Product
        {
            Id = 1,
            Name = name,
            ContainsAlcohol = false,
            PhotoUrl = "https://example.com/regular.jpg",
            Packages = new List<Package>()
        };
    }

    public static Product CreateAlcoholProduct(string name = "Alcoholic Product")
    {
        return new Product
        {
            Id = 2,
            Name = name,
            ContainsAlcohol = true,
            PhotoUrl = "https://example.com/alcohol.jpg",
            Packages = new List<Package>()
        };
    }

    public static Product WithName(this Product product, string name)
    {
        product.Name = name;
        return product;
    }

    public static Product WithAlcohol(this Product product, bool containsAlcohol = true)
    {
        product.ContainsAlcohol = containsAlcohol;
        return product;
    }
}