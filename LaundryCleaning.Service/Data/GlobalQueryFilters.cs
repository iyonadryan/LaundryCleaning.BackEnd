using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LaundryCleaning.Service.Data
{
    public static class GlobalQueryFilters
    {
        public static void Apply(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var clrType = entityType.ClrType;

                var deletedProp = clrType.GetProperty("Deleted");
                var validToProp = clrType.GetProperty("ValidTo");

                if (deletedProp?.PropertyType == typeof(bool) &&
                    validToProp?.PropertyType == typeof(DateTime?))
                {
                    var parameter = Expression.Parameter(clrType, "e");

                    // e.Deleted == false
                    var deletedExpr = Expression.Equal(
                        Expression.Property(parameter, deletedProp),
                        Expression.Constant(false)
                    );

                    // e.ValidTo == null || e.ValidTo > DateTime.Now
                    var validToNullExpr = Expression.Equal(
                        Expression.Property(parameter, validToProp),
                        Expression.Constant(null, typeof(DateTime?))
                    );

                    var nowExpr = Expression.Constant(DateTime.Now, typeof(DateTime));
                    var validToGreaterExpr = Expression.GreaterThan(
                        Expression.Property(parameter, validToProp),
                        nowExpr
                    );

                    var validToExpr = Expression.OrElse(validToNullExpr, validToGreaterExpr);

                    // Combine: !Deleted && (ValidTo == null || ValidTo > Now)
                    var combined = Expression.AndAlso(deletedExpr, validToExpr);
                    var lambda = Expression.Lambda(combined, parameter);

                    modelBuilder.Entity(clrType).HasQueryFilter(lambda);
                }
            }
        }
    }

}
