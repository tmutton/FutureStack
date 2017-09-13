using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ToDoGitterApp.Adapter.Gitter
{
    class GitterClient
    {
        //https://developer.gitter.im/apps
        private string _token = "";
        private string _requestUri = "https://api.gitter.im/v1/rooms/55252fbf15522ed4b3deafdb/chatMessages";

        public async Task<string> Send(string message)
        {
            if (string.IsNullOrEmpty(_token))
            {
                Console.WriteLine("**********************************************************************************");
                Console.WriteLine(message);
                Console.WriteLine("**********************************************************************************");

                return "done";
            }
            
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

                var response = client.PostAsync(_requestUri,
                    new StringContent($"{{\"text\":\"{message}\"}}", Encoding.UTF8, "application/json"));

                if (response.Result.IsSuccessStatusCode)
                {
                    return await response.Result.Content.ReadAsStringAsync();
                }

                return response.Result.ReasonPhrase;
            }
        }
    }
}
