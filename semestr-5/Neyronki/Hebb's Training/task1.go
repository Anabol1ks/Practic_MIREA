package main

import (
	"fmt"
	"math/rand"
)

// Функция обучения по правилу Хебба
func hebbLearn(inputs []float64, outputs []float64, w []float64, n float64) []float64 {
	for i := 0; i < len(inputs); i++ {
		w[i] += n * inputs[i] * outputs[i]
	}
	return w
}

// Функция для вычисления предсказания на основе текущих весов
func predict(input float64, weight float64) int {
	if input*weight > 0.5 { // Порог
		return 1
	}
	return 0
}

func main() {
	// Входы для операции И
	input1 := []float64{0.0, 0.0, 1.0, 1.0}
	input2 := []float64{0.0, 1.0, 0.0, 1.0}

	// Генерация входных данных как результат операции И
	inputs := make([]float64, len(input1))
	for i := 0; i < len(input1); i++ {
		if input1[i] == 1.0 && input2[i] == 1.0 {
			inputs[i] = 1.0
		} else {
			inputs[i] = 0.0
		}
	}

	// Выходы
	outputs := []float64{0.0, 0.0, 0.0, 1.0}

	// Инициализация весов
	weights := make([]float64, len(inputs))
	for i := 0; i < len(weights); i++ {
		weights[i] = rand.Float64() * 0.1 // Начальные веса
	}

	n := 0.1     // Скорость обучения
	epochs := 10 // Количество эпох

	fmt.Printf("Начальные веса: %.2f\n", weights[0])

	// Обучение
	for epoch := 1; epoch <= epochs; epoch++ {
		weights = hebbLearn(inputs, outputs, weights, n)
		fmt.Printf("Эпоха %d - Обновленные веса: %v\n", epoch, weights)
	}

	// Проверка модели
	fmt.Println("\nРезультаты после обучения:")
	for i := 0; i < len(inputs); i++ {
		pred := predict(inputs[i], weights[i])
		fmt.Printf("Входы: [%d %d], Предсказание: %d, Ожидаемый результат: %d\n",
			int(input1[i]), int(input2[i]), pred, int(outputs[i]))
	}
}
