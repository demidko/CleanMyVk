using System;
using OpenQA.Selenium;

internal class GoogleClient : IDisposable
{
    private readonly IWebDriver _driver;


    public GoogleClient(IWebDriver driver)
    {
        _driver = driver;
    }

    public void Search(string text)
    {
        _driver.Manage().Window.Maximize();
        _driver.Navigate().GoToUrl("https://google.com");
        var q = _driver.FindElement(By.Name("q"));
        q.SendKeys("search info about selenium");
        q.Submit();
    }

    public void Dispose()
    {
        _driver.Dispose();
    }
}