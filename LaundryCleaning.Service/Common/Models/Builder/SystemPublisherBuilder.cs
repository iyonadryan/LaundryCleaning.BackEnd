using LaundryCleaning.Service.Common.Models.Entities;
using LaundryCleaning.Service.Data;
using LaundryCleaning.Service.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LaundryCleaning.Service.Common.Models.Builder
{
    public class SystemPublisherBuilder
    {
        private readonly ApplicationDbContext _dbContext;

        public SystemPublisherBuilder(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Configure(EntityTypeBuilder<SystemPublisher> builder)
        {
            builder.
                ApplyEntityDefaults(_dbContext);

            builder
                .Property(x => x.Topic)
                .HasMaxLength(50);
        }
    }
}
