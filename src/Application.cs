using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.VisualBasic;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Safari;
using static System.ConsoleColor;
using static System.IO.Directory;
using static System.TimeSpan;
using static Authorization;
using static OpenQA.Selenium.By;

/// <summary>
/// Точка входа описывает логику приложения на высоком уровне
/// </summary>
internal static class Application
{
    private static void Main(string[] args)
    {
        
        using var driver = new ChromeDriver();
        driver.Manage().Window.Maximize();
        driver.Navigate().GoToUrl("https://google.com");
        var el = driver.FindElement(Name("q"));
        el.SendKeys("search info about selenium");
        el.Submit();
        Thread.Sleep(FromSeconds(10));
    }
}