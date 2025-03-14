Feature: Recuperação de Palavra-passe
  Como um utilizador que se esqueceu da palavra-passe
  Quero recuperar o meu acesso ao sistema
  Para redefinir a minha palavra-passe e aceder à minha conta

  @recuperacao-palavra-passe
  Scenario: Recuperação de palavra-passe bem-sucedida
    Given que o utilizador está na página de recuperação de palavra-passe
    And possui uma conta registada com o email "utilizador@example.com"
    When ele insere o email "utilizador@example.com"
    Then ele deve receber um link de redefinição de palavra-passe

  Scenario: Recuperação de palavra-passe com email não registado
    Given que o utilizador está na página de recuperação de palavra-passe
    When ele insere um email que não está registado
    Then ele deve ver uma mensagem de erro "Email não encontrado"