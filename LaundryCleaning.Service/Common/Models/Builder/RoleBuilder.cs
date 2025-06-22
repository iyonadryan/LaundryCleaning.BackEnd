using LaundryCleaning.Service.Common.Models.Entities;
using LaundryCleaning.Service.Data;
using LaundryCleaning.Service.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LaundryCleaning.Service.Common.Models.Builder
{
    public class RoleBuilder
    {
        private readonly ApplicationDbContext _dbContext;

        public RoleBuilder(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.
                ApplyEntityDefaults(_dbContext);

            builder
                .Property(x => x.Name)
                .HasMaxLength(20);

            builder
                .Property(x => x.Code)
                .HasMaxLength(20);
        }
    }
}
