using System;
using System.Collections.Generic;

namespace Nop.Ithoot.Plugin.Payments.Monobank.Data
{
    public class CancelList
    {
        public string Status { get; set; }
        public int Amount { get; set; }
        public int Ccy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ApprovalCode { get; set; }
        public string Rrn { get; set; }
        public string ExtRef { get; set; }
    }

    public class WalletData
    {
        public string CardToken { get; set; }
        public string WalletId { get; set; }
        public string Status { get; set; }
    }

    public class MonoCallback
    {
        public string InvoiceId { get; set; }
        public string Status { get; set; }
        public string FailureReason { get; set; }
        public string ErrCode { get; set; }
        public int Amount { get; set; }
        public int Ccy { get; set; }
        public int FinalAmount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Reference { get; set; }
        public List<CancelList> CancelList { get; set; }
        public WalletData WalletData { get; set; }
    }

}
