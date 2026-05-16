using MediatR;
using Microsoft.AspNetCore.Mvc;
using HtmlToPdf.Application.Commands;
using System.Threading.Tasks;

namespace HtmlToPdf.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PdfController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PdfController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Generates a PDF based on the provided identifier (e.g., UserId or DocumentId).
        /// </summary>
        [HttpPost("generate")]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<IActionResult> GeneratePdf([FromBody] GeneratePdfRequest request)
        {
            var command = new GeneratePdfCommand { Identifier = request.Identifier };
            var pdfBytes = await _mediator.Send(command);

            return File(pdfBytes, "application/pdf", $"Document_{request.Identifier}.pdf");
        }
    }

    public class GeneratePdfRequest
    {
        public string Identifier { get; set; }
    }
}
