namespace SoliGestAutomationTests
{
    using System;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using Xunit;
    using OpenQA.Selenium.Support.UI;
    using SeleniumExtras.WaitHelpers;

    /// <summary>
    /// Teste automatizado para verificar a edição de um funcionário no sistema.
    /// </summary>
    public class FuncionarioEditTest1 : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        /// <summary>
        /// Inicializa o teste configurando o WebDriver para o navegador Chrome e aguardando o tempo adequado para ações no UI.
        /// </summary>
        public FuncionarioEditTest1()
        {
            var options = new ChromeOptions();
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--allow-insecure-localhost");
            options.AddArgument("--disable-web-security");

            _driver = new ChromeDriver(options);
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// Testa a edição de um funcionário e exibe um alerta com a mensagem correta.
        /// </summary>
        [Fact]
        public void Funcionario_Edit_Should_Show_Alert_With_Correct_Message()
        {
            _driver.Navigate().GoToUrl("https://soligest.azurewebsites.net/funcionario");

            // Espera até que o botão de edição do funcionário esteja visível
            _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("edit-btn")));

            // Clica no botão de editar do funcionário com o e-mail especificado
            _driver.FindElement(By.Id("edit/soligestesa@gmail.com")).Click();

            // Espera até que o campo de nome do funcionário seja visível
            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("name")));

            // Preenche o campo "nome" com um novo valor
            _driver.FindElement(By.Id("name")).Clear();
            TypeSlowly(_driver.FindElement(By.Id("name")), "SoliGest Supervisor (New Name)");

            // Clica no botão de enviar para salvar as alterações
            _driver.FindElement(By.ClassName("submit-btn")).Click();

            // Espera até que o alerta de sucesso apareça
            _wait.Until(ExpectedConditions.AlertIsPresent());

            // Switch para o alerta e valida a mensagem
            IAlert alert = _driver.SwitchTo().Alert();
            Assert.Equal("Utilizador atualizado com sucesso!", alert.Text);
            alert.Accept();
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
