using LaundryCleaning.Service.Common.Models.Entities;
using LaundryCleaning.Service.Data;
using LaundryCleaning.Service.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LaundryCleaning.Service.Common.Models.Builder
{
    public class SystemReceivedBuilder
    {
        private readonly ApplicationDbContext _dbContext;

        public SystemReceivedBuilder(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Configure(EntityTypeBuilder<SystemReceived> builder)
        {
            builder.
                ApplyEntityDefaults(_dbContext);

            builder
                .Property(x => x.Topic)
                .HasMaxLength(50);
        }
    }
}
