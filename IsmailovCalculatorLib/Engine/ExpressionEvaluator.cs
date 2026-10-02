using System;
using System.Collections.Generic;
using System.Globalization;

namespace IsmailovCalculatorLib.Engine;

public static class ExpressionEvaluator
{
    public static decimal Evaluate(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
            throw new FormatException("Выражение не может быть пустым.");

        // Удаление пробелов
        expression = expression.Replace(" ", "");

        // Нормализация унарных операторов (+ и - в начале строки или после скобки)
        if (expression.StartsWith("-") || expression.StartsWith("+"))
        {
            expression = "0" + expression;
        }
        expression = expression.Replace("(-", "(0-").Replace("(+", "(0+");

        var tokens = Tokenize(expression);
        var rpn = ToRPN(tokens);
        return EvaluateRPN(rpn);
    }

    private static List<string> Tokenize(string expr)
    {
        var tokens = new List<string>();
        int i = 0;

        while (i < expr.Length)
        {
            char c = expr[i];

            if (char.IsDigit(c) || c == '.' || c == ',')
            {
                string number = "";
                while (i < expr.Length && (char.IsDigit(expr[i]) || expr[i] == '.' || expr[i] == ','))
                {
                    number += expr[i] == ',' ? '.' : expr[i];
                    i++;
                }
                tokens.Add(number);
            }
            else if ("+-*/^()".Contains(c))
            {
                tokens.Add(c.ToString());
                i++;
            }
            else
            {
                throw new FormatException($"Недопустимый символ в выражении: {c}");
            }
        }

        return tokens;
    }

    private static List<string> ToRPN(List<string> tokens)
    {
        var output = new List<string>();
        var operators = new Stack<string>();
        int openParens = 0;

        foreach (var token in tokens)
        {
            if (decimal.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
            {
                output.Add(token);
            }
            else if (token == "(")
            {
                operators.Push(token);
                openParens++;
            }
            else if (token == ")")
            {
                if (openParens == 0)
                    throw new FormatException("Несогласованные скобки в выражении.");

                while (operators.Count > 0 && operators.Peek() != "(")
                {
                    output.Add(operators.Pop());
                }

                if (operators.Count == 0)
                    throw new FormatException("Несогласованные скобки в выражении.");

                operators.Pop();
                openParens--;
            }
            else if (IsOperator(token))
            {
                while (operators.Count > 0 && IsOperator(operators.Peek()) &&
                       GetPrecedence(operators.Peek()) >= GetPrecedence(token))
                {
                    output.Add(operators.Pop());
                }
                operators.Push(token);
            }
        }

        if (openParens > 0)
            throw new FormatException("Несогласованные скобки в выражении.");

        while (operators.Count > 0)
        {
            var op = operators.Pop();
            if (op == "(" || op == ")")
                throw new FormatException("Несогласованные скобки в выражении.");
            output.Add(op);
        }

        return output;
    }

    private static decimal EvaluateRPN(List<string> rpn)
    {
        var stack = new Stack<decimal>();

        foreach (var token in rpn)
        {
            if (decimal.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal number))
            {
                stack.Push(number);
            }
            else if (IsOperator(token))
            {
                if (stack.Count < 2)
                    throw new FormatException("Некорректное выражение.");

                decimal b = stack.Pop();
                decimal a = stack.Pop();

                switch (token)
                {
                    case "+":
                        stack.Push(a + b);
                        break;
                    case "-":
                        stack.Push(a - b);
                        break;
                    case "*":
                        stack.Push(a * b);
                        break;
                    case "/":
                        if (b == 0)
                            throw new DivideByZeroException("Деление на ноль невозможно.");
                        stack.Push(a / b);
                        break;
                    case "^":
                        stack.Push((decimal)Math.Pow((double)a, (double)b));
                        break;
                }
            }
        }

        if (stack.Count != 1)
            throw new FormatException("Некорректное выражение.");

        return stack.Pop();
    }

    private static bool IsOperator(string token) => token is "+" or "-" or "*" or "/" or "^";

    private static int GetPrecedence(string op)
    {
        return op switch
        {
            "+" or "-" => 1,
            "*" or "/" => 2,
            "^" => 3,
            _ => 0
        };
    }
}