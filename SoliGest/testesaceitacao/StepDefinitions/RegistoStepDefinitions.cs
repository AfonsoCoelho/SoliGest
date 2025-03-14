using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reqnroll;

namespace testesaceitacao.StepDefinitions
{
    
    

    namespace YourTestProject
    {
        [Binding]
        public class UserRegistrationSteps
        {
            private readonly ScenarioContext _scenarioContext;
            private string _email;
            private string _password;
            private string _confirmPassword;
            private bool _registrationResult;
            private string _errorMessage;

            public UserRegistrationSteps(ScenarioContext scenarioContext)
            {
                _scenarioContext = scenarioContext;
            }

            [Given(@"que o utilizador está na página de registo")]
            public void GivenQueOUtilizadorEstaNaPaginaDeRegisto()
            {
                // Implementação para navegar até à página de registo
                // Exemplo: _driver.Navigate().GoToUrl("https://yourapp.com/register");
            }

            [When(@"ele insere um email válido e uma palavra-passe válida")]
            public void WhenEleInsereUmEmailValidoEUmaPalavraPasseValida()
            {
                _email = "novo_utilizador@example.com";
                _password = "SenhaSegura123!";
            }

            [When(@"confirma a palavra-passe corretamente")]
            public void WhenConfirmaAPalavraPasseCorretamente()
            {
                _confirmPassword = _password;
                _registrationResult = RegisterUser(_email, _password, _confirmPassword);
            }

            [When(@"confirma uma palavra-passe diferente")]
            public void WhenConfirmaUmaPalavraPasseDiferente()
            {
                _confirmPassword = "OutraSenha123!";
                _registrationResult = RegisterUser(_email, _password, _confirmPassword);
            }

            [Given(@"já existe um utilizador com o email ""(.*)""")]
            public void GivenJaExisteUmUtilizadorComOEmail(string email)
            {
                // Simula a existência de um utilizador com o email fornecido
                _email = email;
                _scenarioContext["ExistingEmail"] = email;
            }

            [When(@"ele tenta registar-se com o mesmo email")]
            public void WhenEleTentaRegistarSeComOMesmoEmail()
            {
                _password = "SenhaSegura123!";
                _confirmPassword = _password;
                _registrationResult = RegisterUser(_email, _password, _confirmPassword);
            }

            [Then(@"a sua conta deve ser criada com sucesso")]
            public void ThenASuaContaDeveSerCriadaComSucesso()
            {
                Assert.IsTrue(_registrationResult, "A conta não foi criada com sucesso.");
            }

            [Then(@"ele deve ver uma mensagem de erro de palavra-passe incorreta")]
            public void ThenEleDeveVerUmaMensagemDeErroDePalavraPasseIncorreta()
            {
                Assert.AreEqual("As palavras-passe não coincidem", _errorMessage);
            }

            [Then(@"ele deve ver uma mensagem de erro de email já registado")]
            public void ThenEleDeveVerUmaMensagemDeErroDeEmailJaRegistado()
            {
                Assert.AreEqual("Este email já está registado", _errorMessage);
            }


            private bool RegisterUser(string email, string password, string confirmPassword)
            {
                // Simula o processo de registo
                if (password != confirmPassword)
                {
                    _errorMessage = "As palavras-passe não coincidem";
                    return false;
                }

                if (_scenarioContext.ContainsKey("ExistingEmail") && email == (string)_scenarioContext["ExistingEmail"])
                {
                    _errorMessage = "Este email já está registado";
                    return false;
                }

                // Simula o sucesso do registo
                return true;
            }
        }
    }
}
