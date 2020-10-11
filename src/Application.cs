﻿using System.IO;
using System.Linq;
using Microsoft.VisualBasic;
using static System.ConsoleColor;
using static System.IO.Directory;
using static Authorization;

/// <summary>
/// Точка входа описывает логику приложения на высоком уровне
/// </summary>
internal static class Application
{
    private static void Main(string[] args)
    {
        var api = Login(args);
        foreach (var m in new CommentCollector(api).ListMentions())
        {
            m.Text.Println();
        }
    }
}