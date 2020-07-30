using System;
using System.Security.Cryptography.X509Certificates;
using VkNet.Model;
using static System.Console;
using static System.ConsoleColor;
using static System.String;

/// <summary>
/// Расширяет System.Console
/// </summary>
internal static class Terminal
{
    internal static void PrintlnAsAttention(this string text) => text.PrintlnWith(Red);

    internal static void Println(this NewsItem post) =>
        $"vk.com/wall{post.SourceId}_{post.PostId}".PrintlnWith(Blue);

    internal static void PrintlnWith(this string beforeLink, Comment comment, ConsoleColor linkColor = Gray)
    {
        $"{beforeLink} vk.com/wall{comment.OwnerId}_{comment.PostId}?reply={comment.Id} ".PrintWith(linkColor);
        if (comment.Text != null)
        {
            WriteLine($"{comment.Text}");
        }
    }

    internal static void Println(this string s) => WriteLine(s);

    private static void PrintWith(this string text, ConsoleColor color)
    {
        var previousColor = ForegroundColor;
        ForegroundColor = color;
        Write(text);
        ForegroundColor = previousColor;
    }

    private static void PrintlnWith(this string text, ConsoleColor color)
    {
        text.PrintWith(color);
        WriteLine();
    }
}