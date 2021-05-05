using System;

namespace Jwt_Core1.Models
{
    public class Response
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public Object Content { get; set; }

        public Response()
        {
            StatusCode = 404;
            Message = "Fail";
            Content = "Error when try bad request!!";
        }
    }
}
