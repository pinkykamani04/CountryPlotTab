// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.KerridgeApi.Server.KerridgeRequestModels
{
    public record RequestSalesOrderDto(
        SalesOrderHeader Header,
        SalesOrderLines Lines);

    public record SalesOrderHeader(
        string Account,
        int Branch,
        string Createuser,
        string Reference,
        string Reference2,
        DateTime DateRequired,
        string Name,
        Address Address,
        string Deliverycountry,
        string Invoicename,
        Address Invoiceaddress,
        string Invoicecountry,
        string Instructions,
        string Narrative,
        string Ordertype,
        string Carrier,
        string Servicelevel,
        bool Calccarriage,
        bool Suspendorder,
        string Id,
        string Suspendmsg,
        string Ordercategory,
        string Deliverymethod,
        int Contactid,
        string Sourceofsale,
        string Webcontact,
        string Sitetelephone,
        string Sitemobile,
        string Contractref,
        string Contactphone,
        string Contactmobile,
        string Contactemail,
        int Sitecontactid,
        string Sitecontactname,
        string Supplystatus,
        string Advicemethod,
        string Acknowledgedprices,
        string Creditreason,
        int Origsalesorderno,
        QuoteActions Quoteactions,
        Payments Payments,
        DataCaptureContainer Datacapture,
        Vouchers Vouchers);

    public record Address(
        List<string> Line,
        string Postcode);

    public record QuoteActions(List<QuoteActionItem> Quoteaction);

    public record QuoteActionItem(int Quoteno, string Action);

    public record Payments(List<Payment> Payment);

    public record Payment(string Type, decimal Value, string Reference);

    public record Vouchers(List<Voucher> Voucher);

    public record Voucher(string Code, string Type, decimal Value, decimal Minspend, string Id);

    public record SalesOrderLines(List<SalesOrderLine> Line);

    public record SalesOrderLine(
        string Product,
        string Manufact,
        string Extendeddescription,
        decimal Quantity,
        string Unit,
        DateOnly Daterequired,
        string Specialinstructions,
        string Internalinstructions,
        decimal Price,
        string Priceunit,
        Special Special,
        decimal Cost,
        int QuoteNo,
        int Quoteline,
        Tally Tally,
        DataCaptureContainer Datacapture);

    public record Special(string Supplier);

    public record Tally(List<TallyItem> Tallyitem);

    public record TallyItem(int Quantity, decimal Length, decimal Width);

    public record DataCaptureContainer(List<DataCaptureItem> Capture);

    public record DataCaptureItem(int Occurs, string Value);
}