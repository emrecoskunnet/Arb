using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;

namespace ArbTech.Infrastructure.DataAccess;

public sealed class ArdalisRepository<T> : RepositoryBase<T>
    where T : class, IBaseEntity
{
    public ArdalisRepository(ArbTechDbContext dbContext) :
        base(dbContext)
    {
    }
 
    public IQueryable<T> ApplySpec(ISpecification<T> specification, bool evaluateCriteriaOnly = false)
        => ApplySpecification(specification, evaluateCriteriaOnly);

    public IQueryable<TResult> ApplySpec<TResult>(ISpecification<T, TResult> specification)
        => ApplySpecification(specification);

}
