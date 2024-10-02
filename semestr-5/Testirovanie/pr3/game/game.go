package game

import (
	"strings"
)

type Player struct {
	Name  string
	Score int
}

type Game struct {
	Players       []*Player
	Melodies      []string
	CurrentPlayer int
	CurrentMelody int
}

func (g *Game) Start(players []string, melodies []string) {
	g.Players = make([]*Player, len(players))
	for i, name := range players {
		g.Players[i] = &Player{Name: name, Score: 0}
	}
	g.Melodies = melodies
	g.CurrentPlayer = 0
	g.CurrentMelody = 0
}

func (g *Game) Guess(playerName, guess string) bool {
	// Найти игрока
	var player *Player
	for _, p := range g.Players {
		if p.Name == playerName {
			player = p
			break
		}
	}
	if player == nil {
		return false
	}

	// Проверить угадывание
	correct := strings.ToLower(g.Melodies[g.CurrentMelody]) == strings.ToLower(guess)
	if correct {
		player.Score++
	}

	// Переход к следующей мелодии и игроку
	g.CurrentMelody = (g.CurrentMelody + 1) % len(g.Melodies)
	g.CurrentPlayer = (g.CurrentPlayer + 1) % len(g.Players)

	return correct
}

func (g *Game) GetScore(playerName string) int {
	for _, p := range g.Players {
		if p.Name == playerName {
			return p.Score
		}
	}
	return 0
}
