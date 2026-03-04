// Copyright (c) IBMG. All rights reserved.

using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace IBMG.SCS.Portal.Web.Client.Components.DateRangeDialog
{
    public partial class DateRangeDialog : ComponentBase
    {
        [Parameter]
        public DateTime InitialFrom { get; set; } = DateTime.Today.AddDays(-7);

        [Parameter]
        public DateTime InitialTo { get; set; } = DateTime.Today;

        [Parameter]
        public Func<DateTime, DateTime, Task> ApplyCallback { get; set; } = default!;

        private DateTime from;
        private DateTime to;
        private DateTime? selectedStart = null;
        private DateTime? selectedEnd = null;
        private DateTime leftMonth;
        private DateTime rightMonth;
        private string activeTab = "Current";
        private DateTime? hoverDate = null;

        private string[] weekDays = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames;

        protected override void OnInitialized()
        {
            this.from = this.InitialFrom.Date;
            this.to = this.InitialTo.Date;

            this.selectedStart = this.from;
            this.selectedEnd = this.to;

            this.leftMonth = new DateTime(this.InitialFrom.Year, this.InitialFrom.Month, 1);
            this.rightMonth = this.leftMonth.AddMonths(1);
        }

        private void PrevMonth()
        {
            this.leftMonth = this.leftMonth.AddMonths(-1);
            this.rightMonth = this.leftMonth.AddMonths(1);
        }

        private void NextMonth()
        {
            this.leftMonth = this.leftMonth.AddMonths(1);
            this.rightMonth = this.leftMonth.AddMonths(1);
        }

        private void SwitchTab(string tab)
        {
            this.activeTab = tab;

            this.selectedStart = null;
            this.selectedEnd = null;

            this.leftMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            this.rightMonth = this.leftMonth.AddMonths(1);

            this.StateHasChanged();
        }

        private void OnDateClicked(DateTime dt)
        {
            if (this.selectedStart == null || (this.selectedStart != null && this.selectedEnd != null))
            {
                this.selectedStart = dt.Date;
                this.selectedEnd = dt.Date;
            }
            else
            {
                if (dt.Date < this.selectedStart.Value)
                {
                    this.selectedEnd = this.selectedStart;
                    this.selectedStart = dt.Date;
                }
                else
                {
                    this.selectedEnd = dt.Date;
                }
            }

            this.hoverDate = null;
            this.from = this.selectedStart ?? from;
            this.to = this.selectedEnd ?? to;

            this.StateHasChanged();
        }

        private RenderFragment RenderMonth(DateTime month) => builder =>
        {
            var start = new DateTime(month.Year, month.Month, 1);
            int days = DateTime.DaysInMonth(month.Year, month.Month);
            int first = ((int)start.DayOfWeek + 7) % 7;

            int seq = 0;

            for (int i = 0; i < first; i++)
            {
                builder.OpenElement(seq++, "div");
                builder.AddAttribute(seq++, "class", "day-cell");
                builder.CloseElement();
            }

            for (int d = 1; d <= days; d++)
            {
                var dt = start.AddDays(d - 1);

                bool inRange = this.selectedStart != null && this.selectedEnd != null &&
                               dt >= this.selectedStart.Value && dt <= this.selectedEnd.Value;

                bool isStart = selectedStart != null && dt == selectedStart.Value;
                bool isEnd = selectedEnd != null && dt == selectedEnd.Value;

                bool inPreview = false;
                if (selectedStart != null && selectedEnd == null && hoverDate != null)
                {
                    DateTime s = selectedStart.Value;
                    DateTime h = hoverDate.Value;

                    if (h >= s && dt >= s && dt <= h)
                    {
                        inPreview = true;
                    }
                    else if (h < s && dt <= s && dt >= h)
                    {
                        inPreview = true;
                    }
                }

                var css = "day-cell";
                if (isStart || isEnd)
                {
                    css += " day-startend";
                }
                else if (inRange)
                {
                    css += " day-inrange";
                }
                else if (inPreview)
                {
                    css += " day-preview";
                }

                builder.OpenElement(seq++, "div");
                builder.AddAttribute(seq++, "class", css);

                builder.AddAttribute(seq++, "onmouseover",
                    EventCallback.Factory.Create(this, () => hoverDate = dt));
                builder.AddAttribute(seq++, "onmouseout",
                    EventCallback.Factory.Create(this, () => hoverDate = null));

                builder.AddAttribute(seq++, "onclick",
                    EventCallback.Factory.Create(this, () => this.OnDateClicked(dt)));

                builder.AddContent(seq++, d.ToString());
                builder.CloseElement();
            }

            int totalFilled = first + days;
            while (totalFilled < 42)
            {
                builder.OpenElement(seq++, "div");
                builder.AddAttribute(seq++, "class", "day-cell");
                builder.CloseElement();
                totalFilled++;
            }
        };

        private void ApplyQuick(int startOffset, int endOffset)
        {
            var now = DateTime.Today;
            this.selectedStart = now.AddDays(startOffset);
            this.selectedEnd = now.AddDays(endOffset);

            this.from = this.selectedStart.Value;
            this.to = this.selectedEnd.Value;

            this.leftMonth = new DateTime(this.from.Year, this.from.Month, 1);
            this.rightMonth = this.leftMonth.AddMonths(1);
        }

        private void ApplyPeriod(int days)
        {
            this.selectedStart = DateTime.Today.AddDays(-(days - 1));
            this.selectedEnd = DateTime.Today;

            this.from = this.selectedStart.Value;
            this.to = this.selectedEnd.Value;
        }

        private void ApplyCurrentWeek()
        {
            var today = DateTime.Today;
            int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            this.selectedStart = today.AddDays(-diff);
            this.selectedEnd = this.selectedStart.Value.AddDays(6);

            this.from = this.selectedStart.Value;
            this.to = this.selectedEnd.Value;
        }

        private void ApplyLastMonth()
        {
            var first = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
            this.selectedStart = first;
            this.selectedEnd = first.AddMonths(1).AddDays(-1);

            this.from = this.selectedStart.Value;
            this.to = this.selectedEnd.Value;
        }

        private void ApplyCurrentMonth()
        {
            var first = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            this.selectedStart = first;
            this.selectedEnd = first.AddMonths(1).AddDays(-1);

            this.from = this.selectedStart.Value;
            this.to = this.selectedEnd.Value;
        }

        private void ApplyLastQuarter()
        {
            var month = DateTime.Today.Month;
            var quarter = (month - 1) / 3;
            var start = new DateTime(DateTime.Today.Year, quarter * 3 + 1, 1).AddMonths(-3);

            this.selectedStart = start;
            this.selectedEnd = start.AddMonths(3).AddDays(-1);

            this.from = this.selectedStart.Value;
            this.to = this.selectedEnd.Value;
        }

        private void ApplyCurrentQuarter()
        {
            var month = DateTime.Today.Month;
            var quarter = (month - 1) / 3;
            var start = new DateTime(DateTime.Today.Year, quarter * 3 + 1, 1);

            this.selectedStart = start;
            this.selectedEnd = start.AddMonths(3).AddDays(-1);

            this.from = this.selectedStart.Value;
            this.to = this.selectedEnd.Value;
        }

        private void ApplyNextMonth()
        {
            var first = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1);

            this.selectedStart = first;
            this.selectedEnd = first.AddMonths(1).AddDays(-1);

            this.from = this.selectedStart.Value;
            this.to = this.selectedEnd.Value;
        }

        private void OnFromChanged(DateTime? value)
        {
            if (value.HasValue)
            {
                from = value.Value.Date;
                selectedStart = value.Value.Date;

                if (selectedEnd.HasValue && selectedEnd < selectedStart)
                    selectedEnd = selectedStart;

                StateHasChanged();
            }
        }

        private void OnToChanged(DateTime? value)
        {
            if (value.HasValue)
            {
                to = value.Value.Date;
                selectedEnd = value.Value.Date;

                if (selectedStart.HasValue && selectedEnd < selectedStart)
                    selectedStart = selectedEnd;

                StateHasChanged();
            }
        }

        private void ApplyNextQuarter()
        {
            var month = DateTime.Today.Month;
            var quarter = (month - 1) / 3;
            var start = new DateTime(DateTime.Today.Year, quarter * 3 + 1, 1).AddMonths(3);

            this.selectedStart = start;
            this.selectedEnd = start.AddMonths(3).AddDays(-1);

            this.from = this.selectedStart.Value;
            this.to = this.selectedEnd.Value;
        }

        private void OnApply()
        {
            this.ApplyCallback?.Invoke(this.from.Date, this.to.Date);
            this.DialogService.Close(true);
        }

        private void OnCancel()
        {
            this.DialogService.Close(false);
        }
    }
}