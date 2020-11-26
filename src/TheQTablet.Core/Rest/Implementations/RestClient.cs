using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Base;
using MvvmCross.Logging;
using TheQTablet.Core.Rest.Interfaces;

namespace TheQTablet.Core.Rest.Implementations
{
    public class RestClient : IRestClient
    {
        private readonly IMvxJsonConverter _jsonConverter;
        private readonly IMvxLog _mvxLog;
        private readonly HttpClient _httpClient;

#if DEBUG
        private const string BASE_URL = "http://127.0.0.1:5002/qsim/";
#else

        private const string BASE_URL = "http://127.0.0.1:5002/qsim/"; // Production URL goes here
#endif

        public RestClient(IMvxJsonConverter jsonConverter, IMvxLog mvxLog)
        {
            _jsonConverter = jsonConverter;
            _mvxLog = mvxLog;
            _httpClient = new HttpClient();
        }

        public async Task<TResult> MakeApiCallAsync<TResult>(string url, HttpMethod method, object data = null) where TResult : class
        {
            url = string.Format("{0}{1}", BASE_URL, url);

            HttpClient httpClient = _httpClient;

            using (var request = new HttpRequestMessage { RequestUri = new Uri(url), Method = method })
            {
                // add content
                if (method != HttpMethod.Get)
                {
                    var json = _jsonConverter.SerializeObject(data);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                //_mvxLog.Trace("RestClient:MakeApiCallAsync: Creating HttpResponseMessage");
                var response = new HttpResponseMessage();
                try
                {
                    response.EnsureSuccessStatusCode();
                    response = await httpClient.SendAsync(request).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _mvxLog.ErrorException("MakeApiCall failed", ex);
                    _mvxLog.Trace("MakeApiCall failed", ex);

                    //TODO Handle Error Networks show error to user
                    throw ex;
                }



                //_mvxLog.Trace("RestClient:MakeApiCallAsync: awaiting response...");
                var stringSerialized = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                _mvxLog.Trace("RestClient:MakeApiCallAsync to :" + url + " with data:" + _jsonConverter.SerializeObject(data).ToString() + " response is: " + stringSerialized);

                try
                {
                    var result = _jsonConverter.DeserializeObject<TResult>(stringSerialized);
                    return result;
                }
                catch (Exception ex)
                {
                    _mvxLog.ErrorException("Parsing result error", ex);
                    _mvxLog.Trace("Parsing result error", ex.Message);

                    //TODO Handle Parsing Errors show error to user
                    throw ex;
                }

            }
        }
    }
}
