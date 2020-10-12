using System;
using System.Threading;
using OpenQA.Selenium;

internal class Google : IDisposable
{
    private readonly IWebDriver _driver;

    public Google(IWebDriver driver)
    {
        _driver = driver;
        _driver.Manage().Window.Maximize();
    }

    public void Search(string text)
    {
        _driver.Navigate().GoToUrl("https://google.com");
        var input  = _driver.FindElement(By.Name("q"));
        input.SendKeys(text);
        input.Submit();
    }
    
    

    public void Dispose()
    {
        _driver.Dispose();
    }
}