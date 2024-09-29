package main

import (
	"bufio"
	"fmt"
	"os"
	"unicode"
)

type Str string

func main() {
	in := bufio.NewReader(os.Stdin)
	var str Str
	fmt.Print("Введите строку: ")
	fmt.Fscan(in, &str)
	fmt.Print("1-Разворот регистра\n2-Обратная строка \nВыберите дальнейшее действие: ")
	var d int
	fmt.Fscan(in, &d)
	switch d {
	case 1:
		fmt.Print("Разворот регистра: ")
		str.Flip()
	case 2:
		fmt.Print("Обратная строка: ")
		str.Reverse()
	}

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
