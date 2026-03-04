using Blazored.SessionStorage;
using BluQube.Commands;
using BluQube.Constants;
using BluQube.Queries;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.Commands;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.Queries;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.QueryResults;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;

namespace IBMG.SCS.Branch.Web.Client.Components.Pages
{
    public partial class AircraftDashboard
    {
        [Inject]
        public ICommander Commander { get; set; } = default!;

        [Inject]
        public IQuerier Querier { get; set; } = default!;

        [Inject]
        public DialogService DialogService { get; set; } = default!;

        [Inject]
        public NotificationService NotificationService { get; set; } = default!;

        [Inject]
        public ISessionStorageService SessionStorage { get; set; } = default!;

        [Inject]
        public AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

        public Guid? CurrentPilotId { get; set; }

        public const string AircraftStorageKey = "LastSelectedAircraftId";

        public bool IsEditMode { get; set; } = false;

        public bool IsAddMode { get; set; } = false;

        public AircraftInfoModel Aircraft { get; set; } = new AircraftInfoModel();

        public AircraftInfoModel SelectedAircraft { get; set; }

        public Guid SelectedAircraftId { get; set; }

        public List<AircraftInfoModel> AircraftList { get; set; } = new();

        public string AircraftHeaderText { get; set; } = string.Empty;

        public static Guid LastSelectedAircraftId;

        public List<BookingInfoModel> CurrentWeekBookings { get; set; } = new();

        public bool IsDropdownOpen { get; set; } = false;

        protected bool IsPageLoading = true;


        public DateTime CurrentWeekStart;
        public List<DateTime> WeekDays = new();
        public string _previousAircraftLocation;
        public string TotalFlyingHours = "00:00";
        public int BookingsThisYear = 0;
        public int TotalBookingsLifetime = 0;

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

        public string WeekDisplayText =>
            this.WeekDays.Any()
                ? $"{this.WeekDays.First():dd MMM} - {this.WeekDays.Last():dd MMM, yyyy}"
                : string.Empty;

        protected override async Task OnInitializedAsync()
        {
            this.IsPageLoading = true;
            this.SetWeek(DateTime.Today);
            this.CurrentPilotId = await this.GetLoggedInPilotIdAsync();
            await this.LoadAircraftList();
            await this.LoadBookingsForCurrentWeek();
            await this.UpdateStatistics();
            this.IsPageLoading = false;
        }

        protected override async Task OnParametersSetAsync()
        {
            if (this.Aircraft != null)
            {
                var currentLocation = this.Aircraft.Location ?? string.Empty;

                if (this._previousAircraftLocation != currentLocation)
                {
                    this._previousAircraftLocation = currentLocation;
                    await this.LoadBookingsForCurrentWeek();
                    this.StateHasChanged();
                }
            }
        }

        private void SetWeek(DateTime date)
        {
            var diff = DayOfWeek.Sunday - date.DayOfWeek;
            this.CurrentWeekStart = date.AddDays(diff);
            this.WeekDays = Enumerable.Range(0, 7).Select(i => this.CurrentWeekStart.AddDays(i)).ToList();
        }

        private async Task PreviousWeek()
        {
            this.SetWeek(this.CurrentWeekStart.AddDays(-7));
            await this.LoadBookingsForCurrentWeek();
            this.StateHasChanged();
        }

        private async Task NextWeek()
        {
            this.SetWeek(this.CurrentWeekStart.AddDays(7));
            await this.LoadBookingsForCurrentWeek();
            this.StateHasChanged();
        }

        private async Task HandleBookingClick((DateTime day, BookingInfoModel? booking) args)
        {
            await OpenBookingDialog(args.day, args.booking);
        }

        private async Task LoadBookingsForCurrentWeek()
        {
            if (this.Aircraft == null || this.Aircraft.AircraftId == Guid.Empty)
            {
                this.CurrentWeekBookings = new List<BookingInfoModel>();
                return;
            }

            var bookingQuery = new GetAllAircraftBookingQuery(
                null,
                this.Aircraft.AircraftId,
                CurrentPilotId
            );

            var bookingResult = await this.Querier.Send<GetAllAircraftBookingQueryResult>(bookingQuery);

            if (bookingResult?.Data?.Bookings != null)
            {
                var weekStart = this.WeekDays.First().Date;
                var weekEnd = this.WeekDays.Last().Date.AddDays(1);

                this.CurrentWeekBookings = bookingResult.Data.Bookings
                    .Where(b => !b.IsRowDeleted)
                    .Where(b =>
                    {
                        var bookingStart = b.FromDate.Date;
                        var bookingEnd = b.ToDate.Date;
                        return bookingStart < weekEnd && bookingEnd >= weekStart;
                    })
                    .Select(b => new BookingInfoModel
                    {
                        Id = b.Id,
                        FromDate = b.FromDate.Date + b.FromTime,
                        ToDate = b.ToDate.Date + b.ToTime,
                        FromLocation = b.FromLocation,
                        ToLocation = b.ToLocation,
                        Status = b.Status,
                        PilotId = b.PilotId,
                    })
                    .ToList();
            }
            else
            {
                this.CurrentWeekBookings = new List<BookingInfoModel>();
            }
        }

