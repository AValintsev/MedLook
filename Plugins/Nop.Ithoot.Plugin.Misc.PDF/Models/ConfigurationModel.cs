using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Ithoot.Plugin.Misc.PDF.Models
{
    public class ConfigurationModel
    {
        [NopResourceDisplayName("Plugins.Misc.PDF.Fields.Title")]
        public string Title { get; set; }
        
        [NopResourceDisplayName("Plugins.Misc.PDF.Fields.FopName")]
        public string FopName { get; set; }
        
        [NopResourceDisplayName("Plugins.Misc.PDF.Fields.FopPhone")]
        public string FopPhone { get; set; }
        
        [NopResourceDisplayName("Plugins.Misc.PDF.Fields.FopBankName")]
        public string FopBankName { get; set; }
        
        [NopResourceDisplayName("Plugins.Misc.PDF.Fields.FopIBAN")]
        public string FopIBAN { get; set; }
        
        [NopResourceDisplayName("Plugins.Misc.PDF.Fields.FopEDRPOU")]
        public string FopEDRPOU { get; set; }
        
        [NopResourceDisplayName("Plugins.Misc.PDF.Fields.FopCardNumber")]
        public string FopCardNumber { get; set; }
    }
}
