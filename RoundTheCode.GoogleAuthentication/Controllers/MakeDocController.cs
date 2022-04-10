using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDBWebAPI.Models;
using MongoDBWebAPI.Services;
using RoundTheCode.GoogleAuthentication.Models;

namespace RoundTheCode.GoogleAuthentication.Controllers
{
    public class MakeDocController : Controller
    {
        private readonly MakeDocService _docService;

        public MakeDocController(MakeDocService docService)
        {
            _docService = docService;
        }

        [HttpGet]
        public ActionResult<List<Models.Doc>> Get() =>
            _docService.Get();

        [AllowAnonymous]
        [HttpGet("MakeDoc")]
        public IActionResult Index()
        {
            ViewBag.Docs = _docService.GetUserDocs(HttpContext.Session.GetString("User"));
            return View();
        }

        [AllowAnonymous]
        [HttpGet("MakeDoc/{id}")]
        public IActionResult Index(string id)
        {
            ViewBag.DocId = id;
            ViewBag.Text = _docService.Get(id).Text;
            return View("EditDoc");
        }

        [HttpPost("MakeDoc")]
        public async Task IndexPost()
        {
            if(Request.Form["FormType"].Equals("NewDoc") && HttpContext.Session.GetString("User") != null)
            {
                Doc appDoc = new Doc
                {
                    Title = "",
                    User = HttpContext.Session.GetString("User"),
                    Text = new List<string>()
                };

                await _docService.CreateDocument(appDoc);
                string id = appDoc.Id;
                Response.Redirect(String.Format("MakeDoc/{0}", id));
                ViewBag.DocId = id;
            } else
            {
                await _docService.UpdateDocument(Request.Form["Text[]"].ToList<string>(), Request.Form["DocId"]);
            }
        }
    }
}
