using BluQube.Commands;
using FluentValidation;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.Commands;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.CommandResults;
using IBMG.SCS.Branch.Web.Services;
using IBMG.SCS.Infrastructure.DBContext;
using IBMG.SCS.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace IBMG.SCS.Branch.Web.Infrastructure.CommandHandlers
{
    public class UpsertPilotInformationCommandHandler
        : CommandHandler<UpsertPilotInformationCommand, UpsertPilotInformationCommandResult>
    {
        private readonly PortalDBContext _context;
        private readonly IProfileImageService _profileImageService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UpsertPilotInformationCommandHandler> _logger;

        public UpsertPilotInformationCommandHandler(
            PortalDBContext context,
            IProfileImageService profileImageService,
            IEnumerable<IValidator<UpsertPilotInformationCommand>> validators,
            ILogger<UpsertPilotInformationCommandHandler> logger,
            IHttpContextAccessor httpContextAccessor) : base(validators, logger)
        {
            this._context = context;
            this._profileImageService = profileImageService;
            this._httpContextAccessor = httpContextAccessor;
            this._logger = logger;
        }

        protected override async Task<CommandResult<UpsertPilotInformationCommandResult>> HandleInternal(
            UpsertPilotInformationCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var currentUser = this._httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

                var pilot = await this._context.PilotInformation
                    .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

                string? profileImageUrl = pilot?.ProfileImageName;

                if (request.ProfileImageBytes?.Length > 0 &&
               !string.IsNullOrWhiteSpace(request.ProfileImageFileName))
                {
                    profileImageUrl = await this._profileImageService.UploadAsync(
                        request.ProfileImageBytes,
                        request.ProfileImageFileName,
                        profileImageUrl,
                        cancellationToken);
                }

                if (pilot == null)
                {
                    pilot = new PilotInformation
                    {
                        Id = request.Id == Guid.Empty ? Guid.NewGuid() : request.Id,
                        FullName = request.FullName,
                        DateOfBirth = request.DateOfBirth,
                        ContactNumber = request.ContactNumber,
                        EmailAddress = request.EmailAddress,
                        ContactAddress = request.ContactAddress,
                        Location = request.Location,
                        LicenseNumber = request.LicenseNumber,
                        LicenseType = request.LicenseType,
                        LicenseInsuranceDate = request.LicenseInsuranceDate,
                        LicenseExpiryDate = request.LicenseExpiryDate,
                        MedicalCertificate = request.MedicalCertificate,
                        ProfileImageName = profileImageUrl,
                        CreatedBy = currentUser,
                        CreatedOn = DateTime.UtcNow,
                    };

                    this._context.PilotInformation.Add(pilot);
                }
                else
                {
                    pilot.FullName = request.FullName;
                    pilot.DateOfBirth = request.DateOfBirth;
                    pilot.ContactNumber = request.ContactNumber;
                    pilot.EmailAddress = request.EmailAddress;
                    pilot.ContactAddress = request.ContactAddress;
                    pilot.Location = request.Location;
                    pilot.LicenseNumber = request.LicenseNumber;
                    pilot.LicenseType = request.LicenseType;
                    pilot.LicenseInsuranceDate = request.LicenseInsuranceDate;
                    pilot.LicenseExpiryDate = request.LicenseExpiryDate;
                    pilot.MedicalCertificate = request.MedicalCertificate;

                    if (!string.IsNullOrEmpty(profileImageUrl))
                    {
                        pilot.ProfileImageName = profileImageUrl;
                    }

                    pilot.ModifiedBy = currentUser;
                    pilot.ModifiedOn = DateTime.UtcNow;
                }

                await this._context.SaveChangesAsync(cancellationToken);

                return CommandResult<UpsertPilotInformationCommandResult>.Succeeded(
                    new UpsertPilotInformationCommandResult(
                        pilot.Id,
                        pilot.FullName,
                        pilot.DateOfBirth,
                        pilot.ContactNumber,
                        pilot.EmailAddress,
                        pilot.ContactAddress,
                        pilot.Location,
                        pilot.LicenseNumber,
                        pilot.LicenseType,
                        pilot.LicenseInsuranceDate,
                        pilot.LicenseExpiryDate,
                        pilot.MedicalCertificate,
                        pilot.ProfileImageName,
                        pilot.CreatedBy,
                        pilot.CreatedOn,
                        pilot.ModifiedBy,
                        pilot.ModifiedOn
                    )
                );
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "UpsertPilotInformation failed");
                throw;
            }
        }
    }
}