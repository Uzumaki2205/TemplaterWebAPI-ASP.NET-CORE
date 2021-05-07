using Jwt_Core1.Models;
using Jwt_Core1.Models.Entities;
using Jwt_Core1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
        public Response GenerateToken([FromBody] UserLogin userParams)
        {
            TblUser u = new UserService().Authenticate(userParams.Username, userParams.Password);
            if(u != null)
                return new Response
                {
                    StatusCode = 200,
                    Content = _tokenService.GenerateToken(userParams.Username),
                    Message = "Token Generated Success"
                };
            return new Response();
        }

        [Authorize]
        [HttpPost("ValidateToken")]
        public Response ValidateToken([FromBody] UserLogin users)
        {
            string token = Request.Headers["Authorization"];
            var usernameRender = _tokenService.ValidateToken(token.Replace("Bearer ", ""));

            var user = new UserService().Authenticate(users.Username, users.Password);
            if(user != null && user.Username == usernameRender)
                return new Response { StatusCode = 200, Content = JsonConvert.SerializeObject(users), Message = "User Validated" };
            return new Response { StatusCode = 401, Content = "UnAuthenticate", Message = "Validate Failed!!" };
        }
    }
}
