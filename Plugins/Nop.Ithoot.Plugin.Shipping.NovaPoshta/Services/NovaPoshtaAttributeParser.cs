using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Services.Common;
using Nop.Services.Localization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nop.Ithoot.Plugin.Shipping.NovaPoshta.Services
{
    public class NovaPoshtaAttributeParser : AddressAttributeParser
    {
        private readonly IAddressAttributeService _addressAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly NovaPoshtaSettings _novaPoshtaSettings;


        public NovaPoshtaAttributeParser(
            IAddressAttributeService addressAttributeService,
            ILocalizationService localizationService,
            NovaPoshtaSettings novaPoshtaSettings)
            : base(addressAttributeService, localizationService)
        {
            _addressAttributeService = addressAttributeService;
            _localizationService = localizationService;
            _novaPoshtaSettings = novaPoshtaSettings;
        }

        public override async Task<string> ParseCustomAddressAttributesAsync(IFormCollection form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var attributesXml = string.Empty;

            foreach (var attribute in await _addressAttributeService.GetAllAddressAttributesAsync())
            {
                var controlId = string.Format(NopCommonDefaults.AddressAttributeControlName, attribute.Id);
                var attributeValues = form[controlId];
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                        if (!StringValues.IsNullOrEmpty(attributeValues)
                            && attribute.Id == _novaPoshtaSettings.WarehouseAttributeId)
                        {
                            attributesXml = AddAddressAttribute(attributesXml, attribute, attributeValues.ToString().Trim());
                        }
                        break;
                    case AttributeControlType.RadioList:
                        if (!StringValues.IsNullOrEmpty(attributeValues) && int.TryParse(attributeValues, out var value) && value > 0)
                            attributesXml = AddAddressAttribute(attributesXml, attribute, value.ToString());
                        break;

                    case AttributeControlType.Checkboxes:
                        foreach (var attributeValue in attributeValues.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (int.TryParse(attributeValue, out value) && value > 0)
                                attributesXml = AddAddressAttribute(attributesXml, attribute, value.ToString());
                        }
                        break;

                    case AttributeControlType.ReadonlyCheckboxes:
                        //load read-only (already server-side selected) values
                        var addressAttributeValues = await _addressAttributeService.GetAddressAttributeValuesAsync(attribute.Id);
                        foreach (var addressAttributeValue in addressAttributeValues)
                        {
                            if (addressAttributeValue.IsPreSelected)
                                attributesXml = AddAddressAttribute(attributesXml, attribute, addressAttributeValue.Id.ToString());
                        }
                        break;

                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        if (!StringValues.IsNullOrEmpty(attributeValues))
                        {
                            attributesXml = AddAddressAttribute(attributesXml, attribute, attributeValues.ToString().Trim());
                        }
                        break;

                    case AttributeControlType.Datepicker:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                    case AttributeControlType.FileUpload:
                    default:
                        break;
                }
            }

            return attributesXml;
        }

    }
}
