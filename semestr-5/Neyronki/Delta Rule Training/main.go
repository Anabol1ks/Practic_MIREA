package main

import (
	"fmt"
	"math/rand/v2"
)

func activate(input, weights []float64, porog float64) (sum float64) {
	for i := 0; i < len(input); i++ {
		sum += input[i] * weights[i]
	}
	if sum > porog {
		return 1.0
	}
	return -1.0
}

func delta(input, weights []float64, target, n, porog float64) []float64 {
	output := activate(input, weights, porog)

	err := target - output

	fmt.Printf("Ошибка: %.2f\n", err)

	for i := 0; i < len(weights); i++ {
		weights[i] += n * err * input[i]
	}
	return weights
}

func main() {
	inputs := []float64{1.0, -1.0, 1.0}
	target := 1.0

	weights := make([]float64, len(inputs))
	for i := 0; i < len(weights); i++ {
		weights[i] = rand.Float64()
	}

	n := 0.1
	porog := 0.5

	fmt.Println("Начальные веса:", weights)

	fmt.Println("Обновлённые веса:", delta(inputs, weights, target, n, porog))
}
