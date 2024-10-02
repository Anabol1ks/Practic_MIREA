package game

import (
	"testing"
)

// Тестирование угадывания мелодии
func TestGuessCorrectMelody(t *testing.T) {
	melodies := []string{"Melody1", "Melody2"}
	g := &Game{}
	g.Start([]string{"Игрок 1"}, melodies)

	if !g.Guess("Игрок 1", "Melody1") {
		t.Error("Expected guess to be correct")
	}

	if g.GetScore("Игрок 1") != 1 {
		t.Errorf("Expected score to be 1, got %d", g.GetScore("Игрок 1"))
	}
}

func TestGuessWrongMelody(t *testing.T) {
	melodies := []string{"Melody1", "Melody2"}
	g := &Game{}
	g.Start([]string{"Игрок 1"}, melodies)

	if g.Guess("Игрок 1", "WrongMelody") {
		t.Error("Expected guess to be incorrect")
	}

	if g.GetScore("Игрок 1") != 0 {
		t.Errorf("Expected score to be 0, got %d", g.GetScore("Игрок 1"))
	}
}

// Тестирование перехода к следующей мелодии и игроку
func TestNextMelodyAndPlayer(t *testing.T) {
	melodies := []string{"Melody1", "Melody2", "Melody3"}
	g := &Game{}
	g.Start([]string{"Игрок 1", "Игрок 2"}, melodies)

	// Первый игрок угадывает правильно
	g.Guess("Игрок 1", "Melody1")

	// Проверка счёта первого игрока
	if g.GetScore("Игрок 1") != 1 {
		t.Errorf("Expected score to be 1 for Игрок 1, got %d", g.GetScore("Игрок 1"))
	}

	// Проверка, что текущая мелодия теперь вторая
	if g.CurrentMelody != 1 {
		t.Errorf("Expected CurrentMelody to be 1, got %d", g.CurrentMelody)
	}

	// Второй игрок угадывает
	g.Guess("Игрок 2", "Melody2")

	// Проверка счёта второго игрока
	if g.GetScore("Игрок 2") != 1 {
		t.Errorf("Expected score to be 1 for Игрок 2, got %d", g.GetScore("Игрок 2"))
	}

	// Проверка, что текущая мелодия теперь третья
	if g.CurrentMelody != 2 {
		t.Errorf("Expected CurrentMelody to be 2, got %d", g.CurrentMelody)
	}
}
