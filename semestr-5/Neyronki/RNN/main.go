package main

import (
	"fmt"
	"math"
	"math/rand"
)

// Параметры обучения
var learningRate = 0.01

// Функция активации (сигмоида)
func sigmoid(x float64) float64 {
	return 1 / (1 + math.Exp(-x))
}

// Производная сигмоиды
func sigmoidDerivative(x float64) float64 {
	return x * (1 - x)
}

// Прямой проход для рекуррентного слоя
func forwardPass(input, hiddenState []float64, inputWeights, recurrentWeights, outputWeights [][]float64) ([]float64, []float64) {
	// Обновляем скрытое состояние
	newHiddenState := make([]float64, len(hiddenState))
	for i := range newHiddenState {
		sum := 0.0
		for j := range input {
			sum += input[j] * inputWeights[i][j]
		}
		for j := range hiddenState {
			sum += hiddenState[j] * recurrentWeights[i][j]
		}
		newHiddenState[i] = sigmoid(sum)
	}

	// Выход сети
	output := make([]float64, len(outputWeights))
	for i := range output {
		sum := 0.0
		for j := range newHiddenState {
			sum += newHiddenState[j] * outputWeights[i][j]
		}
		output[i] = sigmoid(sum)
	}
	return newHiddenState, output
}

// Обратное распространение ошибки и обновление весов
func backwardPass(input, hiddenState, output, target []float64, inputWeights, recurrentWeights, outputWeights [][]float64) {
	// Ошибка на выходе
	outputError := make([]float64, len(output))
	for i := range output {
		outputError[i] = (target[i] - output[i]) * sigmoidDerivative(output[i])
	}

	// Ошибка скрытого состояния
	hiddenError := make([]float64, len(hiddenState))
	for i := range hiddenState {
		sum := 0.0
		for j := range outputError {
			sum += outputError[j] * outputWeights[j][i]
		}
		hiddenError[i] = sum * sigmoidDerivative(hiddenState[i])
	}

	// Обновляем веса выходного слоя
	for i := range outputWeights {
		for j := range outputWeights[i] {
			outputWeights[i][j] += learningRate * outputError[i] * hiddenState[j]
		}
	}

	// Обновляем рекуррентные и входные веса
	for i := range inputWeights {
		for j := range inputWeights[i] {
			inputWeights[i][j] += learningRate * hiddenError[i] * input[j]
		}
		for j := range recurrentWeights[i] {
			recurrentWeights[i][j] += learningRate * hiddenError[i] * hiddenState[j]
		}
	}
}

// Обучение сети
func trainRNN(inputs, targets [][]float64, inputWeights, recurrentWeights, outputWeights [][]float64, epochs int) {
	hiddenState := make([]float64, len(inputWeights)) // начальное скрытое состояние

	for epoch := 0; epoch < epochs; epoch++ {
		fmt.Printf("Итерация: %d\n", epoch)
		for i, input := range inputs {
			// Прямой проход
			hiddenState, output := forwardPass(input, hiddenState, inputWeights, recurrentWeights, outputWeights)

			// Печать значений для отладки
			fmt.Printf("Вход: %v\n", input)
			fmt.Printf("Скрытое состояние: %v\n", hiddenState)
			fmt.Printf("Выход: %v\n", output)
			fmt.Printf("Цель: %v\n", targets[i])

			// Обратное распространение ошибки
			backwardPass(input, hiddenState, output, targets[i], inputWeights, recurrentWeights, outputWeights)
		}
	}
}

func main() {
	// Входные данные и целевые значения
	inputs := [][]float64{
		{0, 0},
		{0, 1},
		{1, 0},
		{1, 1},
	}
	targets := [][]float64{
		{0},
		{0},
		{0},
		{1},
	}

	// Инициализация весов
	inputWeights := [][]float64{
		{rand.Float64(), rand.Float64()},
		{rand.Float64(), rand.Float64()},
	}
	recurrentWeights := [][]float64{
		{rand.Float64(), rand.Float64()},
		{rand.Float64(), rand.Float64()},
	}
	outputWeights := [][]float64{
		{rand.Float64(), rand.Float64()},
	}

	// Обучение сети
	trainRNN(inputs, targets, inputWeights, recurrentWeights, outputWeights, 100)
}