        private async Task UpdateStatistics()
        {
            if (this.Aircraft == null || this.Aircraft.AircraftId == Guid.Empty)
            {
                this.TotalFlyingHours = "00:00";
                this.BookingsThisYear = 0;
                this.TotalBookingsLifetime = 0;
                return;
            }

            if (this.CurrentPilotId == null)
            {
                this.TotalFlyingHours = "00:00";
                this.BookingsThisYear = 0;
                this.TotalBookingsLifetime = 0;
                return;
            }

            var bookingQuery = new GetAllAircraftBookingQuery(
                Id: null,
                AircraftId: this.Aircraft.AircraftId,
                PilotId: CurrentPilotId
            );

            var bookingResult = await this.Querier.Send<GetAllAircraftBookingQueryResult>(bookingQuery);

            if (bookingResult?.Data?.Bookings != null)
            {
                var bookings = bookingResult.Data.Bookings.Where(b => !b.IsRowDeleted).ToList();
                this.TotalBookingsLifetime = bookings.Count;
                var currentYear = DateTime.Now.Year;
                this.BookingsThisYear = bookings.Count(b => b.FromDate.Year == currentYear);
                TimeSpan totalHours = TimeSpan.Zero;
                foreach (var booking in bookings)
                {
                    var bookingStart = booking.FromDate.Date + booking.FromTime;
                    var bookingEnd = booking.ToDate.Date + booking.ToTime;
                    var duration = bookingEnd - bookingStart;
                    totalHours = totalHours.Add(duration);
                }

                this.TotalFlyingHours = $"{(int)totalHours.TotalHours:D2}:{totalHours.Minutes:D2}";

                this.StateHasChanged();
            }
        }

        private async Task OpenBookingDialog(DateTime day, BookingInfoModel existingBooking = null)
        {
            bool isEditMode = existingBooking != null;

            if (isEditMode && existingBooking != null)
            {
                var now = DateTime.Now;
                var fromDateTime = existingBooking.FromDate + existingBooking.FromTime;
                var toDateTime = existingBooking.ToDate + existingBooking.ToTime;

                if (now >= fromDateTime && now <= toDateTime)
                {
                    this.ShowNotification(NotificationSeverity.Error,
                        "Cannot Edit",
                        "This flight is currently in progress and cannot be edited.");
                    return;
                }

                if (now > toDateTime)
                {
                    this.ShowNotification(NotificationSeverity.Error,
                        "Cannot Edit",
                        "Past flights cannot be edited.");
                    return;
                }
            }

            if (this.CurrentPilotId == null)
            {
                this.ShowNotification(NotificationSeverity.Error,
                    "Pilot Not Found",
                    "Logged-in pilot information could not be found.");
                return;
            }

            var aircraftBookingQuery = new GetAllAircraftBookingQuery(
                Id: null,
                AircraftId: this.Aircraft.AircraftId,
                PilotId: null
            );

            var aircraftBookingResult =
                await this.Querier.Send<GetAllAircraftBookingQueryResult>(aircraftBookingQuery);

            var pilotBookingQuery = new GetAllAircraftBookingQuery(
                Id: null,
                AircraftId: null,
                PilotId: CurrentPilotId
            );

            var pilotBookingResult =
                await this.Querier.Send<GetAllAircraftBookingQueryResult>(pilotBookingQuery);

            var existingBookings =
                aircraftBookingResult?.Data?.Bookings
                    ?.Concat(pilotBookingResult?.Data?.Bookings ?? Enumerable.Empty<GetAllAircraftBookingQueryResult.AircraftBookingItemDto>())
                    .Where(b => !b.IsRowDeleted)
                    .Select(b => new BookingDialogModel
                    {
                        Id = b.Id,
                        AircraftId = b.AircraftId,
                        FromDate = b.FromDate,
                        FromTime = b.FromTime,
                        ToDate = b.ToDate,
                        ToTime = b.ToTime,
                        FromLocation = b.FromLocation,
                        ToLocation = b.ToLocation,
                        PilotId = b.PilotId,
                    })
                    .ToList()
                ?? new List<BookingDialogModel>();

            var parameters = new Dictionary<string, object>
            {
                { "SelectedDate", day },
                { "Aircraft", this.Aircraft },
                { "ExistingBookings", existingBookings },
                { "IsEditMode", isEditMode },
                { "PilotId", this.CurrentPilotId},
            };

            if (isEditMode)
            {
                parameters.Add("ExistingBooking", new BookingDialogModel
                {
                    Id = existingBooking.Id,
                    AircraftId = existingBooking.AircraftId,
                    FromDate = existingBooking.FromDate.Date,
                    FromTime = existingBooking.FromDate.TimeOfDay,
                    ToDate = existingBooking.ToDate.Date,
                    ToTime = existingBooking.ToDate.TimeOfDay,
                    FromLocation = existingBooking.FromLocation,
                    ToLocation = existingBooking.ToLocation,
                    PilotId = existingBooking.PilotId,
                });
            }

            var dialogResult = await this.DialogService.OpenAsync<BookingDialog>(
                "Booking",
                parameters,
                new DialogOptions
                {
                    ShowTitle = false,
                    CloseDialogOnEsc = true,
                }
            );

            if (dialogResult is not BookingDialogModel result)
            {
                return;
            }

            if (result.IsRowDeleted)
            {
                await this.DeleteBooking(result.Id);
                return;
            }

            await this.SaveBooking(result, isEditMode);
        }

