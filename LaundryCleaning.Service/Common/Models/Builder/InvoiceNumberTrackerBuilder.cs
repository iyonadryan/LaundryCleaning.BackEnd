using LaundryCleaning.Service.Common.Models.Entities;
using LaundryCleaning.Service.Data;
using LaundryCleaning.Service.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LaundryCleaning.Service.Common.Models.Builder
{
    public class InvoiceNumberTrackerBuilder
    {
        private readonly ApplicationDbContext _dbContext;

        public InvoiceNumberTrackerBuilder(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Configure(EntityTypeBuilder<InvoiceNumberTracker> builder)
        {
            builder.
                ApplyEntityDefaults(_dbContext);

            builder
                .Property(x => x.Code)
                .HasMaxLength(20);
        }
    }
}
