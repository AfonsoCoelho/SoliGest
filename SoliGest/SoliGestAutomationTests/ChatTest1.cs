namespace SoliGestAutomationTests
{
    using System;
    using System.Linq;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using Xunit;
    using OpenQA.Selenium.Support.UI;
    using SeleniumExtras.WaitHelpers;

    public class ChatTest1 : IDisposable
    {
        private readonly IWebDriver _driver;
        private const string LoginUrl = "https://soligest.azurewebsites.net/login";
        private const string TechnicianEmail = "technician@mail.com";
        private const string TechnicianPassword = "Tech1!";
        private const int DefaultTimeoutSeconds = 5;

        public ChatTest1()
        {
            var options = new ChromeOptions();
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--allow-insecure-localhost");
            options.AddArgument("--disable-web-security");

            _driver = new ChromeDriver(options);
        }

        [Fact]
        public void LoginAndSendMessage_Should_Display_Sent_Message()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(DefaultTimeoutSeconds));

            // Navigate to login page
            _driver.Navigate().GoToUrl(LoginUrl);
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("email")));

            // Perform login
            TypeSlowly(_driver.FindElement(By.Id("email")), TechnicianEmail);
            TypeSlowly(_driver.FindElement(By.Id("password")), TechnicianPassword);
            _driver.FindElement(By.ClassName("submit-btn")).Click();

            // Wait for success alert and accept
            wait.Until(ExpectedConditions.AlertIsPresent());
            var alert = _driver.SwitchTo().Alert();
            Assert.Equal("Login efetuado com sucesso!", alert.Text);
            alert.Accept();

            // Navigate to Chat section
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("a[routerLink='/chat']")));
            _driver.FindElement(By.CssSelector("a[routerLink='/chat']")).Click();

            // Wait for chat page to load
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".chat-title")));

            // Select the first conversation
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".conversation-item")));
            var firstConversation = _driver.FindElement(By.CssSelector(".conversation-item"));
            firstConversation.Click();

            // Wait for input to be available
            var messageInput = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input[placeholder='Escrever...']")));

            // Send a unique test message
            string testMessage = "Madje dodo " + DateTime.Now.Ticks;
            TypeSlowly(messageInput, testMessage);
            _driver.FindElement(By.CssSelector("button.send-btn")).Click();

            // Aumentar tempo de espera
            var longWait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            // Normalizar o texto da mensagem esperada
            string normalizedTestMessage = testMessage.Trim();

            // Esperar até que a mensagem enviada apareça no DOM
            longWait.Until(driver =>
            {
                var messages = driver.FindElements(By.CssSelector(".message.sent"));
                return messages.Any(m => m.Text.Trim().Contains(normalizedTestMessage));
            });

            // Obter a mensagem e verificar o conteúdo
            var sentMessage = _driver.FindElements(By.CssSelector(".message.sent"))
                                     .FirstOrDefault(m => m.Text.Trim().Contains(normalizedTestMessage));

            Assert.NotNull(sentMessage);
            Assert.Contains(normalizedTestMessage, sentMessage.Text.Trim());
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
