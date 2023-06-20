using Microsoft.AspNetCore.Mvc;
using RestApi.Models;
using RestApi.Services;

namespace RestApi.Controllers
{
    [Route("v1/category")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _categoryService;

        public CategoryController()
        {
            _categoryService = new CategoryService();
        }

        [HttpGet(Name = nameof(GetCategoryList))]
        [ResponseCache(CacheProfileName = "Default")]
        public IActionResult GetCategoryList()
            => Ok(_categoryService.GetCategories());

        [HttpGet("{categoryId}", Name = nameof(GetCategoryById))]
        [ResponseCache(CacheProfileName = "Default")]
        public IActionResult GetCategoryById([FromRoute] int categoryId)
        {
            var category = _categoryService.Get(categoryId);
            if (category is null)
                return NotFound();

            return Ok(category);
        }

        [HttpPost(Name = nameof(CreateCategory))]
        public IActionResult CreateCategory([FromBody] Category categoryToCreate)
            => CreatedAtAction(nameof(GetCategoryById), new { categoryId = _categoryService.Add(categoryToCreate) }, categoryToCreate);

        [HttpPut("{categoryId}", Name = nameof(UpdateCategory))]
        public IActionResult UpdateCategory([FromRoute] int categoryId, [FromBody] Category categoryToUpdate)
        {
            categoryToUpdate.Id = categoryId;
            _categoryService.Update(categoryToUpdate);
            return NoContent();
        }

        [HttpDelete("{categoryId}", Name = nameof(DeleteCategory))]
        public IActionResult DeleteCategory([FromRoute] int categoryId)
        {
            _categoryService?.Delete(categoryId);
            return NoContent();
        }
    }
}
