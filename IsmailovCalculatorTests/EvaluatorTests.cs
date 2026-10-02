using IsmailovCalculatorLib.Engine;
using IsmailovCalculatorLib.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;

namespace IsmailovCalculatorTests;

[TestClass]
public class EvaluatorTests
{
    [TestMethod]
    [DataRow("2+2", "4")]
    [DataRow("2+2*2", "6")]
    [DataRow("(2+2)*2", "8")]
    [DataRow("2^3", "8")]
    [DataRow("-5+10", "5")]
    [DataRow("10/2", "5")]
    [DataRow("3.5+2.5", "6")]
    public void Evaluate_ValidExpressions_ReturnsCorrectResult(string expression, string expectedString)
    {
        decimal expected = decimal.Parse(expectedString, CultureInfo.InvariantCulture);
        decimal actual = ExpressionEvaluator.Evaluate(expression);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Evaluate_DivideByZero_ThrowsDivideByZeroException()
    {
        try
        {
            ExpressionEvaluator.Evaluate("10/0");
            Assert.Fail("Ожидалось исключение DivideByZeroException.");
        }
        catch (DivideByZeroException)
        {
            // Успешное прохождение теста
        }
    }

    [TestMethod]
    public void Evaluate_MismatchedParentheses_ThrowsFormatException()
    {
        try
        {
            ExpressionEvaluator.Evaluate("(2+3");
            Assert.Fail("Ожидалось исключение FormatException.");
        }
        catch (FormatException)
        {
            // Успешное прохождение теста
        }
    }

    [TestMethod]
    public void Evaluate_InvalidSymbol_ThrowsFormatException()
    {
        try
        {
            ExpressionEvaluator.Evaluate("2+$3");
            Assert.Fail("Ожидалось исключение FormatException.");
        }
        catch (FormatException)
        {
            // Успешное прохождение теста
        }
    }

    [TestMethod]
    public void CalculatorEngine_CalculateAndHistory_WorksCorrectly()
    {
        string tempHistory = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"hist_{Guid.NewGuid()}.json");
        string tempSettings = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"sett_{Guid.NewGuid()}.json");

        try
        {
            var engine = new CalculatorEngine(tempHistory, tempSettings);
            decimal result = engine.Calculate("15+5");

            Assert.AreEqual(20m, result);
            Assert.HasCount(1, engine.GetHistory());
            Assert.AreEqual("15+5", engine.GetHistory()[0].Expression);
            Assert.AreEqual(20m, engine.GetHistory()[0].Result);

            engine.ClearHistory();
            Assert.IsEmpty(engine.GetHistory());
        }
        finally
        {
            if (System.IO.File.Exists(tempHistory)) System.IO.File.Delete(tempHistory);
            if (System.IO.File.Exists(tempSettings)) System.IO.File.Delete(tempSettings);
        }
    }
}