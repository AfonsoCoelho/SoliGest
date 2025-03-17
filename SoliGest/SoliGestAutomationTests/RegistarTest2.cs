namespace SoliGestAutomationTests;

using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

public class RegistarTest2 : IDisposable
{
    private readonly IWebDriver _driver;

    public RegistarTest2()
    {
        var options = new ChromeOptions();
        options.AddArgument("--ignore-certificate-errors");
        options.AddArgument("--allow-insecure-localhost");
        options.AddArgument("--disable-web-security");

        _driver = new ChromeDriver(options);
    }

    [Fact]
    public void Register_Should_Show_Alert_With_Invalid_Email_Message()
    {
        _driver.Navigate().GoToUrl("https://127.0.0.1:49893/registar");

        // Wait for form to load
        WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
        wait.Until(ExpectedConditions.ElementIsVisible(By.Id("email")));

        // Fill the form with an invalid email
        TypeSlowly(_driver.FindElement(By.Id("name")), "afonso");
        TypeSlowly(_driver.FindElement(By.Id("email")), "aiooo");
        TypeSlowly(_driver.FindElement(By.Id("password")), "Password1!");
        TypeSlowly(_driver.FindElement(By.Id("confirmPassword")), "Password1!");

        _driver.FindElement(By.ClassName("submit-btn")).Click();

        // Wait for alert
        wait.Until(ExpectedConditions.AlertIsPresent());

        // Switch to alert
        IAlert alert = _driver.SwitchTo().Alert();

        string alertText = alert.Text;
        // Normalize alert text encoding (if necessary)
        alertText = System.Text.Encoding.UTF8.GetString(System.Text.Encoding.Default.GetBytes(alertText));

        // Validate and accept the alert
        Assert.Contains("Form inv", alertText);
        alert.Accept();

    }

    private void TypeSlowly(IWebElement element, string text)
    {
        element.Clear();
        foreach (char c in text)
        {
            element.SendKeys(c.ToString());
            Thread.Sleep(50); // Small delay to prevent missing characters
        }
    }

    public void Dispose()
    {
        _driver.Quit();
    }
}
