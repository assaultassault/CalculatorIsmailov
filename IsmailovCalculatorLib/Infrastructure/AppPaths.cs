using System;
using System.IO;

namespace IsmailovCalculatorLib.Infrastructure;

internal static class AppPaths
{
    private const string AppFolderName = "IsmailovCalculator";

    public static string GetFilePath(string fileName)
    {
        var baseDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(baseDir, AppFolderName, fileName);
    }
}