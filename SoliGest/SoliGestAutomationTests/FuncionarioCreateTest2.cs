namespace SoliGestAutomationTests
{
    using System;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using Xunit;
    using OpenQA.Selenium.Support.UI;
    using SeleniumExtras.WaitHelpers;

    /// <summary>
    /// Teste automatizado para verificar a criação de um funcionário com validação de formulário.
    /// </summary>
    public class FuncionarioCreateTest2 : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;
        private string _uniqueEmail;

        /// <summary>
        /// Inicializa o teste configurando o WebDriver para o navegador Chrome e criando um email único para cada execução do teste.
        /// </summary>
        public FuncionarioCreateTest2()
        {
            var options = new ChromeOptions();
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--allow-insecure-localhost");
            options.AddArgument("--disable-web-security");

            _driver = new ChromeDriver(options);
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
            _uniqueEmail = $"testuser{DateTime.Now.Ticks}@gmail.com";
        }

        /// <summary>
        /// Testa a criação de um novo funcionário e verifica se o alerta de erro é exibido quando o formulário contém dados inválidos.
        /// </summary>
        [Fact]
        public void Funcionario_Create_Should_Show_Alert_With_Correct_Message()
        {
            _driver.Navigate().GoToUrl("https://soligest.azurewebsites.net/funcionario");

            // Espera até que o botão de adicionar funcionário esteja visível
            _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("add-button")));

            // Clica no botão de adicionar funcionário
            _driver.FindElement(By.ClassName("add-button")).Click();

            // Preenche o formulário lentamente para evitar condições de corrida na UI
            TypeSlowly(_driver.FindElement(By.Id("name")), "");
            TypeSlowly(_driver.FindElement(By.Id("email")), _uniqueEmail);
            TypeSlowly(_driver.FindElement(By.Id("phoneNumber")), "999999999");
            TypeSlowly(_driver.FindElement(By.Id("address1")), "address1");
            TypeSlowly(_driver.FindElement(By.Id("address2")), "address2");
            TypeSlowly(_driver.FindElement(By.Id("birthDate")), DateTime.Now.ToString("dd-MM-yyyy"));
            TypeSlowly(_driver.FindElement(By.Id("password")), "p");
            TypeSlowly(_driver.FindElement(By.Id("confirmPassword")), "1");

            // Seleciona o cargo
            WebElement roleDropdown = (WebElement)_driver.FindElement(By.Id("cargo"));
            SelectElement roleSelectObject = new SelectElement(roleDropdown);
            roleSelectObject.SelectByIndex(1); // Administrativo

            // Seleciona o dia de folga
            WebElement dayOffDropdown = (WebElement)_driver.FindElement(By.Id("dayOff"));
            SelectElement dayOffSelectObject = new SelectElement(dayOffDropdown);
            dayOffSelectObject.SelectByIndex(1); // Terça-feira

            // Preenche as datas de férias
            TypeSlowly(_driver.FindElement(By.Id("startHoliday")), DateTime.Now.ToString("dd-MM-yyyy"));
            TypeSlowly(_driver.FindElement(By.Id("endHoliday")), DateTime.Now.ToString("dd-MM-yyyy"));

            // Envia o formulário
            _driver.FindElement(By.ClassName("submit-btn")).Click();

            // Espera até que o alerta de validação de erro apareça
            _wait.Until(ExpectedConditions.AlertIsPresent());

            // Troca para o alerta
            IAlert alert = _driver.SwitchTo().Alert();

            // Valida a mensagem do alerta e aceita
            Assert.Equal("Por favor corriga os erros do formulário!", alert.Text);
            alert.Accept();
        }

        /// <summary>
        /// Digita texto em um campo de entrada de forma lenta, caractere por caractere com um pequeno atraso.
        /// </summary>
        /// <param name="element">O elemento de entrada onde o texto será digitado.</param>
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
        /// Finaliza o WebDriver após a execução dos testes.
        /// </summary>
        public void Dispose()
        {
            _driver.Quit();
        }
    }
}
