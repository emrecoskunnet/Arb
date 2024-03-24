namespace ArbTech.SharedKernel.Querying;

public class ListInputModel : IPagingInputModel, ISortingInputModel
{
    public string? SearchText { get; set; }
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public string? OrderBy { get; set; }

    public string? SortBy { get; set; }
}

public interface IPagingInputModel
{
    int PageNumber { get; }
    int PageSize { get; }
}

public interface ISortingInputModel
{
    public string? OrderBy { get; }

    public string? SortBy { get; }
}