        private async Task SaveBooking(BookingDialogModel result, bool isEditMode)
        {
            var newFrom = result.FromDate.Date + result.FromTime;
            var newTo = result.ToDate.Date + result.ToTime;

            if (newFrom >= newTo)
            {
                this.ShowNotification(NotificationSeverity.Warning, "Invalid Time",
                    "From Date & Time must be earlier than To Date & Time.");
                return;
            }

            if (!isEditMode && newFrom < DateTime.Now)
            {
                this.ShowNotification(NotificationSeverity.Warning, "Past Booking",
                    "You cannot create a booking in the past.");
                return;
            }

            if (string.IsNullOrWhiteSpace(result.FromLocation) || string.IsNullOrWhiteSpace(result.ToLocation))
            {
                this.ShowNotification(NotificationSeverity.Warning, "Missing Locations",
                    "Departure and Arrival locations are required.");
                return;
            }

            if (result.FromLocation.Trim().Equals(result.ToLocation.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                this.ShowNotification(NotificationSeverity.Warning, "Invalid Locations",
                    "Departure and Arrival locations cannot be the same.");
                return;
            }

            if (this.CurrentPilotId == null)
            {
                this.ShowNotification(NotificationSeverity.Error,
                    "Pilot Not Found",
                    "Logged-in pilot information could not be found.");
                return;
            }

            var aircraftBookingQuery = new GetAllAircraftBookingQuery(
                Id: null,
                AircraftId: this.Aircraft.AircraftId,
                PilotId: null
            );

            var aircraftBookingResult =
                await this.Querier.Send<GetAllAircraftBookingQueryResult>(aircraftBookingQuery);

            var pilotBookingQuery = new GetAllAircraftBookingQuery(
                Id: null,
                AircraftId: null,
                PilotId: CurrentPilotId
            );

            var pilotBookingResult =
                await this.Querier.Send<GetAllAircraftBookingQueryResult>(pilotBookingQuery);

            var existingBookings =
                aircraftBookingResult?.Data?.Bookings
                    ?.Concat(pilotBookingResult?.Data?.Bookings ?? Enumerable.Empty<GetAllAircraftBookingQueryResult.AircraftBookingItemDto>())
                    .Where(b => !b.IsRowDeleted)
                    .Where(b => !isEditMode || b.Id != result.Id)
                    .ToList()
                ?? new List<GetAllAircraftBookingQueryResult.AircraftBookingItemDto>();

            var pilotId = await this.GetLoggedInPilotIdAsync();

            if (pilotId == null)
            {
                this.ShowNotification(NotificationSeverity.Error,
                    "Pilot Not Found",
                    "Logged-in pilot information could not be found.");
                return;
            }

            result.PilotId = pilotId.Value;

            foreach (var existing in existingBookings)
            {
                var existingFrom = existing.FromDate.Date + existing.FromTime;
                var existingTo = existing.ToDate.Date + existing.ToTime;

                bool overlap = newFrom < existingTo && newTo > existingFrom;

                if (!overlap)
                {
                    continue;
                }

                if (existing.AircraftId == this.Aircraft.AircraftId)
                {
                    this.ShowNotification(NotificationSeverity.Error, "Booking Conflict",
                        $"Aircraft {this.Aircraft.TailNumber} is already booked " +
                        $"from {existingFrom:dd MMM yyyy HH:mm} to {existingTo:dd MMM yyyy HH:mm}.");
                    return;
                }

                if (existing.PilotId == pilotId)

                {
                    this.ShowNotification(NotificationSeverity.Error, "Booking Conflict",
                        $"Pilot is already booked on another aircraft " +
                        $"from {existingFrom:dd MMM yyyy HH:mm} to {existingTo:dd MMM yyyy HH:mm}.");
                    return;
                }

            }

            await this.Commander.Send(new UpsertAircraftBookingCommand(
               result.Id,
               Aircraft.AircraftId,
               result.PilotId,
               Aircraft.TailNumber,
               result.FromDate,
               result.FromTime,
               result.ToDate,
               result.ToTime,
               result.FromLocation,
               result.ToLocation,
               "Booked",
               result.IsRowDeleted,
               isEditMode ? null : "Admin",
               isEditMode ? DateTime.MinValue : DateTime.UtcNow,
               isEditMode ? "Admin" : null,
               isEditMode ? DateTime.UtcNow : DateTime.MinValue
            ));

            this.ShowNotification(NotificationSeverity.Success,
                isEditMode ? "Booking Updated" : "Booking Confirmed",
                $"Flight booking {(isEditMode ? "updated" : "saved")} successfully.");
            await this.LoadBookingsForCurrentWeek();
            await this.UpdateStatistics();
            this.StateHasChanged();
        }

        private async Task DeleteBooking(Guid bookingId)
        {
            var bookingQuery = new GetAllAircraftBookingQuery(Id: bookingId, AircraftId: null, PilotId: null);
            var bookingResult = await this.Querier.Send<GetAllAircraftBookingQueryResult>(bookingQuery);
            var booking = bookingResult?.Data?.Bookings?.FirstOrDefault();

            if (booking == null)
            {
                this.ShowNotification(NotificationSeverity.Error, "Error", "Booking not found.");
                return;
            }

            await this.Commander.Send(new UpsertAircraftBookingCommand(
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
                true,
                booking.CreatedBy,
                booking.CreatedOn,
                "Admin",
                DateTime.UtcNow
            ));

            this.ShowNotification(NotificationSeverity.Success, "Booking Deleted",
                "Flight booking deleted successfully.");

            await this.LoadBookingsForCurrentWeek();
            await this.UpdateStatistics();
            this.StateHasChanged();
        }

        private void ShowNotification(NotificationSeverity severity, string summary, string detail)
        {
            this.NotificationService.Notify(new NotificationMessage
            {
                Severity = severity,
                Summary = summary,
                Detail = detail,
                Duration = 4000,
            });
        }

        private async Task OnAircraftIdChange(Guid aircraftId)
        {
            var aircraft = this.AircraftList.FirstOrDefault(a => a.AircraftId == aircraftId);

            if (aircraft != null)
            {
                this.Aircraft = new AircraftInfoModel
                {
                    Id = aircraft.Id,
                    AircraftId = aircraft.AircraftId,
                    TailNumber = aircraft.TailNumber,
                    Location = aircraft.Location,
                    Model = aircraft.Model,
                    Manufacturer = aircraft.Manufacturer,
                    YearOfManufacture = aircraft.YearOfManufacture,
                    AircraftType = aircraft.AircraftType,
                    VariantType = aircraft.VariantType,
                    TypeRating = aircraft.TypeRating,
                    CreatedBy = aircraft.CreatedBy,
                    CreatedOn = aircraft.CreatedOn,
                    ModifiedBy = aircraft.ModifiedBy,
                    ModifiedOn = aircraft.ModifiedOn,
                };

                this.SelectedAircraftId = aircraft.AircraftId;
                this.AircraftHeaderText = aircraft.TailNumber;

                await this.SessionStorage.SetItemAsync(AircraftStorageKey, aircraft.AircraftId);

                this.IsEditMode = false;
                this.IsAddMode = false;

                this._previousAircraftLocation = aircraft.Location;

                await this.LoadBookingsForCurrentWeek();
                await this.UpdateStatistics();

                this.StateHasChanged();
            }
        }

        private void ToggleEdit(AircraftInfoModel selectedAircraft)
        {
            if (selectedAircraft == null)
            {
                return;
            }

            this.SelectedAircraft = new AircraftInfoModel
            {
                Id = selectedAircraft.Id,
                AircraftId = selectedAircraft.AircraftId,
                TailNumber = selectedAircraft.TailNumber,
                Location = selectedAircraft.Location,
                Model = selectedAircraft.Model,
                Manufacturer = selectedAircraft.Manufacturer,
                YearOfManufacture = selectedAircraft.YearOfManufacture,
                AircraftType = selectedAircraft.AircraftType,
                VariantType = selectedAircraft.VariantType,
                TypeRating = selectedAircraft.TypeRating,
                CreatedBy = selectedAircraft.CreatedBy,
                CreatedOn = selectedAircraft.CreatedOn,
                ModifiedBy = selectedAircraft.ModifiedBy,
                ModifiedOn = selectedAircraft.ModifiedOn,
            };

            this.Aircraft = new AircraftInfoModel
            {
                Id = selectedAircraft.Id,
                AircraftId = selectedAircraft.AircraftId,
                TailNumber = selectedAircraft.TailNumber,
                Location = selectedAircraft.Location,
                Model = selectedAircraft.Model,
                Manufacturer = selectedAircraft.Manufacturer,
                YearOfManufacture = selectedAircraft.YearOfManufacture,
                AircraftType = selectedAircraft.AircraftType,
                VariantType = selectedAircraft.VariantType,
                TypeRating = selectedAircraft.TypeRating,
                CreatedBy = selectedAircraft.CreatedBy,
                CreatedOn = selectedAircraft.CreatedOn,
                ModifiedBy = selectedAircraft.ModifiedBy,
                ModifiedOn = selectedAircraft.ModifiedOn,
            };

            this.IsEditMode = true;
            this.IsAddMode = false;
            this.AircraftHeaderText = this.Aircraft.TailNumber ?? "Edit Aircraft";
        }

        private void AddNewAircraft()
        {
            if (!this.IsAddMode && this.Aircraft != null)
            {
                this.SelectedAircraft = new AircraftInfoModel
                {
                    Id = this.Aircraft.Id,
                    AircraftId = this.Aircraft.AircraftId,
                    TailNumber = this.Aircraft.TailNumber,
                    Location = this.Aircraft.Location,
                    Model = this.Aircraft.Model,
                    Manufacturer = this.Aircraft.Manufacturer,
                    YearOfManufacture = this.Aircraft.YearOfManufacture,
                    AircraftType = this.Aircraft.AircraftType,
                    VariantType = this.Aircraft.VariantType,
                    TypeRating = this.Aircraft.TypeRating,
                    CreatedBy = this.Aircraft.CreatedBy,
                    CreatedOn = this.Aircraft.CreatedOn,
                    ModifiedBy = this.Aircraft.ModifiedBy,
                    ModifiedOn = this.Aircraft.ModifiedOn,
                };
            }

            this.Aircraft = new AircraftInfoModel();
            this.AircraftHeaderText = "New Aircraft";
            this.IsAddMode = true;
            this.IsEditMode = false;
        }

        private async Task SaveAircraft()
        {
            var command = new UpsertAircraftCommand(
               this.IsAddMode ? Guid.Empty : this.Aircraft.Id,
               this.IsAddMode ? Guid.NewGuid() : this.Aircraft.AircraftId,
               this.Aircraft.TailNumber ?? string.Empty,
               this.Aircraft.Location ?? string.Empty,
               this.Aircraft.Model ?? string.Empty,
               this.Aircraft.YearOfManufacture,
               this.Aircraft.Manufacturer ?? string.Empty,
               this.Aircraft.AircraftType ?? string.Empty,
               this.Aircraft.VariantType ?? string.Empty,
               this.Aircraft.TypeRating ?? string.Empty,
               false,
               this.IsAddMode ? "Admin" : this.Aircraft.CreatedBy ?? "Admin",
               this.IsAddMode ? DateTime.UtcNow : this.Aircraft.CreatedOn,
               this.IsAddMode ? null : "Admin",
               this.IsAddMode ? DateTime.MinValue : DateTime.UtcNow
            );

            await this.Commander.Send(command);
            this.AircraftHeaderText = this.Aircraft.TailNumber ?? "Aircraft";
            this.IsEditMode = false;
            this.IsAddMode = false;

            await this.SessionStorage.SetItemAsync(AircraftStorageKey, this.Aircraft.AircraftId);
            this._previousAircraftLocation = Aircraft.Location;
            await this.LoadAircraftList();
            await this.LoadBookingsForCurrentWeek();
            await this.UpdateStatistics();
            this.StateHasChanged();
        }

        private void CancelAircraftEdit()
        {
            if (this.SelectedAircraft != null)
            {
                this.Aircraft = new AircraftInfoModel
                {
                    Id = this.SelectedAircraft.Id,
                    AircraftId = this.SelectedAircraft.AircraftId,
                    TailNumber = this.SelectedAircraft.TailNumber,
                    Location = this.SelectedAircraft.Location,
                    Model = this.SelectedAircraft.Model,
                    Manufacturer = this.SelectedAircraft.Manufacturer,
                    YearOfManufacture = this.SelectedAircraft.YearOfManufacture,
                    AircraftType = this.SelectedAircraft.AircraftType,
                    VariantType = this.SelectedAircraft.VariantType,
                    TypeRating = this.SelectedAircraft.TypeRating,
                    CreatedBy = this.SelectedAircraft.CreatedBy,
                    CreatedOn = this.SelectedAircraft.CreatedOn,
                    ModifiedBy = this.SelectedAircraft.ModifiedBy,
                    ModifiedOn = this.SelectedAircraft.ModifiedOn,
                };
            }

            this.IsEditMode = false;
            this.IsAddMode = false;

            this.AircraftHeaderText = this.Aircraft?.TailNumber ?? "Aircraft";

            this.StateHasChanged();
        }

        private async Task LoadAircraftList()
        {
            var query = new GetAllAircraftQuery(null, null);
            var result = await this.Querier.Send<GetAllAircraftQueryResult>(query);

            if (result != null &&
                result.Status == QueryResultStatus.Succeeded &&
                result.Data?.Aircrafts != null &&
                result.Data.Aircrafts.Any())
            {
                this.AircraftList = result.Data.Aircrafts
                    .OrderByDescending(a => a.CreatedOn)
                    .Select(a => new AircraftInfoModel
                    {
                        Id = a.Id,
                        AircraftId = a.AircraftId,
                        TailNumber = a.TailNumber,
                        Location = a.Location,
                        Model = a.Model,
                        Manufacturer = a.Manufacturer,
                        YearOfManufacture = a.YearOfManufacture,
                        AircraftType = a.AircraftType,
                        VariantType = a.VariantType,
                        TypeRating = a.TypeRating,
                        CreatedBy = a.CreatedBy,
                        CreatedOn = a.CreatedOn,
                        ModifiedBy = a.ModifiedBy,
                        ModifiedOn = a.ModifiedOn,
                    })
                    .ToList();

                var savedAircraftId = await this.SessionStorage.GetItemAsync<Guid>(AircraftStorageKey);

                if (savedAircraftId != Guid.Empty)
                {
                    var selected = this.AircraftList.FirstOrDefault(a => a.AircraftId == savedAircraftId);

                    if (selected != null)
                    {
                        this.Aircraft = selected;
                        this.SelectedAircraftId = selected.AircraftId;
                        this.AircraftHeaderText = selected.TailNumber;
                        this._previousAircraftLocation = selected.Location;
                        return;
                    }
                }

                this.Aircraft = this.AircraftList.First();
                this.SelectedAircraftId = this.Aircraft.AircraftId;
                this.AircraftHeaderText = this.Aircraft.TailNumber;
                this._previousAircraftLocation = this.Aircraft.Location;
                await this.SessionStorage.SetItemAsync(AircraftStorageKey, this.Aircraft.AircraftId);
            }
            else
            {
                this.AircraftList = new List<AircraftInfoModel>();
                this.Aircraft = new AircraftInfoModel();
                this.IsAddMode = false;
                this.IsEditMode = false;

                this.SelectedAircraftId = Guid.Empty;
                this.AircraftHeaderText = "New Aircraft";

                this._previousAircraftLocation = string.Empty;

                await this.SessionStorage.RemoveItemAsync(AircraftStorageKey);
            }
        }
    }
}