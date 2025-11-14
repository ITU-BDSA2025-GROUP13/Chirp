// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Chirp.Core.Models;

namespace Chirp.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<ChirpUser> _signInManager;
        private readonly UserManager<ChirpUser> _userManager;
        private readonly IUserStore<ChirpUser> _userStore;
        private readonly IUserEmailStore<ChirpUser> _emailStore;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ExternalLoginModel> _logger;

        public ExternalLoginModel(
            SignInManager<ChirpUser> signInManager,
            UserManager<ChirpUser> userManager,
            IUserStore<ChirpUser> userStore,
            ILogger<ExternalLoginModel> logger,
            IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _logger = logger;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ProviderDisplayName { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        public IActionResult OnGet() => RedirectToPage("./Login");

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            ProviderDisplayName = info.ProviderDisplayName;

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrWhiteSpace(email))
            {
                ErrorMessage = $"{info.LoginProvider} did not provide an email address.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                var existingLogins = await _userManager.GetLoginsAsync(existingUser);
                var alreadyLinked = existingLogins.Any(login => login.LoginProvider == info.LoginProvider && login.ProviderKey == info.ProviderKey);
                if (!alreadyLinked)
                {
                    var addLoginToExisting = await _userManager.AddLoginAsync(existingUser, info);
                    if (!addLoginToExisting.Succeeded)
                    {
                        ErrorMessage = string.Join(" ", addLoginToExisting.Errors.Select(e => e.Description));
                        return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
                    }
                }

                if (_userManager.Options.SignIn.RequireConfirmedAccount && !await _userManager.IsEmailConfirmedAsync(existingUser))
                {
                    await SendConfirmationEmailAsync(existingUser, email);
                    return RedirectToPage("./RegisterConfirmation", new { Email = email });
                }

                await _signInManager.SignInAsync(existingUser, isPersistent: false, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }

            var userName = ResolveUserName(info, email);
            if (string.IsNullOrWhiteSpace(userName))
            {
                ErrorMessage = $"{info.LoginProvider} did not provide a valid username.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            var user = CreateUser();

            await _userStore.SetUserNameAsync(user, userName, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, email, CancellationToken.None);

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                ErrorMessage = string.Join(" ", createResult.Errors.Select(e => e.Description));
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            var addLoginResult = await _userManager.AddLoginAsync(user, info);
            if (!addLoginResult.Succeeded)
            {
                ErrorMessage = string.Join(" ", addLoginResult.Errors.Select(e => e.Description));
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

            await SendConfirmationEmailAsync(user, email);

            if (_userManager.Options.SignIn.RequireConfirmedAccount)
            {
                return RedirectToPage("./RegisterConfirmation", new { Email = email });
            }

            await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
            return LocalRedirect(returnUrl);
        }

        private async Task SendConfirmationEmailAsync(ChirpUser user, string email)
        {
            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId, code },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(email, "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
        }

        private string ResolveUserName(ExternalLoginInfo info, string email)
        {
            // Prefer provider-specific login claim when available (e.g. GitHub)
            var candidate = info.Principal.FindFirstValue("urn:github:login") ??
                            info.Principal.FindFirstValue(ClaimTypes.Name) ??
                            info.Principal.Identity?.Name;

            if (string.IsNullOrWhiteSpace(candidate) && !string.IsNullOrWhiteSpace(email))
            {
                var atIndex = email.IndexOf('@');
                candidate = atIndex > 0 ? email.Substring(0, atIndex) : email;
            }

            if (string.IsNullOrWhiteSpace(candidate))
            {
                return null;
            }

            candidate = candidate.Trim();

            if (candidate.Length < 2 && !string.IsNullOrWhiteSpace(email))
            {
                var fallback = email.Split('@')[0];
                if (!string.IsNullOrWhiteSpace(fallback))
                {
                    candidate = fallback;
                }
            }

            if (candidate.Length > 32)
            {
                candidate = candidate.Substring(0, 32);
            }

            return candidate.Length >= 2 ? candidate : null;
        }

        private ChirpUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ChirpUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ChirpUser)}'. " +
                    $"Ensure that '{nameof(ChirpUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the external login page in /Areas/Identity/Pages/Account/ExternalLogin.cshtml");
            }
        }

        private IUserEmailStore<ChirpUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ChirpUser>)_userStore;
        }
    }
}
