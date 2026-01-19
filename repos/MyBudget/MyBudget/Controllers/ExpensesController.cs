using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyBudget.Application.Common;
using MyBudget.Application.Dtos.ExpenseDtos;
using MyBudget.Application.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyBudget.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExpensesController : ControllerBase
    {
        private readonly IExpensesService _expensesService;

        public ExpensesController(IExpensesService expensesService)
        {
            _expensesService = expensesService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllExpenses()
        {
            var userId = ReturnUserId();
            return Ok(await _expensesService.GetAllAsync(userId));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetExpenseById(Guid id)
        {
            var userId = ReturnUserId();
            return Ok(await _expensesService.GetAsync(userId, id));
        }

        [HttpPost]
        public async Task<IActionResult> AddExpense(CreateExpenseDto dto)
        {
            var userId = ReturnUserId();
            var created = await _expensesService.AddAsync(userId, dto);

            return CreatedAtAction(nameof(GetExpenseById), new { id = created.Id }, created);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteExpense(Guid id)
        {
            var userId = ReturnUserId();
            await _expensesService.DeleteAsync(userId, id);
            return NoContent();
        }

        private Guid ReturnUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out var userId))
                throw new UnauthorizedAccessException();

            return userId;
        }
    }

}
