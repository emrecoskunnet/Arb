namespace ArbTech.SharedKernel.Querying;

public interface IPagedList
{
    int Total { get; }

    IPagingInputModel? Paging { get; }

    ISortingInputModel? Sorting { get; }
}

public interface IPagedList<out T> : IPagedList, IEnumerable<T>
{
    T this[int index] { get; }

    int Count { get; }
}
