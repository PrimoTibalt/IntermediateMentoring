using RestApi.Models;

namespace RestApi.Domain
{
    public static class Items
    {
        public static readonly List<Item> List = new()
        {
            new Item { Id = 1, Name = "Rubber" },
            new Item { Id = 2, Name = "Steel" },
            new Item { Id = 3, Name = "Wood" },
            new Item { Id = 4, Name = "Chair" },
            new Item { Id = 5, Name = "Wardrobe" },
            new Item { Id = 6, Name = "Table" },
            new Item { Id = 7, Name = "Armchair" },
            new Item { Id = 8, Name = "Bowl" },
            new Item { Id = 9, Name = "Plate" },
            new Item { Id = 10, Name = "Fork" },
            new Item { Id = 11, Name = "Spoon" },
            new Item { Id = 12, Name = "Knife" },
            new Item { Id = 13, Name = "Laptop" }
        };
    }
}
