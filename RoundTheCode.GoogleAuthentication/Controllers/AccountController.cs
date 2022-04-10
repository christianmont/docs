using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDBWebAPI.Models;
using MongoDBWebAPI.Services;

namespace RoundTheCode.GoogleAuthentication.Controllers
{
    [AllowAnonymous, Route("account")]
    public class AccountController : Controller
    {
        private readonly UserService _userService;
        public AccountController(UserService userService)
        {
            _userService = userService;
        }

        [Route("google-login")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [Route("facebook-login")]
        public IActionResult FacebookLogin()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("FacebookResponse") };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [Route("twitter-login")]
        public IActionResult TwitterLogin()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("TwitterResponse") };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [Route("google-response")]
        public async Task GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var UserId = result.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;
            var DisplayName = result.Principal.FindFirst(ClaimTypes.Name).Value;
            var FirstName = result.Principal.FindFirst(ClaimTypes.GivenName).Value;
            var LastName = result.Principal.FindFirst(ClaimTypes.Surname).Value;
            var Email = result.Principal.FindFirst(ClaimTypes.Email).Value;

            User u = _userService.GetSiteUser("Google", UserId);

            if (u != null)
            {
                HttpContext.Session.SetString("User", u.Id.ToString());
            }
            else
            {
                User appUser = new User
                {
                    Site = "Google",
                    UserId = UserId,
                    UserName = "",
                    Email = Email,
                    Password = "",
                    DisplayName = DisplayName,
                    FirstName = FirstName,
                    LastName = LastName,
                    Image = ""
                };

                await _userService.CreateDocument(appUser);

                HttpContext.Session.SetString("User", appUser.Id.ToString());
            }
            Response.Redirect("/Home");
        }

        [Route("facebook-response")]
        public async Task FacebookResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var UserId = result.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;
            var DisplayName = result.Principal.FindFirst(ClaimTypes.Name).Value;
            var FirstName = result.Principal.FindFirst(ClaimTypes.GivenName).Value;
            var LastName = result.Principal.FindFirst(ClaimTypes.Surname).Value;
            var Email = result.Principal.FindFirst(ClaimTypes.Email).Value;

            User u = _userService.GetSiteUser("Facebook", "UserId");

            if (u != null)
            {
                HttpContext.Session.SetObject("User", u);
            }
            else
            {
                User appUser = new User
                {
                    Site = "Facebook",
                    UserId = UserId,
                    UserName = "",
                    Email = Email,
                    Password = "",
                    DisplayName = DisplayName,
                    FirstName = FirstName,
                    LastName = LastName,
                    Image = ""
                };

                await _userService.CreateDocument(appUser);

                HttpContext.Session.SetString("User", appUser.Id.ToString());
            }
            Response.Redirect("/Home");
        }

        [Route("twitter-response")]
        public async Task TwitterResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var UserId = result.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;
            var DisplayName = result.Principal.FindFirst(ClaimTypes.Name).Value;
            var FirstName = result.Principal.FindFirst(ClaimTypes.GivenName).Value;
            var LastName = result.Principal.FindFirst(ClaimTypes.Surname).Value;
            var Email = result.Principal.FindFirst(ClaimTypes.Email).Value;

            User u = _userService.GetSiteUser("Twitter", "UserId");

            if (u != null)
            {
                HttpContext.Session.SetObject("User", u);
            }
            else
            {
                User appUser = new User
                {
                    Site = "Twitter",
                    UserId = UserId,
                    UserName = "",
                    Email = Email,
                    Password = "",
                    DisplayName = DisplayName,
                    FirstName = FirstName,
                    LastName = LastName,
                    Image = ""
                };

                await _userService.CreateDocument(appUser);

                HttpContext.Session.SetString("User", appUser.Id.ToString());
            }
            Response.Redirect("/Home");
        }
    }
}
