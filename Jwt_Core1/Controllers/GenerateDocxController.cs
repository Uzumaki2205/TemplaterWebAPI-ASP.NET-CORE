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
using Microsoft.Net.Http.Headers;

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
            //Authorize when Access page
            //string token = HttpContext.Session.GetString("Session.Token");
            //if (string.IsNullOrEmpty(token))
            //{
            //    HttpContext.Session.Clear();
            //    return RedirectToAction("Login", "Accounts");
            //}

            var response = new RequestHelper(factory).GetRequest("api/FillDocx/GetAllFile");
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

                    //var res = new RequestHelper(factory).PostRequest("api/FillDocx/Generate",
                    //    HttpContext.Session.GetString("Session.Token"), multiContent);
                    var res = new RequestHelper(factory).PostRequest("api/FillDocx/Generate", multiContent);
                    if (res.StatusCode == 200)
                    {
                        return Download(res.Content.ToString());
                    }
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
        public IActionResult Download(string file)
        {
            FileDownload files = new FileDownload() { filename = file };
            var response = new RequestHelper(factory).PostRequestStream("api/FillDocx/Download", files);

            if (response.Result.StatusCode == 200)
                return new FileStreamResult(response.Result.Content as Stream,
                    new MediaTypeHeaderValue("application/octet-stream"))
                {
                    FileDownloadName = file
                };

            return RedirectToAction("Index");
        }
    }
}
