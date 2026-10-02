namespace IsmailovCalculatorLib.Models;

public sealed class ThemeSettings
{
    public static readonly ThemeSettings Default = new(isDarkTheme: true, accentColor: "#6C5CE7");

    public bool IsDarkTheme { get; set; } = true;
    public string AccentColor { get; set; } = "#6C5CE7";

    public ThemeSettings() { }

    public ThemeSettings(bool isDarkTheme, string accentColor)
    {
        IsDarkTheme = isDarkTheme;
        AccentColor = accentColor;
    }
}