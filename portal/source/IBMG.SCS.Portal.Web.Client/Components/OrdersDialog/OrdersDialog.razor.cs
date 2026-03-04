// Copyright (c) IBMG. All rights reserved.

using IBMG.SCS.Portal.Web.Client.Models;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json.Linq;
using Radzen;

namespace IBMG.SCS.Portal.Web.Client.Components.OrdersDialog
{
    public partial class OrdersDialog 
    {
        [Parameter]
        public List<OrderDetails> Orders { get; set; } = new();

        [Parameter]
        public string Operative { get; set; }

        [Parameter]
        public DateTime Date { get; set; }

        [Parameter]
        public string GroupBy { get; set; }

        private List<dynamic> RawOrders = new();

        protected override async Task OnInitializedAsync()
        {
            string? operativeFilter =
                string.IsNullOrWhiteSpace(this.Operative)
                    ? null
                    : this.Operative;

            var result = await this.Client.ListCustomerOrdersAsync(
                6702124,
                this.Date.Date,
                this.Date.Date.AddDays(1),
                operativeFilter,
                null,
                null
            );

            if (result?.Data is JArray array)
            {
                this.RawOrders = array.ToObject<List<dynamic>>() ?? new();
                this.BuildUiModels();
            }
        }

        private void BuildUiModels()
        {
            this.Orders = this.RawOrders.Select(o =>
            {
                var dateValue = o.Transaction_Date ?? o.date;
                var totalValue = o.Invoiced_Sales_Total ?? o.total;

                var dateTime = dateValue != null
                    ? DateTime.Parse(dateValue.ToString())
                    : DateTime.MinValue;

                return new OrderDetails
                {
                    Date = dateTime.Date,
                    Time = dateTime != DateTime.MinValue
                        ? dateTime.ToString("HH:mm")
                        : string.Empty,

                    OrderNo = o.Operative_Code
                        ?? o.orderNo
                        ?? o.order_Number
                        ?? string.Empty,

                    Operative = o.Operative_Desc
                        ?? o.operative
                        ?? "Unassigned",

                    Branch = o.Branch_Desc
                        ?? o.branch
                        ?? string.Empty,

                    Total = totalValue != null
                        ? Convert.ToDecimal(totalValue)
                        : 0m,
                };
            }).ToList();
        }

        private void Close()
        {
            this.DialogService.Close(null);
        }

        private void OnOrderRowClick(DataGridRowMouseEventArgs<OrderDetails> args)
        {
            var parameters = new Dictionary<string, object>
    {
        { "OrderId", args.Data.OrderNo },
    };

            this.DialogService.Open<OrderLinesDialog.OrderLinesDialog>(
                "Order Lines",
                parameters,
                new DialogOptions
                {
                    Width = "750px",
                    Height = "500px",
                    ShowClose = true,
                });
        }

    }
}