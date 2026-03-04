using BluQube.Commands;
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
    public partial class PilotDashboard : ComponentBase
    {
        [Inject]
        protected ICommander Commander { get; set; } = default!;

        [Inject]
        protected IQuerier Querier { get; set; } = default!;

        [Inject]
        protected NotificationService NotificationService { get; set; } = default!;

        [Inject] 
        public AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

        protected PilotInfoModel OriginalPilot = new();

        protected PilotInfoModel EditablePilot = null;

        protected bool isEditMode = false;
        protected bool isAddingNew = false;
        protected bool showFullDetails = false;

        protected List<FlightInfoModel> Flights = new();
        protected bool isLoading = true;
        protected byte[]? ProfileImageBytes;
        protected string? ProfileImageFileName;
        protected string? ProfileImageContentType;
        protected string? ImagePreviewUrl;
        private bool IsNewPilot = false;

        public double TotalFlyingHours { get; set; } = 0;

        public int FlightsThisYear { get; set; } = 0;

        public int TotalFlights { get; set; } = 0;

        public List<FlightInfoModel> PastFlights = new();

        public List<FlightInfoModel> UpcomingFlights = new();

        public string SelectedImageName { get; set; } = string.Empty;

        protected string ContactNumberString = string.Empty;
        public DateTime? DateOfBirthTemp;
        public DateTime? LicenseExpiryTemp;
        public DateTime? MedicalCertificateTemp;
        public DateTime? LicenseInsuranceDateTemp;
        private string? _profileImageUrl;

        private const string DefaultProfileImagePath = "/images/default-profile.jpg";

        protected override async Task OnInitializedAsync()
        {
            this.isLoading = true;
            this.PastFlights = new();
            this.UpcomingFlights = new();
            this.TotalFlights = 0;
            this.FlightsThisYear = 0;
            this.TotalFlyingHours = 0;

            var authState = await this.AuthStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            var email =
                user.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ??
                user.FindFirst("email")?.Value ??
                user.FindFirst("preferred_username")?.Value;

            if (string.IsNullOrWhiteSpace(email))
            {
                this.ShowNotification(NotificationSeverity.Error,
                    "Error",
                    "No email found for logged-in user.");
                return;
            }

            await this.LoadPilotDataByEmail(email);

            if (!this.IsNewPilot)
            {
                await this.LoadFlightsData();
            }
            else
            {
                this.PastFlights = new();
                this.UpcomingFlights = new();
            }

            this.isLoading = false;
        }

        private async Task LoadPilotDataByEmail(string email)
        {
            var query = new GetPilotInformationQuery(
                Id: null,
                EmailAddress: email
            );

            var result = await this.Querier.Send<GetPilotInformationQueryResult>(query);

            var pilotDto = result?.Data?.Pilots?.FirstOrDefault();

            if (pilotDto != null)
            {
                this.IsNewPilot = false;

                this.OriginalPilot = new PilotInfoModel
                {
                    Id = pilotDto.Id,
                    FullName = pilotDto.FullName ?? string.Empty,
                    EmailAddress = pilotDto.EmailAddress ?? string.Empty,
                    ContactNumber = pilotDto.ContactNumber,
                    Location = pilotDto.Location ?? string.Empty,
                    LicenseNumber = pilotDto.LicenseNumber ?? string.Empty,
                    LicenseType = pilotDto.LicenseType ?? string.Empty,

                    DateOfBirth = pilotDto.DateOfBirth,
                    LicenseExpiryDate = pilotDto.LicenseExpiryDate,
                    MedicalCertificate = pilotDto.MedicalCertificate,

                    ContactAddress = pilotDto.ContactAddress ?? string.Empty,
                    ProfileImageName = pilotDto.ProfileImageName ?? string.Empty,

                    CreatedBy = pilotDto.CreatedBy ?? "System",
                    CreatedOn = pilotDto.CreatedOn,
                };
                await this.InvokeAsync(this.StateHasChanged);
                return;
            }

            this.IsNewPilot = true;

            this.ShowNotification(NotificationSeverity.Info,
                "New Pilot Profile",
                "No pilot record found. Creating new profile...");

            this.OriginalPilot = new PilotInfoModel
            {
                Id = Guid.NewGuid(),
                FullName = email.Split("@")[0],
                EmailAddress = email,
                ContactNumber = 0,
                Location = string.Empty,
                LicenseNumber = string.Empty,
                LicenseType = string.Empty,

                DateOfBirth = DateTime.MinValue,
                LicenseExpiryDate = DateTime.MinValue,
                MedicalCertificate = DateTime.MinValue,

                ContactAddress = string.Empty,
                ProfileImageName = string.Empty,
                CreatedBy = "System",
                CreatedOn = DateTime.UtcNow,
            };

            var command = new UpsertPilotInformationCommand(
                this.OriginalPilot.Id,
                this.OriginalPilot.FullName,
                DateTime.MinValue,
                0,
                this.OriginalPilot.EmailAddress,
                ContactAddress: string.Empty,
                Location: string.Empty,
                LicenseNumber: string.Empty,
                LicenseType: string.Empty,
                LicenseInsuranceDate: DateTime.MinValue,
                LicenseExpiryDate: DateTime.MinValue,
                MedicalCertificate: DateTime.MinValue,
                ProfileImageName: string.Empty,
                ProfileImageBytes: null,
                ProfileImageFileName: string.Empty,
                ProfileImageContentType: string.Empty,
                CreatedBy: "System",
                CreatedOn: DateTime.UtcNow,
                ModifiedBy: string.Empty,
                ModifiedOn: DateTime.MinValue
            );

            await this.Commander.Send(command);

            this.ShowNotification(NotificationSeverity.Success,
                "Profile Created",
                $"Pilot profile created for {email}");
        }

        public async Task<Guid?> GetLoggedInPilotIdAsync()
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

        private async Task LoadPilotData(Guid pilotId)
        {
            var query = new GetPilotInformationQuery(
                Id: pilotId,
                EmailAddress: null
            );

            var result = await this.Querier.Send<GetPilotInformationQueryResult>(query);
            var pilotDto = result?.Data?.Pilots?.FirstOrDefault();

            if (pilotDto == null)
            {
                this.ShowNotification(NotificationSeverity.Warning, "Info", "No pilot information found.");
                return;
            }

            this.OriginalPilot = new PilotInfoModel
            {
                Id = pilotDto.Id,
                FullName = pilotDto.FullName ?? string.Empty,
                DateOfBirth = pilotDto.DateOfBirth,
                ContactNumber = pilotDto.ContactNumber,
                EmailAddress = pilotDto.EmailAddress ?? string.Empty,
                Location = pilotDto.Location ?? string.Empty,
                LicenseNumber = pilotDto.LicenseNumber ?? string.Empty,
                LicenseType = pilotDto.LicenseType ?? string.Empty,
                LicenseInsuranceDate = pilotDto.LicenseInsuranceDate,
                LicenseExpiryDate = pilotDto.LicenseExpiryDate,
                MedicalCertificate = pilotDto.MedicalCertificate,
                ContactAddress = pilotDto.ContactAddress ?? string.Empty,
                ProfileImageName = pilotDto.ProfileImageName ?? string.Empty,
                CreatedBy = pilotDto.CreatedBy ?? "System",
                CreatedOn = pilotDto.CreatedOn,
                ModifiedBy = pilotDto.ModifiedBy,
                ModifiedOn = pilotDto.ModifiedOn,
            };
        }

        private void PreparePilotForEdit(bool isFullEdit = true)
        {
            if (this.OriginalPilot == null)
            {
                return;
            }

            this.EditablePilot = new PilotInfoModel
            {
                Id = this.OriginalPilot.Id,
                FullName = this.OriginalPilot.FullName,
                DateOfBirth = this.OriginalPilot.DateOfBirth,
                ContactNumber = this.OriginalPilot.ContactNumber,
                EmailAddress = this.OriginalPilot.EmailAddress,
                Location = this.OriginalPilot.Location,
                LicenseNumber = this.OriginalPilot.LicenseNumber,
                LicenseType = this.OriginalPilot.LicenseType,
                LicenseInsuranceDate = this.OriginalPilot.LicenseInsuranceDate,
                LicenseExpiryDate = this.OriginalPilot.LicenseExpiryDate,
                MedicalCertificate = this.OriginalPilot.MedicalCertificate,
                ContactAddress = this.OriginalPilot.ContactAddress,
                ProfileImageName = this.OriginalPilot.ProfileImageName,
                CreatedBy = this.OriginalPilot.CreatedBy,
                CreatedOn = this.OriginalPilot.CreatedOn,
            };

            if (isFullEdit)
            {
                this.EditablePilot.ModifiedBy = this.OriginalPilot.ModifiedBy;
                this.EditablePilot.ModifiedOn = this.OriginalPilot.ModifiedOn;

                this.ContactNumberString = this.EditablePilot.ContactNumber > 0
                    ? this.EditablePilot.ContactNumber.ToString()
                    : string.Empty;

                this.isAddingNew = false;
            }

            this.isEditMode = true;
            this.showFullDetails = false;
        }

        private async Task SavePilotInfo()
        {
            if (this.EditablePilot == null)
            {
                return;
            }

            if (this.EditablePilot.ContactNumber < 1000000000 || this.EditablePilot.ContactNumber > 9999999999)
            {
                this.ShowNotification(NotificationSeverity.Error, "Validation Error", "Contact Number must be exactly 10 digits.");
                return;
            }

            this.EditablePilot.FullName = this.OriginalPilot.FullName;
            this.EditablePilot.EmailAddress = this.OriginalPilot.EmailAddress;

            var command = new UpsertPilotInformationCommand(
                this.EditablePilot.Id,
                this.OriginalPilot.FullName,
                this.EditablePilot.DateOfBirth,
                this.EditablePilot.ContactNumber,
                this.OriginalPilot.EmailAddress,
                this.EditablePilot.ContactAddress ?? string.Empty,
                this.EditablePilot.Location ?? string.Empty,
                this.EditablePilot.LicenseNumber ?? string.Empty,
                this.EditablePilot.LicenseType ?? string.Empty,
                this.EditablePilot.LicenseInsuranceDate,
                this.EditablePilot.LicenseExpiryDate,
                this.EditablePilot.MedicalCertificate,
                this.EditablePilot.ProfileImageName ?? string.Empty,
                this.ProfileImageBytes?.Length > 0 ? this.ProfileImageBytes : null,
                this.ProfileImageBytes?.Length > 0 ? this.ProfileImageFileName : null,
                this.ProfileImageBytes?.Length > 0 ? this.ProfileImageContentType : null,
                this.OriginalPilot.CreatedBy,
                this.OriginalPilot.CreatedOn,
                "Pilot",
                DateTime.UtcNow
            );

            await this.Commander.Send(command);

            await this.LoadPilotData(this.EditablePilot.Id);
            this.EditablePilot = null;
            this.isEditMode = false;

            this.ShowNotification(NotificationSeverity.Success, "Success", "Profile saved successfully.");
        }

        private void CancelEdit()
        {
            this.EditablePilot = null;
            this.ProfileImageBytes = null;
            this.ProfileImageFileName = null;
            this.ProfileImageContentType = null;
            this.ImagePreviewUrl = null;

            this.isEditMode = false;
            this.isAddingNew = false;

            this.StateHasChanged();
        }

        private void LoadMoreDetails() => this.showFullDetails = true;

        private void CloseFullDetails() => this.showFullDetails = false;

        public string GetInitials(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
            {
                return "PI";
            }

            var parts = fullName.Split(' ');
            return parts.Length >= 2
                ? $"{parts[0][0]}{parts[1][0]}".ToUpper()
                : fullName.Substring(0, 2).ToUpper();
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

        protected string ProfileImageUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(this.ImagePreviewUrl))
                {
                    return this.ImagePreviewUrl;
                }

                var pilot = this.isEditMode ? this.EditablePilot : this.OriginalPilot;

                if (pilot != null && !string.IsNullOrWhiteSpace(pilot.ProfileImageName))
                {
                    return $"{pilot.ProfileImageName}?v={DateTime.UtcNow.Ticks}";
                }

                return DefaultProfileImagePath;
            }
        }

        protected async Task OnProfileImageSelected(UploadChangeEventArgs args)
        {
            var file = args.Files.FirstOrDefault();
            if (file == null)
            {
                return;
            }

            using var ms = new MemoryStream();
            await file.OpenReadStream().CopyToAsync(ms);

            this.ProfileImageBytes = ms.ToArray();
            this.ProfileImageFileName = file.Name;

            this.ImagePreviewUrl = $"data:image;base64,{Convert.ToBase64String(this.ProfileImageBytes)}";

            await this.InvokeAsync(this.StateHasChanged);
        }

        private async Task LoadFlightsData()
        {
            if (this.IsNewPilot || this.OriginalPilot == null || this.OriginalPilot.Id == Guid.Empty)
            {
                this.PastFlights = new();
                this.UpcomingFlights = new();
                this.TotalFlights = 0;
                this.FlightsThisYear = 0;
                this.TotalFlyingHours = 0;
                return;
            }

            var query = new GetAllAircraftBookingQuery(
                Id: null,
                AircraftId: null,
                PilotId: this.OriginalPilot.Id
            );

            var result = await this.Querier.Send<GetAllAircraftBookingQueryResult>(query);

            var bookings = result?.Data?.Bookings?
                .Where(b => !b.IsRowDeleted)
                .Where(b => b.PilotId == this.OriginalPilot.Id)
                .ToList() ?? new();

            if (!bookings.Any())
            {
                this.PastFlights = new();
                this.UpcomingFlights = new();
                this.TotalFlights = 0;
                this.FlightsThisYear = 0;
                this.TotalFlyingHours = 0;
                return;
            }

            var aircraftDict = new Dictionary<Guid, GetAllAircraftQueryResult.AircraftItemDto>();
            foreach (var booking in bookings)
            {
                if (!aircraftDict.ContainsKey(booking.AircraftId))
                {
                    var aircraftQuery = new GetAllAircraftQuery(null, booking.AircraftId);
                    var aircraftResult = await this.Querier.Send<GetAllAircraftQueryResult>(aircraftQuery);
                    var aircraft = aircraftResult?.Data?.Aircrafts?.FirstOrDefault(a => !a.IsRowDeleted);
                    if (aircraft != null)
                    {
                        aircraftDict[booking.AircraftId] = aircraft;
                    }
                }
            }

            var now = DateTime.Now;

            this.PastFlights = bookings
                .Where(b => b.ToDate.Add(b.ToTime) < now)
                 .Select(b => MapFlightWithAircraft(b, aircraftDict))
                .ToList();

            this.UpcomingFlights = bookings
                .Where(b => b.FromDate.Add(b.FromTime) >= now)
                 .Select(b => MapFlightWithAircraft(b, aircraftDict))
                .ToList();

            this.TotalFlights = bookings.Count;

            this.FlightsThisYear = bookings.Count(b =>
                b.FromDate.Year == DateTime.Now.Year
            );

            this.TotalFlyingHours = bookings.Sum(b =>
            {
                var start = b.FromDate.Add(b.FromTime);
                var end = b.ToDate.Add(b.ToTime);
                return (end - start).TotalHours;
            });
        }

        private static FlightInfoModel MapFlightWithAircraft(
        GetAllAircraftBookingQueryResult.AircraftBookingItemDto booking,
        Dictionary<Guid, GetAllAircraftQueryResult.AircraftItemDto> aircraftDict)
        {
            var aircraft = aircraftDict.ContainsKey(booking.AircraftId) ? aircraftDict[booking.AircraftId] : null;
            var tailNumber = aircraft?.TailNumber ?? "N/A";

            return new FlightInfoModel
            {
                Id = booking.Id,
                DepartureCode = tailNumber,
                ArrivalCode = tailNumber,
                DepartureTime = booking.FromDate.Add(booking.FromTime).ToString("HH:mm"),
                ArrivalTime = booking.ToDate.Add(booking.ToTime).ToString("HH:mm"),
                Date = booking.FromDate.ToString("ddd dd MMM, yyyy"),
                FullFromLocation = booking.FromLocation ?? "-",
                FullToLocation = booking.ToLocation ?? "-",
            };
        }
    }
}