using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace ToDoSlackerApp.Adapter.Slack
{
    public class SlackClient
    {
        private readonly Uri _uri;
        private readonly Encoding _encoding = new UTF8Encoding();

        public SlackClient(string urlWithAccessToken)
        {
            _uri = new Uri(urlWithAccessToken);
        }

        //Post a message using simple strings
        public void PostMessage(string text, string username = null, string channel = null)
        {
            Payload payload = new Payload()
            {
                Channel = channel,
                Username = username,
                Text = text
            };

            PostMessage(payload);
        }

        //Post a message using a Payload object
        public void PostMessage(Payload payload)
        {
            string payloadJson = JsonConvert.SerializeObject(payload);

            //using (HttpClient client = new HttpClient())
            //{
            //    //NameValueCollection data = new NameValueCollection();
            //    //data["payload"] = payloadJson;

            //    var response = client.PostAsync(_uri, new StringContent(payloadJson, Encoding.UTF8, "application/json"));

            //    response.

            //    if (response.IsSuccessStatusCode)
            //    {
            //        RandomString = await response.Content.ReadAsStringAsync();
            //    }
            //    //var response = client.UploadValues(_uri, "POST", data);

            //    //The response text is usually "ok"
            //    string responseText = _encoding.GetString(response.);
            //}
        }
    }
}