using RestSharp;
using System.Collections.Generic;
using TenmoClient.Models;

namespace TenmoClient.Services
{
    public class TenmoApiService : AuthenticatedApiService
    {
        public readonly string ApiUrl;

        public TenmoApiService(string apiUrl) : base(apiUrl) { }

        // Add methods to call api here...

        public decimal GetBalance(int id)
        {
            RestRequest request = new RestRequest($"account/{id}");
            IRestResponse<decimal> response = client.Get<decimal>(request);
            return response.Data;
        }
    }
}
