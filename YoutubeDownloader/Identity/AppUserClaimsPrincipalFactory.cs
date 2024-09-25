using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using YoutubeDownloader.Areas.Admin.Models;

namespace YoutubeDownloader.Identity;

/**<p>
 * <b>ClaimsPrincipal</b> is the object that the ASP.NET Core framework uses to represent the authenticated user.
 * Represents the current user, holding their identities and claims. It is central to authentication and authorization in ASP.NET.
 * </p>
 * <p>
 * <b>ClaimsPrincipalFactory</b> is a factory class responsible for creating ClaimsPrincipal objects.
 * In ASP.NET Core Identity, it is used to generate a ClaimsPrincipal from an existing user entity (e.g., an instance of the ApplicationUser class).
 * </p>
 * 
 * <a href="https://dev.to/pbouillon/understanding-identity-in-net-2169">More info about Claims</a>
 *
 * This principal factory allows hierarchical roles (All admins have )
 */
public class AppUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<User, IdentityRole>
{
    public AppUserClaimsPrincipalFactory(
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, roleManager, optionsAccessor)
    {
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        Console.WriteLine();
        Console.WriteLine();
        foreach (var claim in identity.Claims)
        {
            Console.WriteLine(claim);
        }
        Console.WriteLine();
        Console.WriteLine();

        if (identity.HasClaim(ClaimTypes.Role, "Admin"))
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, "Regular"));
        }

        return identity;
    }
}