using Jwt_Core1.Models;
using Jwt_Core1.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Jwt_Core1.Controllers.API
{
    [Route("api/[controller]")]
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

        [HttpGet("GetAllFile")]
        public Response GetAllFile()
        {
            using (ApitemplatereportContext context = new ApitemplatereportContext())
            {
                //TblFileDetailFileId
                var allFiles = context.TblFileDetails.Select(x => x).ToList();
                if (allFiles != null)
                {
                    List<TblFileDetail> list = new List<TblFileDetail>(allFiles);
                    return new Response { StatusCode = 200, Message = "All Files Uploaded", Content = list };
                }
            }
            return new Response();
        }

        [HttpPost("Generate")]
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

                return new Response 
                { 
                    StatusCode = 200, 
                    Content = info.TimeStamp + ".Report.docx", 
                    Message = "Generate Success" 
                };
            }
            catch (Exception ex)
            {
                return new Response { StatusCode = 404, Content = ex.Message, Message = "Fail To Generate" };
            }
        }

        [HttpPost("Download")]
        public IActionResult Download([FromBody] FileDownload file)
        {
            try
            {
                var path = _rootPath.WebRootPath + "\\Renders\\" + file.filename;
                var stream = System.IO.File.OpenRead(path);
                return new FileStreamResult(stream, "application/octet-stream");
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}
