using LaundryCleaning.Service.Common.Domain;

namespace LaundryCleaning.Service.Common.Models.Entities
{
    public class InvoiceNumberTracker : EntityBase
    {
        public string Code { get; set; } = string.Empty;
        public int LastNumber { get; set; } 
    }
}
