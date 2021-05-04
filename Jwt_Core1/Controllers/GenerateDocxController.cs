using Jwt_Core1.Helpers;
using Jwt_Core1.Models;
using Jwt_Core1.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Jwt_Core1.Controllers
{
    public class GenerateDocxController : Controller
    {
        private IHttpClientFactory factory;
        public GenerateDocxController(IHttpClientFactory factory)
        {
            this.factory = factory;
        }
        public IActionResult Index(ListFile model)
        {
            string token = HttpContext.Session.GetString("Session.Token");
            if (string.IsNullOrEmpty(token))
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Accounts");
            }

            var response = new RequestHelper(factory).GetRequest("api/Files/GetAllFile", token);
            if (response.StatusCode == 200)
            {
                model.FileList = JsonConvert.DeserializeObject<List<TblFileDetail>>(response.Content.ToString());
                return View(model);
            }

            return View(null);
        }

        [HttpPost]
        public IActionResult Generate(string templatename, IFormFile file)
        {
            var fileExt = Path.GetExtension(file.FileName).Substring(1);
            if (fileExt != "json")
                return RedirectToAction("Index");

            try
            {
                // Send Multipart From to api
                if (file != null && file.Length > 0)
                {
                    byte[] data;
                    using (var br = new BinaryReader(file.OpenReadStream()))
                        data = br.ReadBytes((int)file.OpenReadStream().Length);

                    ByteArrayContent bytes = new ByteArrayContent(data);
                    MultipartFormDataContent multiContent = new MultipartFormDataContent();

                    multiContent.Add(bytes, "files", file.FileName);
                    multiContent.Add(new StringContent(templatename));

                    var res = new RequestHelper(factory).PostRequest("api/FillDocx/Generate",
                        HttpContext.Session.GetString("Session.Token"), multiContent);
                    if (res.StatusCode == 200)
                        return RedirectToAction("Index");
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}
