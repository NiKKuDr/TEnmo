using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class StringifiedTransfer
    {
        public int TransferId { get; set; }
        public string TransferType { get; set; }
        public string TransferStatus { get; set; }
        public string UserTo { get; set; }
        public string UserFrom { get; set; }
        public decimal Amount { get; set; }
    }
    public class Transfer
    {
        public int TransferId { get; set; }
        public int TransferType { get; set; }
        public int TransferStatus { get; set; }
        public int AccountFrom { get; set; }
        public int AccountTo { get; set; }
        public decimal Amount { get; set; }
    }
}
