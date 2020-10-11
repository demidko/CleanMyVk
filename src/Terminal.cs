using System;
using static System.Console;
using static System.ConsoleColor;

internal static class Terminal
{
    internal static void Println<T>(this T obj, ConsoleColor color = Black)
    {
        var previousColor = ForegroundColor;
        ForegroundColor = color;
        WriteLine(obj);
        ForegroundColor = previousColor;
    }
}