namespace SoliGestAutomationTests
{
    using System;
    using System.Linq;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using Xunit;
    using OpenQA.Selenium.Support.UI;
    using SeleniumExtras.WaitHelpers;

    public class NotificationTest1 : IDisposable
    {
        private readonly IWebDriver _driver;
        private const string LoginUrl = "https://soligest.azurewebsites.net/login";
        private const string TechnicianEmail = "technician@mail.com";
        private const string TechnicianPassword = "Tech1!";
        private const int DefaultTimeoutSeconds = 5;

        public NotificationTest1()
        {
            var options = new ChromeOptions();
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--allow-insecure-localhost");
            options.AddArgument("--disable-web-security");

            _driver = new ChromeDriver(options);
        }

        [Fact]
        public void NotificationDelete_Should_Show_DeletedAlert()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(DefaultTimeoutSeconds));

            _driver.Navigate().GoToUrl(LoginUrl);
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("email")));

            TypeSlowly(_driver.FindElement(By.Id("email")), TechnicianEmail);
            TypeSlowly(_driver.FindElement(By.Id("password")), TechnicianPassword);
            _driver.FindElement(By.ClassName("submit-btn")).Click();

            wait.Until(ExpectedConditions.AlertIsPresent());
            var alert = _driver.SwitchTo().Alert();
            Assert.Equal("Login efetuado com sucesso!", alert.Text);
            alert.Accept();

            // Ensure header is present on homepage
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("header.soligest-header")));

            // improvise adapt overcome, for that we must refreh the page antes de tentar aceder a notif
            Thread.Sleep(1000);
            _driver.Navigate().Refresh();

            var notificationIcon = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(".notification-icon")));
            notificationIcon.Click();

            // Wait for notifications panel to appear
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".notifications-panel")));

            // Attempt to dismiss the first notification
            var dismissButtons = _driver.FindElements(By.CssSelector(".notification-item .dismiss-btn"));
            if (dismissButtons.Any())
            {
                dismissButtons.First().Click();

                // Wait for delete success alert
                wait.Until(ExpectedConditions.AlertIsPresent());
                var deleteAlert = _driver.SwitchTo().Alert();
                Assert.Equal("Notificação apagada com sucesso!", deleteAlert.Text);
                deleteAlert.Accept();
            }
            else
            {
                throw new Exception("Nenhuma notificação disponível para apagar.");
            }
        }

        private void TypeSlowly(IWebElement element, string text)
        {
            element.Clear();
            foreach (char c in text)
            {
                element.SendKeys(c.ToString());
                System.Threading.Thread.Sleep(50);
            }
        }

        public void Dispose()
        {
            _driver.Quit();
        }
    }
}
