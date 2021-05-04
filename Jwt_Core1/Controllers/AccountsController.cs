using Jwt_Core1.Helpers;
using Jwt_Core1.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Jwt_Core1.Controllers
{
    public class AccountsController : Controller
    {
        private IHttpClientFactory factory;
        public AccountsController(IHttpClientFactory factory)
        {
            this.factory = factory;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var parameters = new Dictionary<string, string> { { "username", username }, { "password", password } };
            var encodedContent = new FormUrlEncodedContent(parameters);

            var response = new RequestHelper(factory).PostRequest("api/Users/Login", encodedContent);
            if (response.StatusCode == 200)
            {
                HttpContext.Session.SetString("Session.Username", username);
                HttpContext.Session.SetString("Session.Token", response.Content.ToString());
                return RedirectToAction("Index", "FileUpload");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Accounts");
        }
    }
}
