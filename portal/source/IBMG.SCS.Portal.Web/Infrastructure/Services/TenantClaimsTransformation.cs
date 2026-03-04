using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using PearDrop.Authentication.Multitenancy.Services;
using PearDrop.Multitenancy.Services.TenantIdentification.Contracts;

namespace IBMG.SCS.Portal.Web.Infrastructure.Services;

public class TenantClaimsTransformation : IClaimsTransformation
{
    private readonly UserPrincipalNameTenantResolver _tenantResolver;
    private readonly ITenantInfoProvider _tenantInfoProvider;
    private readonly ILogger<TenantClaimsTransformation> _logger;

    public TenantClaimsTransformation(
        UserPrincipalNameTenantResolver tenantResolver,
        ITenantInfoProvider tenantInfoProvider,
        ILogger<TenantClaimsTransformation> logger)
    {
        _tenantResolver = tenantResolver;
        _tenantInfoProvider = tenantInfoProvider;
        _logger = logger;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        // Skip if not authenticated
        if (principal.Identity is not { IsAuthenticated: true })
        {
            return principal;
        }

        // Skip if TenantId claim already exists (avoid duplicate transformation)
        if (principal.HasClaim(c => c.Type == "TenantId"))
        {
            return principal;
        }

        try
        {
            // Get the username from claims
            var userPrincipalName = principal.FindFirst(ClaimTypes.Name)?.Value
                                    ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                    ?? principal.FindFirst("preferred_username")?.Value;

            if (string.IsNullOrEmpty(userPrincipalName))
            {
                _logger.LogWarning("Cannot resolve tenant: no user identifier claim found");
                return principal;
            }

            // Use the tenant resolver to get the tenant identifier for this user
            var tenantIdentifier = await _tenantResolver.ResolveTenantFromUsername(userPrincipalName);

            if (string.IsNullOrEmpty(tenantIdentifier))
            {
                _logger.LogWarning("Failed to resolve tenant for user {User}", userPrincipalName);
                return principal;
            }

            // Look up the tenant by identifier to get the TenantId
            var tenantMaybe = await _tenantInfoProvider.GetByIdentifier(tenantIdentifier);

            if (!tenantMaybe.HasValue)
            {
                _logger.LogWarning("Tenant with identifier {Identifier} not found for user {User}",
                    tenantIdentifier, userPrincipalName);
                return principal;
            }

            var tenantId = tenantMaybe.Value.Id;

            // Add TenantId claim to the user's identity
            var claimsIdentity = (ClaimsIdentity)principal.Identity;
            claimsIdentity.AddClaim(new Claim("TenantId", tenantId));

            _logger.LogInformation("Added TenantId claim {TenantId} for user {User} (tenant: {TenantName})",
                tenantId, userPrincipalName, tenantMaybe.Value.Name);

            return principal;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error transforming claims for tenant resolution");
            return principal;
        }
    }
}