using System;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.VisualBasic;
using OpenQA.Selenium.Firefox;
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
        using var driver = new FirefoxDriver();
        driver.Manage().Window.Maximize();
        driver.Navigate().GoToUrl("https://google.com");
        Thread.Sleep(TimeSpan.FromSeconds(30));
    }
}