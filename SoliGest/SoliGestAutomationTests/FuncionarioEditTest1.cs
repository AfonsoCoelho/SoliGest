namespace SoliGestAutomationTests;

using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

public class FuncionarioEditTest1 : IDisposable
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;

    public FuncionarioEditTest1()
    {
        var options = new ChromeOptions();
        options.AddArgument("--ignore-certificate-errors");
        options.AddArgument("--allow-insecure-localhost");
        options.AddArgument("--disable-web-security");

        _driver = new ChromeDriver(options);
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Funcionario_Edit_Should_Show_Alert_With_Correct_Message()
    {
        WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));

        _driver.Navigate().GoToUrl("https://soligest.azurewebsites.net/funcionario");

        TimeSpan.FromSeconds(5);

        _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("edit-btn")));

        TimeSpan.FromSeconds(5);

        _driver.FindElement(By.Id("edit/soligestesa@gmail.com")).Click();

        TimeSpan.FromSeconds(5);

        _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("name")));

        // Fill the form slowly to prevent UI race conditions
        _driver.FindElement(By.Id("name")).Clear();
        TypeSlowly(_driver.FindElement(By.Id("name")), "SoliGest Supervisor (New Name)");

        // Click submit
        _driver.FindElement(By.ClassName("submit-btn")).Click();

        // Wait for validation message to appear
        _wait.Until(ExpectedConditions.AlertIsPresent());

        // Switch to alert
        IAlert alert = _driver.SwitchTo().Alert();

        // Validate and accept the alert
        Assert.Equal("Utilizador atualizado com sucesso!", alert.Text);
        alert.Accept();

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
