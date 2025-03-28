namespace SoliGestAutomationTests;

using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

public class FuncionarioCreateTest : IDisposable
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;

    public FuncionarioCreateTest()
    {
        var options = new ChromeOptions();
        options.AddArgument("--ignore-certificate-errors");
        options.AddArgument("--allow-insecure-localhost");
        options.AddArgument("--disable-web-security");

        _driver = new ChromeDriver(options);
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Funcionario_Create_Should_Show_Alert_With_Correct_Message()
    {
        //WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));

        _driver.Navigate().GoToUrl("https://127.0.0.1:49893/funcionario");

        _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("add-button")));

        _driver.FindElement(By.ClassName("add-button")).Click();

        // Fill the form slowly to prevent UI race conditions
        TypeSlowly(_driver.FindElement(By.Id("name")), "afonso");
        TypeSlowly(_driver.FindElement(By.Id("email")), "test@mail.com");
        TypeSlowly(_driver.FindElement(By.Id("phoneNumber")), "999999999");
        TypeSlowly(_driver.FindElement(By.Id("address1")), "address1");
        TypeSlowly(_driver.FindElement(By.Id("address2")), "address2");
        TypeSlowly(_driver.FindElement(By.Id("birthDate")), DateTime.Now.ToString("dd-MM-yyyy"));
        TypeSlowly(_driver.FindElement(By.Id("password")), "Password1!");
        TypeSlowly(_driver.FindElement(By.Id("confirmPassword")), "Password1!");
        // role
        WebElement roleDropdown = (WebElement)_driver.FindElement(By.Id("cargo"));
        SelectElement roleSelectObject = new SelectElement(roleDropdown);
        roleSelectObject.SelectByIndex(1);
        // dayOff
        WebElement dayOffDropdown = (WebElement)_driver.FindElement(By.Id("dayOff"));
        SelectElement dayOffSelectObject = new SelectElement(dayOffDropdown);
        dayOffSelectObject.SelectByIndex(1);
        TypeSlowly(_driver.FindElement(By.Id("startHoliday")), DateTime.Now.ToString("dd-MM-yyyy"));
        TypeSlowly(_driver.FindElement(By.Id("endHoliday")), DateTime.Now.ToString("dd-MM-yyyy"));



        // Click submit
        _driver.FindElement(By.ClassName("submit-btn")).Click();

        // Wait for validation message to appear
        _wait.Until(ExpectedConditions.AlertIsPresent());

        // Switch to alert
        IAlert alert = _driver.SwitchTo().Alert();

        // Validate and accept the alert
        Assert.Equal("Registo bem sucedido!", alert.Text);
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
