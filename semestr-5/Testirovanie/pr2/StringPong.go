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
		fmt.Print("1-Разворот регистра\n2-Обратная строка \n3-Разделить строку \n4-Удалить буквы \n5-Удалить числа \n6-Выход \n0-Обновить строку \nВыберите дальнейшее действие: ")
		var d int
		fmt.Fscan(in, &d)
		switch d {
		case 1:
			fmt.Print("Разворот регистра: ")
			str.Flip()
		case 2:
			fmt.Print("Обратная строка: ")
			str.Reverse()
		case 3:
			var n int
			fmt.Print("Введите количество символов для разделения: ")
			fmt.Fscan(in, &n)
			str.Del(n)
		case 4:
			fmt.Print("Удаление букв: ")
			str.OnlyDigit()
		case 5:
			fmt.Print("Удаление букв: ")
			str.OnlySim()
		case 6:
			fmt.Println("Завершение...")
			time.Sleep(3 * time.Second)
			os.Exit(0)
		case 0:
			fmt.Print("Введите строку: ")
			fmt.Fscan(in, &str)
		default:
			fmt.Print("Нет такой команды")
		}
		time.Sleep(1 * time.Second)
	}
}

func (s Str) OnlyDigit() {
	var result []rune
	for _, r := range s {
		if unicode.IsDigit(r) {
			result = append(result, r)
			result = append(result, r) // Это для ошибки (выводит числа два раза)
		}
	}
	fmt.Println(string(result))
}

func (s Str) OnlySim() {
	var result []rune
	for _, r := range s {
		if !unicode.IsDigit(r) {
			result = append(result, r)
		}
	}
	fmt.Println(string(result))
}

func (s Str) Del(n int) {
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
	fmt.Println(res)
}

func (s Str) Reverse() {
	res := ""
	for _, ch := range s {
		res = string(ch) + res
	}
	fmt.Println(res)
}

func (s Str) Flip() {
	runes := []rune(s)
	for i, r := range runes {
		if unicode.IsLower(r) {
			runes[i] = unicode.ToUpper(r)
		} else if unicode.IsUpper(r) {
			runes[i] = unicode.ToLower(r)
		}
	}
	fmt.Println(string(runes))
}
