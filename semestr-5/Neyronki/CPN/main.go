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

// Обновление весов карты Кохонена
func updateKohonenWeights(winnerIndex int, input []float64, weights [][]float64, learningRate, radius float64) {
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

// Функция для нахождения победителя на карте Кохонена
func findKohonenWinner(input []float64, kohonenWeights [][]float64) int {
	minIndex := 0
	minDist := euclideanDistance(input, kohonenWeights[0])

	for i := 1; i < len(kohonenWeights); i++ {
		dist := euclideanDistance(input, kohonenWeights[i])
		if dist < minDist {
			minDist = dist
			minIndex = i
		}
	}
	return minIndex
}

// Обратное распространение ошибки для выходного слоя
func backpropagationError(output, target float64) float64 {
	return target - output
}

// Прямое распространение на выходной слой
func forwardOutput(winnerIndex int, outputWeights []float64) float64 {
	// Веса выходного слоя умножаются на победивший нейрон карты Кохонена
	return outputWeights[winnerIndex]
}

// Обновление весов выходного слоя
func updateOutputWeights(winnerIndex int, outputError float64, outputWeights []float64) {
	outputWeights[winnerIndex] += learningRate * outputError
}

func main() {
	// Входные данные
	inputs := [][]float64{
		{0, 0},
		{0, 1},
		{1, 0},
		{1, 1},
	}

	// Целевые значения для логической операции И
	targets := []float64{0, 0, 0, 1}

	// Инициализация весов карты Кохонена
	kohonenWeights := [][]float64{
		{rand.Float64(), rand.Float64()},
		{rand.Float64(), rand.Float64()},
		{rand.Float64(), rand.Float64()},
		{rand.Float64(), rand.Float64()},
	}

	// Инициализация весов выходного слоя
	outputWeights := []float64{rand.Float64(), rand.Float64(), rand.Float64(), rand.Float64()}

	// Обучение сети встречного распространения
	for epoch := 0; epoch < 10; epoch++ {
		for i, input := range inputs {
			// Находим победителя на карте Кохонена
			winnerIndex := findKohonenWinner(input, kohonenWeights)
			fmt.Printf("Итерация: %d, Победитель: %d\n", i, winnerIndex)
			fmt.Printf("Веса до обновления: %v\n", kohonenWeights)

			// Обновляем веса карты Кохонена
			updateKohonenWeights(winnerIndex, input, kohonenWeights, learningRate, radius)
			fmt.Printf("Веса после обновления: %v\n", kohonenWeights)
			// Прямое распространение на выходной слой
			output := forwardOutput(winnerIndex, outputWeights)
			fmt.Println("~~~~~~~~~~~~~~~~~~")
			fmt.Printf("Веса выходного слоя: %v\n", outputWeights)
			fmt.Printf("Предсказанный результат: %.4f\n", output)

			// Вычисление ошибки
			outputError := backpropagationError(output, targets[i])
			fmt.Printf("Ошибка: %.4f\n", outputError)

			// Обновляем веса выходного слоя
			updateOutputWeights(winnerIndex, outputError, outputWeights)
			fmt.Printf("Обновленные веса выходного слоя: %v\n", outputWeights)

			fmt.Println("-----------------------------")
		}
	}
}
