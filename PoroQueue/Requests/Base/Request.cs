using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PoroQueue
{
    static class Request
    {
        private static AuthenticationHeaderValue AuthHeader;

        static Request()
        {
        }

        public static void SetUserData(string user, string password)
        {
            var BaseString = string.Format("{0}:{1}", user, password);
            var Base64Data = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(BaseString));
            AuthHeader = new AuthenticationHeaderValue("Basic", Base64Data);
        }

        public static async Task<string> Get(string URL)
        {
            HttpClient Client = new HttpClient();
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; }; // idc lol
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Client.Timeout = new TimeSpan(0, 0, 15);
            Client.DefaultRequestHeaders.Authorization = AuthHeader;

            using (Client)
            {
                HttpResponseMessage Response = await Client.GetAsync(URL);
                Response.EnsureSuccessStatusCode();

                return await Response.Content.ReadAsStringAsync();
            }
        }

        public static async Task<string> Put(string URL, string Body)
        {
            HttpClient Client = new HttpClient();
            StringContent Content = new StringContent(Body, System.Text.Encoding.UTF8, "application/json");
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; }; // idc lol
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Client.Timeout = new TimeSpan(0, 0, 15);
            Client.DefaultRequestHeaders.Authorization = AuthHeader;

            using (Client)
            {
                HttpResponseMessage Response = await Client.PutAsync(URL, Content);
                Response.EnsureSuccessStatusCode();

                return await Response.Content.ReadAsStringAsync();
            }
        }
    }
}
