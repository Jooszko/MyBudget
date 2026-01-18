using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyBudget.Application.Dtos;
using MyBudget.Application.Interfaces;
using System.Security.Claims;

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
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userIdString))
                return Unauthorized();

            var userId= Guid.Parse(userIdString);

            var expense = await _expensesService.GetAllAsync(userId);
            return Ok(expense);

        }

        [HttpPost]
        public async Task<IActionResult> AddExpense(CreateExpenseDto dto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userIdString))
                return Unauthorized();

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var userId = Guid.Parse(userIdString);
            var expense = await _expensesService.AddAsync(userId, dto);
            return Ok(expense);
        }
    }
}
