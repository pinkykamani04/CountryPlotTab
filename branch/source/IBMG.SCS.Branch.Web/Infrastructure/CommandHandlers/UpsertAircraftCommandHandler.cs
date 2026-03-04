using BluQube.Commands;
using FluentValidation;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.CommandResults;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.Commands;
using IBMG.SCS.Infrastructure.DBContext;
using IBMG.SCS.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace IBMG.SCS.Branch.Web.Infrastructure.CommandHandlers
{
    public class UpsertAircraftCommandHandler
          : CommandHandler<UpsertAircraftCommand, UpsertAircraftCommandResult>
    {
        private readonly PortalDBContext _context;

        public UpsertAircraftCommandHandler(
            PortalDBContext context,
            IEnumerable<IValidator<UpsertAircraftCommand>> validators,
            ILogger<UpsertAircraftCommandHandler> logger) : base(validators, logger)
        {
            this._context = context;
        }

        protected override async Task<CommandResult<UpsertAircraftCommandResult>> HandleInternal(
      UpsertAircraftCommand request,
      CancellationToken cancellationToken)
        {
            AircraftInfo aircraft;
            aircraft = await _context.AircraftInfo.FirstOrDefaultAsync(a => a.AircraftId == request.AircraftId && !a.IsRowDeleted, cancellationToken);

            if (aircraft == null)
            {
                aircraft = new AircraftInfo
                {
                    Id = Guid.NewGuid(),
                    AircraftId = request.AircraftId == Guid.Empty
                        ? Guid.NewGuid()
                        : request.AircraftId,

                    TailNumber = request.TailNumber,
                    Location = request.Location,
                    Model = request.Model,
                    Manufacturer = request.Manufacturer,
                    YearOfManufacture = request.YearOfManufacture,
                    AircraftType = request.AircraftType,
                    VariantType = request.VariantType,
                    TypeRating = request.TypeRating,
                    IsRowDeleted = false,
                    CreatedBy = "Test",
                    CreatedOn = DateTime.UtcNow,
                    ModifiedBy = null,
                    ModifiedOn = null,
                };

                this._context.AircraftInfo.Add(aircraft);
            }
            else
            {
                aircraft.TailNumber = request.TailNumber;
                aircraft.Location = request.Location;
                aircraft.Model = request.Model;
                aircraft.Manufacturer = request.Manufacturer;
                aircraft.YearOfManufacture = request.YearOfManufacture;
                aircraft.AircraftType = request.AircraftType;
                aircraft.VariantType = request.VariantType;
                aircraft.TypeRating = request.TypeRating;

                aircraft.ModifiedBy = "Admin";
                aircraft.ModifiedOn = DateTime.UtcNow;
            }

            await this._context.SaveChangesAsync(cancellationToken);

            return CommandResult<UpsertAircraftCommandResult>.Succeeded(
                new UpsertAircraftCommandResult(
                    aircraft.Id,
                    aircraft.AircraftId,
                    aircraft.TailNumber,
                    aircraft.Location,
                    aircraft.Model,
                    aircraft.YearOfManufacture,
                    aircraft.Manufacturer,
                    aircraft.AircraftType,
                    aircraft.VariantType,
                    aircraft.TypeRating,
                    aircraft.IsRowDeleted,
                    aircraft.CreatedBy,
                    aircraft.CreatedOn,
                    aircraft.ModifiedBy,
                    aircraft.ModifiedOn)
            );
        }
    }
}