using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands
{
    public record CreateContactCommand(CreateContactDto Contact, string IpAddress)
        : IRequest<ApiResponse<Guid>>;

    public class CreateContactCommandHandler(
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        ILogger<CreateContactCommandHandler> logger
    ) : IRequestHandler<CreateContactCommand, ApiResponse<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IEmailService _emailService = emailService;
        private readonly ILogger<CreateContactCommandHandler> _logger = logger;

        public async Task<ApiResponse<Guid>> Handle(
            CreateContactCommand request,
            CancellationToken cancellationToken
        )
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                Contact contact = new()
                {
                    Id = Guid.NewGuid(),
                    Name = request.Contact.Name,
                    Email = request.Contact.Email,
                    Subject = request.Contact.Subject,
                    Message = request.Contact.Message,
                    CreatedAt = DateTime.UtcNow,
                    IpAddress = request.IpAddress,
                };

                await _unitOfWork.Contacts.AddAsync(contact);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                bool emailSent = await _emailService.SendContactEmailAsync(contact);
                if (!emailSent)
                {
                    _logger.LogWarning("Failed to send email for contact {ContactId}", contact.Id);
                }

                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation(
                    "Contact created successfully with ID: {ContactId}",
                    contact.Id
                );
                return ApiResponse<Guid>.SuccessResponse(
                    contact.Id,
                    "Contact message sent successfully"
                );
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating contact");
                return ApiResponse<Guid>.ErrorResponse(
                    "An error occurred while processing your contact message"
                );
            }
        }
    }
}
