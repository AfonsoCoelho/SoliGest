Feature: Login do Utilizador
  Como um utilizador registado
  Quero fazer login no sistema
  Para aceder à minha conta

  @login
  Scenario: Login bem-sucedido
    Given que o utilizador está na página de login
    And possui uma conta registada com o email1 "utilizador@example.com" e palavra-passe "PalavraPasse123"
    When ele insere o email1 "utilizador@example.com" e a palavra-passe "PalavraPasse123"
    Then ele deve ser autenticado com sucesso

  Scenario: Login com palavra-passe incorreta
    Given que o utilizador está na página de login
    And possui uma conta registada com o email "utilizador@example.com" e palavra-passe "PalavraPasse123"
    When ele insere o email "utilizador@example.com" e a palavra-passe "PalavraErrada"
    Then ele deve ver uma mensagem de erro de Credenciais inválidas

  Scenario: Login com email não registado
    Given que o utilizador está na página de login
    When ele insere um email que não está registado1
    Then ele deve ver uma mensagem de erro de Email não encontrado

