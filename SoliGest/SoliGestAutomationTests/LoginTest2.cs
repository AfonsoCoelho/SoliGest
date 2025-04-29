namespace SoliGestAutomationTests
{
    using System;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using Xunit;
    using OpenQA.Selenium.Support.UI;
    using SeleniumExtras.WaitHelpers;

    /// <summary>
    /// Teste automatizado para verificar o login com um e-mail e senha inválidos.
    /// </summary>
    public class LoginTest2 : IDisposable
    {
        private readonly IWebDriver _driver;
        private const string BaseUrl = "https://soligest.azurewebsites.net/login";
        private const string FixedEmail = "non-existing-user@gmail.com"; // E-mail inválido para o teste
        private const string FixedPassword = "aaaaaaaa"; // Senha inválida para o teste

        /// <summary>
        /// Inicializa o teste configurando o WebDriver para o navegador Chrome.
        /// </summary>
        public LoginTest2()
        {
            var options = new ChromeOptions();
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--allow-insecure-localhost");
            options.AddArgument("--disable-web-security");

            _driver = new ChromeDriver(options);
        }

        /// <summary>
        /// Testa o processo de login com credenciais inválidas e verifica se o alerta de erro é exibido corretamente.
        /// </summary>
        [Fact]
        public void Login_Should_Show_Alert_With_Error_Message()
        {
            _driver.Navigate().GoToUrl(BaseUrl);

            // Espera até que o campo de e-mail esteja visível
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("email")));

            // Preenche o formulário de login com e-mail e senha inválidos
            TypeSlowly(_driver.FindElement(By.Id("email")), FixedEmail);
            TypeSlowly(_driver.FindElement(By.Id("password")), FixedPassword);

            // Clica no botão de submit
            _driver.FindElement(By.ClassName("submit-btn")).Click();

            // Espera até que o alerta de erro seja exibido
            wait.Until(ExpectedConditions.AlertIsPresent());

            // Troca para o alerta e valida a mensagem
            IAlert alert = _driver.SwitchTo().Alert();
            Assert.Equal("Email ou password inválidos!", alert.Text);
            alert.Accept();
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
