namespace SoliGestAutomationTests
{
    using System;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using Xunit;
    using OpenQA.Selenium.Support.UI;
    using SeleniumExtras.WaitHelpers;

    /// <summary>
    /// Teste automatizado para verificar a exibição e fechamento do modal de detalhes de um funcionário.
    /// </summary>
    public class FuncionarioDetailsTest1 : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        /// <summary>
        /// Inicializa o teste configurando o WebDriver para o navegador Chrome e aguardando o tempo adequado para ações no UI.
        /// </summary>
        public FuncionarioDetailsTest1()
        {
            var options = new ChromeOptions();
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--allow-insecure-localhost");
            options.AddArgument("--disable-web-security");

            _driver = new ChromeDriver(options);
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// Testa a exibição e o fechamento do modal de detalhes de um funcionário sem erros.
        /// </summary>
        [Fact]
        public void Funcionario_Details_Should_Show_And_Close_Model_Without_Errors()
        {
            _driver.Navigate().GoToUrl("https://soligest.azurewebsites.net/funcionario");

            // Espera até que o botão de detalhes esteja visível
            _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("details-btn")));

            // Clica no botão de detalhes do primeiro funcionário
            _driver.FindElement(By.Id("details/soligestesa@gmail.com")).Click();

            // Espera até que o botão de fechar do modal esteja visível
            _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("close")));

            // Clica no botão de fechar para fechar o modal
            _driver.FindElement(By.ClassName("close")).Click();
        }

        /// <summary>
        /// Função para digitar texto lentamente, caractere por caractere, com um pequeno atraso entre eles.
        /// </summary>
        /// <param name="element">Elemento no qual o texto será digitado.</param>
        /// <param name="text">Texto a ser digitado.</param>
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
        /// Finaliza o WebDriver após a execução dos testes.
        /// </summary>
        public void Dispose()
        {
            _driver.Quit();
        }
    }
}
