using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class TransferController : ControllerBase
    {
        private readonly ITransferDao TransferDao;
        public TransferController(ITransferDao transferDao)
        {
            TransferDao = transferDao;
        }
        [HttpPost("send")]
        public StringifiedTransfer SendFunds(Transfer transfer)
        {
            return TransferDao.SendFunds(transfer.Amount, GetUserIdFromToken(), transfer.AccountTo);
        }
        public int GetUserIdFromToken()
        {
            int userId = -1;
            try
            {
                userId = int.Parse(User.FindFirst("sub")?.Value);
            }
            catch
            {

            }
            return userId;
        }
        [HttpGet]
        public List<StringifiedTransfer> GetTransfers()
        {
            return TransferDao.GetTransfers(GetUserIdFromToken());
        }

        [HttpGet ("{id}")]
        public StringifiedTransfer GetTransferById(int transferId)
        {
            return TransferDao.GetTransferById(transferId);
        }
        [HttpPost("request")]
        public StringifiedTransfer RequestFunds(Transfer transfer)
        {
            return TransferDao.RequestFunds(transfer.Amount, GetUserIdFromToken(), transfer.AccountFrom);
        }
        [HttpGet("pending")]
        public List<StringifiedTransfer> GetPendingTransfers()
        {
            return TransferDao.GetPendingTransfers(GetUserIdFromToken());
        }
        [HttpPut("reject")]
        public StringifiedTransfer RejectTransferRequest(StringifiedTransfer transfer)
        {
            return TransferDao.RejectTransferRequest(transfer);
        }
        [HttpPut("accept")]
        public StringifiedTransfer AcceptTransferRequest(StringifiedTransfer transfer)
        {
            return TransferDao.AcceptTransferRequest(transfer);
        }
    }
}
