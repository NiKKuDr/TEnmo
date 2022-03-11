using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface ITransferDao
    {
        public bool SendFunds(decimal transferAmount, int senderId, int recipientId);
        public List<Transfer> GetTransfers(int userId);
        public Account GetAccount(int id);
        public Transfer GetTransferById(int transferId);
    }
}
