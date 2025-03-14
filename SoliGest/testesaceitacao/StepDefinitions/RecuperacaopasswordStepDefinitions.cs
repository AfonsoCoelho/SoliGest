using System;
using System.Collections.Generic;
using Reqnroll;
//using Xunit;

namespace testesaceitacao.StepDefinitions
{
    [Binding]
    public class RecuperacaoPalavraPasseSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private string _email;
        private bool _resetLinkSent;
        private string _errorMessage;
        private readonly HashSet<string> _registeredUsers;

        public RecuperacaoPalavraPasseSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _registeredUsers = new HashSet<string> { "utilizador@example.com" }; // Simulação de utilizadores registados
        }

        [Given(@"que o utilizador está na página de recuperação de palavra-passe")]
        public void GivenQueOUtilizadorEstaNaPaginaDeRecuperacaoDePalavraPasse()
        {
            // Simular navegação para a página de recuperação de palavra-passe
        }

        [Given(@"possui uma conta registada com o email ""(.*)""")]
        public void GivenPossuiUmaContaRegistadaComOEmail(string email)
        {
            _registeredUsers.Add(email);
        }

        [When(@"ele insere o email ""(.*)""")]
        public void WhenEleInsereOEmail(string email)
        {
            _email = email;
            _resetLinkSent = SendPasswordResetLink(_email);
        }

        [When(@"ele insere um email que não está registado")]
        public void WhenEleInsereUmEmailQueNaoEstaRegistado()
        {
            _email = "naoexiste@example.com";
            _resetLinkSent = SendPasswordResetLink(_email);
        }

        [Then(@"ele deve receber um link de redefinição de palavra-passe")]
        public void ThenEleDeveReceberUmLinkDeRedefinicaoDePalavraPasse()
        {
            Assert.IsTrue(_resetLinkSent, "O link de redefinição de palavra-passe não foi enviado.");
        }

        [Then(@"ele deve ver uma mensagem de erro ""(.*)""")]
        public void ThenEleDeveVerUmaMensagemDeErro(string expectedErrorMessage)
        {
            Assert.AreEqual(expectedErrorMessage, _errorMessage);
        }

        private bool SendPasswordResetLink(string email)
        {
            if (!_registeredUsers.Contains(email))
            {
                _errorMessage = "Email não encontrado";
                return false;
            }

            return true; // Simula o envio do link de redefinição
        }
    }
}
