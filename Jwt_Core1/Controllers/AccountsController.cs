using Jwt_Core1.Helpers;
using Jwt_Core1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

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
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Session.Username")) &&
                !string.IsNullOrEmpty(HttpContext.Session.GetString("Session.Password")) &&
                !string.IsNullOrEmpty(HttpContext.Session.GetString("Session.Token")))
                return RedirectToAction("Index", "FileUpload");

            HttpContext.Session.Clear();
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            //var parameters = new Dictionary<string, string> { { "username", username }, { "password", password } };
            //var encodedContent = new FormUrlEncodedContent(parameters);
            var encodedContent = new UserLogin { Username = username, Password = password };

            var response = new RequestHelper(factory).PostRequest("api/Users/Login", encodedContent);
            if (response.StatusCode == 200)
            {
                HttpContext.Session.SetString("Session.Username", username);
                HttpContext.Session.SetString("Session.Password", new HashMd5().CreateMD5Hash(password));
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
