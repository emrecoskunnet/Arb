using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata;
using SmartEnum.EFCore;
using ArbTech.Infrastructure.Messaging.Outbox;

namespace ArbTech.Infrastructure.DataAccess;

public abstract class ArbTechDbContext : DbContext
{
#pragma warning disable CS8618
    protected ArbTechDbContext(DbContextOptions options)
#pragma warning restore CS8618
        : base(options) { }
    
    
    protected abstract void ApplyConfigurationOnModelCreating(ModelBuilder builder);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ConfigureSmartEnum();

        ApplyConfigurationOnModelCreating(modelBuilder);
        ApplyThruDateEqualNullQueryFilter(modelBuilder);
        
        modelBuilder.UseIntegrationEventLogs();
    }

    private static void ApplyThruDateEqualNullQueryFilter(ModelBuilder modelBuilder)
    {
        foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
        {
            IMutableProperty? thruDateProperty = entityType.FindProperty("ThruDate");
            if (thruDateProperty?.ClrType != typeof(DateTime?)
                || !(entityType.ClrType.BaseType?.Name ?? string.Empty).Contains(nameof(InsertOnlyBaseEntity))) continue;
            ParameterExpression item = Expression.Parameter(entityType.ClrType, "item");
            MemberExpression prop = Expression.Property(item, "ThruDate");
            ConstantExpression soap = Expression.Constant(null);
            BinaryExpression equal = Expression.Equal(prop, soap);
            LambdaExpression lambda = Expression.Lambda(equal, item);
            entityType.SetQueryFilter(lambda);
        }
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.ConfigureSmartEnum();
    }
}
