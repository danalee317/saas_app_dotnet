using System.Text;
using System.Text.Json;

namespace MultiFamilyPortal.Extensions
{
    public static class HttpClientExtensions
    {
        private static readonly JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        public static Task<HttpResponseMessage> PostAsync(this HttpClient client, string requestUri, object body)
        {
            var json = JsonSerializer.Serialize(body, options);
            using var content = new StringContent(json, Encoding.Default, "application/json");
            return client.PostAsync(requestUri, content);
        }
    }
}
