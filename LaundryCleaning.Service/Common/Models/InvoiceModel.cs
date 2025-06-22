namespace LaundryCleaning.Service.Common.Models
{
    public class InvoiceModel
    {
        public string Logo { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime InvoicePaymentDueDate { get; set; }
        public string CustomerName { get; set; }

        public decimal SubTotal { get; set; }
        public decimal InvoiceTax { get; set; }
        public decimal Total { get; set; }

        public List<InvoiceItem> Items { get; set; }
        public InvoicePaymentDetail PaymentDetail { get; set; }
    }

    public class InvoiceItem
    {
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class InvoicePaymentDetail
    {
        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankAccountName { get; set; }
        public string BankVirtualAccount { get; set; }
    }
}
