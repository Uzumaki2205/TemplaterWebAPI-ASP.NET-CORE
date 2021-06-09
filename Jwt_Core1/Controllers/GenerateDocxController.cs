using Jwt_Core1.Helpers;
using Jwt_Core1.Models;
using Jwt_Core1.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

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
        public IActionResult GenerateWithFile(string templatename, IFormFile file)
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

                    MultipartFormDataContent multiContent = new MultipartFormDataContent();
                    ByteArrayContent bytes = new ByteArrayContent(data);
                        
                    multiContent.Add(bytes, "files", file.FileName);
                    multiContent.Add(new StringContent(templatename));
                   
                    var res = new RequestHelper(factory).PostRequest("api/FillDocx/GenerateWithFile", multiContent);
                    if (res.StatusCode == 200)
                        return Download(new FileDownload { filename = res.Content.ToString() });
                }

                return BadRequest("Error With File Upload");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Generate(FileGenerate file)
        {
            var res = new RequestHelper(factory).PostRequest("api/FillDocx/Generate", file);
            if (res.StatusCode == 200)
                return Download(new FileDownload { filename = res.Content.ToString() });
            return BadRequest();
        }

        public IActionResult Download(FileDownload file)
        {
            //FileDownload files = new FileDownload() { filename = file.filename };
            var response = new RequestHelper(factory).PostRequestStream("api/FillDocx/Download", file);

            if (response.Result.StatusCode == 200)
                return new FileStreamResult(response.Result.Content as Stream, "application/octec-stream")
                { FileDownloadName = file.filename };

            return RedirectToAction("Index");
        }
    }
}
