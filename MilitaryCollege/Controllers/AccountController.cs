using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MilitaryCollege.Models;
using MilitaryCollege.Models.ViewModels;

namespace MilitaryCollege.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private IHttpContextAccessor _httpContextAccessor;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IHttpContextAccessor httpContextAccessor)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginVM model, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, false);
                if (result.Succeeded)
                { 
                
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    // if user is general or 3akid aboud daher
                    if (await _userManager.IsInRoleAsync(user, "Viewer") || await _userManager.IsInRoleAsync(user, "SuperAdmin"))
                    {
                        return RedirectToAction("Index", "Tournaments");

                    }
                    else
                    return RedirectToAction("OfficerListOfcard", "Officers");

                }
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}