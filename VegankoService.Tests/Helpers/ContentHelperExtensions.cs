using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace VegankoService.Tests.Helpers
{
    public static class ContentHelperExtensions
    {
        public static StringContent GetStringContent(this object obj)
        {
            return new StringContent(JsonConvert.SerializeObject(obj), Encoding.Default, "application/json");
        }

        public static string GetJson(this HttpResponseMessage httpResponseMessage)
        {
            return httpResponseMessage.Content.ReadAsStringAsync()
                                              .GetAwaiter()
                                              .GetResult();
        }
    }
}
