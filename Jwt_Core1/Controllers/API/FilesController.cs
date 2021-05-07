using Jwt_Core1.Models;
using Jwt_Core1.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Jwt_Core1.Controllers.API
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FilesController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Get All File
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllFile")]
        public Response GetAllFile()
        {
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
        
        /// <summary>
        /// Upload File
        /// </summary>
        /// <returns></returns>
        [HttpPost("Upload")]
        public Response Upload()
        {
            if (Request.HasFormContentType)
            {
                var form = Request.Form;
                foreach (var formFile in form.Files)
                {
                    var targetDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "Template");
                    var fileName = formFile.FileName;
                    if(Path.GetExtension(fileName) == ".docx" || Path.GetExtension(fileName) == ".doc")
                    {
                        using (ApitemplatereportContext context = new ApitemplatereportContext())
                        {
                            TblFileDetail file = context.TblFileDetails.Where(file => file.Filename == fileName).FirstOrDefault();
                            if (file != null)
                                fileName = formFile.FileName + "_" + GetTimestamp(DateTime.Now)
                                    + Path.GetExtension(formFile.FileName);

                            var savePath = Path.Combine(targetDirectory, fileName);

                            using (var fileStream = new FileStream(savePath, FileMode.Create))
                                formFile.CopyTo(fileStream);

                            var fileUrl = Url.Content(Path.Combine("~/Template/", fileName));
                            context.TblFileDetails.Add(new TblFileDetail { Filename = fileName, Fileurl = fileUrl });
                            context.SaveChanges();
                        }

                        return new Response { StatusCode = 200, Content = fileName + "is Uploaded", Message = "File Uploaded" };
                    }
                }
            }

            return new Response { StatusCode = 404, Content = "File type is not support!!", Message = "Upload Error" };
        }


        /// <summary>
        /// Download File
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        [HttpPost("Download")]
        public async System.Threading.Tasks.Task<IActionResult> Download([FromBody] FileDownload filename)
        {
            using (ApitemplatereportContext context = new ApitemplatereportContext())
            {
                TblFileDetail file = context.TblFileDetails.Where(file => file.Filename == filename.filename).FirstOrDefault();
                if(file != null)
                {
                    var path = _webHostEnvironment.WebRootPath + "\\Template\\" + file.Filename;

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
                }
            }

            return NotFound();
        }

        private void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[32768];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }

        /// <summary>
        /// Delete File
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        [HttpPost("Delete")]
        public Response Delete([FromBody] string filename)
        {
            using (ApitemplatereportContext context = new ApitemplatereportContext())
            {
                TblFileDetail file = context.TblFileDetails.Where(file => file.Filename == filename).FirstOrDefault();
                if (context.TblFileDetails.Contains(file))
                {
                    if (DeleteFile(filename) == true)
                    {
                        context.TblFileDetails.Remove(file);
                        context.SaveChanges();
                        return new Response
                        {
                            StatusCode = 200,
                            Content = $"{file.Filename} is deleted",
                            Message = "Delete Success"
                        };
                    }    
                }
            }

            return new Response { StatusCode = 404, Content = $"Error when delete {filename}", Message = "Delete Failed" };
        }

        private static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }

        private bool DeleteFile(string fileName)
        {
            if (!System.IO.File.Exists(Path.Combine(_webHostEnvironment.WebRootPath, "Template", fileName)))
                return false;
            
            System.IO.File.Delete(Path.Combine(_webHostEnvironment.WebRootPath, "Template", fileName));
            return true;
        }
    }
}
