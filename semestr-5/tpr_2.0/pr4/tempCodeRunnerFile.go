func AntColony(grid *Grid) []Point {
	bestPath := []Point{}
	bestLength := GridSize * GridSize
	rand.Seed(time.Now().UnixNano())

	for iter := 0; iter < MaxIters; iter++ {
		paths := [][]Point{}
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

				// Вывод всех путей при обновлении лучшего результата
				fmt.Printf("Итерация %d: найден новый лучший путь (%d шагов)\n", iter+1, bestLength)
				for i, p := range paths {
					fmt.Printf("Муравей %d: %v\n", i+1, p)
				}
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
		fmt.Printf("Итерация %d: лучший путь = %d шагов\n", iter+1, bestLength)
	}

	return bestPath
}
