using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Ithoot.Plugin.Payments.Monobank.Data
{
    public class MonoResponse
    {
        public string InvoiceId { get; set; }   
        public string PageUrl { get; set; }

        public string ErrCode { get; set; }

        public string ErrText { get; set; }
    }
}
