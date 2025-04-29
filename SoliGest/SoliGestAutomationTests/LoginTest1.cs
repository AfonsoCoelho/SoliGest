namespace SoliGestAutomationTests
{
    using System;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using Xunit;
    using OpenQA.Selenium.Support.UI;
    using SeleniumExtras.WaitHelpers;

    /// <summary>
    /// Teste automatizado para verificar o login de um usuário no sistema.
    /// </summary>
    public class LoginTest1 : IDisposable
    {
        private readonly IWebDriver _driver;
        private const string BaseUrl = "https://soligest.azurewebsites.net/login";
        private const string FixedEmail = "soligestesa@gmail.com";
        private const string FixedPassword = "Admin1!";

        /// <summary>
        /// Inicializa o teste configurando o WebDriver para o navegador Chrome.
        /// </summary>
        public LoginTest1()
        {
            var options = new ChromeOptions();
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--allow-insecure-localhost");
            options.AddArgument("--disable-web-security");

            _driver = new ChromeDriver(options);
        }

        /// <summary>
        /// Testa o processo de login e verifica se o alerta de sucesso é exibido com a mensagem correta.
        /// </summary>
        [Fact]
        public void Login_Should_Show_Alert_With_Correct_Message()
        {
            _driver.Navigate().GoToUrl(BaseUrl);

            // Espera até que o campo de e-mail esteja visível
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("email")));

            // Preenche o formulário de login com o e-mail fixo e senha
            TypeSlowly(_driver.FindElement(By.Id("email")), FixedEmail);
            TypeSlowly(_driver.FindElement(By.Id("password")), FixedPassword);

            // Clica no botão de submit
            _driver.FindElement(By.ClassName("submit-btn")).Click();

            // Espera até que o alerta seja exibido
            wait.Until(ExpectedConditions.AlertIsPresent());

            // Troca para o alerta e valida a mensagem
            IAlert alert = _driver.SwitchTo().Alert();
            Assert.Equal("Login efetuado com sucesso!", alert.Text);
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
