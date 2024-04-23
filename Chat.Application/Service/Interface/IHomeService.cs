using Chat.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Application.Service.Interface
{
    public interface IHomeService
    {
        Task<IdentityResult> Register(string EmailRef, string NameRef, string Password);
        Task<SignInResult> Login(string Email, string Password, bool Remember);
        Task<ApplicationUser> Profile(ClaimsPrincipal User);
        Task<IdentityResult> ProfileUp(ClaimsPrincipal User, ApplicationUser UserProfile);
        Task<ApplicationUser> GetUserToEmail(string Email);
        void Logout();
    }
}
