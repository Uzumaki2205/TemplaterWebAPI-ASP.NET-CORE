using Jwt_Core1.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace Jwt_Core1.Helpers
{
    public class RequestHelper
    {
        private IHttpClientFactory factory;
        public RequestHelper(IHttpClientFactory factory)
        {
            this.factory = factory;
        }

        public Response GetRequest(string uriAPI)
        {
            HttpClient client = factory.CreateClient("callapi");
            var response = client.GetAsync(uriAPI).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return JsonConvert.DeserializeObject<Response>(response.Content.ReadAsStringAsync().Result);
            return new Response();
        }

        public Response GetRequest(string uriAPI, string token)
        {
            HttpClient client = factory.CreateClient("callapi");
            client.DefaultRequestHeaders.Add("Accept", "application/x-www-form-urlencoded");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = client.GetAsync(uriAPI).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return JsonConvert.DeserializeObject<Response>(response.Content.ReadAsStringAsync().Result);
            return new Response();
        }

        public Response PostRequest(string uriAPI, FormUrlEncodedContent content)
        {
            HttpClient client = factory.CreateClient("callapi");
            var response = client.PostAsync(uriAPI, content).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return JsonConvert.DeserializeObject<Response>(response.Content.ReadAsStringAsync().Result);
            return new Response();
        }

        public Response PostRequest(string uriAPI, string token, FormUrlEncodedContent content)
        {
            HttpClient client = factory.CreateClient("callapi");
            client.DefaultRequestHeaders.Add("Accept", "application/x-www-form-urlencoded");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = client.PostAsync(uriAPI, content).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return JsonConvert.DeserializeObject<Response>(response.Content.ReadAsStringAsync().Result);
            return new Response();
        }

        public async Task<Response> PostRequestStream(string uriAPI, string token, FormUrlEncodedContent content)
        {
            HttpClient client = factory.CreateClient("callapi");
            client.DefaultRequestHeaders.Add("Accept", "application/x-www-form-urlencoded");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = client.PostAsync(uriAPI, content).Result;

            var stream = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(await response.Content.ReadAsStringAsync()));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return new Response
                {
                    StatusCode = 200,
                    Content = stream,
                    Message = "Downloading"
                };
            return new Response();
        }


        public Response PostRequest(string uriAPI, string token, MultipartFormDataContent content)
        {
            HttpClient client = factory.CreateClient("callapi");
            client.DefaultRequestHeaders.Add("Accept", "application/x-www-form-urlencoded");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = client.PostAsync(uriAPI, content).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return JsonConvert.DeserializeObject<Response>(response.Content.ReadAsStringAsync().Result);
            return new Response();
        }
    }
}