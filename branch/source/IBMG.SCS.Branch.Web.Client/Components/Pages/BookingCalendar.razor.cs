using IBMG.SCS.Branch.Web.Client.Infrastructure.Models;
using Microsoft.AspNetCore.Components;

namespace IBMG.SCS.Branch.Web.Client.Components.Pages
{
    public partial class BookingCalendar
    {
        [Parameter]
        public bool IsLoading { get; set; }

        [Parameter]
        public List<DateTime> WeekDays { get; set; } = new();

        [Parameter]
        public string WeekDisplayText { get; set; } = string.Empty;

        [Parameter]
        public List<BookingInfoModel> CurrentWeekBookings { get; set; } = new();

        [Parameter]
        public EventCallback OnPreviousWeek { get; set; }

        [Parameter]
        public EventCallback OnNextWeek { get; set; }


        [Parameter]
        public EventCallback<(DateTime day, BookingInfoModel? booking)> OnBookingClick { get; set; }

        private bool CanBook(DateTime day) => day >= DateTime.Today;


        private List<BookingInfoModel> GetBookingsForDay(DateTime day)
        {
            return this.CurrentWeekBookings
                .Where(b => b.FromDate.Date <= day.Date && b.ToDate.Date >= day.Date)
                .ToList();
        }

        private async Task HandlePreviousWeek()
        {
            await this.OnPreviousWeek.InvokeAsync();
        }

        private async Task HandleNextWeek()
        {
            await this.OnNextWeek.InvokeAsync();
        }

        private async Task HandleBookingClick(DateTime day, BookingInfoModel? booking)
        {
            await this.OnBookingClick.InvokeAsync((day, booking));
        }
    }
}