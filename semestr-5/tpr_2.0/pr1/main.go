package main

import (
	"bufio"
	"fmt"
	"os"
	"strings"
)

// Класс "Курсы"
type Course struct {
	Title       string
	Description string
	Languages   []Program_Languages // Привязка: курс содержит программные языки
	Format      []string            // очный, заочный
	Difficulty  string
	Duration    string
	Lesson_plan []Lesson
	Teaches     []Teacher
}

type Program_Languages struct {
	Title                string
	Scope_of_application string // область применения
	Type                 string
}

// Класс "Занятия"
type Lesson struct {
	Title     string
	Materials string
	Practical []Practical_exercises
	Test      []Test
}

type Practical_exercises struct {
	Conditions   string
	Evaluation   int      //оценка
	Requirements []string //требования
}

type Test struct {
	Question       string
	Answer_options []string //варианты ответы
	Evaluation     int
	Right_answer   string // правильный ответ
}

// Класс "Учащиеся"
type Student struct {
	Name          string
	Email         string
	Takes_courses []Course // Привязка: учащийся зарегистрирован на несколько курсов
}

// Класс "Преподаватели"
type Teacher struct {
	Name   string
	Status string
	Area   []string //область экспертности
}

// Класс "Форум"
type Forum struct {
	Title    string
	Question string
	UsersSt  []Student
	UsersT   []Teacher
	Answers  []string
}

// Класс "Инструменты/Платформы"
type Tool struct {
	Name        string
	Integration []Course
}

func main() {
	plang1 := Program_Languages{
		Title:                "GoLang",
		Scope_of_application: "микросервисы",
		Type:                 "скомпилированный",
	}
	test1 := Test{
		Question: "Как вывести значение переменной a = 101? Go",
		Answer_options: []string{
			"fmt.Println(a)",
			"fmt.Println('a')",
			"print(a)",
			"fPrint(a)",
		},
		Evaluation:   2,
		Right_answer: "fmt.Println(a)",
	}
	pr1 := Practical_exercises{
		Conditions: "Вывести в консоль числа от 1 до 10",
		Evaluation: 1,
		Requirements: []string{
			"Использовать цикл for",
			"Использовать только 1 цикл",
		},
	}
	pr2 := Practical_exercises{
		Conditions: "Вывести в консоль числа от 1 до 10, потом вывести его в одну строчку в обратном порядке",
		Evaluation: 3,
		Requirements: []string{
			"Использовать цикл for",
			"Использовать только 1 цикл",
			"Использовать sort",
		},
	}

	lesson1 := Lesson{
		Title:     "Введение",
		Materials: "Лекция",
		Practical: []Practical_exercises{pr1},
		Test:      []Test{test1},
	}

	lesson2 := Lesson{
		Title:     "Основы GO",
		Materials: "Лекция",
		Practical: []Practical_exercises{pr2},
		Test: []Test{ // добавим тест
			{
				Question: "Как объявить переменную в Go?",
				Answer_options: []string{
					"var a int",
					"int a",
					"a := int",
					"let a = int",
				},
				Evaluation:   2,
				Right_answer: "var a int",
			},
		},
	}

	teacher1 := Teacher{
		"Генадий",
		"активный",
		[]string{"golang"},
	}
	course1 := Course{
		Title:       "GoLang база",
		Description: "Обучение основам языка программирования GoLang",
		Languages:   []Program_Languages{plang1},
		Format:      []string{"очно", "заочно"},
		Difficulty:  "лёгкая",
		Duration:    "1 месяц",
		Lesson_plan: []Lesson{lesson1, lesson2},
		Teaches:     []Teacher{teacher1},
	}
	student1 := Student{
		"Антон",
		"aaaaa@dd.dd",
		[]Course{course1},
	}
	student2 := Student{
		Name:          "Толик",
		Email:         "ttttt@dd.dd",
		Takes_courses: []Course{course1},
	}
	courses := []Course{course1}
	students := []Student{student1, student2}

	fmt.Println("Что вы хотите найти?\n 1.Курс по названию \n 2.Ученика по имени \n 3.Курс по языку программирования")
	var otv int
	fmt.Scan(&otv)
	fmt.Scanln()
	reader := bufio.NewReader(os.Stdin)
	switch otv {
	case 1:
		courseTitle := ""
		fmt.Println("Введите название курса:")

		// Используем bufio.NewReader для считывания всей строки
		courseTitle, _ = reader.ReadString('\n')

		// Убираем символ новой строки
		courseTitle = strings.TrimSpace(courseTitle)

		foundCourse := FindCourseByTitle(courses, courseTitle)
		if foundCourse != nil {
			PrintCourseHierarchy(foundCourse)
		} else {
			fmt.Println("Курс с названием", courseTitle, "не найден.")
		}

	case 2:
		studentName := ""
		fmt.Println("Введите имя студента:")

		studentName, _ = reader.ReadString('\n')
		studentName = strings.TrimSpace(studentName)

		foundStudent := FindStudentByName(students, studentName)
		if foundStudent != nil {
			PrintStudentHierarchy(foundStudent)
		} else {
			fmt.Println("Студент с именем", studentName, "не найден.")
		}
	case 3:
		language := ""
		fmt.Println("Введите язык программирования:")

		language, _ = reader.ReadString('\n')
		language = strings.TrimSpace(language)

		foundCourses := FindCoursesByLanguage(courses, language)
		if len(foundCourses) > 0 {
			fmt.Println("Курсы, в которых используется язык", language, ":")
			for _, course := range foundCourses {
				PrintCourseHierarchy(&course)
			}
		} else {
			fmt.Println("Курсы с использованием языка", language, "не найдены.")
		}
	default:
		fmt.Println("Ошибка!")
	}
}

