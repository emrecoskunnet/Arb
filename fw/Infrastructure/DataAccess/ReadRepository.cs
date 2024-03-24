using System.Linq.Expressions;
using System.Reflection;
using Ardalis.Specification;
using ArbTech.Infrastructure.Extensions;
using ArbTech.SharedKernel.Exceptions;

namespace ArbTech.Infrastructure.DataAccess;

public class ReadRepository<T>(ArbTechDbContext dbContext) : IReadRepository<T>
    where T : class, IBaseEntity
{
    internal ArdalisRepository<T> InnerRepository { get; set; } = new(dbContext);

    // ReSharper disable once MemberCanBeProtected.Global

    public async Task<T> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default)
        where TId : notnull
    {
        T? result = await InnerRepository.GetByIdAsync(id, cancellationToken);
        if (result == null) throw new DomainObjectNotFoundException("Id", id.ToString() ?? string.Empty);
        return result;
    }

    public Task<T?> GetByIdOrDefaultAsync<TId>(TId id, CancellationToken cancellationToken = default)
        where TId : notnull => InnerRepository.GetByIdAsync(id, cancellationToken);

    public Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        => InnerRepository.FirstOrDefaultAsync(specification, cancellationToken);

    public Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<T, TResult> specification,
        CancellationToken cancellationToken = default)
        => InnerRepository.FirstOrDefaultAsync(specification, cancellationToken);

    public async Task<T> FirstAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        T? result = await InnerRepository.FirstOrDefaultAsync(specification, cancellationToken);
        if (result == null)
            throw new DomainObjectNotFoundException("Specification", specification.ToString() ?? string.Empty);

        return result;
    }

    public async Task<TResult> FirstAsync<TResult>(ISpecification<T, TResult> specification,
        CancellationToken cancellationToken = default)
    {
        TResult? result = await InnerRepository.FirstOrDefaultAsync(specification, cancellationToken);
        if (result == null)
            throw new DomainObjectNotFoundException("Specification", specification.ToString() ?? string.Empty);

        return result;
    }

    public Task<T?> SingleOrDefaultAsync(ISingleResultSpecification<T> specification,
        CancellationToken cancellationToken = default)
        => InnerRepository.SingleOrDefaultAsync(specification, cancellationToken);

    public Task<TResult?> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<T, TResult> specification,
        CancellationToken cancellationToken = default)
        => InnerRepository.SingleOrDefaultAsync(specification, cancellationToken);

    public async Task<T> SingleAsync(ISingleResultSpecification<T> specification,
        CancellationToken cancellationToken = default)
    {
        T? result = await InnerRepository.SingleOrDefaultAsync(specification, cancellationToken);
        if (result == null) throw new DomainObjectNotFoundException();

        return result;
    }

    public async Task<TResult> SingleAsync<TResult>(ISingleResultSpecification<T, TResult> specification,
        CancellationToken cancellationToken = default)
    {
        TResult? result = await InnerRepository.SingleOrDefaultAsync(specification, cancellationToken);
        if (result == null) throw new DomainObjectNotFoundException();

        return result;
    }

    public async Task<List<T>> ListAsync(CancellationToken cancellationToken = default)
    {
        List<T> result = await InnerRepository.ListAsync(cancellationToken);
        if (result == null || result.Count == 0)
            throw new DomainObjectNotFoundException();

        return result;
    }

    public async Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        List<T> result = await InnerRepository.ListAsync(specification, cancellationToken);
        if (result == null || result.Count == 0)
            throw new DomainObjectNotFoundException();

        return result;
    }

    public async Task<List<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification,
        CancellationToken cancellationToken = default)
    {
        var result = await InnerRepository.ListAsync(specification, cancellationToken);
        if (result == null || result.Count == 0)
            throw new DomainObjectNotFoundException();

        return result;
    }

    public async Task<List<T>?> ListOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        List<T> result = await InnerRepository.ListAsync(cancellationToken);
        return result.Count == 0 ? null : result;
    }

    public async Task<List<T>?> ListOrDefaultAsync(ISpecification<T> specification,
        CancellationToken cancellationToken = default)
    {
        List<T> result = await InnerRepository.ListAsync(specification, cancellationToken);
        return result.Count == 0 ? null : result;
    }

    public async Task<List<TResult>?> ListOrDefaultAsync<TResult>(ISpecification<T, TResult> specification,
        CancellationToken cancellationToken = default)
    {
        List<TResult> result = await InnerRepository.ListAsync(specification, cancellationToken);
        return result.Count == 0 ? null : result;
    }

    public Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        => InnerRepository.CountAsync(specification, cancellationToken);

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
        => InnerRepository.CountAsync(cancellationToken);

    public Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        => InnerRepository.AnyAsync(specification, cancellationToken);

    public Task<bool> AnyAsync(CancellationToken cancellationToken = default)
        => InnerRepository.AnyAsync(cancellationToken);

    public async Task<IPagedList<T>> PagedListAsync<TInputModel>(ISpecification<T> specification,
        TInputModel? listInputModel, CancellationToken cancellationToken = default)
        where TInputModel : ListInputModel
    {
        var result = await PagedListOrDefaultAsync(specification, listInputModel, cancellationToken);

        if (result == null) throw new DomainObjectNotFoundException("Specification", string.Empty);

        return result;
    }

    public async Task<IPagedList<TP>> PagedListAsync<TP, TInputModel>(ISpecification<T, TP> specification,
        TInputModel? listInputModel, CancellationToken cancellationToken = default)
        where TInputModel : ListInputModel
    {
        var result = await PagedListOrDefaultAsync(specification, listInputModel, cancellationToken);

        if (result == null || result.Count == 0)
            throw new DomainObjectNotFoundException(typeof(TP).Name, specification.ToString() ?? string.Empty);

        return result;
    }

    public async Task<IPagedList<T>?> PagedListOrDefaultAsync<TInputModel>(ISpecification<T> specification,
        TInputModel? listInputModel,
        CancellationToken cancellationToken = default) where TInputModel : ListInputModel
    {
        // TODO remove all extra implementation of pagination then add ardalis specification methods 
        var query = InnerRepository.ApplySpec(specification);

        int totalCount = listInputModel != null
            ? await query.CountAsync(cancellationToken)
            : 0;


        if (!string.IsNullOrWhiteSpace(listInputModel?.OrderBy) &&
            listInputModel.SortBy is "Asc" or "Desc")
            query = OrderHelper<T>.CalculateOrder(query, listInputModel);

        if (listInputModel?.PageSize > 0)
            // if (string.IsNullOrWhiteSpace(listInputModel?.OrderBy) || !(listInputModel.SortBy is "Asc" or "Desc"))
            // note order by first column    
            query = query.Skip(PaginationHelper.CalculateSkip(listInputModel))
                .Take(PaginationHelper.CalculateTake(listInputModel));

        var subset = await query.ToListAsync(cancellationToken);

        return listInputModel != null
            ? new StaticPagedList<T>(subset,
                listInputModel,
                totalCount)
            : null;
    }

    public async Task<IPagedList<TP>?> PagedListOrDefaultAsync<TP, TInputModel>(ISpecification<T, TP> specification,
        TInputModel? listInputModel,
        CancellationToken cancellationToken = default) where TInputModel : ListInputModel
    {
        var query = InnerRepository.ApplySpec(specification);

        int totalCount = listInputModel != null
            ? await query.CountAsync(cancellationToken)
            : 0;

        if (!string.IsNullOrWhiteSpace(listInputModel?.OrderBy) &&
            listInputModel.SortBy is "Asc" or "Desc")
            query = OrderHelper<TP>.CalculateOrder(query, listInputModel);

        if (listInputModel?.PageSize > 0)
        {
            if (string.IsNullOrWhiteSpace(listInputModel.OrderBy) || !(listInputModel.SortBy is "Asc" or "Desc"))
            {
                query = OrderHelper<TP>.OrderByFirstColumn(query);
            }

            query = query.Skip(PaginationHelper.CalculateSkip(listInputModel))
                .Take(PaginationHelper.CalculateTake(listInputModel));
        }


        var subset = await query.ToListAsync(cancellationToken);

        return listInputModel != null
            ? new StaticPagedList<TP>(subset,
                listInputModel,
                totalCount)
            : null;
    }


    private static class PaginationHelper
    {
        private static int DefaultPage => 1;
        private static int DefaultPageSize => 10;

        private static int CalculateTake(int pageSize)
        {
            return pageSize <= 0 ? DefaultPageSize : pageSize;
        }

        private static int CalculateSkip(int pageSize, int page)
        {
            page = page <= 0 ? DefaultPage : page;

            return CalculateTake(pageSize) * (page - 1);
        }

        public static int CalculateTake(IPagingInputModel? listInputModel)
        {
            return CalculateTake(listInputModel?.PageSize ?? DefaultPageSize);
        }

        public static int CalculateSkip(IPagingInputModel? listInputModel)
        {
            return CalculateSkip(listInputModel?.PageSize ?? DefaultPageSize,
                listInputModel?.PageNumber ?? DefaultPage);
        }
    }

    private static class OrderHelper<T1>
    {
        internal static IQueryable<T1> CalculateOrder(IQueryable<T1> q, ISortingInputModel? listInputModel)
        {
            if (string.IsNullOrEmpty(listInputModel?.OrderBy)) return q; // nothing
            bool isAscSort = listInputModel.SortBy == "Asc";

            q = isAscSort
                ? OrderBy(q, listInputModel.OrderBy)
                : (IQueryable<T1>)OrderByDescending(q, listInputModel.OrderBy);
            return q;
        }

        internal static IQueryable<T1> OrderByFirstColumn(IQueryable<T1> q)
        {
            PropertyInfo? firstProperty = typeof(T1).GetProperties().FirstOrDefault();
            if (firstProperty == null) return q;

            q = OrderBy(q, firstProperty.Name);
            return q;
        }

        private static IOrderedQueryable<TIn> OrderBy<TIn>(IQueryable<TIn> source, string propertyName)
        {
            return source.OrderBy(ToLambda<TIn>(propertyName.ConvertCamelCaseToPascalCase()));
        }

        private static IOrderedQueryable<TIn> OrderByDescending<TIn>(IQueryable<TIn> source, string propertyName)
        {
            return source.OrderByDescending(ToLambda<TIn>(propertyName));
        }

        private static Expression<Func<TIn, object>> ToLambda<TIn>(string propertyName)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TIn));
            MemberExpression property = Expression.Property(parameter, propertyName);
            UnaryExpression propAsObject = Expression.Convert(property, typeof(object));

            return Expression.Lambda<Func<TIn, object>>(propAsObject, parameter);
        }
    }
}
