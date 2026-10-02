using System;
using System.Collections.Generic;
using System.Linq;
using IsmailovCalculatorLib.Models;
using IsmailovCalculatorLib.Services;

namespace IsmailovCalculatorLib.Engine;

public sealed class CalculatorEngine
{
    private const int MaxHistoryCapacity = 200;
    private readonly HistoryService _historyService;
    private readonly SettingsService _settingsService;
    private readonly List<CalculationHistoryItem> _history;
    private ThemeSettings _themeSettings;

    public CalculatorEngine(string? historyFilePath = null, string? settingsFilePath = null)
    {
        _historyService = new HistoryService(historyFilePath);
        _settingsService = new SettingsService(settingsFilePath);

        _history = _historyService.Load().ToList();
        _themeSettings = _settingsService.Load();
    }

    public decimal Evaluate(string expression)
    {
        return ExpressionEvaluator.Evaluate(expression);
    }

    public decimal Calculate(string expression)
    {
        var result = Evaluate(expression);
        AddHistoryRecord(new CalculationHistoryItem(expression, result, DateTime.Now));
        return result;
    }

    public IReadOnlyList<CalculationHistoryItem> GetHistory() => _history.AsReadOnly();

    public void ClearHistory()
    {
        _history.Clear();
        SaveAll();
    }

    public ThemeSettings GetThemeSettings() => _themeSettings;

    public void UpdateThemeSettings(ThemeSettings settings)
    {
        _themeSettings = settings ?? throw new ArgumentNullException(nameof(settings));
        SaveAll();
    }

    public void SaveAll()
    {
        _historyService.Save(_history);
        _settingsService.Save(_themeSettings);
    }

    private void AddHistoryRecord(CalculationHistoryItem item)
    {
        _history.Add(item);
        if (_history.Count > MaxHistoryCapacity)
        {
            _history.RemoveAt(0);
        }
    }
}