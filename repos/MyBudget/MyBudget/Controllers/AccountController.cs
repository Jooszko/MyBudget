using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBudget.Dtos;
using MyBudget.Infrastructure.Identity;
using MyBudget.Services;
using System.Security.Claims;

namespace MyBudget.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly TokenService _tokenService;

        public AccountController(UserManager<AppUser> userManager, TokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null) return Unauthorized();

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!result) return Unauthorized();

            return Ok(new UserDto
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await _userManager.Users.AnyAsync(x => x.UserName == registerDto.UserName))
                return BadRequest("Username is taken.");

            var user = new AppUser
            {
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                Currency = registerDto.Currency,
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
                return Ok(new UserDto
                {
                    UserName = user.UserName,
                    Token = _tokenService.CreateToken(user)
                });
            }

            return BadRequest("Problem with registering user");
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);

            return Ok(new UserDto
            {
                UserName = user.UserName,
                Currency = user.Currency,
                Token = _tokenService.CreateToken(user)
            });
        }
    }
}
