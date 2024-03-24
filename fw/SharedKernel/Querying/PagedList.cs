using System.Collections;

namespace ArbTech.SharedKernel.Querying;

public class StaticPagedList<T> : IPagedList<T>
{
    private readonly IEnumerable<T> _subset;

    public StaticPagedList(IEnumerable<T> subset)
        // ReSharper disable once PossibleMultipleEnumeration
        : this(subset, null, null, subset.Count())
    {
    }

    public StaticPagedList(IEnumerable<T> subset, ListInputModel listInputModel, int total)
        : this(subset, listInputModel, listInputModel, total)
    {
    }

    public StaticPagedList(IEnumerable<T> subset, IPagingInputModel? pagingInputModel,
        ISortingInputModel? sortingInputModel, int total)
    {
        _subset = subset;
        Paging = pagingInputModel;
        Sorting = sortingInputModel;
        Total = total;
    }

    public int Total { get; }

    public IPagingInputModel? Paging { get; }

    public ISortingInputModel? Sorting { get; }

    public IEnumerator<T> GetEnumerator()
    {
        return _subset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _subset.GetEnumerator();
    }

    public T this[int index] => _subset.ElementAt(index);

    public int Count => _subset.Count();

    public static StaticPagedList<T> Empty()
    {
        return new StaticPagedList<T>(new List<T>(), new ListInputModel(), 0);
    }
}
