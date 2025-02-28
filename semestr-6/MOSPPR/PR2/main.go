package main

import (
	"fmt"
	"math"
)

// f вычисляет значение функции:
// f(x, y) = x^2 + x*y + y^2 + 2*x + 3*y
func f(x, y float64) float64 {
	return x*x + x*y + y*y + 2*x + 3*y
}

// steepestDescent реализует метод наискорейшего градиентного спуска.
// initialX, initialY — начальная точка; tol — допустимая точность;
// maxIter — максимальное число итераций.
func steepestDescent(initialX, initialY, tol float64, maxIter int) (float64, float64, float64) {
	x, y := initialX, initialY
	iter := 0

	for iter < maxIter {
		// Вычисляем градиент: grad = [∂f/∂x, ∂f/∂y]
		gradX := 2*x + y + 2 // ∂f/∂x = 2x + y + 2
		gradY := x + 2*y + 3 // ∂f/∂y = x + 2y + 3
		normGrad := math.Sqrt(gradX*gradX + gradY*gradY)

		fmt.Printf("Итерация %d:\n", iter)
		fmt.Printf("  Текущая точка: (%.6f, %.6f), f = %.6f\n", x, y, f(x, y))
		fmt.Printf("  Градиент: (%.6f, %.6f), Норма = %.6f\n", gradX, gradY, normGrad)

		// Если норма градиента меньше tol, считаем, что достигнута сходимость.
		if normGrad < tol {
			fmt.Println("  Достигнута требуемая точность. Выход из цикла.")
			break
		}

		// Для квадратной функции оптимальный шаг можно вычислить аналитически.
		// Оптимальный шаг по направлению антиградиента:
		// t = (||grad||^2) / (grad^T * H * grad)
		// Для H = [[2, 1], [1, 2]]:
		// grad^T * H * grad = 2*gradX^2 + 2*gradX*gradY + 2*gradY^2.
		gradNormSq := gradX*gradX + gradY*gradY
		denom := 2 * (gradX*gradX + gradX*gradY + gradY*gradY)
		t := gradNormSq / denom
		fmt.Printf("  Оптимальный шаг t = %.6f\n", t)

		// Обновляем точку по правилу: x_new = x - t * grad
		x = x - t*gradX
		y = y - t*gradY

		fmt.Println("-----------------------------------------------------")
		iter++
	}

	return x, y, f(x, y)
}

func main() {
	// Задаём начальную точку (одинаковую для всех работ)
	initialX := 3.0
	initialY := -2.0

	// Параметры алгоритма: допустимая точность и максимальное число итераций
	tol := 1e-6
	maxIter := 10000

	// Запускаем метод наискорейшего градиентного спуска
	bestX, bestY, bestVal := steepestDescent(initialX, initialY, tol, maxIter)
	fmt.Printf("Найденный минимум методом наискорейшего градиентного спуска:\n")
	fmt.Printf("  x = %.6f, y = %.6f, f(x,y) = %.6f\n", bestX, bestY, bestVal)

	// Ручной (аналитический) расчёт для проверки:
	analyticX := -1.0 / 3.0
	analyticY := -4.0 / 3.0
	analyticVal := f(analyticX, analyticY)
	fmt.Printf("\nАналитически найденный минимум:\n")
	fmt.Printf("  x = %.6f, y = %.6f, f(x,y) = %.6f\n", analyticX, analyticY, analyticVal)
}
