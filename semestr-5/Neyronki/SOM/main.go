package main

import (
	"fmt"
	"math"
	"math/rand"
)

// Параметры обучения
var learningRate = 0.1
var radius = 1.0

// Функция для вычисления евклидова расстояния
func euclideanDistance(v1, v2 []float64) float64 {
	sum := 0.0
	for i := range v1 {
		sum += math.Pow(v1[i]-v2[i], 2)
	}
	return math.Sqrt(sum)
}

// Обновление весов победителя и его соседей
func updateWeights(winnerIndex int, input []float64, weights [][]float64, learningRate, radius float64) {
	for i := range weights {
		// Вычисляем расстояние между победителем и текущим нейроном
		distance := math.Abs(float64(i - winnerIndex))

		// Если нейрон находится в радиусе соседства, обновляем его веса
		if distance <= radius {
			for j := range weights[i] {
				weights[i][j] += learningRate * (input[j] - weights[i][j])
			}
		}
	}
}

// Функция для нахождения победителя (нейрон с минимальным расстоянием)
func findWinner(input []float64, weights [][]float64) int {
	minIndex := 0
	minDist := euclideanDistance(input, weights[0])

	for i := 1; i < len(weights); i++ {
		dist := euclideanDistance(input, weights[i])
		if dist < minDist {
			minDist = dist
			minIndex = i
		}
	}
	return minIndex
}

// Прямой проход (обучение сети)
func trainKohonen(inputs [][]float64, weights [][]float64) {
	for i, input := range inputs {
		// Находим победителя
		winner := findWinner(input, weights)

		fmt.Printf("Итерация: %d ----------\n", i)
		fmt.Printf("Входные данные: %v\n", input)
		fmt.Printf("Веса до обновления: %v\n", weights)
		fmt.Printf("Победитель: %d\n", winner)

		// Обновляем веса
		updateWeights(winner, input, weights, learningRate, radius)

		fmt.Printf("Веса после обновления: %v\n", weights)
		fmt.Println("-----------------------------")
	}
}

func main() {
	// Входные данные (обучающая выборка)
	inputs := [][]float64{
		{0, 0},
		{0, 1},
		{1, 0},
		{1, 1},
	}

	// Инициализация весов сети (например, 4 нейрона, каждый имеет по 2 веса)
	weights := [][]float64{
		{rand.Float64(), rand.Float64()},
		{rand.Float64(), rand.Float64()},
		{rand.Float64(), rand.Float64()},
		{rand.Float64(), rand.Float64()},
	}

	// Обучение сети
	for epoch := 0; epoch < 10; epoch++ {
		fmt.Println(epoch)
		trainKohonen(inputs, weights)
	}
}
