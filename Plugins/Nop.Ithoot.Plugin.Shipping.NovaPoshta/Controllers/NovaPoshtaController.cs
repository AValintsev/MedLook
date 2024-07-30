using Microsoft.AspNetCore.Mvc;
using Nop.Ithoot.Plugin.Shipping.NovaPoshta.Models;
using Nop.Ithoot.Plugin.Shipping.NovaPoshta.Services;
using System;
using System.Collections.Generic;

namespace Nop.Ithoot.Plugin.Shipping.NovaPoshta.Controllers
{
    public class NovaPoshtaController : Controller
    {
        private readonly INovaPoshtaService _novaPoshtaService;

        public NovaPoshtaController(INovaPoshtaService novaPoshtaService)
        {
            _novaPoshtaService = novaPoshtaService;
        }

        public IEnumerable<AutocompleteModel> GetCities(string term)
        {
            return _novaPoshtaService.FindCities(term);
        }

        public IEnumerable<AutocompleteModel> GetWirehouses(Guid cityId, string term)
        {
            return _novaPoshtaService.FindWirehouses(cityId, term);
        }
    }
}
