using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlToPdf.Application.Interfaces;
using HtmlToPdf.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace HtmlToPdf.Infrastructure.Services
{
    public class ExternalDataClient : IExternalDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ExternalDataClient> _logger;

        public ExternalDataClient(HttpClient httpClient, ILogger<ExternalDataClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<string> FetchHtmlDataAsync(string identifier, CancellationToken cancellationToken)
        {
            // Treat the identifier as a URL if it starts with http, otherwise fetch from Wikipedia
            var requestUrl = identifier.StartsWith("http") ? identifier : $"https://en.wikipedia.org/wiki/{identifier}";
            
            _logger.LogInformation("Fetching data for identifier {Identifier} from {Url}", identifier, requestUrl);

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            requestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

            var response = await _httpClient.SendAsync(requestMessage, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new DataNotFoundException($"Document with ID {identifier} was not found on the external API.");
                }

                _logger.LogError("External API call failed with status code {StatusCode}", response.StatusCode);
                throw new DomainException("Failed to fetch data from the external API.");
            }

            var htmlContent = await response.Content.ReadAsStringAsync(cancellationToken);

            // Inject <base> tag to fix relative URLs (like CSS, images)
            var baseTag = $"<base href=\"{requestUrl}\" />";
            if (htmlContent.Contains("<head>", System.StringComparison.OrdinalIgnoreCase))
            {
                htmlContent = System.Text.RegularExpressions.Regex.Replace(htmlContent, "<head>", $"<head>{baseTag}", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }
            else
            {
                htmlContent = baseTag + htmlContent;
            }

            // Disable lazy loading for images so Puppeteer can capture them immediately
            htmlContent = htmlContent.Replace("loading=\"lazy\"", "loading=\"eager\"");

            return htmlContent;
        }
    }
}
