namespace SoliGestAutomationTests;

using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

public class PaineisTest : IDisposable
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;

    public PaineisTest()
    {
        var options = new ChromeOptions();
        options.AddArgument("--ignore-certificate-errors");
        options.AddArgument("--allow-insecure-localhost");
        options.AddArgument("--disable-web-security");

        _driver = new ChromeDriver(options);
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Painel_Create_Should_Show_Alert_With_Correct_Message()
    {
        
        _driver.Navigate().GoToUrl("https://soligest.azurewebsites.net/paineis-solares");

        _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("create-btn")));
        _driver.FindElement(By.ClassName("create-btn")).Click();
        _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h2[text()='Criar Novo Painel']"))); //verifica a abertura do modal pelo titulo

        //preenchimento dos campos de criação do painel solar
        var nomeInput = _driver.FindElement(By.Id("create-name"));
        TypeSlowly(nomeInput, "Painel Solar Teste");
        var prioridadeSelect = new SelectElement(_driver.FindElement(By.Id("create-priority")));
        prioridadeSelect.SelectByText("Alta");
        var statusSelect = new SelectElement(_driver.FindElement(By.Id("create-status")));
        statusSelect.SelectByText("Verde");
        var descricaoInput = _driver.FindElement(By.Id("create-description"));
        TypeSlowly(descricaoInput, "Painel de teste para automação");
        var telefoneInput = _driver.FindElement(By.Id("create-phone"));
        TypeSlowly(telefoneInput, "123456789");
        var emailInput = _driver.FindElement(By.Id("create-email"));
        TypeSlowly(emailInput, "teste@exemplo.com");

        _driver.FindElement(By.ClassName("confirm-btn")).Click();

        _wait.Until(ExpectedConditions.AlertIsPresent());

        IAlert alert = _driver.SwitchTo().Alert();

        Assert.Equal("Novo painel solar criado com sucesso!", alert.Text);
        alert.Accept();
    }

    [Fact]
    public void Painel_Create_Should_Show_Alert_With_Warning_Message()
    {

        _driver.Navigate().GoToUrl("https://soligest.azurewebsites.net/paineis-solares");

        _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("create-btn")));
        _driver.FindElement(By.ClassName("create-btn")).Click();
        _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h2[text()='Criar Novo Painel']"))); //verifica a abertura do modal pelo titulo

        var prioridadeSelect = new SelectElement(_driver.FindElement(By.Id("create-priority")));
        prioridadeSelect.SelectByText("Alta");
        var statusSelect = new SelectElement(_driver.FindElement(By.Id("create-status")));
        statusSelect.SelectByText("Verde");
        var nomeInput = _driver.FindElement(By.Id("create-name"));
        TypeSlowly(nomeInput, "Painel Solar Teste");

        _driver.FindElement(By.ClassName("confirm-btn")).Click();

        try
        {
            _wait.Until(ExpectedConditions.AlertIsPresent());
            IAlert errorAlert = _driver.SwitchTo().Alert();

            Assert.Contains("Ocorreu um erro. Por favor tente novamente mais tarde.", errorAlert.Text);
            errorAlert.Accept();
        }
        catch (WebDriverTimeoutException)
        {
            Assert.Fail("Nenhum alerta de erro foi exibido ao não preencher os campos obrigatórios.");
        }

        var descricaoInput = _driver.FindElement(By.Id("create-description"));
        TypeSlowly(descricaoInput, "Painel de teste para automação");
        _driver.FindElement(By.ClassName("confirm-btn")).Click();

        try
        {
            _wait.Until(ExpectedConditions.AlertIsPresent());
            IAlert errorAlert = _driver.SwitchTo().Alert();

            Assert.Contains("Ocorreu um erro. Por favor tente novamente mais tarde.", errorAlert.Text);
            errorAlert.Accept();
        }
        catch (WebDriverTimeoutException)
        {
            Assert.Fail("Nenhum alerta de erro foi exibido ao não preencher os campos obrigatórios.");
        }

        var telefoneInput = _driver.FindElement(By.Id("create-phone"));
        TypeSlowly(telefoneInput, "123456789");
        _driver.FindElement(By.ClassName("confirm-btn")).Click();

        try
        {
            _wait.Until(ExpectedConditions.AlertIsPresent());
            IAlert errorAlert = _driver.SwitchTo().Alert();

            Assert.Contains("Ocorreu um erro. Por favor tente novamente mais tarde.", errorAlert.Text);
            errorAlert.Accept();
        }
        catch (WebDriverTimeoutException)
        {
            Assert.Fail("Nenhum alerta de erro foi exibido ao não preencher os campos obrigatórios.");
        }

        var emailInput = _driver.FindElement(By.Id("create-email"));
        TypeSlowly(emailInput, "teste@exemplo.com");

        _driver.FindElement(By.ClassName("confirm-btn")).Click();

        _wait.Until(ExpectedConditions.AlertIsPresent());

        IAlert alert = _driver.SwitchTo().Alert();

        Assert.Equal("Novo painel solar criado com sucesso!", alert.Text);
        alert.Accept();
    }

    [Fact]
    public void EditarPainelCamposObrigatorios_DeveAtualizarComSucesso()
    {
        _driver.Navigate().GoToUrl("https://soligest.azurewebsites.net/paineis-solares");

        _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("panels-container")));

        var editButtons = _driver.FindElements(By.ClassName("edit-btn"));
        Assert.NotEmpty(editButtons);
        editButtons[0].Click();

        _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h2[contains(text(),'Editar Painel ID')]")));

        var nomeInput = _driver.FindElement(By.Id("edit-name"));
        TypeSlowly(nomeInput, "Painel Editado");

        var descricaoInput = _driver.FindElement(By.Id("edit-description"));
        TypeSlowly(descricaoInput, "Descrição atualizada via teste automático");

        var phoneInput = _driver.FindElement(By.Id("edit-phone"));
        TypeSlowly(phoneInput, "987654321");

        var emailInput = _driver.FindElement(By.Id("edit-email"));
        TypeSlowly(emailInput, "painel_editado@exemplo.com");

        _driver.FindElement(By.ClassName("confirm-btn")).Click();

        _wait.Until(ExpectedConditions.AlertIsPresent());
        IAlert successAlert = _driver.SwitchTo().Alert();

        Assert.Equal("Painel solar atualizado com sucesso!", successAlert.Text);
        successAlert.Accept();
        
    }

    [Fact]
    public void ApagarPainelExistente_DeveRemoverComSucesso()
    {
        _driver.Navigate().GoToUrl("https://soligest.azurewebsites.net/paineis-solares");

        _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("panels-container")));

        var deleteButtons = _driver.FindElements(By.ClassName("delete-btn"));
        Assert.NotEmpty(deleteButtons);
        deleteButtons[0].Click();

        _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h2[text()='Confirmar Exclusão']")));

        _driver.FindElement(By.ClassName("confirm-btn")).Click();

        _wait.Until(ExpectedConditions.AlertIsPresent());
        IAlert successAlert = _driver.SwitchTo().Alert();

        Assert.Equal("Painel solar removido com sucesso!", successAlert.Text);
        successAlert.Accept();
       
    }

    // Function to type text character by character with small delay
    private void TypeSlowly(IWebElement element, string text)
    {
        element.Clear();
        foreach (char c in text)
        {
            element.SendKeys(c.ToString());
            Thread.Sleep(50); // Small delay to simulate human typing
        }
    }

    public void Dispose()
    {
        _driver.Quit();
    }
}
