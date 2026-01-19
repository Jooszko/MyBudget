using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBudget.Application.Interfaces;
using MyBudget.Dtos;
using MyBudget.Infrastructure.Identity;
using System.Security.Claims;

namespace MyBudget.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly TokenService _tokenService;
        private readonly IAccountService _accountService;

        public AccountController(UserManager<AppUser> userManager, TokenService tokenService, IAccountService accountService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _accountService = accountService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var result = await _accountService.LoginAsync(loginDto);

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            var result = await _accountService.RegisterAsync(registerDto);

            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var result = await _accountService.GetCurrentUserAsync(email);
            return Ok(result);
        }
    }
}
