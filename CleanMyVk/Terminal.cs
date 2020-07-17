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
    internal static void Attention(this string text) => text.Print(Red);

    internal static void Print(this Comment comment)
    {
        $"vk.com/wall{comment.OwnerId}_{comment.PostId}?reply={comment.Id}".Print(DarkGray);
        if (comment.Text != null)
        {
            WriteLine($"  {comment.Text}");
        }
    }

    internal static string ToTextBlock(this Comment comment) =>
        $"vk.com/wall{comment.OwnerId}_{comment.PostId}?reply={comment.Id}\n" +
        comment.Text == null
            ? $"  {comment.Text}"
            : Empty;

    internal static void Print(this string text, ConsoleColor color)
    {
        var previousColor = ForegroundColor;
        ForegroundColor = color;
        WriteLine(text);
        ForegroundColor = previousColor;
    }
}