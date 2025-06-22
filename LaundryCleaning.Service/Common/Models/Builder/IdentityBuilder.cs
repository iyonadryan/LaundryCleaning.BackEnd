using LaundryCleaning.Service.Common.Models.Entities;
using LaundryCleaning.Service.Data;
using LaundryCleaning.Service.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LaundryCleaning.Service.Common.Models.Builder
{
    public class IdentityBuilder
    {
        private readonly ApplicationDbContext _dbContext;

        public IdentityBuilder(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Configure(EntityTypeBuilder<Identity> builder)
        {
            builder.
                ApplyEntityDefaults(_dbContext);
        }
    }
}
