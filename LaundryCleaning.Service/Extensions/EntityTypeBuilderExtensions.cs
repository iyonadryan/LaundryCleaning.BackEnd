using LaundryCleaning.Service.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LaundryCleaning.Service.Extensions
{
    public static class EntityTypeBuilderExtensions
    {
        public static void ApplyEntityDefaults<TEntity>(this EntityTypeBuilder<TEntity> builder, ApplicationDbContext applicationDbContext)
            where TEntity : class
        {
            builder.HasKey("Id");
            builder.Property("Created").HasColumnType("datetime").HasDefaultValueSql("GETDATE()").ValueGeneratedOnAdd(); // OR "HasDefaultValueSql(SYSDATETIME())"  
            builder.Property("Modified").HasColumnType("datetime").HasDefaultValueSql("GETDATE()").ValueGeneratedOnAddOrUpdate();
            builder.Property("ValidTo").HasColumnType("datetime").HasDefaultValue(new DateTime(9998, 12, 31, 23, 59, 59, 997));
            builder.Property("Deleted").HasDefaultValue(false);
        }
    }
}
