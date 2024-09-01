using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using YoutubeDownloader.Models.Database;
using YoutubeDownloader.ViewModels;

namespace YoutubeDownloader.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IEmailSender _emailSender;

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IEmailSender emailSender)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
    }

    [HttpGet]
    public IActionResult Login(string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel credentials, [FromQuery] string returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return View(credentials);
        }
        
        var user = await _userManager.FindByEmailAsync(credentials.Email);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(credentials);
        }
        
        var result = await _signInManager.PasswordSignInAsync(user.UserName, credentials.Password, credentials.RememberMe, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(credentials);
        }
        
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        
        if (ModelState.IsValid)
        {
            var user = new User { UserName = model.UserName, Email = model.Email, ProfilePicSource = "default", CreatedOn = DateTime.UtcNow};
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Action(
                    "EmailConfirmation", 
                    "Account", 
                    new { userId = user.Id, code = code }, 
                    protocol: HttpContext.Request.Scheme);
                
                await _userManager.AddToRoleAsync(user, "Regular");
                await _emailSender.SendEmailAsync(model.Email, "Confirm your email",
                    $"Please confirm your account by clicking <a href='{callbackUrl}'>here</a>.");

                return RedirectToAction("RegisterConfirmation");
                
                /*await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");*/
            }
            
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return View(model);
    }
    
    [HttpGet]
    public IActionResult RegisterConfirmation()
    {
        return View();
    }
    
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }
    
    [HttpGet]
    public async Task<IActionResult> EmailConfirmation(string userId, string code)
    {
        if (userId == null || code == null)
        {
            return RedirectToAction("Index", "Home");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return View("Error");
        }

        var result = await _userManager.ConfirmEmailAsync(user, code);
        if (!result.Succeeded)
        {
            return View("Error");
        }
        
        return View();

    }
    
    [HttpGet]
    public IActionResult PasswordReset()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> PasswordReset(PasswordResetViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }
        
        var user = await _userManager.FindByEmailAsync(viewModel.Email);
        if (user == null)
        {
            return View("PasswordResetConfirmation");
        }

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        var callbackUrl = Url.Action(
            "PasswordChange", 
            "Account", 
            new { userId = user.Id, code = code }, 
            protocol: HttpContext.Request.Scheme);
        
        await _emailSender.SendEmailAsync(viewModel.Email, "Reset your password",
            $"Please reset your password by clicking <a href='{callbackUrl}'>here</a>.");
        
        return View("PasswordResetConfirmation");
    }

    
    [HttpGet]
    public async Task<IActionResult> PasswordChange(string userId, string code)
    {
        if (userId == null || code == null)
        {
            return RedirectToAction("Index", "Home");
        }
        
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return View("Error");
        }
        
        ViewData["userId"] = userId;
        ViewData["code"] = code;

        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> PasswordChange(PasswordChangeViewModel viewModel, [FromQuery] string userId, [FromQuery] string code)
    {
        if (userId == null || code == null)
        {
            return RedirectToAction("Index", "Home");
        }
        
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return View("Error");
        }

        var result = await _userManager.ResetPasswordAsync(user, code, viewModel.NewPassword);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            
            ViewData["userId"] = userId;
            ViewData["code"] = code;
            return View(viewModel);
        }
        
        return View("PasswordChangeConfirmation");
    }
}