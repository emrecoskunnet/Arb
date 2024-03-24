using Ardalis.Specification;
using ArbTech.SharedKernel.Querying;

namespace ArbTech.SharedKernel.Interfaces;

public interface IReadRepository<T> where T : class, IBaseEntity
{
    Task<T> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull;
    
    Task<T?> GetByIdOrDefaultAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull;
    
    Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
    
    Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default);
    
    Task<T> FirstAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

    Task<TResult> FirstAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default);
    
    Task<T?> SingleOrDefaultAsync(ISingleResultSpecification<T> specification, CancellationToken cancellationToken = default);
    
    Task<TResult?> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<T, TResult> specification, CancellationToken cancellationToken = default);
    
    Task<T> SingleAsync(ISingleResultSpecification<T> specification, CancellationToken cancellationToken = default);
    
    Task<TResult> SingleAsync<TResult>(ISingleResultSpecification<T, TResult> specification, CancellationToken cancellationToken = default);
    
    Task<List<T>> ListAsync(CancellationToken cancellationToken = default);
    
    Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
    
    Task<List<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default);

    Task<List<T>?> ListOrDefaultAsync(CancellationToken cancellationToken = default);
    
    Task<List<T>?> ListOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
    
    Task<List<TResult>?> ListOrDefaultAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default);

    
    Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
    
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    
    Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
    
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
    
    Task<IPagedList<T>> PagedListAsync<TInputModel>(ISpecification<T> specification,
        TInputModel? listInputModel, CancellationToken cancellationToken = default) where TInputModel : ListInputModel;

    Task<IPagedList<TP>> PagedListAsync<TP, TInputModel>(ISpecification<T, TP> specification,
        TInputModel? listInputModel, CancellationToken cancellationToken = default) where TInputModel : ListInputModel;
    
    Task<IPagedList<T>?> PagedListOrDefaultAsync<TInputModel>(ISpecification<T> specification,
        TInputModel? listInputModel, CancellationToken cancellationToken = default) where TInputModel : ListInputModel;

    Task<IPagedList<TP>?> PagedListOrDefaultAsync<TP, TInputModel>(ISpecification<T, TP> specification,
        TInputModel? listInputModel, CancellationToken cancellationToken = default) where TInputModel : ListInputModel;
}
