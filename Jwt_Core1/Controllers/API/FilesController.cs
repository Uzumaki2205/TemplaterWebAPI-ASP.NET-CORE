using Jwt_Core1.Models;
using Jwt_Core1.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
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
                }

                return new Response { StatusCode = 200, Content = null, Message = "File Uploaded" };
            }

            return new Response();
        }

        [HttpPost("Download")]
        public IActionResult Download([FromForm] string filename)
        {
            using (ApitemplatereportContext context = new ApitemplatereportContext())
            {
                TblFileDetail file = context.TblFileDetails.Where(file => file.Filename == filename).FirstOrDefault();
                if(file != null)
                {
                    var path = _webHostEnvironment.WebRootPath + "\\Template\\" + file.Filename;
                    var stream = System.IO.File.OpenRead(path);
                    return new FileStreamResult(stream, "application/octet-stream");
                }            
            }

            return NotFound();
        }

        [HttpPost("Delete")]
        public Response Delete([FromForm] string filename)
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
