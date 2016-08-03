using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using PHDS.Identity.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PHDS.Identity.BLL
{
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager) :
            base(userManager, authenticationManager)
        { }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }

        private async Task<SignInStatus> SignInOrTwoFactor(ApplicationUser user, bool isPersistent)
        {
            var id = Convert.ToString(user.Id);
            if (await UserManager.GetTwoFactorEnabledAsync(user.Id)
                && (await UserManager.GetValidTwoFactorProvidersAsync(user.Id)).Count > 0
                && !await AuthenticationManager.TwoFactorBrowserRememberedAsync(id))
            {
                var identity = new ClaimsIdentity(DefaultAuthenticationTypes.TwoFactorCookie);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, id));
                AuthenticationManager.SignIn(identity);
                return SignInStatus.RequiresVerification;
            }
            await SignInAsync(user, isPersistent, false);
            return SignInStatus.Success;
        }

        public override async Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
        {
            if (UserManager == null)
            {
                return SignInStatus.Failure;
            }
            ApplicationUser user;
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(userName))
            {
                user = await UserManager.FindByEmailAsync(userName);
            }
            else
            {
                user = await UserManager.FindByNameAsync(userName);
            }

            if (user == null)
            {
                return SignInStatus.Failure;
            }
            if (await UserManager.IsLockedOutAsync(user.Id))
            {
                return SignInStatus.LockedOut;
            }
            if (await UserManager.CheckPasswordAsync(user, password))
            {
                await UserManager.ResetAccessFailedCountAsync(user.Id);
                return await SignInOrTwoFactor(user, isPersistent);
            }
            if (shouldLockout)
            {
                // If lockout is requested, increment access failed count which might lock out the user
                await UserManager.AccessFailedAsync(user.Id);
                if (await UserManager.IsLockedOutAsync(user.Id))
                {
                    return SignInStatus.LockedOut;
                }
            }
            return SignInStatus.Failure;
        }
    }
}
