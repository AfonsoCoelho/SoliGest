namespace SoliGestAutomationTests
{
    using System;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using Xunit;
    using OpenQA.Selenium.Support.UI;
    using SeleniumExtras.WaitHelpers;

    /// <summary>
    /// Testes automatizados para a funcionalidade de alteração de senha.
    /// </summary>
    public class ChangePasswordTest1 : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="ChangePasswordTest1"/> e configura o ambiente de teste.
        /// </summary>
        public ChangePasswordTest1()
        {
            var options = new ChromeOptions();
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--allow-insecure-localhost");
            options.AddArgument("--disable-web-security");

            _driver = new ChromeDriver(options);
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// Testa a funcionalidade de alteração de senha, verificando se é exibido um erro quando as senhas não coincidem.
        /// </summary>
        [Fact]
        public void ChangePassword_Should_Show_Error_When_Passwords_Do_Not_Match()
        {
            _driver.Navigate().GoToUrl("https://soligest.azurewebsites.net/changepw");

            // Preenche o formulário com a senha nova e a confirmação de senha
            TypeSlowly(_driver.FindElement(By.Id("newPassword")), "NewPassword123!");
            TypeSlowly(_driver.FindElement(By.Id("confirmPassword")), "DifferentPassword456!");

            _driver.FindElement(By.TagName("body")).Click();

            // Verifica se a mensagem de erro é exibida
            IWebElement errorFeedback = _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("error-feedback")));
            Assert.Contains("As palavras-passes não coincidem.", errorFeedback.Text);
        }

        /// <summary>
        /// Função que simula a digitação do texto com um pequeno atraso entre os caracteres.
        /// </summary>
        /// <param name="element">O elemento no qual o texto será digitado.</param>
        /// <param name="text">O texto a ser digitado.</param>
        private void TypeSlowly(IWebElement element, string text)
        {
            element.Clear();
            foreach (char c in text)
            {
                element.SendKeys(c.ToString());
                Thread.Sleep(50); // Pequeno atraso para simular a digitação humana
            }
        }

        /// <summary>
        /// Libera os recursos utilizados pelo driver do Selenium após o término dos testes.
        /// </summary>
        public void Dispose()
        {
            _driver.Quit();
        }
    }
}
