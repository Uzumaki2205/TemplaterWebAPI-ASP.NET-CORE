using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Jwt_Core1.Services;
using Jwt_Core1.Models.Entities;
using System;
using System.Net.Http;
using System.Collections.Generic;
using Jwt_Core1.Helpers;
using Jwt_Core1.Models;
using Microsoft.AspNetCore.Http;

namespace Jwt_Core.Controllers.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private IHttpClientFactory factory;
        public UsersController(IHttpClientFactory factory)
        {
            this.factory = factory;
        }

        /// <summary>
        /// Login and create session
        /// </summary>
        /// <param name="userParams"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Login")]
        public Response Login([FromBody] UserLogin userParams)
        {
            //TblUser u = new UserService().Authenticate(userParams.Username, userParams.Password);

            //if(u != null)
            //{
                //var parameters = new Dictionary<string, string> { { "username", u.Username }, { "password", u.Password } };
                //var encodedContent = new FormUrlEncodedContent(parameters);

                var response = new RequestHelper(factory).PostRequest("api/Token/GenerateToken", userParams);
                if(response.StatusCode == 200)
                    return new Response() { StatusCode = 200, Message = "Login Success", Content = response.Content };  
            //}

            return new Response { StatusCode = 400, Message = "Fail To Login", Content = null };
        }
    }
}
