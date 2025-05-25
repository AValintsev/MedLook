using Nop.Core.Configuration;

namespace Nop.Ithoot.Plugin.Order.OneClick
{
    /// <summary>
    /// Represents settings of the "Fixed or by weight" shipping plugin
    /// </summary>
    public class OrderOneClickSettings : ISettings
    {
        public string NotificationEmails { get; set; }
    }
}