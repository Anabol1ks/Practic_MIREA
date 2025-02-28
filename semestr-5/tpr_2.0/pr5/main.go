package main

import (
	"fmt"
	"math"
	"math/rand"
	"time"
)

// Параметры алгоритма
const (
	a, b           = 1.0, 100.0 // параметры функции Розенброка
	S              = 30         // количество пчел-разведчиков
	n              = 8          // количество лучших участков
	m              = 10         // количество перспективных участков
	N              = 15         // пчелы в лучших участках
	M              = 10         // пчелы в перспективных участках
	Delta          = 0.5        // размер локальной области поиска
	StopIterations = 1000       // критерий останова
	SearchMin      = -2.0       // минимальная граница поиска
	SearchMax      = 2.0        // максимальная граница поиска
	DistanceThresh = 0.2        // порог для объединения участков
)

// Пчела
type Bee struct {
	X, Y float64 // координаты
	F    float64 // значение функции
}

// Функция Розенброка
func rosenbrock(x, y float64) float64 {
	return math.Pow(a-x, 2) + b*math.Pow(y-x*x, 2)
}

// Генерация случайной точки
func randomPoint() (float64, float64) {
	return SearchMin + rand.Float64()*(SearchMax-SearchMin),
		SearchMin + rand.Float64()*(SearchMax-SearchMin)
}

// Оценка пчелы
func evaluateBee(bee *Bee) {
	bee.F = rosenbrock(bee.X, bee.Y)
}

// Генерация разведчиков
func generateScoutBees() []Bee {
	bees := make([]Bee, S)
	for i := 0; i < S; i++ {
		bees[i].X, bees[i].Y = randomPoint()
		evaluateBee(&bees[i])
	}
	return bees
}

// Сортировка пчел по значению функции
func sortBees(bees []Bee) {
	for i := 0; i < len(bees)-1; i++ {
		for j := 0; j < len(bees)-i-1; j++ {
			if bees[j].F > bees[j+1].F {
				bees[j], bees[j+1] = bees[j+1], bees[j]
			}
		}
	}
}

// Вычисление Евклидова расстояния
func euclideanDistance(b1, b2 Bee) float64 {
	return math.Sqrt(math.Pow(b1.X-b2.X, 2) + math.Pow(b1.Y-b2.Y, 2))
}

// Удаление близких точек
func mergeCloseAreas(bees []Bee, threshold float64) []Bee {
	merged := []Bee{bees[0]} // Начинаем с первой точки
	for i := 1; i < len(bees); i++ {
		tooClose := false
		for _, b := range merged {
			if euclideanDistance(bees[i], b) < threshold {
				tooClose = true
				break
			}
		}
		if !tooClose {
			merged = append(merged, bees[i])
		}
	}
	return merged
}

// Поиск в окрестности
func searchNeighborhood(center Bee, numBees int, delta float64) []Bee {
	neighborhood := make([]Bee, numBees)
	for i := 0; i < numBees; i++ {
		neighborhood[i].X = center.X + (rand.Float64()*2-1)*delta
		neighborhood[i].Y = center.Y + (rand.Float64()*2-1)*delta
		evaluateBee(&neighborhood[i])
	}
	return neighborhood
}

// Основной алгоритм
func beeAlgorithm() Bee {
	rand.Seed(time.Now().UnixNano())
	scoutBees := generateScoutBees()

	for iteration := 0; iteration < StopIterations; iteration++ {
		// Сортируем разведчиков по значению функции
		sortBees(scoutBees)

		// Удаляем слишком близкие участки
		scoutBees = mergeCloseAreas(scoutBees, DistanceThresh)

		// Вывод текущего состояния
		bestBee := scoutBees[0]
		fmt.Printf("Итерация %d: Лучшее решение: X = %.4f, Y = %.4f, F(X,Y) = %.4f\n",
			iteration+1, bestBee.X, bestBee.Y, bestBee.F)

		// Лучшие и перспективные участки
		bestBees := scoutBees[:min(len(scoutBees), n)]
		promisingBees := scoutBees[n:min(len(scoutBees), n+m)]

		// Локальный поиск
		newBees := []Bee{}
		for _, bee := range bestBees {
			newBees = append(newBees, searchNeighborhood(bee, N, Delta)...)
		}
		for _, bee := range promisingBees {
			newBees = append(newBees, searchNeighborhood(bee, M, Delta)...)
		}

		// Обновляем популяцию
		scoutBees = append(newBees, scoutBees[n+m:]...)
		sortBees(scoutBees)
	}

	// Лучшее решение
	return scoutBees[0]
}

func min(a, b int) int {
	if a < b {
		return a
	}
	return b
}

func main() {
	bestSolution := beeAlgorithm()
	fmt.Printf("\nФинальное решение: X = %.4f, Y = %.4f, F(X,Y) = %.4f\n",
		bestSolution.X, bestSolution.Y, bestSolution.F)
}
