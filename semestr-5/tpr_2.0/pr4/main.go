package main

import (
	"fmt"
	"math"
	"math/rand"
	"time"
)

// Размер сетки
const (
	GridSize   = 10
	NumAnts    = 10
	MaxIters   = 100
	EvapRate   = 0.5 // Коэффициент испарения
	Alpha      = 1.0 // Влияние феромона
	Beta       = 2.0 // Влияние эвристической информации (расстояния)
	InitPherom = 0.1 // Начальная концентрация феромона
)

// Координаты
type Point struct {
	X, Y int
}

// Сетка
type Grid struct {
	Cells      [][]float64 // Уровень феромонов на ячейках
	Obstacles  map[Point]bool
	Start, End Point
}

// Инициализация сетки
func NewGrid(start, end Point, obstacles []Point) *Grid {
	cells := make([][]float64, GridSize)
	for i := range cells {
		cells[i] = make([]float64, GridSize)
		for j := range cells[i] {
			cells[i][j] = InitPherom
		}
	}
	obstacleMap := make(map[Point]bool)
	for _, obs := range obstacles {
		obstacleMap[obs] = true
	}
	return &Grid{Cells: cells, Obstacles: obstacleMap, Start: start, End: end}
}

// Проверка допустимости перемещения
func (g *Grid) IsValid(point Point) bool {
	return point.X >= 0 && point.X < GridSize && point.Y >= 0 && point.Y < GridSize && !g.Obstacles[point]
}

// Генерация соседей
func (g *Grid) GetNeighbors(point Point) []Point {
	directions := []Point{{0, 1}, {1, 0}, {0, -1}, {-1, 0}}
	neighbors := []Point{}
	for _, dir := range directions {
		neighbor := Point{point.X + dir.X, point.Y + dir.Y}
		if g.IsValid(neighbor) {
			neighbors = append(neighbors, neighbor)
		}
	}
	return neighbors
}

// Вычисление вероятностей перехода
func (g *Grid) TransitionProbabilities(current, end Point, neighbors []Point) []float64 {
	probs := make([]float64, len(neighbors))
	total := 0.0
	for i, neighbor := range neighbors {
		pheromone := g.Cells[neighbor.X][neighbor.Y]
		distance := math.Sqrt(math.Pow(float64(end.X-neighbor.X), 2) + math.Pow(float64(end.Y-neighbor.Y), 2))
		// Защита от деления на 0
		if distance == 0 {
			distance = 1e-6
		}
		// Обновляем вероятность
		probs[i] = math.Pow(pheromone, Alpha) * math.Pow(1.0/distance, Beta)
		total += probs[i]
	}
	// Нормализация
	for i := range probs {
		probs[i] /= total
	}
	return probs
}

// Выбор следующей ячейки
func ChooseNext(neighbors []Point, probs []float64) Point {
	r := rand.Float64()
	cumProb := 0.0
	for i, prob := range probs {
		cumProb += prob
		if r <= cumProb {
			return neighbors[i]
		}
	}
	return neighbors[len(neighbors)-1]
}

// Муравьиный алгоритм
func AntColony(grid *Grid) []Point {
	bestPath := []Point{}
	bestLength := GridSize * GridSize
	rand.Seed(time.Now().UnixNano())

	for iter := 0; iter < MaxIters; iter++ {
		paths := [][]Point{}
		foundBetter := false

		for ant := 0; ant < NumAnts; ant++ {
			current := grid.Start
			path := []Point{current}
			visited := map[Point]bool{current: true}

			for current != grid.End {
				neighbors := grid.GetNeighbors(current)
				if len(neighbors) == 0 {
					break
				}
				probs := grid.TransitionProbabilities(current, grid.End, neighbors)
				next := ChooseNext(neighbors, probs)

				if visited[next] {
					break
				}
				visited[next] = true
				path = append(path, next)
				current = next
			}

			// Добавляем путь муравья в список всех путей
			paths = append(paths, path)

			// Если найден путь до цели, обновляем лучший результат
			if current == grid.End && len(path) < bestLength {
				bestLength = len(path)
				bestPath = path
				foundBetter = true
			}
		}

		// Обновление феромонов
		for i := range grid.Cells {
			for j := range grid.Cells[i] {
				grid.Cells[i][j] *= (1 - EvapRate) // Испарение
			}
		}
		for _, path := range paths {
			if len(path) == bestLength { // Только лучший путь усиливает феромоны
				for _, point := range path {
					grid.Cells[point.X][point.Y] += 1.0 / float64(len(path))
				}
			}
		}

		// Выводим пути, если найден новый лучший путь
		if foundBetter {
			fmt.Printf("\nИтерация %d: найден новый лучший путь (%d шагов)\n", iter+1, bestLength)
			for i, p := range paths {
				fmt.Printf("Муравей %d: %v\n", i+1, p)
			}
		}

		// Общий вывод для итерации
		fmt.Printf("Итерация %d: лучший путь = %d шагов\n", iter+1, bestLength)
	}

	return bestPath
}

// Визуализация пути
func PrintPath(grid *Grid, path []Point) {
	display := make([][]rune, GridSize)
	for i := range display {
		display[i] = make([]rune, GridSize)
		for j := range display[i] {
			if grid.Obstacles[Point{i, j}] {
				display[i][j] = '#'
			} else {
				display[i][j] = '.'
			}
		}
	}
	for _, p := range path {
		display[p.X][p.Y] = '*'
	}
	display[grid.Start.X][grid.Start.Y] = 'S'
	display[grid.End.X][grid.End.Y] = 'E'

	for _, row := range display {
		fmt.Println(string(row))
	}
}

func GenerateObstacles(numObstacles int) []Point {
	rand.Seed(time.Now().UnixNano())
	obstacles := []Point{}
	for i := 0; i < numObstacles; i++ {
		x := rand.Intn(GridSize)
		y := rand.Intn(GridSize)
		obstacle := Point{x, y}
		obstacles = append(obstacles, obstacle)
	}
	return obstacles
}

func main() {
	start := Point{9, 0}
	end := Point{0, 9}
	obstacles := GenerateObstacles(0)

	grid := NewGrid(start, end, obstacles)
	bestPath := AntColony(grid)
	fmt.Println("Лучший путь найден:")
	PrintPath(grid, bestPath)
}