func FindCoursesByLanguage(courses []Course, language string) []Course {
	var result []Course
	for _, course := range courses {
		for _, lang := range course.Languages {
			if lang.Title == language {
				result = append(result, course)
			}
		}
	}
	return result
}

func FindStudentByName(students []Student, name string) *Student {
	for _, student := range students {
		if student.Name == name {
			return &student
		}
	}
	return nil
}

// Функция для вывода иерархии студента и его курсов
func PrintStudentHierarchy(student *Student) {
	fmt.Println("Имя студента:", student.Name)
	fmt.Println("Email студента:", student.Email)
	// fmt.Println("Курсы, которые проходит студент:")
	// for _, course := range student.Takes_courses {
	// 	PrintCourseHierarchy(&course)
	// }
}

func FindCourseByTitle(courses []Course, title string) *Course {
	for _, course := range courses {
		if course.Title == title {
			return &course
		}
	}
	return nil
}

// Функция для вывода полной иерархии курса
func PrintCourseHierarchy(course *Course) {
	fmt.Println("Название курса:", course.Title)
	fmt.Println("Описание:", course.Description)
	fmt.Println("Языки программирования:")
	for _, lang := range course.Languages {
		fmt.Printf("- %s (%s, %s)\n", lang.Title, lang.Scope_of_application, lang.Type)
	}
	fmt.Println("Преподователи:")
	for _, teacher := range course.Teaches {
		fmt.Println(" ", teacher.Name)
	}
	fmt.Println("Формат обучения:", course.Format)
	fmt.Println("Сложность:", course.Difficulty)
	fmt.Println("Длительность:", course.Duration)
	fmt.Println("План уроков:")
	for _, lesson := range course.Lesson_plan {
		fmt.Println("  Урок:", lesson.Title)
		fmt.Println("  Материалы:", lesson.Materials)
		fmt.Println("  Практические задания:")
		for _, pr := range lesson.Practical {
			fmt.Printf("    - Условие: %s, Оценка: %d\n", pr.Conditions, pr.Evaluation)
		}
		fmt.Println("  Тесты:")
		for _, test := range lesson.Test {
			fmt.Printf("Вопрос: %s\n", test.Question)
			fmt.Println("Варианты ответов:")
			for i, answer := range test.Answer_options {
				fmt.Println("   ", i+1, ": ", answer)
			}
			fmt.Printf("  Правильный ответ: %s\n", test.Right_answer)
		}
	}
}
