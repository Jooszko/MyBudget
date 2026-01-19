using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyBudget.Application.Common;
using MyBudget.Application.Interfaces;
using MyBudget.Dtos;
using MyBudget.Infrastructure.Data;
using MyBudget.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Infrastructure.Services
{
    public class AccountService : IAccountService
    {
        private readonly MyBudgetDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly TokenService _tokenService;
        public AccountService(MyBudgetDbContext context, UserManager<AppUser> userManager, TokenService tokenService)
        {
            _context = context;
            _userManager = userManager;
            _tokenService = tokenService;
        }
        public async Task<UserDto> GetCurrentUserAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            return (new UserDto
            {
                UserName = user.UserName,
                Currency = user.Currency,
                Token = _tokenService.CreateToken(user)
            });
        }

        public async Task<UserDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null) throw new NotFoundException("User not found");

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!result) throw new ForbiddenException("Bad password");

            return (new UserDto
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            });
        }

        public async Task<UserDto> RegisterAsync(RegisterDto registerDto)
        {
            if (await _userManager.Users.AnyAsync(x => x.UserName == registerDto.UserName))
                throw new ConflictException("Username is taken");


            var user = new AppUser
            {
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                Currency = registerDto.Currency,
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                var msg = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new ValidationException(msg);
            }

            return new UserDto
            {
                UserName = user.UserName!,
                Token = _tokenService.CreateToken(user)
            };
        }
    }
}
