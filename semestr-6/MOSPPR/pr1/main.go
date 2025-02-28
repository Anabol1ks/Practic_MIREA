package main

import (
	"fmt"
	"math"
)

// Функция, которую требуется минимизировать:
// f(x, y) = x^2 + x*y + y^2 + 2*x + 3*y
func f(x, y float64) float64 {
	return x*x + x*y + y*y + 2*x + 3*y
}

// exploratorySearch осуществляет эксплоративный поиск вокруг точки (x, y)
// с фиксированным шагом step. Функция поочерёдно проверяет улучшения вдоль осей.
func exploratorySearch(x, y, step float64) (float64, float64) {
	bestX, bestY := x, y
	bestVal := f(x, y)

	// Поиск по оси X: сначала в положительном направлении
	trialX := bestX + step
	trialVal := f(trialX, bestY)
	if trialVal < bestVal {
		bestX = trialX
		bestVal = trialVal
	} else {
		// Если не улучшилось, пробуем в отрицательном направлении
		trialX = bestX - step
		trialVal = f(trialX, bestY)
		if trialVal < bestVal {
			bestX = trialX
			bestVal = trialVal
		}
	}

	// Поиск по оси Y: сначала в положительном направлении
	trialY := bestY + step
	trialVal = f(bestX, trialY)
	if trialVal < bestVal {
		bestY = trialY
		bestVal = trialVal
	} else {
		// Если не улучшилось, пробуем в отрицательном направлении
		trialY = bestY - step
		trialVal = f(bestX, trialY)
		if trialVal < bestVal {
			bestY = trialY
			bestVal = trialVal
		}
	}

	return bestX, bestY
}

// hookeJeeves реализует метод Хука-Дживса для минимизации функции f.
// initialX, initialY — начальная точка; step — начальный размер шага;
// tol — допустимая точность; maxIter — максимальное число итераций.
func hookeJeeves(initialX, initialY, step, tol float64, maxIter int) (float64, float64, float64) {
	baseX, baseY := initialX, initialY
	newX, newY := initialX, initialY
	iter := 0

	for step > tol && iter < maxIter {
		fmt.Printf("Итерация %d:\n", iter)
		fmt.Printf("  Базовая точка: (%.6f, %.6f)  f = %.6f\n", baseX, baseY, f(baseX, baseY))
		fmt.Printf("  Текущая точка: (%.6f, %.6f)  f = %.6f\n", newX, newY, f(newX, newY))
		fmt.Printf("  Размер шага: %.6f\n", step)

		// Эксплоративный поиск вокруг текущей точки
		xExpl, yExpl := exploratorySearch(newX, newY, step)
		fmt.Printf("  Предварительный результат поиска: (%.6f, %.6f)  f = %.6f\n", xExpl, yExpl, f(xExpl, yExpl))

		// Если улучшение отсутствует, уменьшаем шаг
		if f(xExpl, yExpl) >= f(newX, newY) {
			fmt.Println("  Улучшений не обнаружено. Шаг сокращения.")
			step = step / 2.0
		} else {
			// Паттерн-движение: пытаемся ускорить поиск
			patternX := xExpl + (xExpl - baseX)
			patternY := yExpl + (yExpl - baseY)
			fmt.Printf("  Шаблон перемещения кандидата: (%.6f, %.6f)\n", patternX, patternY)
			newPatternX, newPatternY := exploratorySearch(patternX, patternY, step)
			fmt.Printf("  Результат после поискового перемещения по шаблону: (%.6f, %.6f)  f = %.6f\n", newPatternX, newPatternY, f(newPatternX, newPatternY))
			// Обновляем базовую точку и текущую точку
			baseX, baseY = xExpl, yExpl
			newX, newY = newPatternX, newPatternY
		}
		fmt.Println("-----------------------------------------------------")
		iter++
	}
	return newX, newY, f(newX, newY)
}

func main() {
	// Задаём начальную точку (одинаковую для всех работ)
	initialX := 3.0
	initialY := -2.0

	// Параметры алгоритма: начальный шаг, точность и максимальное число итераций
	step := 1.0
	tol := 1e-6
	maxIter := 10000

	// Запускаем метод Хука-Дживса
	bestX, bestY, bestVal := hookeJeeves(initialX, initialY, step, tol, maxIter)
	fmt.Printf("Найденный минимум методом Хука-Дживса:\n")
	fmt.Printf("  x = %.6f, y = %.6f, f(x,y) = %.6f\n", bestX, bestY, bestVal)

	// Ручной (аналитический) расчёт для проверки:
	analyticX := -1.0 / 3.0
	analyticY := -4.0 / 3.0
	analyticVal := f(analyticX, analyticY)
	fmt.Printf("\nАналитически найденный минимум:\n")
	fmt.Printf("  x = %.6f, y = %.6f, f(x,y) = %.6f\n", analyticX, analyticY, analyticVal)

	// Вывод разницы между полученными значениями
	diffX := math.Abs(bestX - analyticX)
	diffY := math.Abs(bestY - analyticY)
	diffVal := math.Abs(bestVal - analyticVal)
	fmt.Printf("\nРазница:\n  Δx = %.6f, Δy = %.6f, Δf = %.6f\n", diffX, diffY, diffVal)
}
