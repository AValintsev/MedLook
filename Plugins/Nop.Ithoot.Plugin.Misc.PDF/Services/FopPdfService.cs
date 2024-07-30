using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Common.Pdf;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using System.Threading.Tasks;

namespace Nop.Ithoot.Plugin.Misc.PDF.Services
{
    public partial class FopPdfService : PdfService
    {
        private readonly PDFSettings _settings;
        private readonly ILocalizationService _localizationService;

        public FopPdfService(AddressSettings addressSettings,
            CatalogSettings catalogSettings,
            CurrencySettings currencySettings,
            IAddressAttributeFormatter addressAttributeFormatter,
            IAddressService addressService,
            ICountryService countryService,
            ICurrencyService currencyService,
            IDateTimeHelper dateTimeHelper,
            IGiftCardService giftCardService,
            IHtmlFormatter htmlFormatter,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IMeasureService measureService,
            INopFileProvider fileProvider,
            IOrderService orderService,
            IPaymentPluginManager paymentPluginManager,
            IPaymentService paymentService,
            IPictureService pictureService,
            IPriceFormatter priceFormatter,
            IProductService productService,
            IRewardPointService rewardPointService,
            ISettingService settingService,
            IShipmentService shipmentService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            IStoreService storeService,
            IVendorService vendorService,
            IWorkContext workContext,
            MeasureSettings measureSettings,
            TaxSettings taxSettings,
            VendorSettings vendorSettings,
            PDFSettings settings)
            : base(addressSettings, catalogSettings, currencySettings, addressAttributeFormatter, addressService, countryService, currencyService, dateTimeHelper, giftCardService, htmlFormatter, languageService, localizationService, measureService, fileProvider, orderService, paymentPluginManager, paymentService, pictureService, priceFormatter, productService, rewardPointService, settingService, shipmentService, stateProvinceService, storeContext, storeService, vendorService, workContext, measureSettings, taxSettings, vendorSettings)
        {
            _localizationService = localizationService;
            _settings = settings;
        }

        protected override async Task<AddressItem> GetBillingAddressAsync(Vendor vendor, Language lang, Order order)
        {
            var addressResult = new AddressItem();

            addressResult.CustomValues.Add(
                (await _localizationService.GetLocaleStringResourceByNameAsync("Plugins.Misc.PDF.Fields.FopName", lang.Id)).ResourceValue,
                _settings.FopName);
            addressResult.CustomValues.Add(
                (await _localizationService.GetLocaleStringResourceByNameAsync("Plugins.Misc.PDF.Fields.FopPhone", lang.Id)).ResourceValue,
                _settings.FopPhone);
            addressResult.CustomValues.Add(
                (await _localizationService.GetLocaleStringResourceByNameAsync("Plugins.Misc.PDF.Fields.FopBankName", lang.Id)).ResourceValue,
                _settings.FopBankName);
            addressResult.CustomValues.Add(
                (await _localizationService.GetLocaleStringResourceByNameAsync("Plugins.Misc.PDF.Fields.FopIBAN", lang.Id)).ResourceValue,
                _settings.FopIBAN);
            addressResult.CustomValues.Add(
                (await _localizationService.GetLocaleStringResourceByNameAsync("Plugins.Misc.PDF.Fields.FopEDRPOU", lang.Id)).ResourceValue,
                _settings.FopEDRPOU);
            addressResult.CustomValues.Add(
                (await _localizationService.GetLocaleStringResourceByNameAsync("Plugins.Misc.PDF.Fields.FopCardNumber", lang.Id)).ResourceValue,
                _settings.FopCardNumber);

            return addressResult;
        }
    }
}
