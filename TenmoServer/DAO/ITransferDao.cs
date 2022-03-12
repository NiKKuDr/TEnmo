using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface ITransferDao
    {
        public StringifiedTransfer SendFunds(decimal transferAmount, int senderId, int recipientId);
        public List<StringifiedTransfer> GetTransfers(int userId);
        public Account GetAccountFromUserId(int userId);
        public StringifiedTransfer GetTransferById(int transferId);
        public StringifiedTransfer RequestFunds(decimal transferAmount, int requesterId, int requestFromAccount);
        public List<StringifiedTransfer> GetPendingTransfers(int userId);
    }
}
