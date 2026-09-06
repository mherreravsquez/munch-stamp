using System.Globalization;

namespace munch_stamp.Converters;

public class InitialsConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string name || string.IsNullOrWhiteSpace(name))
            return "";

        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length == 1)
            return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpperInvariant();

        var initials = string.Concat(parts.Take(2).Select(part => char.ToUpperInvariant(part[0])));
        return initials.Length > 2 ? initials[..2] : initials;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => value ?? string.Empty;
}
