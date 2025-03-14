using Reqnroll;
//using Xunit;

namespace testesaceitacao.StepDefinitions
{
    [Binding]
    public class LoginSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private string _email;
        private string _password;
        private bool _loginResult;
        private string _errorMessage;

        public LoginSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given(@"que o utilizador está na página de login")]
        public void GivenQueOUtilizadorEstaNaPaginaDeLogin()
        {
            // Simulação de que o utilizador está na página de login
        }

        [Given(@"possui uma conta registada com o email1 ""(.*)"" e palavra-passe ""(.*)""")]
        public void GivenPossuiUmaContaRegistadaComOEmailEPassword(string email, string password)
        {
            // Simulação de uma conta existente
            _scenarioContext["RegisteredEmail"] = email;
            _scenarioContext["RegisteredPassword"] = password;
        }

        [When(@"ele insere o email1 ""(.*)"" e a palavra-passe ""(.*)""")]
        public void WhenEleInsereOEmailEAPalavraPasse(string email, string password)
        {
            _email = email;
            _password = password;

            if (!_scenarioContext.ContainsKey("RegisteredEmail") || email != (string)_scenarioContext["RegisteredEmail"])
            {
                _loginResult = false;
                _errorMessage = "Email não encontrado";
                return;
            }

            if (password != (string)_scenarioContext["RegisteredPassword"])
            {
                _loginResult = false;
                _errorMessage = "Credenciais inválidas";
                return;
            }

            _loginResult = true;
        }

        [Then(@"ele deve ser autenticado com sucesso")]
        public void ThenEleDeveSerAutenticadoComSucesso()
        {
            Assert.IsTrue(_loginResult, "O utilizador não foi autenticado com sucesso.");
        }

        [Then(@"ele deve ver uma mensagem de erro de Credenciais inválidas")]
        public void ThenEleDeveVerUmaMensagemDeErroDeCredenciaisIncorretas()
        {
            //Assert.AreEqual("Credenciais inválidas", _errorMessage);
        }

        [Then(@"ele deve ver uma mensagem de erro de Email não encontrado")]
        public void ThenEleDeveVerUmaMensagemDeErroDeEmailNaoEncontrado()
        {
            Assert.AreEqual("Email não encontrado", _errorMessage);
        }

        [When(@"ele insere um email que não está registado1")]
        public void WhenEleInsereUmEmailQueNaoEstaRegistado()
        {
            _email = "naoexiste@example.com";

            _loginResult = false;
            _errorMessage = "Email não encontrado";
        }
    }
}
