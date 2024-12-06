package main

import (
	"fmt"
	"math/rand"
	"time"
)

// Функция активации
func activate(input, weights []float64, porog float64) (sum float64) {
	for i := 0; i < len(input); i++ {
		sum += input[i] * weights[i]
	}
	if sum > porog {
		return 1.0
	}
	return -1.0
}

// Дельта-правило для обновления весов
func delta(input, weights []float64, target, n, porog float64) (updatedWeights []float64, err float64) {
	output := activate(input, weights, porog)
	err = target - output // Ошибка
	for i := 0; i < len(weights); i++ {
		weights[i] += n * err * input[i] // Обновление весов
	}
	return weights, err
}

func main() {
	rand.Seed(time.Now().UnixNano()) // Для генерации случайных чисел

	// Входные данные и цели
	inputs := [][]float64{
		{1.0, 1.0},
		{1.0, -1.0},
		{-1.0, 1.0},
		{-1.0, -1.0},
	}
	targets := []float64{1.0, -1.0, -1.0, -1.0} // Цели для операции И

	// Инициализация весов
	weights := make([]float64, len(inputs[0]))
	for i := 0; i < len(weights); i++ {
		weights[i] = rand.Float64() - 0.5 // Случайные веса от -0.5 до 0.5
	}

	// Параметры обучения
	n := 0.1     // Скорость обучения
	porog := 0.0 // Порог активации
	maxEpochs := 100
	fmt.Println("Начальные веса:", weights)

	// Обучение
	for epoch := 1; epoch <= maxEpochs; epoch++ {
		totalError := 0.0

		for i := 0; i < len(inputs); i++ {
			var err float64
			weights, err = delta(inputs[i], weights, targets[i], n, porog)
			totalError += err * err // Сумма квадратов ошибок
		}

		fmt.Printf("Эпоха %d, Общая ошибка: %.2f, Веса: %v\n", epoch, totalError, weights)

		if totalError == 0 {
			fmt.Printf("Обучение завершено на эпохе %d\n", epoch)
			break
		}
	}

	// Проверка предсказаний
	fmt.Println("\nРезультаты после обучения:")
	for i := 0; i < len(inputs); i++ {
		prediction := activate(inputs[i], weights, porog)
		fmt.Printf("Вход: %v, Предсказание: %.0f, Цель: %.0f\n", inputs[i], prediction, targets[i])
	}
}
