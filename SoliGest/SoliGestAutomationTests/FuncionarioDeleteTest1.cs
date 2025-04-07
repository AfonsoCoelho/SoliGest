﻿namespace SoliGestAutomationTests;

using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

public class FuncionarioDeleteTest1 : IDisposable
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;

    public FuncionarioDeleteTest1()
    {
        var options = new ChromeOptions();
        options.AddArgument("--ignore-certificate-errors");
        options.AddArgument("--allow-insecure-localhost");
        options.AddArgument("--disable-web-security");

        _driver = new ChromeDriver(options);
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10)); // Aumentando o tempo de espera
    }

    [Fact]
    public void Funcionario_DeveRemoverComSucesso()
    {
        _driver.Navigate().GoToUrl("https://soligest.azurewebsites.net/funcionario");

        // Espera até que a tabela de funcionários esteja visível
        _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("table-container")));

        // Seleciona todas as linhas da tabela que representam os usuários
        var funcionarios = _driver.FindElements(By.XPath("//tbody/tr"));

        // Diagnóstico: imprimir o número de funcionários encontrados
        Console.WriteLine($"Número de funcionários encontrados: {funcionarios.Count}");

        // Verifica se há funcionários listados
        Assert.NotEmpty(funcionarios);

        // Seleciona o primeiro funcionário
        var primeiroFuncionario = funcionarios[0]; // Seleciona o primeiro funcionário

        // Clica no botão de apagar do primeiro funcionário
        var botaoDeletar = primeiroFuncionario.FindElement(By.ClassName("delete-btn"));
        botaoDeletar.Click();

        // Espera o modal de confirmação aparecer
        _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("modal-content")));

        // Confirma a exclusão
        _driver.FindElement(By.ClassName("confirm-delete-btn")).Click();

        // Espera um breve momento para que a exclusão seja processada
        System.Threading.Thread.Sleep(1000); // Isso pode ser substituído por um melhor uso de espera, dependendo do feedback da sua aplicação

        // Verifica se o funcionário foi removido da tabela
        var funcionariosDepois = _driver.FindElements(By.XPath("//tbody/tr"));
        Assert.Equal(funcionarios.Count - 1, funcionariosDepois.Count);
    }

    public void Dispose()
    {
        _driver.Quit();
    }
}
