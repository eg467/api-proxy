using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace ApiProxy
{
    public class ApiHttpClient
    {
        private readonly HttpRequestMessage _request;

        public ApiHttpClient(HttpRequestMessage request)
        {
            _request = request;
        }

        public async Task<HttpResponseMessage> PostAsync(string uri, IReadOnlyDictionary<string, string> headers, object requestBody)
        {
            return await MakeCall(headers, client => client.PostAsJsonAsync(uri, requestBody));
        }

        public async Task<HttpResponseMessage> PutAsync(string uri, IReadOnlyDictionary<string, string> headers, object requestBody)
        {
            return await MakeCall(headers, client => client.PutAsJsonAsync(uri, requestBody));
        }

        public async Task<HttpResponseMessage> DeleteAsync(string uri, IReadOnlyDictionary<string, string> headers)
        {
            return await MakeCall(headers, client => client.DeleteAsync(uri));
        }

        public async Task<HttpResponseMessage> GetAsync(string uri, IReadOnlyDictionary<string, string> headers)
        {
            return await MakeCall(headers, client => client.DeleteAsync(uri));
        }

        private HttpResponseMessage CreateErrorResponse(HttpStatusCode code, Exception exception)
        {
#if DEBUG
            return _request.CreateErrorResponse(code, exception);
#else
            // Hide raw responses since they sometimes (especially JsonBin) return sensitive API keys.
            return _request.CreateErrorResponse(code, "There was an error processing your request.");
#endif
        }

        private async Task<HttpResponseMessage> MakeCall(IReadOnlyDictionary<string, string> headers, Func<HttpClient, Task<HttpResponseMessage>> httpCall)
        {
            try
            {
                var client = CreateClientWithHeaders();
                var response = await httpCall(client);
                await EnsureSuccessfulResponse(response);
                var responseBody = ReadResponseBody(response);
                return _request.CreateResponse(HttpStatusCode.OK, responseBody);
            }
            catch (HttpException exc)
            {
                return CreateErrorResponse((HttpStatusCode)exc.GetHttpCode(), exc);
            }
            catch (Exception exc)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, exc);
            }

            HttpClient CreateClientWithHeaders()
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();

                headers
                    .ToList()
                    .ForEach(kv => client.DefaultRequestHeaders.Add(kv.Key, kv.Value));
                return client;
            }

            async Task EnsureSuccessfulResponse(HttpResponseMessage response)
            {
                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    throw new HttpException((int)response.StatusCode, content);
                }
            }
        }

        public async Task<object> ReadResponseBody(HttpResponseMessage response)
        {
            return await response.Content.ReadAsAsync<object>();
        }
    }
}