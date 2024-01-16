using System.Collections.Generic;

namespace Nop.Ithoot.Plugin.Payments.Monobank.Data
{
    public class MonoRefundRequestItem
    {
        public string Name { get; set; }
        public int Qty { get; set; }
        public int Sum { get; set; }
        public string Code { get; set; }
        public string Barcode { get; set; }
        public string Header { get; set; }
        public string Footer { get; set; }
        public List<int> Tax { get; set; }
        public string Uktzed { get; set; }
    }

    public class MonoRefundRequest
    {
        public string InvoiceId { get; set; }
        public string ExtRef { get; set; }
        public int? Amount { get; set; }
        public List<MonoRefundRequestItem> Items { get; set; }
    }
}

