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
        public Account GetAccount()
        {
            RestRequest request = new RestRequest($"account/{UserId}");
            IRestResponse<Account> response = client.Get<Account>(request);
            CheckForError(response);
            return response.Data;
        }

    }
}
