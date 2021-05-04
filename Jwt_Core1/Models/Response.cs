using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            Content = null;
        }
    }
}
