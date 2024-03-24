using System.Runtime.CompilerServices;
using Ardalis.GuardClauses;
using System.Diagnostics.CodeAnalysis;

namespace ArbTech.SharedKernel.Extensions;

public static class GuardClauseExtensions
{
    public static T InvalidState<T>(this IGuardClause guardClause, T input,
        params T[] availableValues)
    { 
        if (!availableValues.Contains(input))
            throw new ArgumentException( $"Input {input} did not valid states in [{availableValues}]");

        return input;
    }
    
    public static T NotNull<T>(this IGuardClause guardClause, T input,
        [CallerArgumentExpression("input")]  string? parameterName = null)
    {
        if (input is not null) 
            throw new ArgumentOutOfRangeException(parameterName);
        return input;
    } 
    
    public static T DomainObjectNotFound<T>(this IGuardClause guardClause,
        [NotNull][ValidatedNotNull] string key,
        [NotNull][ValidatedNotNull] T input,
        [CallerArgumentExpression("input")] string? parameterName = null)
    {
        guardClause.NullOrEmpty(key);

        if (input is null)
        {
            throw new Exceptions.DomainObjectNotFoundException(parameterName ?? "parameterName", key);
        }

        return input;
    }
    
    public static T DomainObjectAlreadyExists<T>(this IGuardClause guardClause,
        [ValidatedNotNull] string key,
        [ValidatedNotNull]T input,
        [CallerArgumentExpression("input")] string? parameterName = null)
    {
        guardClause.NullOrEmpty(key);

        if (input is not null)
        {
            throw new Exceptions.DomainObjectAlreadyExistsException(parameterName ?? "parameterName", key);
        }

        return input;
    }
    
    public static DateTime OutOfDateRange( this IGuardClause guardClause,
        DateTime input,
        [CallerArgumentExpression("input")] string? parameterName = null,
        string? message = null) 
    {
        // System.Data is unavailable in .NET Standard so we can't use SqlDateTime.
        const long sqlMinDateTicks = 599266080010000000;    // 1900/1/1
        const long sqlMaxDateTicks = 665851103990000000;    // 2011/1/1

        return guardClause.OutOfRange(input, parameterName!, new DateTime(sqlMinDateTicks), new DateTime(sqlMaxDateTicks), message);
    }
    
    public static int NegativeOrZero( this IGuardClause guardClause,
        int? input,
        [CallerArgumentExpression("input")] string? parameterName = null,
        string? message = null)
    {
        return Ardalis.GuardClauses.GuardClauseExtensions.NegativeOrZero(guardClause, input ?? 1, parameterName, message);
    }
}
