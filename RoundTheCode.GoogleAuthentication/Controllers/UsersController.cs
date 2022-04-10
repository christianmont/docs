using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDBWebAPI.Models;
using MongoDBWebAPI.Services;
using Newtonsoft.Json;
using NuGet.Configuration;
using RoundTheCode.GoogleAuthentication.Controllers;
using User = MongoDBWebAPI.Models.User;

namespace MongoDBWebAPI.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public ActionResult<List<Models.User>> Get() =>
            _userService.Get();

        [HttpGet("{id:length(24)}", Name = "GetUser")]
        public ActionResult<Models.User> Get(string id)
        {
            var emp = _userService.Get(id);

            if (emp == null)
            {
                return NotFound();
            }

            return emp;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(User user)
        {
            if (ModelState.IsValid)
            {
                User appUser = new User
                {
                    Site = "Local",
                    UserId = "",
                    UserName = user.UserName,
                    Email = user.Email,
                    Password = Crypto.HashPassword(user.Password),
                    DisplayName = user.DisplayName,
                    FirstName = "",
                    LastName = "",
                    Image = user.Image
                };

                await _userService.CreateDocument(appUser);

                HttpContext.Session.SetObject("User", appUser);
            }
            return View(user);
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            if (ModelState.IsValid)
            {
                var u = _userService.GetUser(user.UserName, user.Password);
                if (u != null)
                {
                    HttpContext.Session.SetObject("User", u);
                }
            }
            return View(user);
        }
    }
}