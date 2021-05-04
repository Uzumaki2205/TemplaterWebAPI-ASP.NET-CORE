using Jwt_Core1.Models;
using Jwt_Core1.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Jwt_Core1.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private ITokenService _tokenService;

        public TokenController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("GenerateToken")]
        public Object GenerateToken([FromForm]string username)
        {
            return new Response { StatusCode = 200, Content = _tokenService.GenerateToken(username), Message = "Token Generated" };
        }

        [HttpPost("ValidateToken")]
        public Response ValidateToken([FromForm] string token, string password)
        {
            string username = _tokenService.ValidateToken(token);
            Models.Entities.TblUser user = new UserService().Authenticate(username, password);

            if(user != null)
                return new Response { StatusCode = 200, Content = _tokenService.ValidateToken(token), Message = "User validated" };
            return new Response { StatusCode = 401, Content = "UnAuthenticate", Message = "Validate Failed!!"};
        }
    }
}
