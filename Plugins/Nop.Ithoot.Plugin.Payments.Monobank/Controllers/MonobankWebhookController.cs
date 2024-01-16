using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Ithoot.Plugin.Payments.Monobank.Data;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Web.Framework.Controllers;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Nop.Ithoot.Plugin.Payments.Monobank.Controllers
{
    public class MonobankWebhookController : BaseController
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;


        #endregion

        #region Ctor

        public MonobankWebhookController(
            ILogger logger,
            IOrderService orderService,
            IOrderProcessingService orderProcessingService)
        {
            _logger = logger;
            _orderService = orderService;
            _orderProcessingService = orderProcessingService;
        }

        #endregion    

        #region Webhook

        [HttpPost]
        public async Task<IActionResult> Callback()
        {
            MonoCallback monoCallback = null;
            try
            {
                using (var streamReader = new StreamReader(HttpContext.Request.Body))
                {
                    var rawRequestString = await streamReader.ReadToEndAsync();
                    monoCallback = JsonConvert.DeserializeObject<MonoCallback>(rawRequestString);
                }

                if (monoCallback == null)
                {
                    throw new Exception("Request is empty");
                }
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync("MonoCallback error", ex);
                return new StatusCodeResult((int)HttpStatusCode.OK);
            }

            //Enum: "created" "processing" "hold" "success" "failure" "reversed" "expired"
            switch (monoCallback.Status)
            {
                case "success":
                    if (!int.TryParse(monoCallback.Reference, out int orderId))
                    {
                        await _logger.ErrorAsync("Monobank callback error. Data is not valid");
                        return new StatusCodeResult((int)HttpStatusCode.OK);
                    }

                    var order = await _orderService.GetOrderByIdAsync(orderId);
                    if (order == null)
                        return new StatusCodeResult((int)HttpStatusCode.OK);

                    if (_orderProcessingService.CanMarkOrderAsPaid(order))
                    {
                        order.CaptureTransactionId = monoCallback.InvoiceId;
                        await _orderService.UpdateOrderAsync(order);
                        await _orderProcessingService.MarkOrderAsPaidAsync(order);
                    }

                    break;
                case "failure":
                    await _logger.ErrorAsync($"Error while mono payment {monoCallback.ErrCode} {monoCallback.FailureReason}");
                    break;
            }

            return new StatusCodeResult((int)HttpStatusCode.OK);
        }

        #endregion
    }
}