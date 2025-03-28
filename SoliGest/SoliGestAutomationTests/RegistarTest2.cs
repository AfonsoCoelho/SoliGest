namespace SoliGestAutomationTests;

using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

public class RegistarTest2 : IDisposable
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;

    public RegistarTest2()
    {
        var options = new ChromeOptions();
        options.AddArgument("--ignore-certificate-errors");
        options.AddArgument("--allow-insecure-localhost");
        options.AddArgument("--disable-web-security");

        _driver = new ChromeDriver(options);
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Register_Should_Show_Error_With_Invalid_Email_Message()
    {
        //WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));

        _driver.Navigate().GoToUrl("https://soligest.azurewebsites.net/registar");

        _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("email")));

        // Fill the form slowly to prevent UI race conditions
        TypeSlowly(_driver.FindElement(By.Id("name")), "afonso");
        TypeSlowly(_driver.FindElement(By.Id("email")), "aiooo");
        TypeSlowly(_driver.FindElement(By.Id("password")), "Password1!");
        TypeSlowly(_driver.FindElement(By.Id("confirmPassword")), "Password1!");

        // Click submit
        //_driver.FindElement(By.ClassName("submit-btn")).Click();

        // Wait for validation message to appear
        IWebElement errorFeedback = _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("error-feedback")));
        Assert.Contains("Por favor insira um email válido.", errorFeedback.Text);

        // Wait for alert
        //wait.Until(ExpectedConditions.AlertIsPresent());

        // Switch to alert
        //IAlert alert = _driver.SwitchTo().Alert();

        // Validate and accept the alert
        //Assert.Equal("Por favor corriga os erros do formulário!", alert.Text);
        //alert.Accept();
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
