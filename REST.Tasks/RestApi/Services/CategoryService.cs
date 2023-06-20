using RestApi.Domain;
using RestApi.Models;

namespace RestApi.Services
{
    public class CategoryService
    {
        public Category? Get(int id) => Categories.List.FirstOrDefault(i => i.Id == id);

        public List<Category> GetCategories() => Categories.List;

        public int Add(Category category)
        {
            _ = category ?? throw new ArgumentNullException(nameof(category));

            if (Categories.List.Count == 0)
                category.Id = 1;
            else
                category.Id = Categories.List.Max(item => item.Id) + 1;

            Categories.List.Add(category);
            return category.Id;
        }

        public void Update(Category category)
        {
            var categoryToUpdate = Categories.List.FirstOrDefault(i => i.Id == category.Id) ?? throw new ArgumentException(nameof(category));
            categoryToUpdate.Name = category.Name.Trim();
            categoryToUpdate.Items = category.Items;
            var itemService = new ItemService();
            foreach (var item in categoryToUpdate.Items)
                if (itemService.Get(item.Id) is null)
                    itemService.Add(item);
        }

        public void Delete(int id)
        {
            var itemService = new ItemService();
            var categoryToRemove = Categories.List.FirstOrDefault(i => i.Id == id) ?? throw new ArgumentException(nameof(id));
            Categories.List.Remove(categoryToRemove);
            foreach (var item in categoryToRemove.Items)
                itemService.Remove(item.Id);
        }
    }
}
