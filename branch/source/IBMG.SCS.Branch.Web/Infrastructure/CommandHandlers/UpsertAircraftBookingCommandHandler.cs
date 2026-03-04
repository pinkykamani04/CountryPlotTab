using BluQube.Commands;
using FluentValidation;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.CommandResults;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.Commands;
using IBMG.SCS.Infrastructure.DBContext;
using IBMG.SCS.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace IBMG.SCS.Branch.Web.Infrastructure.CommandHandlers
{
    public class UpsertAircraftBookingCommandHandler
        : CommandHandler<UpsertAircraftBookingCommand, UpsertAircraftBookingCommandResult>
    {
        private readonly PortalDBContext _context;

        public UpsertAircraftBookingCommandHandler(
            PortalDBContext context,
            IEnumerable<IValidator<UpsertAircraftBookingCommand>> validators,
            ILogger<UpsertAircraftBookingCommandHandler> logger) : base(validators, logger)
        {
            this._context = context;
        }

        protected override async Task<CommandResult<UpsertAircraftBookingCommandResult>> HandleInternal(
            UpsertAircraftBookingCommand request,
            CancellationToken cancellationToken)
        {
            AircraftBooking booking;

            if (request.Id == Guid.Empty)
            {
                booking = new AircraftBooking
                {
                    Id = Guid.NewGuid(),
                    AircraftId = request.AircraftId,
                    PilotId = request.PilotId,
                    TailNumber = request.TailNumber,
                    FromDate = request.FromDate,
                    FromTime = request.FromTime,
                    ToDate = request.ToDate,
                    ToTime = request.ToTime,
                    FromLocation = request.FromLocation,
                    ToLocation = request.ToLocation,
                    Status = request.Status,
                    IsRowDeleted = request.IsRowDeleted, 
                    CreatedBy = "System",
                    CreatedOn = DateTime.UtcNow,
                    ModifiedBy = null,
                    ModifiedOn = null,
                };

                this._context.AircraftBookings.Add(booking);
            }
            else
            {
                booking = await _context.AircraftBookings.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

                if (booking == null)
                {
                    return CommandResult<UpsertAircraftBookingCommandResult>.Succeeded(null);
                }

                booking.PilotId = request.PilotId;
                booking.FromDate = request.FromDate;
                booking.FromTime = request.FromTime;
                booking.ToDate = request.ToDate;
                booking.ToTime = request.ToTime;
                booking.FromLocation = request.FromLocation;
                booking.ToLocation = request.ToLocation;
                booking.Status = request.Status;
                booking.IsRowDeleted = request.IsRowDeleted;
                booking.ModifiedBy = "System";
                booking.ModifiedOn = DateTime.UtcNow;
            }

            await this._context.SaveChangesAsync(cancellationToken);

            return CommandResult<UpsertAircraftBookingCommandResult>.Succeeded(
                new UpsertAircraftBookingCommandResult(
                    booking.Id,
                    booking.AircraftId,
                    booking.PilotId,
                    booking.TailNumber,
                    booking.FromDate,
                    booking.FromTime,
                    booking.ToDate,
                    booking.ToTime,
                    booking.FromLocation,
                    booking.ToLocation,
                    booking.Status,
                    booking.IsRowDeleted,
                    booking.CreatedBy,
                    booking.CreatedOn,
                    booking.ModifiedBy,
                    booking.ModifiedOn
                )
            );
        }
    }
}