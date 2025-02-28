package main

import (
	"fmt"
	"math"
	"math/rand"
	"time"
)

// Параметры алгоритма
const (
	alpha = 2.0
	beta  = 1.0
	rho   = 0.15
)

// Матрица расстояний
var distances = [][]float64{
	{math.Inf(1), 4, 7, 8, 3, 9},
	{4, math.Inf(1), 5, 6, 4, 5},
	{7, 5, math.Inf(1), 1, 7, 6},
	{8, 6, 1, math.Inf(1), 2, 3},
	{3, 4, 7, 2, math.Inf(1), 8},
	{9, 5, 6, 3, 8, math.Inf(1)},
}

var (
	numVertices = len(distances)
	pheromones  = make([][]float64, numVertices)
	eta         = make([][]float64, numVertices)
)

func init() {
	rand.Seed(time.Now().UnixNano())

	// Инициализация матрицы феромонов и эвристической информации
	for i := range pheromones {
		pheromones[i] = make([]float64, numVertices)
		eta[i] = make([]float64, numVertices)
		for j := range pheromones[i] {
			pheromones[i][j] = 1.0
			if distances[i][j] != math.Inf(1) {
				eta[i][j] = 1 / distances[i][j]
			} else {
				eta[i][j] = 0
			}
		}
	}
}

// Расчет вероятностей перехода
func calculateProbabilities(currentVertex int, unvisited []int) []float64 {
	numerator := make([]float64, len(unvisited))
	denominator := 0.0

	for i, v := range unvisited {
		numerator[i] = math.Pow(pheromones[currentVertex][v], alpha) * math.Pow(eta[currentVertex][v], beta)
		denominator += numerator[i]
	}

	probabilities := make([]float64, len(unvisited))
	for i := range probabilities {
		probabilities[i] = numerator[i] / denominator
	}
	return probabilities
}

// Выбор следующей вершины на основе вероятностей
func chooseNextVertex(probabilities []float64, unvisited []int) int {
	r := rand.Float64()
	cumulative := 0.0
	for i, p := range probabilities {
		cumulative += p
		if r <= cumulative {
			return unvisited[i]
		}
	}
	return unvisited[len(unvisited)-1]
}

// Маршрут муравья
func antTravel(startVertex int) ([]int, float64) {
	currentVertex := startVertex
	unvisited := make([]int, 0, numVertices-1)
	for i := 0; i < numVertices; i++ {
		if i != currentVertex {
			unvisited = append(unvisited, i)
		}
	}

	path := []int{currentVertex}
	totalLength := 0.0

	for len(unvisited) > 0 {
		probabilities := calculateProbabilities(currentVertex, unvisited)
		nextVertex := chooseNextVertex(probabilities, unvisited)

		totalLength += distances[currentVertex][nextVertex]
		path = append(path, nextVertex)
		currentVertex = nextVertex

		// Удаляем посещенную вершину из списка
		for i, v := range unvisited {
			if v == currentVertex {
				unvisited = append(unvisited[:i], unvisited[i+1:]...)
				break
			}
		}
	}

	// Возврат к стартовой вершине
	path = append(path, startVertex)
	totalLength += distances[currentVertex][startVertex]

	return path, totalLength
}

// Обновление феромонов
func updatePheromones(paths [][]int, lengths []float64) {
	for i := range pheromones {
		for j := range pheromones[i] {
			pheromones[i][j] *= (1 - rho)
		}
	}

	for k, path := range paths {
		deltaTau := 1 / lengths[k]
		for i := 0; i < len(path)-1; i++ {
			from, to := path[i], path[i+1]
			pheromones[from][to] += deltaTau
			pheromones[to][from] += deltaTau
		}
	}
}

// Основная функция муравьиного алгоритма
func antColonyOptimization(numAnts, numIterations, startVertex int) ([]int, float64) {
	var bestPath []int
	bestLength := math.Inf(1)

	for iteration := 0; iteration < numIterations; iteration++ {
		paths := make([][]int, 0, numAnts)
		lengths := make([]float64, 0, numAnts)

		fmt.Printf("\nИтерация %d:\n", iteration+1)

		for ant := 0; ant < numAnts; ant++ {
			path, length := antTravel(startVertex)
			paths = append(paths, path)
			lengths = append(lengths, length)

			fmt.Printf("  Муравей %d: Путь = %v, Длина = %.2f\n", ant+1, path, length)
		}

		updatePheromones(paths, lengths)

		// Обновление глобального лучшего пути
		for i, length := range lengths {
			if length < bestLength {
				bestLength = length
				bestPath = paths[i]
			}
		}
	}

	return bestPath, bestLength
}

func main() {
	numAnts := 5
	numIterations := 100
	startVertex := 0

	bestPath, bestLength := antColonyOptimization(numAnts, numIterations, startVertex)
	fmt.Printf("\nЛучший путь: %v\nЛучшая длина: %.2f\n", bestPath, bestLength)
}
