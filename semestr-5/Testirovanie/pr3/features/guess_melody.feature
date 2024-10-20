Feature: Угадывание мелодий

  Scenario: Правильное угадывание мелодии
    Given Игра начата с мелодиями "Melody1", "Melody2", "Melody3"
    When Игрок "Игрок 1" угадывает мелодию "Melody1"
    Then У "Игрок 1" должно быть 1 очко

  Scenario: Неправильное угадывание мелодии
    Given Игра начата с мелодиями "Melody1", "Melody2", "Melody3"
    When Игрок "Игрок 2" угадывает мелодию "WrongMelody"
    Then У "Игрок 2" должно быть 0 очков

  Scenario: Очки после нескольких угадываний
    Given Игра начата с мелодиями "Melody1", "Melody2", "Melody3"
    When Игрок "Игрок 1" угадывает мелодию "Melody1"
    And Игрок "Игрок 2" угадывает мелодию "Melody2"
    And Игрок "Игрок 3" угадывает мелодию "WrongMelody"
    Then У "Игрок 1" должно быть 1 очко
    And У "Игрок 2" должно быть 1 очко
    And У "Игрок 3" должно быть 0 очков