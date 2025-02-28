package main

import (
	"bufio"
	"fmt"
	"gpr3/game"
	"os"
)

func main() {
	// Список игроков
	players := []string{"Игрок 1", "Игрок 2", "Игрок 3"}
	// Список мелодий
	melodies := []string{"Melody1", "Melody2", "Melody3"}

	// Инициализация игры
	game := &game.Game{}
	game.Start(players, melodies)

	// Основная игровая логика
	scanner := bufio.NewScanner(os.Stdin)
	for {
		player := game.Players[game.CurrentPlayer]
		fmt.Printf("Ход игрока %s. Угадайте мелодию: ", player.Name)
		scanner.Scan()
		guess := scanner.Text()

		if game.Guess(player.Name, guess) {
			fmt.Println("Правильно!")
		} else {
			correctMelody := game.Melodies[(game.CurrentMelody-1+len(game.Melodies))%len(game.Melodies)]
			fmt.Printf("Неправильно! Правильный ответ: %s\n", correctMelody)
		}

		fmt.Printf("Очки игрока %s: %d\n", player.Name, player.Score)
	}
}
