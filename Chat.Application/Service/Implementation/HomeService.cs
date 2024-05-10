using Chat.Application.Service.Interface;
using Chat.Domain.Entities;
using Chat.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Application.Service.Implementation
{
    public class HomeService : IHomeService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _db;



        public HomeService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

        public IEnumerable<Conversation> GetAllUserConversations(string userId)
        {
            var conversationList = _db.Conversations.Where(x => x.UserRefId == userId).OrderByDescending(x => x.LatestMessageDateTime).ToList();
            return conversationList;
        }

        public IEnumerable<ApplicationUser> GetAllUsers()
        {
            var userList = _db.Users.ToList();
            return userList;
        }

        public Task<ApplicationUser> GetUserToEmail(string Email)
        {
            return _userManager.FindByEmailAsync(Email);
        }

        public Task<SignInResult> Login(string Email, string Password, bool Remember)
        {
            return _signInManager.PasswordSignInAsync(Email, Password, Remember, lockoutOnFailure: false);

        }

        public void Logout()
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser> Profile(ClaimsPrincipal User)
        {

            return _userManager.GetUserAsync(User);

        }

        public async Task<IdentityResult> ProfileUp(ClaimsPrincipal User, ApplicationUser UserProfile)
        {
            var user = await _userManager.GetUserAsync(User);
            user.Email = UserProfile.Email ?? user.Email;
            user.Name = UserProfile.Name ?? user.Name;
            user.PhoneNumber = UserProfile.PhoneNumber ?? user.PhoneNumber;
            user.DateOfBirthday = UserProfile.DateOfBirthday ?? user.DateOfBirthday;
            user.Address = UserProfile.Address ?? user.Address;
            user.ImageUrl = UserProfile.ImageUrl ?? user.ImageUrl;
            var result = await _userManager.UpdateAsync(user);
            return result;
        }

        public async Task<IdentityResult> Register(string EmailRef, string NameRef, string Password)
        {
            ApplicationUser user = new()
            {
                Email = EmailRef,
                Name = NameRef,
                NormalizedEmail = EmailRef.ToUpper(),
                EmailConfirmed = true,
                UserName = EmailRef,
                CreateAt = DateOnly.FromDateTime(DateTime.Now),
                ImageUrl = "https://villa.demondev.games/images/logo.png"
            };

            var res = await _userManager.CreateAsync(user, Password);
            if (res.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
            }
            else
            {
                throw new Exception("Register failed");
            }
            return res;
        }
    }
}
