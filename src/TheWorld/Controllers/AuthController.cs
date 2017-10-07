using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TheWorld.Models;
using TheWorld.ViewModels;

namespace TheWorld.Controllers
{
    public class AuthController : Controller
    {
        private SignInManager<WorldUser> _manager;

        public AuthController(SignInManager<WorldUser> manager)
        {
            _manager = manager;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Trips", "App");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel vm, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var signInResult = await _manager.PasswordSignInAsync(vm.Username, vm.Password, true, false);

                if (signInResult.Succeeded)
                {
                    if (string.IsNullOrWhiteSpace(returnUrl))
                    {
                        return RedirectToAction("Trips", "App");
                    }
                    else
                    {
                        return Redirect(returnUrl);
                    }
                   
                }
                else
                {
                    ModelState.AddModelError("", "Username or password incorrect"); // Adds error, returned with View
                }
            }

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                await _manager.SignOutAsync();
            }

            return RedirectToAction("Index", "App");
        }
    }
}
