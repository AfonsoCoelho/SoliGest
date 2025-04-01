namespace SoliGestAutomationTests;

using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

public class FuncionarioDeleteTest1 : IDisposable
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;

    public FuncionarioDeleteTest1()
    {
        var options = new ChromeOptions();
        options.AddArgument("--ignore-certificate-errors");
        options.AddArgument("--allow-insecure-localhost");
        options.AddArgument("--disable-web-security");

        _driver = new ChromeDriver(options);
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Funcionario_Details_Should_Show_And_Close_Model_Without_Errors()
    {
        WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));

        _driver.Navigate().GoToUrl("https://soligest.azurewebsites.net/funcionario");

        TimeSpan.FromSeconds(5);

        _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("delete-btn")));

        _driver.FindElement(By.Id("delete/test@mail.com")).Click();

        _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("confirm-delete-btn")));

        _driver.FindElement(By.ClassName("confirm-delete-btn")).Click();

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
