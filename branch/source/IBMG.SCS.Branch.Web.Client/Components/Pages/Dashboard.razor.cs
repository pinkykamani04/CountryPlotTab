using Radzen;
using Radzen.Blazor;

namespace IBMG.SCS.Branch.Web.Client.Components.Pages;

public partial class Dashboard(NotificationService notificationService)
{
    ValidationFormModel Model = new();
    bool IsValidated = false;
    bool showValue = true;
    double value = 100;
    IEnumerable<GaugeTickPosition> tickPositions = Enum.GetValues(typeof(GaugeTickPosition)).Cast<GaugeTickPosition>();

    GaugeTickPosition tickPosition = GaugeTickPosition.Outside;

    List<string> Customers = new() { "Loremipsum LTD", "Alpha Corp", "Test Customer" };

    public class ValidationFormModel
    {
        public string Customer { get; set; }
        public string JobNumber { get; set; }
        public string TradeCardNumber { get; set; }
        public string OperativeId { get; set; }
    }
    class DataItem
    {
        public string Quarter { get; set; }
        public double Revenue { get; set; }
    }
    class DemoRow
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public bool IsHighlighted { get; set; }
    }
    List<DemoRow> demo = new()
 {
     new DemoRow{ Id=1, Name="John Doe", Status="Active", IsHighlighted=true },
     new DemoRow{ Id=2, Name="Jane Smith", Status="Inactive" },
     new DemoRow{ Id=3, Name="David Lee", Status="Active" }
 };
    DataItem[] revenue = new DataItem[] {
        new DataItem
        {
            Quarter = "Q1",
            Revenue = 30000
        },
        new DataItem
        {
            Quarter = "Q2",
            Revenue = 40000
        },
        new DataItem
        {
            Quarter = "Q3",
            Revenue = 50000
        },
        new DataItem
        {
            Quarter = "Q4",
            Revenue = 80000
        },
    };
    void OnSubmit(ValidationFormModel data)
    {
        if (string.IsNullOrWhiteSpace(Model.Customer) ||
            string.IsNullOrWhiteSpace(Model.JobNumber) ||
            string.IsNullOrWhiteSpace(Model.TradeCardNumber) ||
            string.IsNullOrWhiteSpace(Model.OperativeId))
        {
            IsValidated = false;
            return;
        }

        IsValidated = true;
    }
    void AddOrder()
    {
        notificationService.Notify(new NotificationMessage
        {
            Severity = NotificationSeverity.Success,
            Summary = "Order Added",
            Detail = "The order has been successfully added.",
            Duration = 4000
        });
    }
}