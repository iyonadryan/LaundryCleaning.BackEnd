using LaundryCleaning.Service.Common.Domain;

namespace LaundryCleaning.Service.Common.Models.Entities
{
    public class Role : EntityBase
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;

        public ICollection<Identity> Identities { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
