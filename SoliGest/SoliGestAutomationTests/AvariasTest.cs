namespace SoliGestAutomationTests;

using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

public class AvariasTest : IDisposable
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;

    public AvariasTest()
    {
        var options = new ChromeOptions();
        options.AddArgument("--ignore-certificate-errors");
        options.AddArgument("--allow-insecure-localhost");
        options.AddArgument("--disable-web-security");

        _driver = new ChromeDriver(options);
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void CriarAvaria_DeveExibirAlertaDeSucesso()
    {
        _driver.Navigate().GoToUrl("https://127.0.0.1:49893/avarias");

        _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("add-button")));
        _driver.FindElement(By.ClassName("add-button")).Click();

        _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h2[text()='Nova Avaria']")));

        var painelSelect = new SelectElement(_driver.FindElement(By.Id("panel")));
        painelSelect.SelectByIndex(0); // Seleciona o primeiro painel da lista

        var prioridadeSelect = new SelectElement(_driver.FindElement(By.Id("priority")));
        prioridadeSelect.SelectByText("Alta");

        var statusSelect = new SelectElement(_driver.FindElement(By.Id("status")));
        statusSelect.SelectByText("Vermelho");

        _driver.FindElement(By.ClassName("save-button")).Click();

        _wait.Until(ExpectedConditions.AlertIsPresent());
        var alert = _driver.SwitchTo().Alert();

        Assert.Equal("Novo pedido de assistência técnica criado com sucesso!", alert.Text);
        alert.Accept();
    }

    [Fact]
    public void CriarAvaria_SemSelecionarNada_DeveExibirErro()
    {
        _driver.Navigate().GoToUrl("https://127.0.0.1:49893/avarias");

        _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("add-button")));
        _driver.FindElement(By.ClassName("add-button")).Click();

        _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h2[text()='Nova Avaria']")));

        // Não seleciona nada
        _driver.FindElement(By.ClassName("save-button")).Click();

        _wait.Until(ExpectedConditions.AlertIsPresent());
        var alert = _driver.SwitchTo().Alert();

        Assert.Equal("Ocorreu um erro. Por favor tente novamente mais tarde.", alert.Text);
        alert.Accept();
    }


    [Fact]
    public void EditarAvaria_DeveAtualizarComSucesso()
    {
        _driver.Navigate().GoToUrl("https://127.0.0.1:49893/avarias");

        _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("panels-container")));

        var avarias = _driver.FindElements(By.ClassName("panel"));
        Assert.NotEmpty(avarias);
        avarias[0].Click();

        _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("edit-button")));
        _driver.FindElement(By.ClassName("edit-button")).Click();

        _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h2[contains(text(),'Editar Avaria ID')]")));

        var prioridadeSelect = new SelectElement(_driver.FindElement(By.Id("edit-priority")));
        prioridadeSelect.SelectByText("Média");

        var statusSelect = new SelectElement(_driver.FindElement(By.Id("edit-status")));
        statusSelect.SelectByText("Amarelo");

        _driver.FindElement(By.ClassName("save-button")).Click();

        _wait.Until(ExpectedConditions.AlertIsPresent());
        var alert = _driver.SwitchTo().Alert();

        Assert.Equal("Pedido de assistência técnica atualizado com sucesso!", alert.Text);
        alert.Accept();
    }

    [Fact]
    public void ApagarAvaria_DeveRemoverComSucesso()
    {
        _driver.Navigate().GoToUrl("https://127.0.0.1:49893/avarias");

        _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("panels-container")));

        var avarias = _driver.FindElements(By.ClassName("panel"));
        Assert.NotEmpty(avarias);
        avarias[0].Click();

        _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("delete-button")));
        _driver.FindElement(By.ClassName("delete-button")).Click();

        _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h2[text()='Confirmar Exclusão']")));

        _driver.FindElement(By.ClassName("save-button")).Click();

        _wait.Until(ExpectedConditions.AlertIsPresent());
        var alert = _driver.SwitchTo().Alert();

        Assert.Equal("Pedido de assistência técnica removido com sucesso!", alert.Text);
        alert.Accept();
    }

    public void Dispose()
    {
        _driver.Quit();
    }
}
