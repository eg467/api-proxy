using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Http;

namespace ApiProxy.Controllers
{
    [RoutePrefix("api/jsonbin")]
    public class JsonbinController : ApiController
    {
        private ApiHttpClient Client => new ApiHttpClient(Request);
        private const string BaseUri = "https://api.jsonbin.io/b";
        private static string UriFromBinId(string binId) => $"{BaseUri}/{binId}";
        private static string SecretKey => ConfigurationManager.AppSettings["JsonbinSecretKey"];
        private const string BookmarkCollectionKey = "5ce92640b4565f1948046cb9";

        private Dictionary<string, string> BaseHeaders() =>
            new Dictionary<string, string>
            {
                {"secret-key", SecretKey}
            };

        [HttpGet]
        [Route("b/{id}", Name = "GetBin")]
        public async Task<object> GetBin(string id)
        {
            return await Client.GetAsync(UriFromBinId(id), BaseHeaders());
        }

        [HttpPost]
        [Route("b", Name = "CreateBin")]
        public async Task<object> CreateBin([FromBody]object data, bool @private = true)
        {
            var headers = BaseHeaders();
            headers.Add("collection-id", BookmarkCollectionKey);
            headers.Add("private", @private.ToString());
            return await Client.PostAsync(BaseUri, BaseHeaders(), data);
        }

        [HttpPut]
        [Route("b/{id}", Name = "UpdateBin")]
        public async Task<object> UpdateBin(string id, [FromBody]object data)
        {
            var headers = BaseHeaders();
            headers.Add("versioning", "false");
            return await Client.PutAsync(UriFromBinId(id), BaseHeaders(), data);
        }

        [HttpDelete]
        [Route("b/{id}", Name = "DeleteBin")]
        public async Task<object> DeleteBin(string id)
        {
            return await Client.DeleteAsync(UriFromBinId(id), BaseHeaders());
        }


    }
}
