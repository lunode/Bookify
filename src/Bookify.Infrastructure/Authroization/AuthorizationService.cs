using Bookify.Application.Abstractions.Caching;
using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Authorization;

internal sealed class AuthorizationService(
    ApplicationDbContext dbContext,
    ICacheService cacheService)
{
    public async Task<UserRolesResponse> GetRolesForUserAsync(string identityId)
    {
        var cacheKey = $"auth:roles:{identityId}";
        var cacheRoles = await cacheService.GetAsync<UserRolesResponse>(cacheKey);
        if (cacheRoles is not null)
        {
            return cacheRoles;
        }
        var roles = await dbContext.Set<User>()
            .Where(u => u.IdentityId == identityId)
            .Select(u => new UserRolesResponse
            {
                UserId = u.Id,
                Roles = u.Roles.ToList()
            })
            .FirstAsync();
        await cacheService.SetAsync(cacheKey, roles);
        return roles;
    }

    public async Task<HashSet<string>> GetPermissionsForUserAsync(string identityId)
    {
        var cacheKey = $"auth:permissions:{identityId}";
        var cachePermissions = await cacheService.GetAsync<HashSet<string>>(cacheKey);

        if (cachePermissions is not null)
        {
            return cachePermissions;
        }
        var permissions = await dbContext.Set<User>()
            .Where(u => u.IdentityId == identityId)
            .SelectMany(u => u.Roles.Select(r => r.Permissions))
            .FirstAsync();

        var permissionSet = permissions.Select(p => p.Name).ToHashSet();

        await cacheService.SetAsync(cacheKey, permissionSet);

        return permissionSet;
    }
}