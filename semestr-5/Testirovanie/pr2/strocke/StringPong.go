package main

import (
	"bufio"
	"fmt"
	"os"
	"time"
	"unicode"
)

type Str string

func main() {
	in := bufio.NewReader(os.Stdin)
	var str Str
	fmt.Print("Введите строку: ")
	fmt.Fscan(in, &str)
	for {
		fmt.Print("1-Разворот регистра\n2-Обратная строка \n3-Разделить строку \n4-Удалить цифры \n5-Удалить буквы \n6-Выход \n0-Обновить строку \nВыберите дальнейшее действие: ")
		var d int
		fmt.Fscan(in, &d)

		switch d {
		case 1:
			fmt.Printf("Разворот регистра: %s\n", str.Flip())
		case 2:
			fmt.Printf("Обратная строка: %s\n", str.Reverse())
		case 3:
			var n int
			fmt.Print("Введите количество символов для разделения: ")
			fmt.Fscan(in, &n)
			fmt.Printf("Разделённая строка: \n%s\n", str.Del(n))
		case 4:
			fmt.Printf("Удаление цифр: %s\n", str.OnlySim())
		case 5:
			fmt.Printf("Удаление букв: %s\n", str.OnlyDigit())
		case 6:
			fmt.Println("Завершение...")
			time.Sleep(1 * time.Second)
			os.Exit(0)
		case 0:
			fmt.Print("Введите новую строку: ")
			fmt.Fscan(in, &str)
		default:
			fmt.Println("Нет такой команды.")
		}

		time.Sleep(1 * time.Second)
	}
}

func (s Str) OnlyDigit() string {
	var result []rune
	for _, r := range s {
		if unicode.IsDigit(r) {
			result = append(result, r)
		}
	}
	return string(result)
}

func (s Str) OnlySim() string {
	var result []rune
	for _, r := range s {
		if !unicode.IsDigit(r) {
			result = append(result, r)
		}
	}
	return string(result)
}

func (s Str) Del(n int) string {
	res := ""
	count := 0
	for _, ch := range s {
		if count == n {
			count = 0
			res += "\n"
		}
		res += string(ch)
		count++
	}
	return res
}

func (s Str) Reverse() string {
	res := ""
	for _, ch := range s {
		res = string(ch) + res
	}
	return res
}

func (s Str) Flip() string {
	runes := []rune(s)
	for i, r := range runes {
		if unicode.IsLower(r) {
			runes[i] = unicode.ToUpper(r)
		} else if unicode.IsUpper(r) {
			runes[i] = unicode.ToLower(r)
		}
	}
	return string(runes)
}
