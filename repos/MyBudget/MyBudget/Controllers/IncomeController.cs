using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyBudget.Application.Dtos.IncomeDtos;
using MyBudget.Application.Interfaces;

namespace MyBudget.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class IncomeController : ControllerBase
    {
        private readonly IIncomeService _incomeService;
        private readonly ICurrentUserService _currentUserService;
        public IncomeController(IIncomeService incomeService, ICurrentUserService currentUserService)
        {
            _incomeService = incomeService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllIncomes([FromQuery] IncomeQueryParamsDto q)
        {
            var userId = _currentUserService.UserId;
            var result = await _incomeService.GetAllAsync(userId, q);

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetIncomeById(Guid id)
        {
            var userId = _currentUserService.UserId;
            var result = await _incomeService.GetAsync(userId, id);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddIncome(CreateIncomeDto dto)
        {
            var userId = _currentUserService.UserId;
            var result = await _incomeService.AddAsync(userId, dto);
            return CreatedAtAction(nameof(GetIncomeById), new { id = result.Id }, result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteIncome(Guid id)
        {
            var userId = _currentUserService.UserId;
            await _incomeService.DeleteAsync(userId, id);
            return NoContent();
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateIncome(Guid id, UpdateIncomeDto dto)
        {
            var userId = _currentUserService.UserId;
            var result = await _incomeService.UpdateAsync(userId, id, dto);
            return Ok(result);
        }

    }
}
