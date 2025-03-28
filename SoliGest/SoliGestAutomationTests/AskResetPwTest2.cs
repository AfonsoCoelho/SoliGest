﻿namespace SoliGestAutomationTests;

using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

public class AskResetPwTest2 : IDisposable
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;

    public AskResetPwTest2()
    {
        var options = new ChromeOptions();
        options.AddArgument("--ignore-certificate-errors");
        options.AddArgument("--allow-insecure-localhost");
        options.AddArgument("--disable-web-security");

        _driver = new ChromeDriver(options);
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void PwRecovery_Should_Show_Error_When_Invalid_Email()
    {
        _driver.Navigate().GoToUrl("https://soligest.azurewebsites.net/pwrecovery");

        // Fill in an invalid email
        TypeSlowly(_driver.FindElement(By.Id("email")), "not-an-email");

        // Click submit
        _driver.FindElement(By.ClassName("submit-btn")).Click();

        // Wait for alert to be present
        _wait.Until(ExpectedConditions.AlertIsPresent());

        // Switch to alert
        IAlert alert = _driver.SwitchTo().Alert();

        // Validate and accept the alert
        Assert.Equal("Por favor corriga os erros do formulário!", alert.Text);
        alert.Accept();
    }

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
