using ChatBot.Serialization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace ChatBot.Services
{
    public class Luis
    {
        public static async Task<Utterance> GetResponse(string message)
        {
            using (var client = new HttpClient())
            {
                 

                var url = $"https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/31b36aa3-c6b4-4c8b-96f8-f83169a70422?subscription-key=841f08a1c9d34fb19860fe212d52f5b0&timezoneOffset=0&verbose=true&q={message}";
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode) return null;
                var result = await response.Content.ReadAsStringAsync();

                var js = new DataContractJsonSerializer(typeof(Utterance));
                var ms = new MemoryStream(Encoding.ASCII.GetBytes(result));
                var list = (Utterance)js.ReadObject(ms);

                return list;
            }
        }
    }
}