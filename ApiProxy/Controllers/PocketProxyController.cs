using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Threading.Tasks;
using System.Web.Http;

namespace ApiProxy.Controllers
{
    [RoutePrefix("api/pocketproxy")]
    public class PocketProxyController : ApiController
    {
        private ApiHttpClient Client => new ApiHttpClient(Request);
        private static string UriFromPage(string page) => $"https://getpocket.com/v3/{page}";
        private static string ConsumerKey => ConfigurationManager.AppSettings["PocketConsumerKey"];

        private Dictionary<string, string> GetHeaders()
        {
            return new Dictionary<string, string>() {
                {"X-Accept", "application/json"}
            };
        }

        private static object ModifyBody(dynamic body)
        {
            body.consumer_key = ConsumerKey;
            return body;
        }

        [HttpPost]
        [Route("{*page}", Name = "Proxy")]
        public async Task<object> Proxy([FromBody] ExpandoObject content, string page)
        {
            var uri = UriFromPage(page);
            var headers = GetHeaders();
            var body = ModifyBody(content);
            return await Client.PostAsync(uri, headers, body);
        }
    }
}
