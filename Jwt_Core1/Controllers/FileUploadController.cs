using Jwt_Core1.Helpers;
using Jwt_Core1.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace Jwt_Core1.Controllers
{
    public class FileUploadController : Controller
    {
        private IHttpClientFactory factory;
        public FileUploadController(IHttpClientFactory factory)
        {
            this.factory = factory;
        }

        public IEnumerable<TblFileDetail> FileList { get; set; }

        [HttpGet]
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
        public IActionResult Upload(IFormFile file)
        {
            var supportedTypes = new[] { "doc", "docx" };
            var fileExt = Path.GetExtension(file.FileName).Substring(1);
            if (!supportedTypes.Contains(fileExt))
                return StatusCode(400);

            try
            {
                if (file != null && file.Length > 0)
                {
                    byte[] data;
                    using (var br = new BinaryReader(file.OpenReadStream()))
                        data = br.ReadBytes((int)file.OpenReadStream().Length);

                    ByteArrayContent bytes = new ByteArrayContent(data);
                    MultipartFormDataContent multiContent = new MultipartFormDataContent();
                    multiContent.Add(bytes, "file", file.FileName);

                    var response = new RequestHelper(factory).PostRequest("api/Files/Upload", 
                        HttpContext.Session.GetString("Session.Token"), multiContent);
                    return RedirectToAction("Index");
                }

                return StatusCode(400);
            }
            catch (Exception)
            {
                // Return Server internal error when upload fail
                return StatusCode(500);
            }
        }

        [HttpPost]
        public IActionResult Download(string filename)
        {
            var parameters = new Dictionary<string, string> { { "filename", filename } };
            var encodedContent = new FormUrlEncodedContent(parameters);

            var response = new RequestHelper(factory).PostRequestStream("api/Files/Download",
                HttpContext.Session.GetString("Session.Token"), encodedContent);

            if (response.Result.StatusCode == 200)
                return new FileStreamResult(response.Result.Content as Stream,
                    new MediaTypeHeaderValue("application/octet-stream"))
                {
                    FileDownloadName = filename
                };

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(string filename)
        {
            var parameters = new Dictionary<string, string> { { "filename", filename } };
            var encodedContent = new FormUrlEncodedContent(parameters);

            var response = new RequestHelper(factory).PostRequest("api/Files/Delete",
                HttpContext.Session.GetString("Session.Token"), encodedContent);

            if (response.StatusCode != 200)
                ModelState.AddModelError("Error", "Delete Fail");

            return RedirectToAction("Index");
        }

        //private string GetContentType(string path)
        //{
        //    var types = GetMimeTypes();
        //    var ext = Path.GetExtension(path).ToLowerInvariant();
        //    return types[ext];
        //}

        //private Dictionary<string, string> GetMimeTypes()
        //{
        //    return new Dictionary<string, string>
        //    {
        //        {".doc", "application/vnd.ms-word"},
        //        {".docx", "application/vnd.ms-word"},
        //    };
        //}
    }
}
