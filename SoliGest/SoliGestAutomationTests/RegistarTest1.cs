namespace SoliGestAutomationTests;

using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

public class RegistarTest1 : IDisposable
{
    private readonly IWebDriver _driver;
    private readonly string _uniqueEmail;

    public RegistarTest1()
    {
        var options = new ChromeOptions();
        options.AddArgument("--ignore-certificate-errors");
        options.AddArgument("--allow-insecure-localhost");
        options.AddArgument("--disable-web-security");

        _driver = new ChromeDriver(options);

        // email unico para cada teste pq n pode haver repetidos
        _uniqueEmail = $"testuser{DateTime.Now.Ticks}@gmail.com";
    }

    [Fact]
    public void Login_Should_Show_Alert_With_Correct_Message()
    {
        _driver.Navigate().GoToUrl("https://soligest.azurewebsites.net/registar");

        // esperar o alerta dar load
        WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
        wait.Until(ExpectedConditions.ElementIsVisible(By.Id("email")));

        // Fill the form with a unique email
        TypeSlowly(_driver.FindElement(By.Id("name")), "testuser");
        TypeSlowly(_driver.FindElement(By.Id("email")), _uniqueEmail);
        TypeSlowly(_driver.FindElement(By.Id("password")), "Password1!");
        TypeSlowly(_driver.FindElement(By.Id("confirmPassword")), "Password1!");

        _driver.FindElement(By.ClassName("submit-btn")).Click();

        // Wait for alert
        wait.Until(ExpectedConditions.AlertIsPresent());

        // Switch to alert
        IAlert alert = _driver.SwitchTo().Alert();

        // Validate and accept the alert
        Assert.Equal("Registo bem sucedido!", alert.Text);
        alert.Accept();
    }

    private void TypeSlowly(IWebElement element, string text)
    {
        element.Clear(); 
        foreach (char c in text)
        {
            element.SendKeys(c.ToString());
            Thread.Sleep(50); // delay entre cada char pa evitar perda de caracteres
        }
    }

    public void Dispose()
    {
        _driver.Quit();
    }
}
