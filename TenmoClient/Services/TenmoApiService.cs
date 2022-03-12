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
        public StringifiedTransfer TransferFunds(int accountToId, decimal transferAmount)
        {
            RestRequest request = new RestRequest("transfer/send");
            Transfer transfer = new Transfer();
            transfer.AccountTo = accountToId;
            transfer.Amount = transferAmount;
            request.AddJsonBody(transfer);
            IRestResponse<StringifiedTransfer> response = client.Post<StringifiedTransfer>(request);
            CheckForError(response);
            return response.Data;
        }
        public List<StringifiedTransfer> GetTransfers()
        {
            RestRequest request = new RestRequest("transfer");
            List<StringifiedTransfer> transfers = new List<StringifiedTransfer>();
            IRestResponse<List<StringifiedTransfer>> response = client.Get<List<StringifiedTransfer>>(request);
            CheckForError(response);
            return response.Data;
        }

        public StringifiedTransfer GetTransferById(int transferId)
        {
            RestRequest request = new RestRequest($"transfer/{transferId}");
            StringifiedTransfer transfer = new StringifiedTransfer();
            IRestResponse<StringifiedTransfer> response = client.Get<StringifiedTransfer>(request);
            CheckForError(response);
            return response.Data;
        }
        public StringifiedTransfer CreateTransferRequest(int requestAccountId, decimal transferAmount)
        {
            RestRequest request = new RestRequest("transfer/request");
            Transfer transfer = new Transfer();
            transfer.AccountFrom = requestAccountId;
            transfer.Amount = transferAmount;
            request.AddJsonBody(transfer);
            IRestResponse<StringifiedTransfer> response = client.Post<StringifiedTransfer>(request);
            CheckForError(response);
            return response.Data;
        }
        public List<StringifiedTransfer> GetPendingTransfers()
        {
            RestRequest request = new RestRequest("transfer/pending");
            IRestResponse<List<StringifiedTransfer>> response = client.Get<List<StringifiedTransfer>>(request);
            CheckForError(response);
            return response.Data;
        }
        public StringifiedTransfer RejectTransferRequest(StringifiedTransfer transfer)
        {
            RestRequest request = new RestRequest($"transfer/reject");
            request.AddJsonBody(transfer);
            IRestResponse<StringifiedTransfer> response = client.Put<StringifiedTransfer>(request);
            CheckForError(response);
            return response.Data;
        }
        public StringifiedTransfer AcceptTransferRequest(StringifiedTransfer transfer)
        {
            RestRequest request = new RestRequest($"transfer/accept");
            request.AddJsonBody(transfer);
            IRestResponse<StringifiedTransfer> response = client.Put<StringifiedTransfer>(request);
            CheckForError(response);
            return response.Data;
        }
    }
}
