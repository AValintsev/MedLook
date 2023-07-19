using Nop.Core.Configuration;

namespace Nop.Ithoot.Plugin.Misc.PDF
{
    /// <summary>
    /// Represents settings of PDF for FOP
    public class PDFSettings : ISettings
    {
        public string Title { get; set; }
        public string FopName { get; set; }
        public string FopPhone { get; set; }
        public string FopBankName { get; set; }
        public string FopIBAN { get; set; }
        public string FopEDRPOU { get; set; }
        public string FopCardNumber { get; set; }
    }
}