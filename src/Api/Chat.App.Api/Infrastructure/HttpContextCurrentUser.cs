using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Chat.App.Application.Common;

namespace Chat.App.Api.Infrastructure;

public sealed class HttpContextCurrentUser(IHttpContextAccessor accessor) : ICurrentUser, ICurrentUserInitializer
{
    private Guid? _explicitUserId;

    public void SetUserId(Guid userId) => _explicitUserId = userId;

    public Guid? UserId
    {
        get
        {
            if (_explicitUserId.HasValue)
                return _explicitUserId;

            var user = accessor.HttpContext?.User;
            var id = user?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? user?.FindFirstValue(JwtRegisteredClaimNames.Sub);

            return Guid.TryParse(id, out var userId) ? userId : null;
        }
    }

    public bool IsInRole(string role) => accessor.HttpContext?.User?.IsInRole(role) ?? false;
}
