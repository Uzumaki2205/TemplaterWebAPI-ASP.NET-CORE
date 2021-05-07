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
            string username = HttpContext.Session.GetString("Session.Username");
            string password = HttpContext.Session.GetString("Session.Password");

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Accounts");
            }

            TblUser u = new ApitemplatereportContext().TblUsers.Where(x => x.Username == username).FirstOrDefault();
            var validatetoken = new RequestHelper(factory).PostRequest("api/Token/ValidateToken", token, u);

            if(validatetoken.StatusCode == 200)
            {
                var us = JsonConvert.DeserializeObject<UserLogin>(validatetoken.Content.ToString());
                string newPass = new HashMd5().CreateMD5Hash(us.Password);
                if(newPass == password)
                {
                    var response = new RequestHelper(factory).GetRequest("api/Files/GetAllFile", token);
                    if (response.StatusCode == 200)
                    {
                        model.FileList = JsonConvert.DeserializeObject<List<TblFileDetail>>(response.Content.ToString());
                        return View(model);
                    }
                    else
                    {
                        HttpContext.Session.Clear();
                        return RedirectToAction("Login", "Accounts");
                    }
                }
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
        public IActionResult Download(FileDownload file)
        {
            var response = new RequestHelper(factory).PostRequestStream("api/Files/Download",
                HttpContext.Session.GetString("Session.Token"), file);

            if (response.Result.StatusCode == 200)
                return new FileStreamResult(response.Result.Content as Stream, "application/octet-stream") 
                { FileDownloadName = file.filename };
               
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(string filename)
        {
            //var parameters = new Dictionary<string, string> { { "filename", filename } };
            //var encodedContent = new FormUrlEncodedContent(parameters);

            var response = new RequestHelper(factory).PostRequest("api/Files/Delete",
                HttpContext.Session.GetString("Session.Token"), filename);

            if (response.StatusCode != 200)
                ModelState.AddModelError("Error", "Delete Fail");

            return RedirectToAction("Index");
        }
    }
}
