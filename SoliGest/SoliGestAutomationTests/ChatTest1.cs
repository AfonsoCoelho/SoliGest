namespace SoliGestAutomationTests
{
    using System;
    using System.Linq;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using Xunit;
    using OpenQA.Selenium.Support.UI;
    using SeleniumExtras.WaitHelpers;

    /// <summary>
    /// Classe de testes automatizados para verificar a funcionalidade de chat, incluindo login e envio de mensagens.
    /// </summary>
    public class ChatTest1 : IDisposable
    {
        private readonly IWebDriver _driver;
        private const string LoginUrl = "https://soligest.azurewebsites.net/login";
        private const string TechnicianEmail = "technician@mail.com";
        private const string TechnicianPassword = "Tech1!";
        private const int DefaultTimeoutSeconds = 5;

        /// <summary>
        /// Inicializa a instância do teste, configurando o WebDriver para o navegador Chrome.
        /// </summary>
        public ChatTest1()
        {
            var options = new ChromeOptions();
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--allow-insecure-localhost");
            options.AddArgument("--disable-web-security");

            _driver = new ChromeDriver(options);
        }

        /// <summary>
        /// Testa o login e o envio de mensagem no chat, verificando se a mensagem enviada aparece na conversa.
        /// </summary>
        [Fact]
        public void LoginAndSendMessage_Should_Display_Sent_Message()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(DefaultTimeoutSeconds));

            // Navega para a página de login
            _driver.Navigate().GoToUrl(LoginUrl);
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("email")));

            // Realiza o login
            TypeSlowly(_driver.FindElement(By.Id("email")), TechnicianEmail);
            TypeSlowly(_driver.FindElement(By.Id("password")), TechnicianPassword);
            _driver.FindElement(By.ClassName("submit-btn")).Click();

            // Aguarda o alerta de sucesso e o aceita
            wait.Until(ExpectedConditions.AlertIsPresent());
            var alert = _driver.SwitchTo().Alert();
            Assert.Equal("Login efetuado com sucesso!", alert.Text);
            alert.Accept();

            // Navega para a seção de chat
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("a[routerLink='/chat']")));
            _driver.FindElement(By.CssSelector("a[routerLink='/chat']")).Click();

            // Aguarda a página de chat carregar
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".chat-title")));

            // Seleciona a primeira conversa
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".conversation-item")));
            var firstConversation = _driver.FindElement(By.CssSelector(".conversation-item"));
            firstConversation.Click();

            // Aguarda o campo de entrada de mensagem ser clicável
            var messageInput = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input[placeholder='Escrever...']")));

            // Envia uma mensagem de teste única
            string testMessage = "Madje dodo " + DateTime.Now.Ticks;
            TypeSlowly(messageInput, testMessage);
            _driver.FindElement(By.CssSelector("button.send-btn")).Click();

            // Aumenta o tempo de espera
            var longWait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            // Normaliza o texto da mensagem esperada
            string normalizedTestMessage = testMessage.Trim();

            // Espera até que a mensagem enviada apareça no DOM
            longWait.Until(driver =>
            {
                var messages = driver.FindElements(By.CssSelector(".message.sent"));
                return messages.Any(m => m.Text.Trim().Contains(normalizedTestMessage));
            });

            // Obtém a mensagem enviada e verifica seu conteúdo
            var sentMessage = _driver.FindElements(By.CssSelector(".message.sent"))
                                     .FirstOrDefault(m => m.Text.Trim().Contains(normalizedTestMessage));

            Assert.NotNull(sentMessage);
            Assert.Contains(normalizedTestMessage, sentMessage.Text.Trim());
        }

        /// <summary>
        /// Digita um texto lentamente em um elemento de entrada, caracter por caracter.
        /// </summary>
        /// <param name="element">O elemento de entrada onde o texto será digitado.</param>
        /// <param name="text">O texto a ser digitado.</param>
        private void TypeSlowly(IWebElement element, string text)
        {
            element.Clear();
            foreach (char c in text)
            {
                element.SendKeys(c.ToString());
                System.Threading.Thread.Sleep(50);
            }
        }

        /// <summary>
        /// Finaliza o WebDriver após a execução dos testes.
        /// </summary>
        public void Dispose()
        {
            _driver.Quit();
        }
    }
}
