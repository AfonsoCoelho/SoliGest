namespace SoliGestAutomationTests
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using Xunit;
    using OpenQA.Selenium.Support.UI;
    using SeleniumExtras.WaitHelpers;

    /// <summary>
    /// Teste de automação para o processo de alocação de técnicos em avarias no sistema.
    /// Testa tanto a alocação automática quanto a manual de técnicos.
    /// </summary>
    public class AvariasAlocTest : IDisposable
    {
        private readonly IWebDriver _driver;
        private const string BaseUrl = "https://soligest.azurewebsites.net";
        private const string AssistanceUrl = BaseUrl + "/avarias";
        private const string TechnicianEmail = "technician@mail.com";
        private const string TechnicianPassword = "Tech1!";
        private const int DefaultTimeoutSeconds = 5;

        /// <summary>
        /// Construtor que inicializa o driver do navegador e a configuração de login.
        /// </summary>
        public AvariasAlocTest()
        {
            var options = new ChromeOptions();
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--allow-insecure-localhost");
            options.AddArgument("--disable-web-security");
            _driver = new ChromeDriver(options);
        }

        /// <summary>
        /// Testa a alocação automática de técnico em uma avaria e verifica se o nome do técnico aparece no InfoWindow.
        /// </summary>
        [Fact]
        public void AutoAllocate_Should_Assign_Technician_And_InfoWindow_Shows_Name()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(DefaultTimeoutSeconds));

            // --- Login ---
            _driver.Navigate().GoToUrl(BaseUrl + "/login");
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("email")));
            TypeSlowly(_driver.FindElement(By.Id("email")), TechnicianEmail);
            TypeSlowly(_driver.FindElement(By.Id("password")), TechnicianPassword);
            _driver.FindElement(By.ClassName("submit-btn")).Click();
            wait.Until(ExpectedConditions.AlertIsPresent());
            _driver.SwitchTo().Alert().Accept();

            // Navega até a página de 'Avarias'
            _driver.Navigate().GoToUrl(AssistanceUrl);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".panel")));

            // Seleciona o primeiro painel de avaria
            var panel = _driver.FindElement(By.CssSelector(".panel"));
            var headerText = panel.FindElement(By.CssSelector(".panel-header h3")).Text;
            var avariaId = headerText.Split(':')[1].Trim();

            // Clica em 'Alocar Automaticamente'
            panel.FindElement(By.CssSelector("button.auto-aloc")).Click();

            // Espera pelo modal de alocação automática
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".modal .modal-content")));

            // Extrai o nome do técnico candidato
            var nameParagraph = _driver.FindElements(By.CssSelector(".modal .modal-content p"))
                .FirstOrDefault(p => p.Text.Contains("Nome:"));
            if (nameParagraph == null)
                throw new Exception("Não foi possível encontrar o parágrafo 'Nome:' no modal de alocação automática.");
            var match = Regex.Match(nameParagraph.Text, @"Nome:\s*(.+)");
            var autoName = match.Success ? match.Groups[1].Value.Trim() : throw new Exception("Falha ao extrair o nome do candidato da alocação automática.");

            // Confirma a alocação automática
            _driver.FindElement(By.CssSelector(".modal .confirm-btn")).Click();
            wait.Until(ExpectedConditions.AlertIsPresent());
            var alert = _driver.SwitchTo().Alert();
            Assert.Contains(autoName, alert.Text);
            alert.Accept();

            // Atualiza os painéis
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".panel")));

            // Clica em 'Consultar' no painel alocado
            var panels = _driver.FindElements(By.CssSelector(".panel"));
            var target = panels.First(p => p.FindElement(By.CssSelector(".panel-header h3")).Text.Contains($"Avaria ID: {avariaId}"));
            target.FindElement(By.CssSelector("button.view-btn")).Click();

            // Verifica se o InfoWindow exibe o nome do técnico
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".gm-style-iw")));
            var info = _driver.FindElement(By.CssSelector(".gm-style-iw")).Text;
            Assert.Contains(autoName, info);
        }

        /// <summary>
        /// Testa a alocação manual de técnico em uma avaria com ID 2 e verifica se o nome do técnico aparece no InfoWindow.
        /// </summary>
        [Fact]
        public void ManualAllocate_Should_Assign_Technician_And_InfoWindow_Shows_Name_For_Id2()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(DefaultTimeoutSeconds));

            // Login
            _driver.Navigate().GoToUrl(BaseUrl + "/login");
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("email")));
            TypeSlowly(_driver.FindElement(By.Id("email")), TechnicianEmail);
            TypeSlowly(_driver.FindElement(By.Id("password")), TechnicianPassword);
            _driver.FindElement(By.ClassName("submit-btn")).Click();
            wait.Until(ExpectedConditions.AlertIsPresent()).Accept();

            // Navega até a página de 'Avarias'
            _driver.Navigate().GoToUrl(AssistanceUrl);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".panel")));

            // Aloca manualmente a avaria com ID 2
            var panel2 = _driver.FindElements(By.CssSelector(".panel")).First(p => p.FindElement(By.CssSelector(".panel-header h3")).Text.Contains("Avaria ID: 2"));
            panel2.FindElement(By.CssSelector("button.manual-aloc")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".modal .table-container tbody tr")));

            // Escolhe o segundo técnico da lista
            var rows = _driver.FindElements(By.CssSelector(".modal .table-container tbody tr"));
            if (rows.Count < 2) throw new Exception("Menos de dois técnicos disponíveis.");
            var secondRow = rows[1];
            var manualName = secondRow.FindElement(By.CssSelector("td:nth-child(2)")).Text.Trim();
            secondRow.FindElement(By.CssSelector("button.allocate-btn")).Click();

            // Verifica o alerta de alocação
            wait.Until(ExpectedConditions.AlertIsPresent());
            var manualAlert = _driver.SwitchTo().Alert();
            Assert.Contains(manualName, manualAlert.Text);
            manualAlert.Accept();

            // Visualiza o InfoWindow para o painel 2
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".panel")));
            var target2 = _driver.FindElements(By.CssSelector(".panel")).First(p => p.FindElement(By.CssSelector(".panel-header h3")).Text.Contains("Avaria ID: 2"));
            target2.FindElement(By.CssSelector("button.view-btn")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".gm-style-iw")));
            var manualInfo = _driver.FindElement(By.CssSelector(".gm-style-iw")).Text;
            Assert.Contains(manualName, manualInfo);
        }

        /// <summary>
        /// Função que simula a digitação lenta de texto no campo de entrada.
        /// </summary>
        /// <param name="element">Elemento de entrada.</param>
        /// <param name="text">Texto a ser digitado.</param>
        private void TypeSlowly(IWebElement element, string text)
        {
            element.Clear();
            foreach (char c in text)
            {
                element.SendKeys(c.ToString());
                System.Threading.Thread.Sleep(50); // Atraso pequeno para simular digitação humana
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
