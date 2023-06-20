using RestApi.Domain;
using RestApi.Models;

namespace RestApi.Services
{
    public class ItemService
    {
        public Item? Get(int id) => Items.List.FirstOrDefault(i => i.Id == id);

        public List<Item> GetItems() => Items.List;

        public List<Item> GetItems(int pageNum, int pageSize, int categoryId)
        {
            const int DEFAULT_PAGE_SIZE = 3;

            List<Item> items;
            if (categoryId > 0)
                items = new CategoryService().Get(categoryId)?.Items ?? Items.List;
            else 
                items = Items.List;

            var size = pageSize > 0 ? pageSize : pageNum > 0 ? DEFAULT_PAGE_SIZE : items.Count;
            var startingPoint = pageNum > 0 ? (pageNum - 1) * size : 0;
            if (startingPoint > items.Count)
                return new List<Item>();

            return items.Take(new Range(startingPoint, startingPoint + size)).ToList();
        }

        public int Add(Item item)
        {
            _ = item ?? throw new ArgumentNullException(nameof(item));

            if (Items.List.Count == 0)
                item.Id = 1;
            else
                item.Id = Items.List.Max(item => item.Id) + 1;

            Items.List.Add(item);
            return item.Id;
        }

        public int Update(Item item)
        {
            var itemToUpdate = Items.List.FirstOrDefault(i => i.Id == item.Id) ?? throw new ArgumentException(nameof(item));
            itemToUpdate.Name = item.Name.Trim();
            return item.Id;
        }

        public int Remove(int id)
        {
            var itemToRemove = Items.List.FirstOrDefault(i => i.Id == id) ?? throw new ArgumentException(nameof(id));
            Items.List.Remove(itemToRemove);
            foreach (var category in new CategoryService().GetCategories())
            {
                var itemToRemoveFromCategory = category.Items.FirstOrDefault(i => i.Id == id);
                if (itemToRemoveFromCategory is not null)
                    category.Items.Remove(itemToRemoveFromCategory);
            }

            return itemToRemove.Id;
        }
    }
}
