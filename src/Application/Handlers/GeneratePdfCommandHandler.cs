using MediatR;
using HtmlToPdf.Application.Interfaces;
using HtmlToPdf.Domain.Exceptions;
using System.Threading;
using System.Threading.Tasks;
using HtmlToPdf.Application.Commands;

namespace HtmlToPdf.Application.Handlers
{
    public class GeneratePdfCommandHandler : IRequestHandler<GeneratePdfCommand, byte[]>
    {
        private readonly IExternalDataClient _externalDataClient;
        private readonly IPdfGenerator _pdfGenerator;

        public GeneratePdfCommandHandler(IExternalDataClient externalDataClient, IPdfGenerator pdfGenerator)
        {
            _externalDataClient = externalDataClient;
            _pdfGenerator = pdfGenerator;
        }

        public async Task<byte[]> Handle(GeneratePdfCommand request, CancellationToken cancellationToken)
        {
            // 1. Fetch data/HTML from the public API based on the identifier
            var htmlContent = await _externalDataClient.FetchHtmlDataAsync(request.Identifier, cancellationToken);

            if (string.IsNullOrWhiteSpace(htmlContent))
            {
                throw new DataNotFoundException($"No data found for identifier: {request.Identifier}");
            }

            // 2. Convert the fetched HTML to PDF
            var pdfBytes = await _pdfGenerator.GeneratePdfFromHtmlAsync(htmlContent, cancellationToken);

            return pdfBytes;
        }
    }
}
