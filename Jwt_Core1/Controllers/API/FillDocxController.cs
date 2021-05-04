using Jwt_Core1.Models;
using Jwt_Core1.Models.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Jwt_Core1.Controllers.API
{
    [Route("api/filldocx/{action}")]
    [ApiController]
    public class FillDocxController : ControllerBase
    {
        private IWebHostEnvironment _rootPath;

        public FillDocxController(IWebHostEnvironment environment)
        {
            _rootPath = environment;
            info = new InfoVuln();
        }

        public InfoVuln info { get; set; }

        /// <summary>
        /// Generate Template
        /// </summary>
        /// <param name="files"></param>
        /// <param name="templatename"></param>
        /// <returns></returns>
        public Response Generate([FromForm] IFormFile files, [FromForm] string templatename)
        {
            var fileExt = Path.GetExtension(files.FileName).Substring(1);
            if (fileExt != "json")
                return new Response();

            //Upload File
            var path = Path.Combine(_rootPath.WebRootPath, "UploadedFiles", "Json", info.TimeStamp);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var savePath = Path.Combine(path, info.TimeStamp + ".json");
            var stream = new FileStream(savePath, FileMode.Create);
            files.CopyToAsync(stream);
            stream.Close();

            try
            {
                var Jsonpath = Path.Combine(_rootPath.WebRootPath, $"UploadedFiles\\Json\\{info.TimeStamp}",
                    info.TimeStamp + ".json");
                info.ProcessDocx(templatename, Jsonpath);

                return new Response { StatusCode = 200, Content = info.TimeStamp + ".Report.docx", Message = "Generate Success" };
            }
            catch (Exception ex)
            {
                return new Response { StatusCode = 404, Content = ex.Message, Message = "Fail To Generate" };
            }
        }
    }
}
