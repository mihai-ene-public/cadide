using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Common.API
{
    //purpose is to help for login into an API on our website
    //will allow to activate licenses
    public class APIHelper
    {

        public APIHelper()
        {
            Init();
        }

        HttpClient apiClient;

        void Init()
        {
            //todo: should be in config
            var api = "https://modernpcbstudio.com";

#if DEBUG
            api = "http://localhost:55555";
#endif

            apiClient = new HttpClient();
            apiClient.BaseAddress = new Uri(api);
            apiClient.DefaultRequestHeaders.Accept.Clear();
            apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<AuthenticatedUser> Authenticate(string username, string password)
        {
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>("grant_type","password"),
                new KeyValuePair<string,string>("username",username),
                new KeyValuePair<string,string>("password",password)
            });

            using (HttpResponseMessage response = await apiClient.PostAsync("/token", data))
            {
                if (response.IsSuccessStatusCode)
                {
                    //aspnet.webapi.client (nuget package) ReadAsAsync<string>()
                    var result = await response.Content.ReadAsAsync<AuthenticatedUser>();
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task<T> GetResponse<T>(string url, Dictionary<string, string> requestHeaders)
        {
            //var data = new FormUrlEncodedContent(requestHeaders);

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/{url}"))
            {
                foreach (var header in requestHeaders)
                    requestMessage.Headers.Add(header.Key, header.Value);

                using (HttpResponseMessage response = await apiClient.SendAsync(requestMessage))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        //aspnet.webapi.client (nuget package) ReadAsAsync<string>()
                        var result = await response.Content.ReadAsAsync<T>();
                        return result;
                    }
                    else
                    {
                        throw new Exception(response.ReasonPhrase);
                    }
                }
            }
        }
    }

    public class AuthenticatedUser
    {
        public string Access_Token { get; set; }

        public string UserName { get; set; }
    }
}
