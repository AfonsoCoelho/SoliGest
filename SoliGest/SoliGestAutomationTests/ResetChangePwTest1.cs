namespace SoliGestAutomationTests;

using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

public class ChangePasswordTest1 : IDisposable
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;

    public ChangePasswordTest1()
    {
        var options = new ChromeOptions();
        options.AddArgument("--ignore-certificate-errors");
        options.AddArgument("--allow-insecure-localhost");
        options.AddArgument("--disable-web-security");

        _driver = new ChromeDriver(options);
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void ChangePassword_Should_Show_Error_When_Passwords_Do_Not_Match()
    {
        _driver.Navigate().GoToUrl("https://127.0.0.1:49893/changepw");

        // Fill the form
        TypeSlowly(_driver.FindElement(By.Id("newPassword")), "NewPassword123!");
        TypeSlowly(_driver.FindElement(By.Id("confirmPassword")), "DifferentPassword456!");

        _driver.FindElement(By.TagName("body")).Click();

        // Click submit
        //_driver.FindElement(By.ClassName("submit-btn")).Click();

        // Wait for validation message to appear
        IWebElement errorFeedback = _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("error-feedback")));
        Assert.Contains("As palavras-passes não coincidem.", errorFeedback.Text);
    }

    // Function to type text character by character with small delay
    private void TypeSlowly(IWebElement element, string text)
    {
        element.Clear();
        foreach (char c in text)
        {
            element.SendKeys(c.ToString());
            Thread.Sleep(50); // Small delay to simulate human typing
        }
    }

    public void Dispose()
    {
        _driver.Quit();
    }
}
