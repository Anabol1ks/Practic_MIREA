package main

import (
	"fmt"
	"math"
	"math/rand"
	"time"
)

const (
	numParticles   = 30  // Количество частиц в рое
	numDimensions  = 2   // Размерность задачи (функция Розенброка)
	maxIterations  = 100 // Максимальное количество итераций
	inertiaWeight  = 0.7 // Вес инерции
	cognitiveCoeff = 1.5 // Коэффициент когнитивной составляющей
	socialCoeff    = 1.5 // Коэффициент социальной составляющей
)

// Функция Розенброка
func rosenbrock(x []float64) float64 {
	sum := 0.0
	for i := 0; i < len(x)-1; i++ {
		sum += 100*math.Pow(x[i+1]-x[i]*x[i], 2) + math.Pow(1-x[i], 2)
	}
	return sum
}

// Частица
type Particle struct {
	position    []float64 // Текущее положение
	velocity    []float64 // Текущая скорость
	bestPos     []float64 // Лучшая позиция частицы
	bestFitness float64   // Лучшее значение функции для частицы
}

// Создание новой частицы
func newParticle(dimensions int) Particle {
	position := make([]float64, dimensions)
	velocity := make([]float64, dimensions)
	bestPos := make([]float64, dimensions)

	for i := 0; i < dimensions; i++ {
		position[i] = rand.Float64()*10 - 5 // Генерация начального положения в пределах [-5, 5]
		velocity[i] = rand.Float64()*2 - 1  // Генерация начальной скорости в пределах [-1, 1]
		bestPos[i] = position[i]
	}

	bestFitness := rosenbrock(position)

	return Particle{
		position:    position,
		velocity:    velocity,
		bestPos:     bestPos,
		bestFitness: bestFitness,
	}
}

func main() {
	rand.Seed(time.Now().UnixNano())

	// Инициализация частиц
	particles := make([]Particle, numParticles)
	for i := range particles {
		particles[i] = newParticle(numDimensions)
	}

	// Глобальные лучшие позиция и значение функции
	globalBestPos := make([]float64, numDimensions)
	globalBestFitness := math.Inf(1)

	// Поиск глобального минимума
	for iteration := 0; iteration < maxIterations; iteration++ {
		for i := range particles {
			fitness := rosenbrock(particles[i].position)

			// Обновление личного лучшего
			if fitness < particles[i].bestFitness {
				particles[i].bestFitness = fitness
				copy(particles[i].bestPos, particles[i].position)
			}

			// Обновление глобального лучшего
			if fitness < globalBestFitness {
				globalBestFitness = fitness
				copy(globalBestPos, particles[i].position)
			}
		}

		// Обновление скоростей и положений
		for i := range particles {
			for d := 0; d < numDimensions; d++ {
				r1 := rand.Float64()
				r2 := rand.Float64()

				cognitive := cognitiveCoeff * r1 * (particles[i].bestPos[d] - particles[i].position[d])
				social := socialCoeff * r2 * (globalBestPos[d] - particles[i].position[d])

				particles[i].velocity[d] = inertiaWeight*particles[i].velocity[d] + cognitive + social
				particles[i].position[d] += particles[i].velocity[d]
			}
		}

		// Вывод результатов текущей итерации на русском
		fmt.Printf("Итерация %d: Лучшее значение функции = %.6f в позиции %v\n",
			iteration, globalBestFitness, globalBestPos)
	}

	// Итоговый вывод на русском
	fmt.Println("Оптимизация завершена")
	fmt.Printf("Лучшая позиция: %v\n", globalBestPos)
	fmt.Printf("Лучшее значение функции: %.6f\n", globalBestFitness)
}
