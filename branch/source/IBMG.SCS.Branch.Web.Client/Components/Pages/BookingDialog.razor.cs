using BluQube.Commands;
using BluQube.Queries;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.Queries;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.QueryResults;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;
using Radzen.Blazor;

namespace IBMG.SCS.Branch.Web.Client.Components.Pages
{
    public partial class BookingDialog : ComponentBase
    {
        [Inject]
        public DialogService DialogService { get; set; } = default!;

        [Inject]
        public NotificationService NotificationService { get; set; } = default!;

        [Parameter]
        public DateTime SelectedDate { get; set; }

        [Parameter]
        public AircraftInfoModel Aircraft { get; set; }

        [Parameter]
        public List<BookingDialogModel> ExistingBookings { get; set; } = new();

        [Parameter]
        public BookingDialogModel ExistingBooking { get; set; }

        [Parameter]
        public bool IsEditMode { get; set; } = false;

        [Inject]
        public IQuerier Querier { get; set; } = default!;

        [Inject]
        public ICommander Commander { get; set; } = default!;

        [Inject]
        public AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

        public DateTime FromDateTime { get; set; }

        public DateTime ToDateTime { get; set; }

        public RadzenTemplateForm<BookingDialogModel> bookingForm;

        public string FromLocation { get; set; }

        public string ToLocation { get; set; }

        public BookingDialogModel BookingModel { get; set; } = new BookingDialogModel();

        [Parameter]
        public Guid? PilotId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (this.IsEditMode && this.ExistingBooking != null)
            {
                this.FromDateTime = this.ExistingBooking.FromDate + this.ExistingBooking.FromTime;
                this.ToDateTime = this.ExistingBooking.ToDate + this.ExistingBooking.ToTime;

                this.FromLocation = this.ExistingBooking.FromLocation;
                this.ToLocation = this.ExistingBooking.ToLocation;

                var now = DateTime.Now;

                if (now >= this.FromDateTime && now <= this.ToDateTime)
                {
                    this.ShowError("This flight is currently in progress and cannot be edited.");
                    await Task.Delay(50);
                    this.DialogService.Close(null);
                    return;
                }

                if (now > this.ToDateTime)
                {
                    this.ShowError("Past flights cannot be edited.");
                    await Task.Delay(50);
                    this.DialogService.Close(null);
                    return;
                }
            }
            else
            {
                var today = DateTime.Today;
                var baseDate = this.SelectedDate >= today ? this.SelectedDate : today;

                this.FromDateTime = baseDate.AddHours(9);
                this.ToDateTime = baseDate.AddHours(11);

                this.FromLocation = string.Empty;
                this.ToLocation = string.Empty;
            }
        }

        private async Task<Guid?> GetLoggedInPilotIdAsync()
        {
            var authState = await this.AuthStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity == null || !user.Identity.IsAuthenticated)
            {
                return null;
            }

            var email =
                user.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ??
                user.FindFirst("email")?.Value ??
                user.FindFirst("preferred_username")?.Value ??
                user.FindFirst("upn")?.Value;

            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            var query = new GetPilotInformationQuery(
                Id: null,
                EmailAddress: email
            );

            var result = await this.Querier.Send<GetPilotInformationQueryResult>(query);

            var pilot = result?.Data?.Pilots?.FirstOrDefault();

            return pilot?.Id;
        }

        private async Task Submit()
        {
            var pilotId = this.PilotId ?? await this.GetLoggedInPilotIdAsync();
            if (pilotId == null)
            {
                this.ShowError("Unable to determine logged-in pilot.");
                return;
            }

            var fromDateTime = this.FromDateTime;
            var toDateTime = this.ToDateTime;

            if (!this.IsEditMode && fromDateTime < DateTime.Now)
            {
                this.ShowError("You cannot book a flight in the past.");
                return;
            }

            if (fromDateTime >= toDateTime)
            {
                this.ShowError("Arrival time must be after departure time.");
                return;
            }

            if (string.IsNullOrWhiteSpace(this.FromLocation))
            {
                this.ShowError("Departure Location is required.");
                return;
            }

            if (string.IsNullOrWhiteSpace(this.ToLocation))
            {
                this.ShowError("Arrival Location is required.");
                return;
            }

            if (this.FromLocation.Trim().Equals(this.ToLocation.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                this.ShowError("Departure and Arrival locations cannot be the same.");
                return;
            }

            var now = DateTime.Now;

            if (this.IsEditMode && fromDateTime <= now)
            {
                this.ShowError("You cannot edit a booking that is currently in progress or has already passed.");
                return;
            }

            foreach (var b in this.ExistingBookings)
            {
                if (this.IsEditMode && this.ExistingBooking != null && b.Id == this.ExistingBooking.Id)
                {
                    continue;
                }

                var existingFrom = b.FromDate.Add(b.FromTime);
                var existingTo = b.ToDate.Add(b.ToTime);

                bool overlap = fromDateTime < existingTo && toDateTime > existingFrom;
                if (!overlap)
                {
                    continue;
                }

                if (b.AircraftId == this.Aircraft.AircraftId)
                {
                    this.ShowError(
                        $"Aircraft {this.Aircraft.TailNumber} is already booked from {existingFrom:dd MMM yyyy HH:mm} to {existingTo:dd MMM yyyy HH:mm}."
                    );
                    return;
                }

                if (b.PilotId == pilotId)
                {
                    this.ShowError(
                        $"You already have a booking from {existingFrom:dd MMM yyyy HH:mm} to {existingTo:dd MMM yyyy HH:mm}."
                    );
                    return;
                }
            }

            var result = new BookingDialogModel
            {
                Id = this.IsEditMode && this.ExistingBooking != null
                    ? this.ExistingBooking.Id
                    : Guid.Empty,

                FromDate = fromDateTime.Date,
                FromTime = fromDateTime.TimeOfDay,

                ToDate = toDateTime.Date,
                ToTime = toDateTime.TimeOfDay,

                FromLocation = this.FromLocation.Trim(),
                ToLocation = this.ToLocation.Trim(),

                PilotId = pilotId.Value,
                IsRowDeleted = false,
            };

            this.DialogService.Close(result);
        }

        private async Task Delete()
        {
            if (!this.IsEditMode || this.ExistingBooking == null)
            {
                return;
            }

            bool? confirmed = await this.DialogService.Confirm(
                "Are you sure you want to delete this booking?",
                "Confirm Delete",
                new ConfirmOptions
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    Icon = "warning",
                }
            );

            if (confirmed == true)
            {
                this.NotificationService.Notify(NotificationSeverity.Success,
                    "Deleted",
                    "Booking removed successfully");

                this.DialogService.Close(new BookingDialogModel
                {
                    Id = this.ExistingBooking.Id,
                    IsRowDeleted = true,
                });
            }
        }

        private void ShowError(string message)
        {
            this.NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = "Validation Error",
                Detail = message,
                Duration = 4000,
            });
        }

        public void Cancel()
        {
            this.DialogService.Close(null);
        }
    }
}