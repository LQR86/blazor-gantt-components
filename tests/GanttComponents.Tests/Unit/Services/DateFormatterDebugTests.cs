using GanttComponents.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GanttComponents.Tests.Unit.Services;

public class DateFormatterDebugTests
{
    [Fact]
    public void Debug_DayNumberFormatting_EnglishVsChinese()
    {
        // This test helps debug the issue where English shows full dates
        // but Chinese shows correct day numbers like "1日", "2日"

        var services = new ServiceCollection();
        services.AddSingleton<ILogger<GanttI18N>>(provider =>
            LoggerFactory.Create(builder => builder.AddConsole())
                .CreateLogger<GanttI18N>());
        services.AddSingleton<IGanttI18N, GanttI18N>();
        services.AddSingleton<DateFormatHelper>();

        var serviceProvider = services.BuildServiceProvider();
        var i18n = serviceProvider.GetRequiredService<IGanttI18N>();
        var dateFormatter = serviceProvider.GetRequiredService<DateFormatHelper>();

        var testDate = new DateTime(2025, 7, 15);

        // Test English formatting
        i18n.SetCulture("en-US");
        var englishPattern = i18n.T("date.day-number");
        var englishResult = dateFormatter.FormatTimelineHeader(testDate, "date.day-number");

        // Test Chinese formatting  
        i18n.SetCulture("zh-CN");
        var chinesePattern = i18n.T("date.day-number");
        var chineseResult = dateFormatter.FormatTimelineHeader(testDate, "date.day-number");

        // Output debug information
        Console.WriteLine($"English pattern: '{englishPattern}'");
        Console.WriteLine($"English result: '{englishResult}'");
        Console.WriteLine($"Chinese pattern: '{chinesePattern}'");
        Console.WriteLine($"Chinese result: '{chineseResult}'");

        // Verify the patterns are correct
        Assert.Equal("%d", englishPattern);
        Assert.Equal("%d日", chinesePattern);

        // Verify the results
        Assert.Equal("15", englishResult); // Should be just the day number
        Assert.Equal("15日", chineseResult); // Should be day number + 日
    }

    [Fact]
    public void Debug_DirectDateToStringFormatting()
    {
        // Test direct .NET DateTime formatting to confirm patterns work
        var testDate = new DateTime(2025, 7, 15);

        // Test the "d" pattern directly
        var englishCulture = new System.Globalization.CultureInfo("en-US");
        var chineseCulture = new System.Globalization.CultureInfo("zh-CN");

        var englishDirect = testDate.ToString("d", englishCulture);
        var chineseDirect = testDate.ToString("d", chineseCulture);
        var englishDayOnly = testDate.ToString("dd", englishCulture);
        var chineseDayOnly = testDate.ToString("dd", chineseCulture);

        Console.WriteLine($"English 'd' pattern: '{englishDirect}'");
        Console.WriteLine($"Chinese 'd' pattern: '{chineseDirect}'");
        Console.WriteLine($"English 'dd' pattern: '{englishDayOnly}'");
        Console.WriteLine($"Chinese 'dd' pattern: '{chineseDayOnly}'");

        // The "d" pattern in .NET means "short date pattern", not "day of month"!
        // We need "%d" to get just the day of month
        var englishJustDay = testDate.ToString("%d", englishCulture);
        var chineseJustDay = testDate.ToString("%d", chineseCulture);

        Console.WriteLine($"English '%d' pattern: '{englishJustDay}'");
        Console.WriteLine($"Chinese '%d' pattern: '{chineseJustDay}'");

        Assert.Equal("15", englishJustDay);
        Assert.Equal("15", chineseJustDay);
    }
}
