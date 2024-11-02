package main

import (
	"fmt"
	"math"
	"math/rand"
)

// Параметры обучения
var learningRate = 0.1

// Функция активации (sigmoid)
func activationFunc(x float64) float64 {
	return 1.0 / (1.0 + math.Exp(-x))
}

// Производная функции активации (sigmoid)
func derivActivationFunc(x float64) float64 {
	return x * (1 - x)
}

// Инициализация весов
func initWeights(rows, cols int) [][]float64 {
	weights := make([][]float64, rows)
	for i := range weights {
		weights[i] = make([]float64, cols)
		for j := range weights[i] {
			weights[i][j] = rand.Float64()
		}
	}
	return weights
}

// Прямое распространение с использованием скрытого состояния
func forward(input, hiddenState []float64, weightsInput, weightsHidden, weightsOutput []float64) (float64, []float64) {
	// Обновление скрытого состояния
	for i := range hiddenState {
		hiddenState[i] = activationFunc(input[i]*weightsInput[i] + hiddenState[i]*weightsHidden[i])
	}

	// Вычисление выхода
	output := 0.0
	for i := range hiddenState {
		output += hiddenState[i] * weightsOutput[i]
	}
	return activationFunc(output), hiddenState
}

// Обратное распространение во времени для обновления весов
func backward(target, output float64, hiddenState, weightsOutput, weightsHidden, weightsInput []float64) {
	// Ошибка на выходе
	outputError := target - output
	deltaOutput := outputError * derivActivationFunc(output)

	// Обновление весов выходного слоя
	for i := range weightsOutput {
		weightsOutput[i] += learningRate * deltaOutput * hiddenState[i]
	}

	// Обратное распространение ошибки в скрытом состоянии
	for i := range hiddenState {
		hiddenError := deltaOutput * weightsOutput[i] * derivActivationFunc(hiddenState[i])
		weightsHidden[i] += learningRate * hiddenError * hiddenState[i]
		weightsInput[i] += learningRate * hiddenError
	}
}

func main() {
	// Входные данные для логической операции И
	inputs := [][]float64{
		{0, 0},
		{0, 1},
		{1, 0},
		{1, 1},
	}

	// Целевые значения для логической операции И
	targets := []float64{0, 0, 0, 1}

	// Инициализация весов
	weightsInput := []float64{rand.Float64(), rand.Float64()}
	weightsHidden := []float64{rand.Float64(), rand.Float64()}
	weightsOutput := []float64{rand.Float64(), rand.Float64()}

	// Инициализация скрытого состояния
	hiddenState := []float64{0, 0}

	// Обучение сети
	for epoch := 0; epoch < 100; epoch++ {
		for i, input := range inputs {
			// Прямое распространение
			output, updatedHiddenState := forward(input, hiddenState, weightsInput, weightsHidden, weightsOutput)
			fmt.Printf("Итерация: %d, шаг: %d, Предсказанный результат: %.4f\n", epoch, i, output)

			// Обратное распространение для обновления весов
			backward(targets[i], output, updatedHiddenState, weightsOutput, weightsHidden, weightsInput)
			hiddenState = updatedHiddenState

			fmt.Printf("Цель: %.2f, Ошибка: %.4f\n", targets[i], targets[i]-output)
			fmt.Printf("Обновленные веса выходного слоя: %v\n", weightsOutput)
			fmt.Println("-----------------------------")
		}
	}
}
