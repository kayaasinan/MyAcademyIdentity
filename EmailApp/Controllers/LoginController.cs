using EmailApp.Entities;
using EmailApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Project;

namespace EmailApp.Controllers
{
    public class LoginController(UserManager<AppUser> _userManager,
                                SignInManager<AppUser> _signInManager) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Bu email sistemde kayıtlı değil");
                return View();
            }
            var result=await _signInManager.PasswordSignInAsync(user,model.Password,false,false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Email veya şifre hatalı");
                return View(model);
            }
            return RedirectToAction("Index", "Message");
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Login");
        }
    }
}
