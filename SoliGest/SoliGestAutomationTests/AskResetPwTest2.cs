namespace SoliGestAutomationTests
{
    using System;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using Xunit;
    using OpenQA.Selenium.Support.UI;
    using SeleniumExtras.WaitHelpers;

    /// <summary>
    /// Teste de automação para o fluxo de recuperação de senha quando o email fornecido é inválido.
    /// </summary>
    public class AskResetPwTest2 : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        /// <summary>
        /// Construtor que inicializa o driver do navegador e a espera explícita.
        /// </summary>
        public AskResetPwTest2()
        {
            var options = new ChromeOptions();
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--allow-insecure-localhost");
            options.AddArgument("--disable-web-security");

            _driver = new ChromeDriver(options);
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// Testa o cenário onde o usuário tenta recuperar a senha com um email inválido (formato incorreto).
        /// </summary>
        [Fact]
        public void PwRecovery_Should_Show_Error_When_Invalid_Email()
        {
            // Navega até a página de recuperação de senha
            _driver.Navigate().GoToUrl("https://soligest.azurewebsites.net/pwrecovery");

            // Preenche um email inválido (formato incorreto)
            TypeSlowly(_driver.FindElement(By.Id("email")), "not-an-email");

            // Clica no botão de envio
            _driver.FindElement(By.ClassName("submit-btn")).Click();

            // Espera até que o alerta esteja presente
            _wait.Until(ExpectedConditions.AlertIsPresent());

            // Muda para o alerta
            IAlert alert = _driver.SwitchTo().Alert();

            // Valida o texto do alerta e aceita
            Assert.Equal("Por favor corriga os erros do formulário!", alert.Text);
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
