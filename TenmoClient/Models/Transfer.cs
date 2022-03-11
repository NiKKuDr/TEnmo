using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Models
{
    public class Transfer
    {
        public int TransferId { get; set; }
        public int TransferType { get; set; }
        public int TransferStatus { get; set; }
        public int AccountTo { get; set; }
        public int AccountFrom { get; set; }
        public decimal Amount { get; set; }
        public string TransferTypeDescription { get; set; }
        public string TransferStatusDescription { get; set; }
        public string OtherUserUsername { get; set; }
    }


}
