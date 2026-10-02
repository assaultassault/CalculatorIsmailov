using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using IsmailovCalculatorLib.Engine;
using IsmailovCalculatorLib.Infrastructure;
using IsmailovCalculatorLib.Models;
using IsmailovCalculatorLib.Services;

namespace IsmailovCalculator.ViewModels;

public sealed class MainViewModel : INotifyPropertyChanged
{
    private readonly CalculatorEngine _engine;
    private readonly RelayCommand _evaluateCommand;

    private string _expression = string.Empty;
    private string _resultText = string.Empty;

    private string _pageBackground = "#0F172A";
    private string _panelBackground = "#1E293B";
    private string _keyBackground = "#334155";
    private string _keyForeground = "#F8FAFC";
    private string _accentKeyBackground = "#6C5CE7";
    private string _accentKeyForeground = "#FFFFFF";
    private string _secondaryText = "#94A3B8";
    private string _displayBorder = "#475569";

    public MainViewModel()
    {
        _engine = new CalculatorEngine();

        AppendCommand = new RelayCommand(p => AppendToken(p as string ?? string.Empty));
        ClearAllCommand = new RelayCommand(_ => ClearAll());
        ClearEntryCommand = new RelayCommand(_ => ClearEntry());
        BackspaceCommand = new RelayCommand(_ => Backspace());

        _evaluateCommand = new RelayCommand(_ => EvaluateAndCommit(), _ => !string.IsNullOrWhiteSpace(Expression));
        EvaluateCommand = _evaluateCommand;

        WindowTitle = "Инженерный Калькулятор";
        ExpressionLabelText = "Выражение";

        UpdateThemeColors();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Expression
    {
        get => _expression;
        set
        {
            if (SetProperty(ref _expression, value ?? string.Empty))
            {
                _evaluateCommand.RaiseCanExecuteChanged();
                UpdatePreview();
            }
        }
    }

    public string ResultText
    {
        get => _resultText;
        set => SetProperty(ref _resultText, value ?? string.Empty);
    }

    public string PageBackground { get => _pageBackground; private set => SetProperty(ref _pageBackground, value); }
    public string PanelBackground { get => _panelBackground; private set => SetProperty(ref _panelBackground, value); }
    public string KeyBackground { get => _keyBackground; private set => SetProperty(ref _keyBackground, value); }
    public string KeyForeground { get => _keyForeground; private set => SetProperty(ref _keyForeground, value); }
    public string AccentKeyBackground { get => _accentKeyBackground; private set => SetProperty(ref _accentKeyBackground, value); }
    public string AccentKeyForeground { get => _accentKeyForeground; private set => SetProperty(ref _accentKeyForeground, value); }
    public string SecondaryText { get => _secondaryText; private set => SetProperty(ref _secondaryText, value); }
    public string DisplayBorder { get => _displayBorder; private set => SetProperty(ref _displayBorder, value); }

    public string WindowTitle { get; private set; }
    public string ExpressionLabelText { get; private set; }

    public ICommand AppendCommand { get; }
    public ICommand ClearAllCommand { get; }
    public ICommand ClearEntryCommand { get; }
    public ICommand EvaluateCommand { get; }
    public ICommand BackspaceCommand { get; }

    public void Persist() => _engine.SaveAll();

    public void AppendToken(string token)
    {
        if (!string.IsNullOrEmpty(token))
            Expression += token;
    }

    public void Backspace()
    {
        if (!string.IsNullOrEmpty(Expression))
            Expression = Expression[..^1];
    }

    private void EvaluateAndCommit()
    {
        if (string.IsNullOrWhiteSpace(Expression))
            return;

        try
        {
            var result = _engine.Calculate(Expression);
            ResultText = result.ToString(CultureInfo.InvariantCulture);
        }
        catch (Exception ex)
        {
            ResultText = $"Ошибка: {ex.Message}";
        }
    }

    private void UpdatePreview()
    {
        if (string.IsNullOrWhiteSpace(Expression))
        {
            ResultText = string.Empty;
            return;
        }

        try
        {
            var result = _engine.Evaluate(Expression);
            ResultText = result.ToString(CultureInfo.InvariantCulture);
        }
        catch
        {
            // Неполное выражение
        }
    }

    private void ClearAll()
    {
        Expression = string.Empty;
        ResultText = string.Empty;
    }

    private void ClearEntry()
    {
        if (string.IsNullOrWhiteSpace(Expression))
            return;

        var expr = Expression;
        int end = expr.Length - 1;

        while (end >= 0 && (char.IsDigit(expr[end]) || expr[end] == '.' || expr[end] == ','))
            end--;

        var removeStart = end + 1;

        if (removeStart > 0 && expr[removeStart - 1] == '-')
        {
            if (removeStart - 1 == 0)
                removeStart--;
            else
            {
                char prev = expr[removeStart - 2];
                if (!char.IsDigit(prev) && prev != '.' && prev != ',' && prev != ')')
                    removeStart--;
            }
        }

        Expression = expr.Remove(removeStart);
        ResultText = string.Empty;
    }

    private void UpdateThemeColors()
    {
        var settings = _engine.GetThemeSettings();

        if (settings.IsDarkTheme)
        {
            PageBackground = "#0F172A";
            PanelBackground = "#1E293B";
            KeyBackground = "#334155";
            KeyForeground = "#F8FAFC";
            SecondaryText = "#94A3B8";
            DisplayBorder = "#475569";
            AccentKeyBackground = "#6C5CE7";
            AccentKeyForeground = "#FFFFFF";
        }
        else
        {
            PageBackground = "#F1F5F9";
            PanelBackground = "#FFFFFF";
            KeyBackground = "#E2E8F0";
            KeyForeground = "#0F172A";
            SecondaryText = "#64748B";
            DisplayBorder = "#CBD5E1";
            AccentKeyBackground = "#4C51BF";
            AccentKeyForeground = "#FFFFFF";
        }
    }

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (Equals(field, value)) return false;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
}