package main

import (
	"fmt"
	"image/color"
	"log"
	"math"
	"math/rand"
	"time"

	"github.com/hajimehoshi/ebiten/v2"
	"github.com/hajimehoshi/ebiten/v2/ebitenutil"
)

const (
	screenWidth   = 800
	screenHeight  = 600
	numParticles  = 10  // Количество частиц в рое
	numNodes      = 5   // Количество вершин графа
	maxIterations = 100 // Максимальное количество итераций алгоритма
)

// Graph - структура для представления графа
type Graph struct {
	nodes  [][]float64  // Матрица расстояний между вершинами
	coords [][2]float64 // Координаты вершин для визуализации
}

// Particle - структура для представления частицы
type Particle struct {
	position []int     // Текущий маршрут
	velocity []float64 // Вектор скоростей
	bestPos  []int     // Лучшая найденная позиция
	bestFit  float64   // Значение фитнесс-функции для лучшей позиции
}

// Swarm - структура для представления роя
type Swarm struct {
	particles []*Particle // Частицы роя
	gBest     []int       // Глобально лучший маршрут
	gBestFit  float64     // Глобально лучшее значение фитнесс-функции
	graph     *Graph      // Граф, на котором работает алгоритм
}

// Создание графа с фиксированным количеством вершин и случайными расстояниями
func CreateGraph(numNodes int) *Graph {
	nodes := make([][]float64, numNodes)
	coords := make([][2]float64, numNodes)
	rand.Seed(time.Now().UnixNano())

	for i := 0; i < numNodes; i++ {
		nodes[i] = make([]float64, numNodes)
		coords[i] = [2]float64{
			rand.Float64() * screenWidth * 0.8,
			rand.Float64() * screenHeight * 0.8,
		}
		for j := 0; j < numNodes; j++ {
			if i != j {
				nodes[i][j] = rand.Float64() * 100
			} else {
				nodes[i][j] = 0
			}
		}
	}

	return &Graph{nodes: nodes, coords: coords}
}

// Функция фитнесс-функции, вычисляющая длину маршрута
func (g *Graph) Fitness(route []int) float64 {
	total := 0.0
	for i := 0; i < len(route)-1; i++ {
		total += g.nodes[route[i]][route[1+i]]
	}
	// Возврат в начальную точку
	total += g.nodes[route[len(route)-1]][route[0]]
	return total
}

// Создание случайного маршрута (перестановка вершин)
func Shuffle(numNodes int) []int {
	route := make([]int, numNodes)
	for i := 0; i < numNodes; i++ {
		route[i] = i
	}
	rand.Shuffle(len(route), func(i, j int) { route[i], route[j] = route[j], route[i] })
	return route
}

// Создание роя и инициализация частиц
func CreateSwarm(graph *Graph, numParticles int) *Swarm {
	swarm := &Swarm{
		particles: make([]*Particle, numParticles),
		gBestFit:  math.MaxFloat64,
		graph:     graph,
	}

	for i := 0; i < numParticles; i++ {
		route := Shuffle(len(graph.nodes))
		fitness := graph.Fitness(route)
		particle := &Particle{
			position: route,
			bestPos:  append([]int{}, route...),
			bestFit:  fitness,
			velocity: make([]float64, len(graph.nodes)),
		}
		swarm.particles[i] = particle
		if fitness < swarm.gBestFit {
			swarm.gBest = append([]int{}, route...)
			swarm.gBestFit = fitness
		}
	}

	return swarm
}

// Обновление состояния роя за одну итерацию
func (s *Swarm) Update() {
	for _, p := range s.particles {
		// Генерация нового случайного маршрута
		newRoute := Shuffle(len(s.graph.nodes))
		fitness := s.graph.Fitness(newRoute)
		if fitness < p.bestFit {
			p.bestFit = fitness
			p.bestPos = append([]int{}, newRoute...)
		}
		if fitness < s.gBestFit {
			s.gBestFit = fitness
			s.gBest = append([]int{}, newRoute...)
		}
		p.position = newRoute
	}

	// Вывод данных текущей итерации
	fmt.Printf("Лучшее значение: %.10f, маршрут: %v\n", s.gBestFit, s.gBest)
}

// VisualizationGame - структура для визуализации
type VisualizationGame struct {
	swarm   *Swarm
	counter int
}

func NewVisualizationGame(graph *Graph, swarm *Swarm) *VisualizationGame {
	return &VisualizationGame{
		swarm: swarm,
	}
}

// Update - обновление состояния визуализации
func (g *VisualizationGame) Update() error {
	if g.counter < maxIterations {
		g.swarm.Update()
		g.counter++
	}
	return nil
}

// Draw - отрисовка графа, частиц и маршрутов
func (g *VisualizationGame) Draw(screen *ebiten.Image) {
	// Отрисовка вершин графа
	for i, coord := range g.swarm.graph.coords {
		ebitenutil.DrawCircle(screen, coord[0], coord[1], 5, color.RGBA{255, 0, 0, 255})
		ebitenutil.DebugPrintAt(screen, fmt.Sprintf("%d", i), int(coord[0]), int(coord[1]))
	}

	// Отрисовка глобального лучшего маршрута (зелёный)
	for i := 0; i < len(g.swarm.gBest)-1; i++ {
		start := g.swarm.graph.coords[g.swarm.gBest[i]]
		end := g.swarm.graph.coords[g.swarm.gBest[i+1]]
		ebitenutil.DrawLine(screen, start[0], start[1], end[0], end[1], color.RGBA{0, 255, 0, 255})
	}
	// Возврат в начальную точку
	start := g.swarm.graph.coords[g.swarm.gBest[len(g.swarm.gBest)-1]]
	end := g.swarm.graph.coords[g.swarm.gBest[0]]
	ebitenutil.DrawLine(screen, start[0], start[1], end[0], end[1], color.RGBA{0, 255, 0, 255})

	// Вывод текстовой информации
	ebitenutil.DebugPrint(screen, fmt.Sprintf(
		"Iteration: %d/%d\nBest Route Length: %.2f\nBest Route: %v",
		g.counter, maxIterations, g.swarm.gBestFit, g.swarm.gBest))
}

// Layout - установка размеров окна
func (g *VisualizationGame) Layout(outsideWidth, outsideHeight int) (int, int) {
	return screenWidth, screenHeight
}

func main() {
	graph := CreateGraph(numNodes)
	swarm := CreateSwarm(graph, numParticles)

	game := NewVisualizationGame(graph, swarm)

	ebiten.SetWindowSize(screenWidth, screenHeight)
	ebiten.SetWindowTitle("Swarm Optimization Visualization")

	if err := ebiten.RunGame(game); err != nil {
		log.Fatal(err)
	}
}
