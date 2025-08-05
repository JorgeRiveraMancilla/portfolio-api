using Application.Commands;
using Application.Common;
using Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController(IMediator mediator, ILogger<ContactController> logger)
        : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger<ContactController> _logger = logger;

        [HttpPost]
        [EnableRateLimiting("contact")]
        public async Task<ActionResult<ApiResponse<Guid>>> CreateAsync(
            [FromBody] CreateContactDto dto
        )
        {
            string ipAddress =
                HttpContext.Connection.RemoteIpAddress?.ToString()
                ?? throw new Exception("Remote IP address not found");

            _logger.LogInformation("Received contact request from IP: {IpAddress}", ipAddress);

            CreateContactCommand command = new(dto, ipAddress);
            ApiResponse<Guid> result = await _mediator.Send(command);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}
