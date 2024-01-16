using System.Collections.Generic;

namespace Nop.Ithoot.Plugin.Payments.Monobank.Data
{
    public class MonoRequest
    {
        public string Token { get; set; }
        public string Cms { get; set; } = "nopCommerce";
        public string CmsVersion { get; set; } = "4.60";

        public PostData Data { get; set; }
    }

    public class PostData
    {
        public int Amount { get; set; }
        public int Ccy { get; set; } = 980;
        public MerchantPaymInfo MerchantPaymInfo { get; set; }
        public string RedirectUrl { get; set; }
        public string WebHookUrl { get; set; }
        public int Validity { get; set; }
        public string PaymentType { get; set; }
        public string QrId { get; set; }
        public string Code { get; set; }
        public SaveCardData SaveCardData { get; set; }
    }

    public class Discount
    {
        public string Type { get; set; }
        public string Mode { get; set; }
        public string Value { get; set; }
    }

    public class BasketOrder
    {
        public string Name { get; set; }
        public int Qty { get; set; }
        public int Sum { get; set; }
        public string Icon { get; set; }
        public string Unit { get; set; }
        public string Code { get; set; }
        public string Barcode { get; set; }
        public string Header { get; set; }
        public string Footer { get; set; }
        public List<object> Tax { get; set; }
        public string Uktzed { get; set; }
        public List<Discount> Discounts { get; set; }
    }

    public class MerchantPaymInfo
    {
        public string Reference { get; set; }
        public string Destination { get; set; }
        public string Comment { get; set; }
        public List<object> CustomerEmails { get; set; }
        public List<BasketOrder> BasketOrder { get; set; }
    }

    public class SaveCardData
    {
        public bool SaveCard { get; set; }
        public string WalletId { get; set; }
    }
}
