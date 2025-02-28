package main

import (
	"fmt"
	"math"
	"math/rand"
	"time"
)

// Функция Розенброка для оценки стабильности полета дрона
func rosenbrock(theta, a float64) float64 {
	return math.Pow((a-1), 2) + 100*math.Pow((theta-a*a), 2)
}

// Генерация случайного числа с распределением Коши
func cauchyRandom() float64 {
	return math.Tan(math.Pi * (rand.Float64() - 0.5))
}

// Алгоритм имитации отжига по Коши
func simulatedAnnealingCauchy(maxIterations int, initialTemp float64, coolingRate float64) (float64, float64, float64) {
	// Инициализация начальных параметров (угол наклона theta и ускорение a)
	theta := rand.Float64() * 10
	a := rand.Float64() * 10
	currentEnergy := rosenbrock(theta, a)

	for i := 0; i < maxIterations; i++ {
		// Вычисление новой температуры
		temperature := initialTemp / float64(i+1)

		// Генерация новых значений theta и a с помощью распределения Коши
		newTheta := theta + cauchyRandom()
		newA := a + cauchyRandom()

		// Вычисление энергии для нового состояния
		newEnergy := rosenbrock(newTheta, newA)

		// Принятие нового состояния с определённой вероятностью
		if newEnergy < currentEnergy || math.Exp((currentEnergy-newEnergy)/temperature) > rand.Float64() {
			theta = newTheta
			a = newA
			currentEnergy = newEnergy
		}

		// Снижение температуры
		initialTemp *= coolingRate
	}

	return theta, a, currentEnergy
}

func main() {
	rand.Seed(time.Now().UnixNano())

	// Настройки алгоритма отжига
	maxIterations := 10000
	initialTemp := 100.0
	coolingRate := 0.99

	// Запуск алгоритма
	optimalTheta, optimalA, minEnergy := simulatedAnnealingCauchy(maxIterations, initialTemp, coolingRate)

	fmt.Printf("Оптимальные параметры для стабилизации полета дрона:\n")
	fmt.Printf("Угол наклона (theta): %.4f\n", optimalTheta)
	fmt.Printf("Ускорение (a): %.4f\n", optimalA)
	fmt.Printf("Минимальная энергия (функция Розенброка): %.4f\n", minEnergy)
}
