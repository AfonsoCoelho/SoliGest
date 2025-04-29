namespace SoliGestAutomationTests
{
    using System;
    using System.Threading;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using Xunit;
    using OpenQA.Selenium.Support.UI;
    using SeleniumExtras.WaitHelpers;

    /// <summary>
    /// Classe de testes automatizados para as funcionalidades de criação, edição e exclusão de avarias.
    /// </summary>
    public class AvariasTest : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        /// <summary>
        /// Inicializa uma nova instância do teste, configurando o WebDriver e o tempo de espera.
        /// </summary>
        public AvariasTest()
        {
            var options = new ChromeOptions();
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--allow-insecure-localhost");
            options.AddArgument("--disable-web-security");

            _driver = new ChromeDriver(options);
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// Testa a criação de uma avaria, verificando se o alerta de sucesso é exibido após a criação.
        /// </summary>
        [Fact]
        public void CriarAvaria_DeveExibirAlertaDeSucesso()
        {
            _driver.Navigate().GoToUrl("https://soligest.azurewebsites.net/avarias");

            _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("create-btn")));
            _driver.FindElement(By.ClassName("create-btn")).Click();

            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("create-panel")));
            var painelSelect = new SelectElement(_driver.FindElement(By.Id("create-panel")));
            painelSelect.SelectByIndex(0);

            var prioridadeSelect = new SelectElement(_driver.FindElement(By.Id("create-priority")));
            prioridadeSelect.SelectByText("Alta");

            var statusSelect = new SelectElement(_driver.FindElement(By.Id("create-status")));
            statusSelect.SelectByText("Vermelho");

            _driver.FindElement(By.ClassName("confirm-btn")).Click();

            _wait.Until(ExpectedConditions.AlertIsPresent());
            var alert = _driver.SwitchTo().Alert();

            Assert.Equal("Novo pedido de assistência técnica criado com sucesso!", alert.Text);
            alert.Accept();
        }

        /// <summary>
        /// Testa a criação de uma avaria sem seleção de painel, verificando se o erro é exibido.
        /// </summary>
        [Fact]
        public void CriarAvaria_SemSelecionarNada_DeveExibirErro()
        {
            _driver.Navigate().GoToUrl("https://soligest.azurewebsites.net/avarias");

            _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("create-btn")));
            _driver.FindElement(By.ClassName("create-btn")).Click();

            _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h2[contains(text(),'Criar Nova Avaria')]")));

            // Não seleciona nada e tenta criar
            _driver.FindElement(By.ClassName("confirm-btn")).Click();

            _wait.Until(ExpectedConditions.AlertIsPresent());
            var alert = _driver.SwitchTo().Alert();

            Assert.Equal("Por favor selecione um painel!", alert.Text);
            alert.Accept();
        }

        /// <summary>
        /// Testa a edição de uma avaria, verificando se a prioridade e o status são atualizados corretamente.
        /// </summary>
        [Fact]
        public void EditarAvaria_DeveAtualizarComSucesso()
        {
            _driver.Navigate().GoToUrl("https://soligest.azurewebsites.net/avarias");

            // Espera até que a seção de painéis esteja visível
            _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("panels-container")));

            // Obtém todas as avarias na página
            var avarias = _driver.FindElements(By.ClassName("panel"));
            Assert.NotEmpty(avarias); // Verifica se há avarias

            // Clica na primeira avaria
            avarias[0].Click();

            // Espera até que o botão de editar esteja visível
            _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("edit-btn")));

            // Clica no botão de editar
            _driver.FindElement(By.ClassName("edit-btn")).Click();

            // Espera até que o modal de edição esteja visível
            _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h2[contains(text(),'Editar Avaria ID')]")));

            // Seleciona nova prioridade
            var prioridadeSelect = new SelectElement(_driver.FindElement(By.Id("edit-priority")));
            prioridadeSelect.SelectByText("Média");

            // Seleciona novo status
            var statusSelect = new SelectElement(_driver.FindElement(By.Id("edit-status")));
            statusSelect.SelectByText("Amarelo");

            // Clica no botão de confirmar
            _driver.FindElement(By.ClassName("confirm-btn")).Click();

            // Espera até que o alerta de sucesso apareça
            _wait.Until(ExpectedConditions.AlertIsPresent());
            var alert = _driver.SwitchTo().Alert();

            // Verifica a mensagem de sucesso
            Assert.Equal("Pedido de assistência técnica atualizado com sucesso!", alert.Text);
            alert.Accept();
        }

        /// <summary>
        /// Testa a remoção de uma avaria, verificando se o alerta de sucesso é exibido após a exclusão.
        /// </summary>
        [Fact]
        public void ApagarAvaria_DeveRemoverComSucesso()
        {
            _driver.Navigate().GoToUrl("https://soligest.azurewebsites.net/avarias");

            _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("panels-container")));

            var avarias = _driver.FindElements(By.ClassName("panel"));
            Assert.NotEmpty(avarias);

            // Encontrar o botão de apagar da primeira avaria e clicar nele
            var primeiroPainel = avarias[0];
            var botaoApagar = primeiroPainel.FindElement(By.ClassName("delete-btn"));
            botaoApagar.Click();

            // Confirmar exclusão
            _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("confirm-btn")));
            _driver.FindElement(By.ClassName("confirm-btn")).Click();

            _wait.Until(ExpectedConditions.AlertIsPresent());
            var alert = _driver.SwitchTo().Alert();

            Assert.Equal("Pedido de assistência técnica removido com sucesso!", alert.Text);
            alert.Accept();
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
