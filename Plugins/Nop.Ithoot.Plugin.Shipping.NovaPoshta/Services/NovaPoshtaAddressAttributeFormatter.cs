using Nop.Core.Domain.Catalog;
using Nop.Core;
using Nop.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.Html;
using Nop.Services.Localization;
using System.Text.Json;
using Nop.Ithoot.Plugin.Shipping.NovaPoshta.Models;

namespace Nop.Ithoot.Plugin.Shipping.NovaPoshta.Services
{
	public partial class NovaPoshtaAddressAttributeFormatter : AddressAttributeFormatter
	{
		#region Fields

		private readonly NovaPoshtaSettings _novaPoshtaSettings;

		private readonly IAddressAttributeParser _addressAttributeParser;
		private readonly IAddressAttributeService _addressAttributeService;
		private readonly IHtmlFormatter _htmlFormatter;
		private readonly ILocalizationService _localizationService;
		private readonly IWorkContext _workContext;

		#endregion

		public NovaPoshtaAddressAttributeFormatter(IAddressAttributeParser addressAttributeParser,
		   IAddressAttributeService addressAttributeService,
		   IHtmlFormatter htmlFormatter,
		   ILocalizationService localizationService,
		   IWorkContext workContext,
		   NovaPoshtaSettings novaPoshtaSettings) : base(addressAttributeParser, addressAttributeService, htmlFormatter, localizationService, workContext)
		{
			_addressAttributeParser = addressAttributeParser;
			_addressAttributeService = addressAttributeService;
			_htmlFormatter = htmlFormatter;
			_localizationService = localizationService;
			_workContext = workContext;
			_novaPoshtaSettings = novaPoshtaSettings;
		}

		/// <summary>
		/// Formats attributes
		/// </summary>
		/// <param name="attributesXml">Attributes in XML format</param>
		/// <param name="separator">Separator</param>
		/// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
		/// <returns>
		/// A task that represents the asynchronous operation
		/// The task result contains the attributes
		/// </returns>
		public override async Task<string> FormatAttributesAsync(string attributesXml,
			string separator = "<br />",
			bool htmlEncode = true)
		{
			var result = new StringBuilder();
			var currentLanguage = await _workContext.GetWorkingLanguageAsync();
			var attributes = await _addressAttributeParser.ParseAddressAttributesAsync(attributesXml);
			for (var i = 0; i < attributes.Count; i++)
			{
				var attribute = attributes[i];
				var valuesStr = _addressAttributeParser.ParseValues(attributesXml, attribute.Id);
				for (var j = 0; j < valuesStr.Count; j++)
				{
					var valueStr = valuesStr[j];
					var formattedAttribute = string.Empty;
					if (!attribute.ShouldHaveValues())
					{
						//no values
						if (attribute.AttributeControlType == AttributeControlType.MultilineTextbox)
						{
							//multiline textbox
							var attributeName = await _localizationService.GetLocalizedAsync(attribute, a => a.Name, currentLanguage.Id);
							//encode (if required)
							if (htmlEncode)
								attributeName = WebUtility.HtmlEncode(attributeName);
							formattedAttribute = $"{attributeName}: {_htmlFormatter.FormatText(valueStr, false, true, false, false, false, false)}";
							//we never encode multiline textbox input
						}
						else if (attribute.AttributeControlType == AttributeControlType.FileUpload)
						{
							//file upload
							//not supported for address attributes
						}
						else
						{
							//other attributes (textbox, datepicker)

							if (attribute.Id == _novaPoshtaSettings.CityAttributeId)
							{
								//valuesStr = JsonSerializer.Serialize<AutocompleteModel>(valuesStr);
								var obj = JsonSerializer.Deserialize<AutocompleteModel>(valueStr);
								valueStr = obj.value;
							}

							formattedAttribute = $"{await _localizationService.GetLocalizedAsync(attribute, a => a.Name, currentLanguage.Id)}: {valueStr}";
							//encode (if required)
							if (htmlEncode)
								formattedAttribute = WebUtility.HtmlEncode(formattedAttribute);
						}
					}
					else
					{
						if (attribute.Id == _novaPoshtaSettings.WarehouseAttributeId)
						{
							//valuesStr = JsonSerializer.Serialize<AutocompleteModel>(valuesStr);
							var obj = JsonSerializer.Deserialize<AutocompleteModel>(valueStr);
							formattedAttribute = obj.value;
						}
						else if (int.TryParse(valueStr, out var attributeValueId))
						{
							var attributeValue = await _addressAttributeService.GetAddressAttributeValueByIdAsync(attributeValueId);
							if (attributeValue != null)
							{
								formattedAttribute = $"{await _localizationService.GetLocalizedAsync(attribute, a => a.Name, currentLanguage.Id)}: {await _localizationService.GetLocalizedAsync(attributeValue, a => a.Name, currentLanguage.Id)}";
							}
							//encode (if required)
							if (htmlEncode)
								formattedAttribute = WebUtility.HtmlEncode(formattedAttribute);
						}
					}

					if (string.IsNullOrEmpty(formattedAttribute))
						continue;

					if (i != 0 || j != 0)
						result.Append(separator);

					result.Append(formattedAttribute);
				}
			}

			return result.ToString();
		}

	}
}
