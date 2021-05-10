using Microsoft.AspNetCore.Http;

namespace Jwt_Core1.Models
{
    public class UploadFile
    {
        public string  templatename { get; set; }
        public IFormFile file { get; set; }
    }

    public class FileDownload
    {
        public string filename { get; set; }
    }

    public class FileGenerate
    {
        public string templatename { get; set; }
        public string content { get; set; }
    }
}
