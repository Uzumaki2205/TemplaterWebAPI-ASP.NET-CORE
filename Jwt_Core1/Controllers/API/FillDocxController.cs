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
            //using (ApitemplatereportContext context = new ApitemplatereportContext())
            //{
            //    //TblFileDetailFileId
            //    var allFiles = context.TblFileDetails.Select(x => x).ToList();
            //    if (allFiles != null)
            //    {
            //        List<TblFileDetail> list = new List<TblFileDetail>(allFiles);
            //        return new Response { StatusCode = 200, Message = "All Files Uploaded", Content = list };
            //    }
            //}
            //return new Response();
            List<TblFileDetail> list = new List<TblFileDetail>();
            using (ApitemplatereportContext context = new ApitemplatereportContext())
            {
                //TblFileDetailFileId

                var allFiles = context.TblFileDetails.Select(x => x).ToList();
                if (allFiles.Count != 0)
                {
                    list = allFiles;
                    var jsonList = JsonConvert.SerializeObject(list);
                    return new Response { StatusCode = 200, Message = "All File", Content = jsonList };
                }
            }

            list.Add(new TblFileDetail());
            var errList = JsonConvert.SerializeObject(list);
            return new Response { StatusCode = 200, Message = "All File", Content = errList };
        }

        [HttpPost("GenerateWithFile")]
        public async System.Threading.Tasks.Task<Response> GenerateWithFile([FromForm] IFormFile files, [FromForm] string templatename)
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
            await files.CopyToAsync(stream);
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
                return new Response { StatusCode = 404, Content = ex.Message, Message = ex.Message };
            }
        }

        [HttpPost("Generate")]
        public Response Generate(FileGenerate file)
        {
            try
            {
                info.ProcessDocxJson(file.templatename, file.content);
                return new Response
                {
                    StatusCode = 200,
                    Content = info.TimeStamp + ".Report.docx",
                    Message = "Generate Success"
                };
            }
            catch (Exception ex)
            {
                return new Response { StatusCode = 404, Content = ex.Message, Message = ex.Message };
            }
        }

        [HttpPost("Download")]
        public async System.Threading.Tasks.Task<IActionResult> Download([FromBody] FileDownload file)
        {
            var path = _rootPath.WebRootPath + "\\Renders\\" + file.filename;
            try
            {
                var memory = new MemoryStream();
                using (var stream = System.IO.File.OpenRead(path))
                    await stream.CopyToAsync(memory);
                memory.Position = 0;

                return new FileStreamResult(memory, "application/octec-stream");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return NotFound();
        }
    }
}
