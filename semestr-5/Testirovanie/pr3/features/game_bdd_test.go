package game_test

import (
	. "gpr3/game"
	"testing"

	. "github.com/onsi/ginkgo/v2"
	. "github.com/onsi/gomega"
)

// Точка входа для обычного запуска через go test
func TestGame(t *testing.T) {
	RegisterFailHandler(Fail)
	RunSpecs(t, "Game Suite")
}

var _ = Describe("Угадывание мелодий", func() {
	var g *Game

	BeforeEach(func() {
		g = &Game{}
	})

	Describe("Правильное угадывание мелодии", func() {
		It("должно начислить 1 очко игроку при правильном угадывании", func() {
			g.Start([]string{"Игрок 1"}, []string{"Melody1", "Melody2", "Melody3"})
			Expect(g.Guess("Игрок 1", "Melody1")).To(BeTrue())
			Expect(g.GetScore("Игрок 1")).To(Equal(1))
		})
	})

	Describe("Неправильное угадывание мелодии", func() {
		It("не должно начислить очки игроку при неправильном угадывании", func() {
			g.Start([]string{"Игрок 2"}, []string{"Melody1", "Melody2", "Melody3"})
			Expect(g.Guess("Игрок 2", "WrongMelody")).To(BeFalse())
			Expect(g.GetScore("Игрок 2")).To(Equal(0))
		})
	})

	Describe("Очки после нескольких угадываний", func() {
		It("должны корректно начисляться после последовательных угадываний", func() {
			g.Start([]string{"Игрок 1", "Игрок 2", "Игрок 3"}, []string{"Melody1", "Melody2", "Melody3"})

			Expect(g.Guess("Игрок 1", "Melody1")).To(BeTrue())
			Expect(g.Guess("Игрок 2", "Melody2")).To(BeTrue())
			Expect(g.Guess("Игрок 3", "WrongMelody")).To(BeFalse())

			Expect(g.GetScore("Игрок 1")).To(Equal(1))
			Expect(g.GetScore("Игрок 2")).To(Equal(1))
			Expect(g.GetScore("Игрок 3")).To(Equal(0))
		})
	})
})
