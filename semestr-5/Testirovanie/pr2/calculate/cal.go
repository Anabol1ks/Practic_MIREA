package main

import (
	"errors"
	"fmt"
)

// Add - функция сложения двух чисел
func Add(a, b float64) float64 {
	return a + b
}

// Subtract - функция вычитания
func Subtract(a, b float64) float64 {
	return a - b
}

// Multiply - функция умножения
func Multiply(a, b float64) float64 {
	return a * b
}

// Divide - функция деления (с ошибкой)
func Divide(a, b float64) (float64, error) {
	if b == 0 {
		return 0, errors.New("divide by zero")
	}

	return a / b, nil // специальная ошибка
}

// Mod - функция нахождения модуля числа
func Mod(a float64) float64 {
	if a < 0 {
		return -a
	}
	return a
}

func main() {
	var a, b float64
	var operation string

	// Ввод чисел
	fmt.Print("Введите первое число: ")
	fmt.Scanln(&a)
	fmt.Print("Введите второе число: ")
	fmt.Scanln(&b)

	// Ввод операции
	fmt.Print("Выберите операцию (+, -, *, /, mod): ")
	fmt.Scanln(&operation)

	// Выполнение операции
	switch operation {
	case "+":
		fmt.Println("Результат сложения:", Add(a, b))
	case "-":
		fmt.Println("Результат вычитания:", Subtract(a, b))
	case "*":
		fmt.Println("Результат умножения:", Multiply(a, b))
	case "/":
		if result, err := Divide(a, b); err != nil {
			fmt.Println("Ошибка:", err)
		} else {
			fmt.Println("Результат деления:", result)
		}
	case "mod":
		fmt.Println("Модуль числа:", Mod(a))
	default:
		fmt.Println("Неверная операция")
	}
}
