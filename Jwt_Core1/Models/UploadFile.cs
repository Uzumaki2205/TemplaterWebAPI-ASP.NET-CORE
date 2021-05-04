using Microsoft.AspNetCore.Http;

namespace Jwt_Core1.Models
{
    public class UploadFile
    {
        public string  templatename { get; set; }
        public IFormFile file { get; set; }
    }
}
