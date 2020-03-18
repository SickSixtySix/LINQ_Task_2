using System;
using System.Linq;

namespace ConsoleApp3
{
    class Program
    {
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
                    if (char.IsDigit(y[0]))
                        operands[operand] += y;
                    else if (y == "*" || y == "/" || y == "+" || y == "-" || y == ")")
                    {
                        // Выполнение последней инициированной операции
                        if (operation != null)
                        {
                            // Вычисление результата операции
                            var operand1 = Convert.ToInt32(operands[0]);
                            var operand2 = Convert.ToInt32(operands[1]);
                            var result = operand1;
                            switch (operation)
                            {
                                case "*": result *= operand2; break;
                                case "/": result /= operand2; break;
                                case "+": result += operand2; break;
                                case "-": result -= operand2; break;
                            }

                            // Обновление обработанного выражения
                            x = x.Replace($"{operand1}{operation}{operand2}", result.ToString());

                            // Обновление операндов (результат вычисления данной операции стал операндом для другой операции)
                            operands[0] = result.ToString();
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
                    .Select(x => parse(x, new string[] { "*", "/" }))   // Высоко-приоритетные операции
                    .Select(x => parse(x, new string[] { "+", "-" }))   // Низко-приоритетные операции
                    .ToArray();

                // Вывод результатов
                for (int i = 0; i <= expressions.Length; i++)
                    if (i == expressions.Length)
                        Console.WriteLine("none");
                    else if ($"({expectation})" == expressions[i])
                    {
                        Console.WriteLine($"index {i}");
                        break;
                    }
            }
        }
    }
}
