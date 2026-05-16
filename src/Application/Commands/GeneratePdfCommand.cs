using MediatR;

namespace HtmlToPdf.Application.Commands
{
    public class GeneratePdfCommand : IRequest<byte[]>
    {
        /// <summary>
        /// The unique identifier (name, username, or ID) provided by the client.
        /// </summary>
        public string Identifier { get; set; }
    }
}
