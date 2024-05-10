using Chat.Application.Service.Implementation;
using Chat.Application.Service.Interface;
using Chat.Domain.Entities;
using Chat.Infrastructure.Data;
using ChatApp.Web.Models;
using ChatApp.Web.Models.DTOs;
using ChatApp.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Diagnostics;
using System.Security.Claims;

namespace ChatApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHomeService _homeService;
        private readonly ApplicationDbContext _db;

        public HomeController(
            SignInManager<ApplicationUser> signInManager,
            IHomeService homeService,
            ApplicationDbContext db

            )
        {
            _signInManager = signInManager;
            _homeService = homeService;
            _db = db;
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
        public IActionResult Chat(int conversationId)
        {
            var conversation = _db.Conversations.FirstOrDefault(x => x.ConversationId == conversationId);
            var messages = new List<Message>();
            string nameConversation;
            string sendToConversation;
            bool isGroup;
            if (conversation.GroupId != null)
            {
                messages = _db.Messages
                       .Where(x => x.GroupId == conversation.GroupId)
                       .Include(x => x.User).
                       Include(x => x.Group)
                       .OrderBy(x => x.SentDate)
                       .ToList();
                nameConversation = conversation.GroupName;
                sendToConversation = conversation.GroupId.ToString();
                isGroup = true;
            }
            else if (conversation.ReceiverId != null)
            {
                messages = _db.Messages
                   .Where(x => (x.ReceiverId == conversation.ReceiverId && x.UserId == conversation.UserRefId) ||
                (x.ReceiverId == conversation.UserRefId && x.UserId == conversation.ReceiverId))
                   .Include(x => x.User).
                   Include(x => x.Receiver)
                   .OrderBy(x => x.SentDate)
                   .ToList();
                nameConversation = conversation.ReceiverName;
                sendToConversation = conversation.ReceiverId;
                isGroup = false;
            }
            else
            {
                return RedirectToAction("Conversation", "Home");
            }



            MessageVM messageVM = new()
            {
                messages = messages,
                conversationId = conversation.ConversationId.ToString(),
                conversationName = nameConversation,
                sendToId = sendToConversation,
                isGroup = isGroup
            };


            return View(messageVM);
        }

        public IActionResult Conversation()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var users = _homeService.GetAllUsers();
            var userDTO = new List<UserDTO>();
            foreach (var user in users)
            {
                userDTO.Add(new UserDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.Name,
                    ImageUrl = user.ImageUrl,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address,
                    LastSeen = DateTime.Now
                });
            }

            var conversations = _homeService.GetAllUserConversations(userId);
            ConversationVM conversationVM = new()
            {
                conversationList = conversations,
                userList = userDTO
            };
            return View(conversationVM);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
