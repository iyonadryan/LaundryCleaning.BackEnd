using LaundryCleaning.Service.Common.Models.Entities;
using LaundryCleaning.Service.Data;
using LaundryCleaning.Service.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LaundryCleaning.Service.Common.Models.Builder
{
    public class RolePermissionBuilder
    {
        private readonly ApplicationDbContext _dbContext;

        public RolePermissionBuilder(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            builder
                .Property(x => x.Permission)
                .HasMaxLength(50);
        }
    }
}
