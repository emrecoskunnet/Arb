namespace Arb.SharedKernel.Extensions;

public static class StringExtensions
{
    public static string Blur(this string source)
        => source.Length > 5 ? source.Replace(source[2..^2],"***") : source;
    
    public static string Mask(this string source)
        => source.Length > 5 ? source.Replace(source[..^4],"***") : source;
}
