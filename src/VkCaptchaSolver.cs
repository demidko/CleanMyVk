using System;
using OpenQA.Selenium;
using VkNet.Utils.AntiCaptcha;
using static System.ConsoleColor;
using static Terminal;

internal class VkCaptchaSolver : ICaptchaSolver
{
    private readonly IWebDriver _driver;

    public VkCaptchaSolver(IWebDriver driver)
    {
        _driver = driver;
    }

    public string Solve(string url)
    {
        _driver.Navigate().GoToUrl(url);
        return Input("Captcha: ", DarkBlue);
    }

    public void CaptchaIsFalse() => "Invalid captcha!".Println(Red);
}