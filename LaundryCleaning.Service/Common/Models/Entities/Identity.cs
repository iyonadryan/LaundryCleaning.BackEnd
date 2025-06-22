using LaundryCleaning.Service.Common.Domain;

namespace LaundryCleaning.Service.Common.Models.Entities
{
    public class Identity : EntityBase
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }

        public User User { get; set; }
        public Role Role { get; set; }
    }
}
