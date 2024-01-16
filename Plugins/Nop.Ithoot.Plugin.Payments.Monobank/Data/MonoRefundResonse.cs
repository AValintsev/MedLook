using System;

namespace Nop.Ithoot.Plugin.Payments.Monobank.Data
{
    public class MonoRefundResonse
    {

        //Enum: "processing" "success" "failure"
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
