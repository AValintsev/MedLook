using Baroque.NovaPoshta.Client;
using Baroque.NovaPoshta.Client.Domain;
using Baroque.NovaPoshta.Client.Services.Documents;
using Nop.Ithoot.Plugin.Shipping.NovaPoshta.Domain;

namespace Nop.Ithoot.Plugin.Shipping.NovaPoshta.Services
{
    public class NovaPoshtaDocumentService : DocumentService
    {
        private readonly INovaPoshtaGateway _novaPoshtaGateway;

        public NovaPoshtaDocumentService(INovaPoshtaGateway novaPoshtaGateway) : base(novaPoshtaGateway)
        {
            _novaPoshtaGateway = novaPoshtaGateway;
        }

        public IResponseEnvelope<NovaPoshtaCreateDocumentResponse.CreationResult> CreateDocument(NovaPoshtaCreateDocumentRequest createDocumentRequest)
        {
            RequestEnvelope<NovaPoshtaCreateDocumentRequest> request = new RequestEnvelope<NovaPoshtaCreateDocumentRequest>(createDocumentRequest)
            {
                ApiKey = _novaPoshtaGateway.ApiKey,
                CalledMethod = "save",
                ModelName = "InternetDocument"
            };
            return _novaPoshtaGateway.CreateRequest<NovaPoshtaCreateDocumentRequest, NovaPoshtaCreateDocumentResponse>(request);
        }
    }
}
