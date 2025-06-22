using LaundryCleaning.Service.Common.Domain;

namespace LaundryCleaning.Service.Common.Models.Entities
{
    public class RolePermission : EntityBase
    {
        public Guid RoleId { get; set; }
        public string Permission { get; set; } = string.Empty;
    }
}
