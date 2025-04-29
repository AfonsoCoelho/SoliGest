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
    /// Teste automatizado para verificar a funcionalidade de exclusão de notificações.
    /// </summary>
    public class NotificationTest1 : IDisposable
    {
        private readonly IWebDriver _driver;
        private const string LoginUrl = "https://soligest.azurewebsites.net/login";
        private const string TechnicianEmail = "technician@mail.com"; // E-mail do técnico para login
        private const string TechnicianPassword = "Tech1!"; // Senha do técnico para login
        private const int DefaultTimeoutSeconds = 5; // Tempo padrão de espera para carregamento de elementos

        /// <summary>
        /// Inicializa o teste configurando o WebDriver para o navegador Chrome.
        /// </summary>
        public NotificationTest1()
        {
            var options = new ChromeOptions();
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--allow-insecure-localhost");
            options.AddArgument("--disable-web-security");

            _driver = new ChromeDriver(options);
        }

        /// <summary>
        /// Testa a exclusão de notificações, verificando se o alerta de sucesso é exibido corretamente.
        /// </summary>
        [Fact]
        public void NotificationDelete_Should_Show_DeletedAlert()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(DefaultTimeoutSeconds));

            // Acessa a página de login
            _driver.Navigate().GoToUrl(LoginUrl);
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("email")));

            // Preenche o formulário de login com as credenciais do técnico
            TypeSlowly(_driver.FindElement(By.Id("email")), TechnicianEmail);
            TypeSlowly(_driver.FindElement(By.Id("password")), TechnicianPassword);
            _driver.FindElement(By.ClassName("submit-btn")).Click();

            // Aguarda e valida a mensagem de alerta de login bem-sucedido
            wait.Until(ExpectedConditions.AlertIsPresent());
            var alert = _driver.SwitchTo().Alert();
            Assert.Equal("Login efetuado com sucesso!", alert.Text);
            alert.Accept();

            // Garante que o cabeçalho está visível na página inicial
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("header.soligest-header")));

            // Refresca a página para garantir que as notificações sejam carregadas corretamente
            Thread.Sleep(1000);
            _driver.Navigate().Refresh();

            // Clica no ícone de notificações
            var notificationIcon = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(".notification-icon")));
            notificationIcon.Click();

            // Aguarda o painel de notificações aparecer
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".notifications-panel")));

            // Tenta descartar a primeira notificação
            var dismissButtons = _driver.FindElements(By.CssSelector(".notification-item .dismiss-btn"));
            if (dismissButtons.Any())
            {
                dismissButtons.First().Click();

                // Aguarda o alerta de sucesso de exclusão
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

        /// <summary>
        /// Função para digitar texto lentamente, caractere por caractere, com um pequeno atraso entre cada caractere.
        /// </summary>
        /// <param name="element">Elemento no qual o texto será digitado.</param>
        /// <param name="text">Texto a ser digitado.</param>
        private void TypeSlowly(IWebElement element, string text)
        {
            element.Clear(); // Limpa o campo antes de digitar
            foreach (char c in text)
            {
                element.SendKeys(c.ToString());
                System.Threading.Thread.Sleep(50); // Pequeno atraso para evitar que os caracteres sejam perdidos
            }
        }

        /// <summary>
        /// Finaliza o WebDriver após a execução dos testes, liberando os recursos.
        /// </summary>
        public void Dispose()
        {
            _driver.Quit();
        }
    }
}
