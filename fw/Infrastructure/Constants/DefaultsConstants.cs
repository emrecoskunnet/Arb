namespace ArbTech.Infrastructure.Constants;

public static class DefaultsConstants
{
    public const int ApplicationPartyId = 1;

    public const string DefaultCulture = "en-US";

    public static readonly string[] SupportedCultures = { "en-US", "tr-TR" };

    public static readonly int DefaultCacheDurationSeconds = 300;
    public static readonly int DefaultLongTermCacheDurationSeconds = 60*60*3; 

    public static readonly int PageNumber = 1;
    public static readonly int PageSize = 20;
    
    public static readonly int CheckUpdateTime = 1000;
}
