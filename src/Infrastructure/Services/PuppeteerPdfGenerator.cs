using System.Threading;
using System.Threading.Tasks;
using HtmlToPdf.Application.Interfaces;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;

namespace HtmlToPdf.Infrastructure.Services
{
    public class PuppeteerPdfGenerator : IPdfGenerator
    {
        private readonly ILogger<PuppeteerPdfGenerator> _logger;

        public PuppeteerPdfGenerator(ILogger<PuppeteerPdfGenerator> logger)
        {
            _logger = logger;
        }

        public async Task<byte[]> GeneratePdfFromHtmlAsync(string htmlContent, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting PDF generation using PuppeteerSharp.");

            // Download Chromium if not already present
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();

            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" } // Recommended for running in Docker/Servers
            });

            await using var page = await browser.NewPageAsync();
            
            // Set the HTML content
            await page.SetContentAsync(htmlContent, new NavigationOptions 
            {
                WaitUntil = new[] { WaitUntilNavigation.Networkidle0 } // Wait until all resources (images, css) are loaded
            });

            // Generate PDF
            var pdfOptions = new PdfOptions
            {
                Format = PuppeteerSharp.Media.PaperFormat.A4,
                PrintBackground = true,
                MarginOptions = new PuppeteerSharp.Media.MarginOptions
                {
                    Top = "20px",
                    Bottom = "20px",
                    Left = "20px",
                    Right = "20px"
                }
            };

            var pdfStream = await page.PdfStreamAsync(pdfOptions);
            
            // Convert stream to byte array
            using var memoryStream = new System.IO.MemoryStream();
            await pdfStream.CopyToAsync(memoryStream, cancellationToken);
            
            _logger.LogInformation("PDF generation completed successfully.");
            
            return memoryStream.ToArray();
        }
    }
}
