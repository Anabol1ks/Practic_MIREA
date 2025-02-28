package main

import (
	"fmt"
	"math"
	"math/rand"
	"time"
)

// Координаты городов
type City struct {
	name string
	x, y float64
}

// Расстояние между двумя городами
func distance(a, b City) float64 {
	return math.Sqrt(math.Pow(a.x-b.x, 2) + math.Pow(a.y-b.y, 2))
}

// Подсчет общего расстояния по маршруту
func totalDistance(route []City) float64 {
	total := 0.0
	for i := 0; i < len(route)-1; i++ {
		total += distance(route[i], route[i+1])
	}
	total += distance(route[len(route)-1], route[0]) // замыкаем маршрут
	return total
}

// Перемешивание для получения нового маршрута
func swap(route []City) []City {
	newRoute := make([]City, len(route))
	copy(newRoute, route)

	i, j := rand.Intn(len(route)), rand.Intn(len(route))
	for i == j {
		j = rand.Intn(len(route))
	}

	newRoute[i], newRoute[j] = newRoute[j], newRoute[i]
	return newRoute
}

// Имитация отжига
func simulatedAnnealing(cities []City, initialTemp, coolingRate float64, maxIter int) []City {
	rand.Seed(time.Now().UnixNano())
	currentRoute := cities
	bestRoute := cities
	currentTemp := initialTemp

	for i := 0; i < maxIter; i++ {
		newRoute := swap(currentRoute)
		currentDistance := totalDistance(currentRoute)
		newDistance := totalDistance(newRoute)

		// Проверка, принимаем ли новый маршрут
		if newDistance < currentDistance || math.Exp((currentDistance-newDistance)/currentTemp) > rand.Float64() {
			currentRoute = newRoute
			if newDistance < totalDistance(bestRoute) {
				bestRoute = newRoute
			}
		}

		currentTemp *= coolingRate

		fmt.Printf("Итерация %d: Текущее расстояние %f, Лучшее расстояние %f, Температура %v\n", i+1, currentDistance, totalDistance(bestRoute), currentTemp)
	}

	return bestRoute
}

func main() {
	// Список городов с координатами (широта, долгота)
	cities := []City{
		{"Лондон", 51.5074, -0.1278},
		{"Париж", 48.8566, 2.3522},
		{"Рим", 41.9028, 12.4964},
		{"Нью-Йорк", 40.7128, -74.0060},
		{"Токио", 35.6895, 139.6917},
		{"Москва", 55.7558, 37.6173},
		{"Пекин", 39.9042, 116.4074},
		{"Сидней", -33.8688, 151.2093},
	}

	initialTemp := 100.0
	coolingRate := 0.99
	maxIter := 100

	bestRoute := simulatedAnnealing(cities, initialTemp, coolingRate, maxIter)
	fmt.Println("Лучший найденный маршрут:")
	for _, city := range bestRoute {
		fmt.Printf("%s (%.2f, %.2f)\n", city.name, city.x, city.y)
	}
	fmt.Printf("Общее расстояние: %f\n", totalDistance(bestRoute))
}
