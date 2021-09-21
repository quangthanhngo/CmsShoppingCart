using CmsShoppingCart.Helper;
using CmsShoppingCart.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CmsShoppingCart.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private IPasswordHasher<AppUser> passwordHasher;
       

        public AccountController(UserManager<AppUser> userManager,
                                SignInManager<AppUser> signInManager,
                                IPasswordHasher<AppUser> passwordHasher)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.passwordHasher = passwordHasher;
        }

        // GET /account/register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
           return View();
        }

        // ORIGIN
        // POST /account/register
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Register(User user)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        AppUser appUser = new AppUser
        //        {
        //            UserName = user.UserName,
        //            Email = user.Email,
        //            EmailConfirmed = false
        //        };

        //        IdentityResult result = await userManager.CreateAsync(appUser, user.Password);
        //        if (result.Succeeded)
        //        {
        //            var token = await userManager.GenerateEmailConfirmationTokenAsync(appUser);

        //            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = appUser.Id, token = token }, protocol: HttpContext.Request.Scheme);

        //            SendMail.SendEmail(user.Email, "Confirm your account", "Please confirm your account by click <a href=\"" + callbackUrl + "\">here</a>", "");

        //            return View("NotificationEmailConfirm");

        //        }
        //        else
        //        {
        //            foreach (IdentityError error in result.Errors)
        //            {
        //                ModelState.AddModelError("", error.Description);
        //            }
        //        }
        //        // add role
        //        //await userManager.AddToRoleAsync(appUser, "customer");
        //    }



        //    return View(user);
        //}

        // <SUCCESSFUL> try to edit code
        // POST /account/register/5
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                var appUser = new AppUser
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    EmailConfirmed = false
                };

                var result = await userManager.CreateAsync(appUser, user.Password);
                if (result.Succeeded)
                {
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(appUser);

                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = appUser.Id, token = token }, protocol: HttpContext.Request.Scheme);

                    SendMail.SendEmail(appUser.Email, "Confirm your account", "Please confirm your account by click <a href=\"" + confirmationLink + "\">here</a>", "");

                    return View("NotificationEmailConfirm");

                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                // add role
                //await userManager.AddToRoleAsync(appUser, "customer");
            }



            return View(user);
        }

        // <SUCCESSFUL> try to edit code
        // GET /account/confirmemail
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return View("Error");
            }

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"The User ID {userId} is invalid";
                return View("NotFound"); // create NotFound class later
            }
            var result = await userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                return View("ConfirmEmail");
            }
            ViewBag.ErrorTitle = "Email cannot be confirmed";
            return View("Error");
        }

        // ORIGIN <DELETE THIS>
        // GET /account/confirmemail
        //[AllowAnonymous]
        //public async Task<IActionResult> ConfirmEmail(AppUser userId, string token)
        //{
        //    if (userId == null || token == null)
        //    {
        //        return View("Error");
        //    }
        //    IdentityResult result = await userManager.ConfirmEmailAsync(userId, token);
        //    return View(result.Succeeded ? "ConfirmEmail" : "Error");
        //}


        // GET /account/login
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            Login login = new Login
            {
                ReturnUrl = returnUrl
            };

            return View(login);
        }

        // ORIGIN <DELETE THIS>
        // POST /account/login
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Login(Login login)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        AppUser appUser = await userManager.FindByEmailAsync(login.Email);
        //        if (appUser != null)
        //        {
        //            Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(appUser, login.Password, false, false);
        //            if (result.Succeeded)
        //                return Redirect(login.ReturnUrl ?? "/");
        //        }
        //        ModelState.AddModelError("", "Login failed, wrong credentials.");
        //    }

        //    return View(login);
        //}

        // <SUCCESSFUL> try to edit Login
        // POST /account/login/5
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login login)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = await userManager.FindByEmailAsync(login.Email);
                if (appUser != null)
                {
                    Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(appUser, login.Password, false, false);
                    if (result.Succeeded)
                        return Redirect(login.ReturnUrl ?? "/");
                }
                ModelState.AddModelError("", "Login failed, wrong credentials.");
            }

            return View(login);
        }


        // GET /account/logout
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            return Redirect("/");
        }

        // GET /account/edit
        public async Task<IActionResult> Edit()
        {
            AppUser appUser = await userManager.FindByNameAsync(User.Identity.Name);
            UserEdit user = new UserEdit(appUser);
            return View(user);
        }

        // POST /account/edit
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEdit user)
        {
            AppUser appUser = await userManager.FindByNameAsync(User.Identity.Name);

            if (ModelState.IsValid)
            {
                appUser.Email = user.Email;
                if (user.Password != null)
                {
                   appUser.PasswordHash = passwordHasher.HashPassword(appUser, user.Password);
                }

                IdentityResult result = await userManager.UpdateAsync(appUser);
                if (result.Succeeded)
                    TempData["Success"] = "Your information has been edited!";
            }

            return View();
        }

        // add forgot password

        // GET /account/forgotpassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST /account/forgotpassword/5
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPassword forgotPassword)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(forgotPassword.Email);
                if(user != null && await userManager.IsEmailConfirmedAsync(user))
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);

                    var passwordResetLink = Url.Action("ResetPassword", "Account", new { email = forgotPassword.Email, token = token }, Request.Scheme);

                    //logger.Log(LogLevel.Warning, passwordResetLink);
                    SendMail.SendEmail(user.Email, "Reset your password", "Please reset your password by click <a href=\"" + passwordResetLink + "\">here</a>", "");

                    return View("ForgotPasswordConfirmation");
                }
                return View("ForgotPasswordConfirmation");
            }
            return View(forgotPassword);
        }

        // GET /account/resetpassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            if(token == null || email == null)
            {
                ModelState.AddModelError("", "Invalid password reset token.");
            }
            return View();
        }

        // POST /account/resetpassword/5
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(resetPassword.Email);
                if(user != null)
                {
                    var result = await userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);
                    if (result.Succeeded)
                    {
                        return View("ResetPasswordConfirmation");
                    }
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(resetPassword);
                }
                return View("ResetPasswordConfirmation");
            }
            return View(resetPassword);
        }

    }
}
