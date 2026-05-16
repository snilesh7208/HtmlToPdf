using System.Threading;
using System.Threading.Tasks;

namespace HtmlToPdf.Application.Interfaces
{
    public interface IExternalDataClient
    {
        Task<string> FetchHtmlDataAsync(string identifier, CancellationToken cancellationToken);
    }

    public interface IPdfGenerator
    {
        Task<byte[]> GeneratePdfFromHtmlAsync(string htmlContent, CancellationToken cancellationToken);
    }
}
