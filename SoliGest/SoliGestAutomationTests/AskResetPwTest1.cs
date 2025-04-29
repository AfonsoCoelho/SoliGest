namespace SoliGestAutomationTests
{
    using System;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using Xunit;
    using OpenQA.Selenium.Support.UI;
    using SeleniumExtras.WaitHelpers;

    /// <summary>
    /// Teste de automação para o fluxo de recuperação de senha quando o email fornecido não existe no sistema.
    /// </summary>
    public class AskResetPwTest1 : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        /// <summary>
        /// Construtor que inicializa o driver do navegador e a espera explícita.
        /// </summary>
        public AskResetPwTest1()
        {
            var options = new ChromeOptions();
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--allow-insecure-localhost");
            options.AddArgument("--disable-web-security");

            _driver = new ChromeDriver(options);
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// Testa o cenário onde o usuário tenta recuperar a senha com um email que não existe no sistema.
        /// </summary>
        [Fact]
        public void PwRecovery_Should_Show_Error_When_Email_Does_Not_Exist()
        {
            // Navega até a página de recuperação de senha
            _driver.Navigate().GoToUrl("https://soligest.azurewebsites.net/pwrecovery");

            // Preenche um email inválido
            TypeSlowly(_driver.FindElement(By.Id("email")), "invalid@example.com");

            // Clica no botão de envio
            _driver.FindElement(By.ClassName("submit-btn")).Click();

            // Espera até que o alerta esteja presente
            _wait.Until(ExpectedConditions.AlertIsPresent());

            // Muda para o alerta
            IAlert alert = _driver.SwitchTo().Alert();

            // Valida o texto do alerta e aceita
            Assert.Equal("Não existe uma conta com esse email no sistema!", alert.Text);
            alert.Accept();
        }

        /// <summary>
        /// Função que simula a digitação lenta de um texto no campo de entrada.
        /// </summary>
        /// <param name="element">O elemento do campo de entrada.</param>
        /// <param name="text">O texto a ser digitado.</param>
        private void TypeSlowly(IWebElement element, string text)
        {
            element.Clear();
            foreach (char c in text)
            {
                element.SendKeys(c.ToString());
                Thread.Sleep(50); // Pequeno atraso para simular digitação humana
            }
        }

        /// <summary>
        /// Libera os recursos utilizados pelo WebDriver ao final do teste.
        /// </summary>
        public void Dispose()
        {
            _driver.Quit();
        }
    }
}
