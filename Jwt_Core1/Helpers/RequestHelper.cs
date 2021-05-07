using Jwt_Core1.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

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

        /// <summary>
        /// Post FormUrlEncoded
        /// </summary>
        /// <param name="uriAPI"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public Response PostRequest(string uriAPI, FormUrlEncodedContent content)
        {
            HttpClient client = factory.CreateClient("callapi");
            var response = client.PostAsync(uriAPI, content).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return JsonConvert.DeserializeObject<Response>(response.Content.ReadAsStringAsync().Result);
            return new Response();
        }

        /// <summary>
        /// Post Json Object
        /// </summary>
        /// <param name="uriAPI"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public Response PostRequest(string uriAPI, Object content)
        {
            HttpClient client = factory.CreateClient("callapi");
            var response = client.PostAsync(uriAPI, new StringContent(JsonConvert.SerializeObject(content), 
                System.Text.Encoding.UTF8, "application/json")).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return JsonConvert.DeserializeObject<Response>(response.Content.ReadAsStringAsync().Result);
            return new Response();
        }

        /// <summary>
        /// Post Json Object
        /// </summary>
        /// <param name="uriAPI"></param>
        /// <param name="token"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public Response PostRequest(string uriAPI, string token, Object content)
        {
            HttpClient client = factory.CreateClient("callapi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = client.PostAsync(uriAPI, new StringContent(JsonConvert.SerializeObject(content),
                System.Text.Encoding.UTF8, "application/json")).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return JsonConvert.DeserializeObject<Response>(response.Content.ReadAsStringAsync().Result);
            return new Response();
        }

        /// <summary>
        /// Post FormUrlEncoded With token
        /// </summary>
        /// <param name="uriAPI"></param>
        /// <param name="token"></param>
        /// <param name="content"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Post Request Stream With Param Object
        /// </summary>
        /// <param name="uriAPI"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<Response> PostRequestStream(string uriAPI, Object content)
        {
            HttpClient client = factory.CreateClient("callapi");
            var response = client.PostAsync(uriAPI, new StringContent(JsonConvert.SerializeObject(content),
                System.Text.Encoding.UTF8, "application/json")).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return new Response
                {
                    StatusCode = 200,
                    Content = await response.Content.ReadAsStreamAsync(),
                    Message = "Downloading"
                };
            return new Response();
        }

        /// <summary>
        /// Post request with param Object + token
        /// </summary>
        /// <param name="uriAPI"></param>
        /// <param name="token"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<Response> PostRequestStream(string uriAPI, string token, Object content)
        {
            HttpClient client = factory.CreateClient("callapi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = client.PostAsync(uriAPI, new StringContent(JsonConvert.SerializeObject(content),
                System.Text.Encoding.UTF8, "application/json")).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return new Response
                {
                    StatusCode = 200,
                    Content = await response.Content.ReadAsStreamAsync(),
                    Message = "Downloading"
                };
            return new Response();
        }

        //public async Task<Response> PostRequestStream(string uriAPI, string token, FormUrlEncodedContent content)
        //{
        //    HttpClient client = factory.CreateClient("callapi");
        //    client.DefaultRequestHeaders.Add("Accept", "application/x-www-form-urlencoded");
        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //    var response = client.PostAsync(uriAPI, content).Result;

        //    var stream = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(await response.Content.ReadAsStringAsync()));
        //    if (response.StatusCode == System.Net.HttpStatusCode.OK)
        //        return new Response
        //        {
        //            StatusCode = 200,
        //            Content = stream,
        //            Message = "Downloading"
        //        };
        //    return new Response();
        //}

        public Response PostRequest(string uriAPI, MultipartFormDataContent content)
        {
            HttpClient client = factory.CreateClient("callapi");
            client.DefaultRequestHeaders.Add("Accept", "application/x-www-form-urlencoded");
            var response = client.PostAsync(uriAPI, content).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return JsonConvert.DeserializeObject<Response>(response.Content.ReadAsStringAsync().Result);
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