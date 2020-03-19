using System;
using System.Globalization;
using System.Linq;

namespace ConsoleApp3
{
    static class Program
    {
        // Полезные методы расширения
        public static string Process(this string number, bool pre = false) =>
            number.Replace(pre ? "-" : ":", pre ? ":" : "-");

        public static decimal Decimate(this string number) =>
            decimal.Parse(number.Process(), new NumberFormatInfo() { NumberDecimalSeparator = "." });

        public static string Stringify(this decimal number) =>
            number.ToString("G29", new NumberFormatInfo() { NumberDecimalSeparator = "." }).Process(false);

        // Делегат разбора выражения
        private static readonly Func<string, string[], string> parse = (expression, priority) =>
        {
            string[] operands = new string[2];  // Последние два операнда (актуальные операнды)
            string operation = null;            // Последняя инициированная операция
            int operand = 0;                    // Индекс текущего операнда

            return expression
                // Преобразование строки выражения в последовательность строк единичной длины
                .ToCharArray()
                .Select(
                    x => x.ToString()
                )
                // Поэлементная агрегация выражения
                .Aggregate(string.Empty, (x, y) =>
                {
                    // Считывание текущего операнда
                    if (char.IsDigit(y[0]) || y == "." || y == ":")
                        operands[operand] += y;
                    else if (y == "*" || y == "/" || y == "+" || y == "-" || y == ")")
                    {
                        // Выполнение последней инициированной операции
                        if (operation != null)
                        {
                            // Вычисление результата операции
                            var operand1 = operands[0].Decimate();
                            var operand2 = operands[1].Decimate();
                            var result = operand1;
                            switch (operation)
                            {
                                case "*": result *= operand2; break;
                                case "/": result /= operand2; break;
                                case "+": result += operand2; break;
                                case "-": result -= operand2; break;
                            }
                            var substitution = result.Stringify();

                            // Обновление обработанного выражения
                            x = x.Replace($"{operands[0]}{operation}{operands[1]}", substitution);

                            // Обновление операндов (результат вычисления данной операции стал операндом для другой операции)
                            operands[0] = substitution;
                            operands[1] = string.Empty;
                            operand = 0;

                            // Помечаем операцию как выполненную
                            operation = null;
                        }

                        // Инициирование операции
                        if (priority.Contains(y))
                            operation = y;

                        // Удаление неакутальных операндов
                        if (++operand > 1 && y != ")")
                        {
                            operands[0] = operands[1];
                            operands[1] = string.Empty;
                            operand = 1;
                        }
                    }

                    return x + y;
                });
        };

        static void Main(string[] args)
        {
            while (true)
            {
                // Считывание данных и разбор выражений
                var expectation = Console.ReadLine();
                var expressions = Console.ReadLine()
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => parse(x, new string[] { "*", "/" }))       // Высокоприоритетные операции
                    .Select(x => parse(x, new string[] { "+", "-" }))       // Низкоприоритетные операции
                    .Select(x => x.Trim('(', ')').Decimate().Stringify())   // Извлечение результата
                    .ToArray();

                // Вывод результатов
                for (int i = 0; i <= expressions.Length; i++)
                    if (i == expressions.Length)
                        Console.WriteLine("none");
                    else if (expectation == expressions[i])
                    {
                        Console.WriteLine($"index {i}");
                        break;
                    }
            }
        }
    }
}
