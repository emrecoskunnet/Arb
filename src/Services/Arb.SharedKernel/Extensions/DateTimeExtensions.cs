namespace Arb.SharedKernel.Extensions;

public static class DateTimeExtensions
{
    public static int ToAge(this DateTime? birthDate)
    {
        if (!birthDate.HasValue) return 0;
        
        int age = DateTime.Today.Year - birthDate.Value.Year;
        if (birthDate.Value.Date > DateTime.Today.AddYears(-age))
        {
            age--;
        }
        return age;
    }

    public static int ToAge(this DateTime birthDate)
        => ((DateTime?)birthDate).ToAge();
    
}
