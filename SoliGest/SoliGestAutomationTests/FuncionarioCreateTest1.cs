using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Threading;

namespace SoliGestAutomationTests
{
    public class FuncionarioCreateTest1 : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;
        private string _uniqueEmail;

        public FuncionarioCreateTest1()
        {
            var options = new ChromeOptions();
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--allow-insecure-localhost");
            options.AddArgument("--disable-web-security");

            _driver = new ChromeDriver(options);
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            _uniqueEmail = $"testuser{DateTime.Now.Ticks}@gmail.com";
        }

        [Fact]
        public void Funcionario_Create_Should_Show_Alert_With_Correct_Message()
        {
            _driver.Navigate().GoToUrl("https://soligest.azurewebsites.net/funcionario-create");

            // Wait until the form is loaded (we'll use the 'name' input as an indicator)
            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("name")));

            TypeSlowly(_driver.FindElement(By.Id("name")), "afonso");
            TypeSlowly(_driver.FindElement(By.Id("email")), _uniqueEmail);
            TypeSlowly(_driver.FindElement(By.Id("phoneNumber")), "999999999");
            TypeSlowly(_driver.FindElement(By.Id("address1")), "address1");
            TypeSlowly(_driver.FindElement(By.Id("address2")), "address2");

            // Set birthDate (format yyyy-MM-dd for input type="date")
            _driver.FindElement(By.Id("birthDate")).SendKeys(DateTime.Now.ToString("yyyy-MM-dd"));
            TypeSlowly(_driver.FindElement(By.Id("password")), "Password1!");
            TypeSlowly(_driver.FindElement(By.Id("confirmPassword")), "Password1!");

            // Select role (cargo)
            SelectElement roleSelect = new SelectElement(_driver.FindElement(By.Id("cargo")));
            roleSelect.SelectByIndex(1); // Administrativo

            // Select day off
            SelectElement dayOffSelect = new SelectElement(_driver.FindElement(By.Id("dayOff")));
            dayOffSelect.SelectByIndex(1); // Terça-feira

            // Holidays
            _driver.FindElement(By.Id("startHoliday")).SendKeys("2025-10-10");
            _driver.FindElement(By.Id("endHoliday")).SendKeys("2025-10-10");

            // Submit
            _driver.FindElement(By.ClassName("submit-btn")).Click();

            // Wait for alert to appear
            _wait.Until(ExpectedConditions.AlertIsPresent());

            // Handle alert
            IAlert alert = _driver.SwitchTo().Alert();
            Assert.Equal("Registo bem sucedido!", alert.Text);
            alert.Accept();
        }

        private void TypeSlowly(IWebElement element, string text)
        {
            element.Clear();
            foreach (char c in text)
            {
                element.SendKeys(c.ToString());
                Thread.Sleep(50);
            }
        }

        public void Dispose()
        {
            _driver.Quit();
        }
    }
}
