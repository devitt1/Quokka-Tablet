using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace TheQTablet.Core.Rest.Interfaces
{
    public interface IRestClient
    {
        Task<TResult> MakeApiCallAsync<TResult>(string url, HttpMethod method, object data = null)
                      where TResult : class;
    }
}
