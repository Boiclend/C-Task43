// В текстовом файле есть ведомость результатов сдачи экзаменов студенческой группы. Ведомость  содержит для каждого студента фамилию,
// имя отчество и оценки по пяти предметам. Студентов в группе не более 20 человек.
// Написать программу, которая предоставляет следующую информацию:

// список студентов (ФИО);
// список студентов, которые сдали все экзамены только на 5;
// список студентов, которые имеют хотя-бы одну тройку по экзаменам;
// список студентов, у которых есть двойки. Если студент, имеет более чем одну двойку, он исключается из списка.
// Добавил возможность добавления нового студента

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program
{
    public class Student
    {
        public string FullName { get; set; }
        public List<int> Grades { get; set; }
    }

    static void Main()
    {
        Console.WriteLine("Введите путь к файлу с данными студентов:");
        string filePath = Console.ReadLine(); // Ввод пути к файлу с консоли

        if (!File.Exists(filePath))
        {
            Console.WriteLine("Файл не найден. Пожалуйста, проверьте путь и попробуйте снова.");
            return;
        }

        List<Student> students;
        try
        {
            students = ReadStudentsFromFile(filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
            return;
        }

        RemoveStudentsWithMoreThanOneTwo(students, filePath);

        while (true)
        {
            Console.WriteLine("Выберите действие: ");
            Console.WriteLine("1. Показать всех студентов");
            Console.WriteLine("2. Показать студентов с оценками только 5");
            Console.WriteLine("3. Показать студентов с хотя бы одной тройкой");
            Console.WriteLine("4. Показать студентов с ровно одной двойкой");
            Console.WriteLine("5. Добавить нового студента");
            Console.WriteLine("6. Выйти");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ListAllStudents(students);
                    break;
                case "2":
                    ListStudentsWithAllFives(students);
                    break;
                case "3":
                    ListStudentsWithAtLeastOneThree(students);
                    break;
                case "4":
                    ListStudentsWithOneTwo(students);
                    break;
                case "5":
                    AddNewStudent(students, filePath);
                    break;
                case "6":
                    return;
                default:
                    Console.WriteLine("Неверный выбор, попробуйте снова.");
                    break;
            }
        }
    }

    static List<Student> ReadStudentsFromFile(string filePath)
    {
        var students = new List<Student>();

        foreach (var line in File.ReadLines(filePath))
        {
            var parts = line.Split();
            if (parts.Length != 8) continue; // ФИО + 5 оценок = 8 частей

            var student = new Student
            {
                FullName = $"{parts[0]} {parts[1]} {parts[2]}",
                Grades = parts.Skip(3).Select(int.Parse).ToList()
            };

            students.Add(student);
        }

        return students;
    }

    static void RemoveStudentsWithMoreThanOneTwo(List<Student> students, string filePath)
    {
        var studentsToRemove = students.Where(s => s.Grades.Count(g => g == 2) > 1).ToList();
        foreach (var student in studentsToRemove)
        {
            Console.WriteLine($"Студент {student.FullName} имеет более одной двойки и он исключен.");
            students.Remove(student);
        }

        UpdateFile(filePath, students);
    }

    static void UpdateFile(string filePath, List<Student> students)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (var student in students)
            {
                var grades = string.Join(" ", student.Grades);
                writer.WriteLine($"{student.FullName} {grades}");
            }
        }
    }

    static void ListAllStudents(List<Student> students)
    {
        Console.WriteLine("Список студентов:");
        foreach (var student in students)
        {
            Console.WriteLine(student.FullName);
        }
        Console.WriteLine();
    }

    static void ListStudentsWithAllFives(List<Student> students)
    {
        var result = students.Where(s => s.Grades.All(g => g == 5)).ToList();

        Console.WriteLine("Студенты, которые сдали все экзамены только на 5:");
        foreach (var student in result)
        {
            Console.WriteLine(student.FullName);
        }
        Console.WriteLine();
    }

    static void ListStudentsWithAtLeastOneThree(List<Student> students)
    {
        var result = students.Where(s => s.Grades.Contains(3)).ToList();

        Console.WriteLine("Студенты, которые имеют хотя бы одну тройку по экзаменам:");
        foreach (var student in result)
        {
            Console.WriteLine(student.FullName);
        }
        Console.WriteLine();
    }

    static void ListStudentsWithOneTwo(List<Student> students)
    {
        var result = students
            .Where(s => s.Grades.Count(g => g == 2) == 1)
            .ToList();

        Console.WriteLine("Студенты, у которых есть ровно одна двойка:");
        foreach (var student in result)
        {
            Console.WriteLine(student.FullName);
        }
        Console.WriteLine();
    }

    static void AddNewStudent(List<Student> students, string filePath)
    {
        Console.WriteLine("Введите ФИО студента:");
        string fullName = Console.ReadLine();

        Console.WriteLine("Введите оценки по пяти предметам через пробел:");
        var gradesInput = Console.ReadLine();
        var grades = gradesInput.Split().Select(int.Parse).ToList();

        if (grades.Count != 5)
        {
            Console.WriteLine("Ошибка: Введите ровно пять оценок.");
            return;
        }

        var newStudent = new Student
        {
            FullName = fullName,
            Grades = grades
        };

        students.Add(newStudent);
        UpdateFile(filePath, students);
        Console.WriteLine("Студент успешно добавлен.");
    }
}
