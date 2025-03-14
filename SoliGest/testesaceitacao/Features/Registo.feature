
Feature: Registo de Utilizador
  Como um novo utilizador
  Quero registar-me no sistema
  Para aceder à minha conta com segurança

  @registo
  Scenario: Registo bem-sucedido
    Given que o utilizador está na página de registo
    When ele insere um email válido e uma palavra-passe válida
    And confirma a palavra-passe corretamente
    Then a sua conta deve ser criada com sucesso

  Scenario: Registo com palavras-passe diferentes
    Given que o utilizador está na página de registo
    When ele insere um email válido e uma palavra-passe válida
    And confirma uma palavra-passe diferente
    Then ele deve ver uma mensagem de erro de palavra-passe incorreta

  Scenario: Registo com email já registado
    Given que o utilizador está na página de registo
    And já existe um utilizador com o email "utilizador@example.com"
    When ele tenta registar-se com o mesmo email
    Then ele deve ver uma mensagem de erro de email já registado



