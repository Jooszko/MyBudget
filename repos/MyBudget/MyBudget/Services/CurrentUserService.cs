using MyBudget.Application.Interfaces;
using System.Security.Claims;

namespace MyBudget.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public Guid UserId
        {
            get
            {
                var userIdString = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if(!Guid.TryParse(userIdString, out var userId))
                    throw new UnauthorizedAccessException();

                return userId;
            }
        }
    }
}
