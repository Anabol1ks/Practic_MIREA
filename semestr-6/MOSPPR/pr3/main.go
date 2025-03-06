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

// newtonRaphson реализует метод Ньютона–Рафсона для минимизации функции f.
// initialX, initialY — начальная точка; tol — допустимая точность;
// maxIter — максимальное число итераций.
func newtonRaphson(initialX, initialY, tol float64, maxIter int) (float64, float64, float64) {
	x, y := initialX, initialY
	iter := 0

	for iter < maxIter {
		// Вычисляем градиент:
		// ∂f/∂x = 2*x + y + 2,   ∂f/∂y = x + 2*y + 3
		gradX := 2*x + y + 2
		gradY := x + 2*y + 3
		normGrad := math.Sqrt(gradX*gradX + gradY*gradY)

		fmt.Printf("Итерация %d:\n", iter)
		fmt.Printf("  Текущая точка: (%.6f, %.6f), f = %.6f\n", x, y, f(x, y))
		fmt.Printf("  Градиент: (%.6f, %.6f), Норма = %.6f\n", gradX, gradY, normGrad)

		// Если норма градиента меньше tol, считаем, что достигнута сходимость
		if normGrad < tol {
			fmt.Println("  Достигнута требуемая точность. Выход из цикла.")
			break
		}

		// Для функции f матрица Гессе равна:
		// H = [ [2, 1],
		//       [1, 2] ]
		// Ее обратная матрица:
		// H⁻¹ = 1/3 * [ [2, -1],
		//               [-1, 2] ]
		//
		// Обновление по методу Ньютона–Рафсона:
		// [x_new, y_new]^T = [x, y]^T - H⁻¹ * grad
		//
		// Вычисляем компоненты шага:
		// dx = (1/3) * (2*gradX - gradY)
		// dy = (1/3) * (-gradX + 2*gradY)
		dx := (1.0 / 3.0) * (2*gradX - gradY)
		dy := (1.0 / 3.0) * (-gradX + 2*gradY)
		fmt.Printf("  Шаг Ньютона: (dx, dy) = (%.6f, %.6f)\n", dx, dy)

		// Обновляем точку
		x = x - dx
		y = y - dy
		fmt.Printf("  Обновленная точка: (%.6f, %.6f), f = %.6f\n", x, y, f(x, y))
		fmt.Println("-----------------------------------------------------")

		iter++
	}

	return x, y, f(x, y)
}

func main() {
	// Задаём начальную точку (одинаковую для всех работ)
	initialX := 3.0
	initialY := -2.0

	// Параметры: допустимая точность и максимальное число итераций
	tol := 1e-6
	maxIter := 10000

	// Запускаем метод Ньютона–Рафсона
	bestX, bestY, bestVal := newtonRaphson(initialX, initialY, tol, maxIter)
	fmt.Printf("Найденный минимум методом Ньютона–Рафсона:\n")
	fmt.Printf("  x = %.6f, y = %.6f, f(x,y) = %.6f\n", bestX, bestY, bestVal)

	// Ручной (аналитический) расчёт для проверки:
	// Решая систему ∇f(x)=0:
	// 2x + y + 2 = 0
	// x + 2y + 3 = 0
	// Получаем x = -1/3, y = -4/3, и f(-1/3,-4/3) = -7/3 ≈ -2.333333
	analyticX := -1.0 / 3.0
	analyticY := -4.0 / 3.0
	analyticVal := f(analyticX, analyticY)
	fmt.Printf("\nАналитически найденный минимум:\n")
	fmt.Printf("  x = %.6f, y = %.6f, f(x,y) = %.6f\n", analyticX, analyticY, analyticVal)
}
