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
            RestRequest request = new RestRequest("accounts");
            IRestResponse<Account> response = client.Get<Account>(request);
            CheckForError(response);
            return response.Data;
        }
        public List<Account> GetAllAccountsExceptUser()
        {
            RestRequest request = new RestRequest("accounts/all");
            IRestResponse<List<Account>> response = client.Get<List<Account>>(request);
            CheckForError(response);
            return response.Data;
        }
        public bool TransferFunds(int accountToId, decimal transferAmount)
        {
            RestRequest request = new RestRequest("transfer/send");
            Transfer transfer = new Transfer();
            transfer.AccountTo = accountToId;
            transfer.Amount = transferAmount;
            request.AddJsonBody(transfer);
            IRestResponse<bool> response = client.Post<bool>(request);
            CheckForError(response);
            return response.Data;
        }
        public List<Transfer> GetTransfers()
        {
            RestRequest request = new RestRequest("transfer");
            List<Transfer> transfers = new List<Transfer>();
            IRestResponse<List<Transfer>> response = client.Get<List<Transfer>>(request);
            CheckForError(response);
            return response.Data;
        }

        public Transfer GetTransferById(int transferId)
        {
            RestRequest request = new RestRequest($"transfer/{transferId}");
            Transfer transfer = new Transfer();
            IRestResponse<Transfer> response = client.Get<Transfer>(request);
            CheckForError(response);
            return response.Data;
        }
    }
}
