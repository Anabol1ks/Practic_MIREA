package main

import (
	"fmt"
	"math"
	"math/rand"
)

// Коэффициент обучения
var learningRate = 0.1

// Функция активации (sin)
func activationFunc(x float64) float64 {
	return math.Sin(x)
}

// Производная функции активации (cos)
func derivActivationFunc(x float64) float64 {
	return math.Cos(x)
}

// Прямой проход (forward pass)
func forward(inputs []float64, weights0 [][]float64, weights1 []float64) (float64, []float64) {
	// Вычисление скрытого слоя
	inputs1 := []float64{
		activationFunc(inputs[0])*weights0[0][0] + activationFunc(inputs[1])*weights0[1][0], // input10
		activationFunc(inputs[1])*weights0[0][1] + activationFunc(inputs[0])*weights0[1][1], // input11
	}

	// Вычисление выхода
	input2 := activationFunc(inputs1[0])*weights1[0] + activationFunc(inputs1[1])*weights1[1]

	// Возвращаем предсказанное значение и скрытые активации
	return activationFunc(input2), inputs1
}

// Обратный проход (backpropagation)
func backward(inputs, inputs1 []float64, output, target float64, weights0 [][]float64, weights1 []float64) {
	// Ошибка на выходе
	error := target - output
	// Вычисление сигма для выходного слоя
	sigma2 := derivActivationFunc(output) * error

	// Обновление весов выходного слоя
	weights1[0] += learningRate * sigma2 * derivActivationFunc(inputs1[0])
	weights1[1] += learningRate * sigma2 * derivActivationFunc(inputs1[1])

	// Вычисление сигма для скрытого слоя
	sigma1_0 := derivActivationFunc(inputs1[0]) * sigma2 * weights1[0]
	sigma1_1 := derivActivationFunc(inputs1[1]) * sigma2 * weights1[1]

	// Обновление весов скрытого слоя
	weights0[0][0] += learningRate * sigma1_0 * derivActivationFunc(inputs[0])
	weights0[0][1] += learningRate * sigma1_1 * derivActivationFunc(inputs[1])
	weights0[1][0] += learningRate * sigma1_0 * derivActivationFunc(inputs[0])
	weights0[1][1] += learningRate * sigma1_1 * derivActivationFunc(inputs[1])
}

func main() {
	// Инициализация весов
	weights0 := [][]float64{
		{rand.Float64(), rand.Float64()},
		{rand.Float64(), rand.Float64()},
	}
	weights1 := []float64{rand.Float64(), rand.Float64()}

	// Входные данные (обучающая выборка)
	inputs := [][]float64{
		{0, 0},
		{0, 1},
		{1, 0},
		{1, 1},
	}

	// Целевые значения
	targets := []float64{0, 1, 1, 1}

	// Результаты предсказания для каждого примера
	OUT := make([]float64, len(inputs))

	// Цикл обучения
	for epoch := 0; epoch < 100; epoch++ {
		for i, input := range inputs {
			fmt.Printf("Итерация: %d ----------\n", i)
			fmt.Printf("Веса ур. 0: %v\n", weights0)
			fmt.Printf("Веса ур. 1: %v\n", weights1)

			// Прямой проход: вычисление выхода сети
			output, inputs1 := forward(input, weights0, weights1)
			OUT[i] = output
			fmt.Printf("Результат: %v\n", OUT)

			// Обратный проход: обновление весов
			backward(input, inputs1, output, targets[i], weights0, weights1)

			fmt.Printf("Цель: %v\n", targets)

			// Вывод обновлённых весов
			fmt.Printf("Новые веса ур. 0: %v\n", weights0)
			fmt.Printf("Новые веса ур. 1: %v\n", weights1)
		}
	}
}
