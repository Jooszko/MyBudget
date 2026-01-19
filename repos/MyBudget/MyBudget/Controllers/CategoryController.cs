using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyBudget.Application.Dtos.CategoryDtos;
using MyBudget.Application.Interfaces;

namespace MyBudget.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ICurrentUserService _currentUserService;
        public CategoryController(ICategoryService categoryService, ICurrentUserService currentUserService )
        {
            _categoryService = categoryService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategory()
        {
            var userId = _currentUserService.UserId;
            var result = await _categoryService.GetAllAsync(userId);

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var userId = _currentUserService.UserId;
            var result = await _categoryService.GetAsync(userId, id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult>  AddCategory(CreateCategoryDto dto)
        {
            var userId = _currentUserService.UserId;
            var result = await _categoryService.AddAsync(userId, dto);
            return CreatedAtAction(nameof(GetCategoryById), new { id = result.Id }, result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var userId = _currentUserService.UserId;
            await _categoryService.DeleteAsync(userId, id);
            return NoContent();
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateCategory(Guid id, UpdateCategoryDto dto)
        {
            var userId = _currentUserService.UserId;
            var result = await _categoryService.UpdateAsync(userId, id, dto);
            return Ok(result);
        }
    }
}
