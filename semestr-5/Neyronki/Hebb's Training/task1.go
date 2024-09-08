package main

import (
	"fmt"
	"math/rand"
)

func hebbLearn(inputs []float64, outputs []float64, w []float64, n float64) []float64 {
	for i := 0; i < len(inputs); i++ {
		w[i] += n * inputs[i] * outputs[i]
	}
	return w
}

func main() {
	inputs := []float64{0.0, 0.0, 1.1, 1.1}
	outputs := []float64{0.0, 1.0, 0.0, 1.0}

	weights := make([]float64, len(inputs))
	for i := 0; i < len(weights); i++ {
		weights[i] = rand.Float64()
	}

	n := 0.1
	w := make([]float64, len(weights))
	for i, _ := range weights {
		w[i] = weights[i]
	}
	fmt.Println("Начальные веса:", weights)
	weights = hebbLearn(inputs, outputs, weights, n)
	fmt.Println("Обновленные веса:", weights)
	// res := make([]float64, len(weights))
	// for i := 0; i < len(res); i++ {
	// 	if w[i] == weights[i] {
	// 		res[i] = 0.0
	// 	} else {
	// 		res[i] = 1.0
	// 	}
	// }
	// fmt.Println(res)

}
