using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Threading;

namespace SoliGestAutomationTests
{
    /// <summary>
    /// Testes automatizados para verificar a criação de um funcionário.
    /// </summary>
    public class FuncionarioCreateTest1 : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;
        private string _uniqueEmail;

        /// <summary>
        /// Inicializa o teste configurando o WebDriver para o navegador Chrome e criando um email único para cada execução do teste.
        /// </summary>
        public FuncionarioCreateTest1()
        {
            var options = new ChromeOptions();
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--allow-insecure-localhost");
            options.AddArgument("--disable-web-security");

            _driver = new ChromeDriver(options);
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            _uniqueEmail = $"testuser{DateTime.Now.Ticks}@gmail.com";
        }

        /// <summary>
        /// Testa a criação de um novo funcionário e verifica se o alerta de sucesso é exibido com a mensagem correta.
        /// </summary>
        [Fact]
        public void Funcionario_Create_Should_Show_Alert_With_Correct_Message()
        {
            _driver.Navigate().GoToUrl("https://soligest.azurewebsites.net/funcionario-create");

            // Espera até que o formulário esteja carregado, usando o campo 'name' como indicador
            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("name")));

            // Preenche os campos do formulário
            TypeSlowly(_driver.FindElement(By.Id("name")), "afonso");
            TypeSlowly(_driver.FindElement(By.Id("email")), _uniqueEmail);
            TypeSlowly(_driver.FindElement(By.Id("phoneNumber")), "999999999");
            TypeSlowly(_driver.FindElement(By.Id("address1")), "address1");
            TypeSlowly(_driver.FindElement(By.Id("address2")), "address2");

            // Preenche a data de nascimento (formato yyyy-MM-dd para input type="date")
            _driver.FindElement(By.Id("birthDate")).SendKeys(DateTime.Now.ToString("yyyy-MM-dd"));
            TypeSlowly(_driver.FindElement(By.Id("password")), "Password1!");
            TypeSlowly(_driver.FindElement(By.Id("confirmPassword")), "Password1!");

            // Seleciona o cargo
            SelectElement roleSelect = new SelectElement(_driver.FindElement(By.Id("cargo")));
            roleSelect.SelectByIndex(1); // Administrativo

            // Seleciona o dia de folga
            SelectElement dayOffSelect = new SelectElement(_driver.FindElement(By.Id("dayOff")));
            dayOffSelect.SelectByIndex(1); // Terça-feira

            // Preenche os campos de férias
            _driver.FindElement(By.Id("startHoliday")).SendKeys("2025-10-10");
            _driver.FindElement(By.Id("endHoliday")).SendKeys("2025-10-10");

            // Envia o formulário
            _driver.FindElement(By.ClassName("submit-btn")).Click();

            // Espera até que o alerta de sucesso apareça
            _wait.Until(ExpectedConditions.AlertIsPresent());

            // Trata o alerta
            IAlert alert = _driver.SwitchTo().Alert();
            Assert.Equal("Registo bem sucedido!", alert.Text);
            alert.Accept();
        }

        /// <summary>
        /// Digita um texto lentamente em um campo de entrada, caractere por caractere.
        /// </summary>
        /// <param name="element">O elemento de entrada onde o texto será digitado.</param>
        /// <param name="text">O texto a ser digitado.</param>
        private void TypeSlowly(IWebElement element, string text)
        {
            element.Clear();
            foreach (char c in text)
            {
                element.SendKeys(c.ToString());
                Thread.Sleep(50);
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
