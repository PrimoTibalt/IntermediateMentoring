using Microsoft.AspNetCore.Mvc;
using RestApi.Models;
using RestApi.Services;

namespace RestApi.Controllers
{
    [Route("v1/item")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly ItemService _itemService;

        public ItemController()
        {
            _itemService = new ItemService();
        }

        [HttpGet(Name = nameof(GetItemList))]
        [ResponseCache(CacheProfileName = "Default")]
        public IActionResult GetItemList([FromQuery] int pageNum, int pageSize, int categoryId)
        {
            var items = _itemService.GetItems(pageNum, pageSize, categoryId);
            if (items.Any())
                return Ok(items);

            return NoContent();
        }

        [HttpGet("{itemId}", Name = nameof(GetItemById))]
        [ResponseCache(CacheProfileName = "Default")]
        public IActionResult GetItemById([FromRoute] int itemId)
        {
            var item = _itemService.Get(itemId);
            if (item is null)
                return NotFound();

            return Ok(item);
        }

        [HttpPost(Name = nameof(CreateItem))]
        public IActionResult CreateItem([FromBody] Item itemToCreate)
            => CreatedAtAction(nameof(GetItemById), new { itemId = _itemService.Add(itemToCreate) }, itemToCreate);

        [HttpPut("{itemId}", Name = nameof(UpdateItem))]
        public IActionResult UpdateItem([FromRoute] int itemId, [FromBody] Item itemToUpdate)
        {
            itemToUpdate.Id = itemId;
            _itemService.Update(itemToUpdate);
            return NoContent();
        }

        [HttpDelete("{itemId}", Name = nameof(DeleteItem))]
        public IActionResult DeleteItem([FromRoute] int itemId)
        {
            _itemService.Remove(itemId);
            return NoContent();
        }
    }
}
