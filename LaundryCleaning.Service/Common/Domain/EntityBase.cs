using System.ComponentModel.DataAnnotations;

namespace LaundryCleaning.Service.Common.Domain
{
    public class EntityBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? ValidTo { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public bool Deleted { get; set; }
    }
}