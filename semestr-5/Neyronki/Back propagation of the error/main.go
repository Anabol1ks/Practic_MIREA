package main

import (
	"fmt"
	"math"
	"math/rand"
)

// Функция активации (сигмоида)
func sigmoid(x float64) float64 {
	return 1 / (1 + math.Exp(-x))
}

// Производная сигмоидальной функции
func sigmoidDerivative(x float64) float64 {
	return x * (1 - x)
}

// Функция для скалярного произведения векторов (без проверки на длину)
func dot(a, b []float64) (sum float64) {
	for i := 0; i < len(a); i++ {
		sum += a[i] * b[i]
	}
	return
}

// Функция для вычисления квадратичной ошибки
func calculateError(target, output float64) float64 {
	return 0.5 * math.Pow(target-output, 2)
}

// Функция обратного распространения ошибки
func backpropagate(input, weightsInputHidden, weightsHiddenOutput []float64, biasHidden, biasOutput *float64, target, learningRate float64) {
	// Прямой проход
	hiddenInput := dot(input, weightsInputHidden) + *biasHidden
	hiddenOutput := sigmoid(hiddenInput)

	outputInput := dot([]float64{hiddenOutput}, weightsHiddenOutput) + *biasOutput
	output := sigmoid(outputInput)

	// Вычисление ошибки на выходе
	outputError := target - output
	quadraticError := calculateError(target, output)
	fmt.Printf("Квадратичная ошибка: %.4f\n", quadraticError)

	// Обратное распространение ошибки
	outputDelta := outputError * sigmoidDerivative(output)
	hiddenError := outputDelta * weightsHiddenOutput[0]
	hiddenDelta := hiddenError * sigmoidDerivative(hiddenOutput)

	// Обновление весов выходного слоя
	for i := range weightsHiddenOutput {
		weightsHiddenOutput[i] += learningRate * outputDelta * hiddenOutput
	}
	// Обновление весов скрытого слоя
	for i := range weightsInputHidden {
		weightsInputHidden[i] += learningRate * hiddenDelta * input[i]
	}

	// Обновление смещений (bias)
	*biasOutput += learningRate * outputDelta
	*biasHidden += learningRate * hiddenDelta

	fmt.Println("Обновленные веса скрытого слоя:", weightsInputHidden)
	fmt.Println("Обновленные веса выходного слоя:", weightsHiddenOutput)
	fmt.Printf("Обновленные смещения: biasHidden = %.4f, biasOutput = %.4f\n", *biasHidden, *biasOutput)
}

func main() {
	// Инициализация весов случайными значениями
	weightsInputHidden := []float64{rand.Float64(), rand.Float64()}
	weightsHiddenOutput := []float64{rand.Float64()}

	// Смещения для скрытого и выходного слоя
	biasHidden := rand.Float64()
	biasOutput := rand.Float64()

	// Входные данные и целевое значение
	input := []float64{1.0, 0.5}
	target := 1.0
	learningRate := 0.1

	fmt.Println("Начальные веса скрытого слоя:", weightsInputHidden)
	fmt.Println("Начальные веса выходного слоя:", weightsHiddenOutput)
	fmt.Printf("Начальные смещения: biasHidden = %.4f, biasOutput = %.4f\n", biasHidden, biasOutput)

	// Выполнение обратного распространения ошибки
	backpropagate(input, weightsInputHidden, weightsHiddenOutput, &biasHidden, &biasOutput, target, learningRate)
}
