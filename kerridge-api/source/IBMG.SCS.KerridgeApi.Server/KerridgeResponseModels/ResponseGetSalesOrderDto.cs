// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.KerridgeApi.Server.KerridgeResponseModels
{
    public class ResponseGetSalesOrderDto
    {
        public Body Body { get; set; }
    }

    public class Body
    {
        public string Account { get; set; }

        public string Accounttype { get; set; }

        public bool Next { get; set; }

        public Orderdetails Orderdetails { get; set; }
    }

    public class Orderdetails
    {
        public List<Orderline> Lines { get; set; }
    }

    public class Orderline
    {
        public int Ordernumber { get; set; }

        public string Customerorder { get; set; }

        public string Customerorder2 { get; set; }

        public int Branch { get; set; }

        public string Branchshortcode { get; set; }

        public DateTime Datecreated { get; set; }

        public TimeSpan Timecreated { get; set; }

        public DateTime Daterequired { get; set; }

        public string Name { get; set; }

        public OrderAddress Address { get; set; }

        public string Deliverycountry { get; set; }

        public string Invoicename { get; set; }

        public string Invoiceaccount { get; set; }

        public OrderAddress Invoiceaddress { get; set; }

        public string Invoicecountry { get; set; }

        public DateTime Quoteexpirydate { get; set; }

        public decimal Ordervalue { get; set; }

        public decimal Deliverycosts { get; set; }

        public decimal Totalexcludingtax { get; set; }

        public decimal Ordervat { get; set; }

        public decimal Totalincludingtax { get; set; }

        public string Currency { get; set; }

        public string Ordercategory { get; set; }

        public string Deliverymethod { get; set; }

        public Ordertype Ordertype { get; set; }

        public Contact Contact { get; set; }

        public Sourceofsale Sourceofsale { get; set; }

        public Datacapture Datacapture { get; set; }
    }

    public class OrderAddress
    {
        public List<string> Line { get; set; }

        public string Postcode { get; set; }
    }

    public class Ordertype
    {
        public string Code { get; set; }

        public string Desc { get; set; }
    }

    public class Contact
    {
        public long Code { get; set; }

        public string Name { get; set; }
    }

    public class Sourceofsale
    {
        public string Code { get; set; }

        public string Desc { get; set; }
    }

    public class Datacapture
    {
        public List<Datacaptureitem> Capture { get; set; }
    }

    public class Datacaptureitem
    {
        public int Occurs { get; set; }

        public string Value { get; set; }

        public string Desc { get; set; }
    }
}