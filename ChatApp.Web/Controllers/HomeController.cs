using Chat.Application.Service.Implementation;
using Chat.Application.Service.Interface;
using Chat.Domain.Entities;
using ChatApp.Web.Models;
using ChatApp.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Diagnostics;

namespace ChatApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHomeService _homeService;

        public HomeController(
            SignInManager<ApplicationUser> signInManager,
            IHomeService homeService

            )
        {
            _signInManager = signInManager;
            _homeService = homeService;
        }
        public IActionResult Index()
        {
            if (_signInManager.IsSignedIn(User))
            {
                return View();

            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var result = await _homeService.Login(model.Email, model.Password, model.RememberMe);
                if (result.Succeeded)
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                ModelState.AddModelError(string.Empty, "Đăng Nhập Không Thành Công");
            }
            return View(model);
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM register)
        {
            if (ModelState.IsValid)
            {
                var result = await _homeService.Register(register.Email, register.Name, register.Password);

                if (result.Succeeded)
                {
                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(register);
        }

        public IActionResult Profile()
        {
            var userPf = _homeService.Profile(User);
            ProfileVM profileVM = new()
            {
                UserProfile = userPf.Result
            };

            return View(profileVM);
        }
        [HttpPost]
        public async Task<IActionResult> Profile(ProfileVM profileVM)
        {
            if (!ModelState.IsValid)
            {

                var result = await _homeService.ProfileUp(User, profileVM.UserProfile);

                //if (profileVM.Image != null)
                //{
                //    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(profileVM.Image.FileName);
                //    string imagePath = Path.Combine(_hostingEnvironment.WebRootPath, @"images\users");
                //    using var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create);

                //    profileVM.Image.CopyTo(fileStream);
                //    @profileVM.ImageUrl = @"\images\user\" + fileName;
                //}
                if (result.Succeeded)
                {
                    return RedirectToAction("Profile", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(profileVM);
        }
        public IActionResult Logout()
        {
            _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Home");
        }
        public IActionResult Chat()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
