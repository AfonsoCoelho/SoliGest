namespace SoliGestAutomationTests
{
    using System;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using Xunit;
    using OpenQA.Selenium.Support.UI;
    using SeleniumExtras.WaitHelpers;

    public class LoginTest2 : IDisposable
    {
        private readonly IWebDriver _driver;
        private const string BaseUrl = "https://127.0.0.1:49893/login";
        private const string FixedEmail = "afonso@gmail.com";
        private const string FixedPassword = "aaaaaaaa";

        public LoginTest2()
        {
            var options = new ChromeOptions();
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--allow-insecure-localhost");
            options.AddArgument("--disable-web-security");

            _driver = new ChromeDriver(options);
        }

        [Fact]
        public void Login_Should_Show_Alert_With_Correct_Message()
        {
            _driver.Navigate().GoToUrl(BaseUrl);

            // Wait for the login form to load
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("email")));

            // Fill the login form with the fixed email and password
            TypeSlowly(_driver.FindElement(By.Id("email")), FixedEmail);
            TypeSlowly(_driver.FindElement(By.Id("password")), FixedPassword);

            _driver.FindElement(By.ClassName("submit-btn")).Click();

            // Wait for alert
            wait.Until(ExpectedConditions.AlertIsPresent());

            // Switch to alert
            IAlert alert = _driver.SwitchTo().Alert();

            // Validate and accept the alert
            Assert.Equal("Email ou password inválidos!", alert.Text);
            alert.Accept();
        }

        private void TypeSlowly(IWebElement element, string text)
        {
            element.Clear(); // Ensure no previous text
            foreach (char c in text)
            {
                element.SendKeys(c.ToString());
                System.Threading.Thread.Sleep(50); // Small delay to prevent missing characters
            }
        }

        public void Dispose()
        {
            _driver.Quit();
        }
    }
}
