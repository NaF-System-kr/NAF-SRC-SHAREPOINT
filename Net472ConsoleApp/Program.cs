using System;

namespace Net472ConsoleApp
{
    internal static class Program
    {
        private static void Main()
        {
            Console.WriteLine("=== .NET Framework 4.7.2 Test Console ===");
            Console.WriteLine("값을 입력하면 테스트 함수를 수행합니다. 종료하려면 빈 줄을 입력하세요.\n");

            while (true)
            {
                Console.Write("입력값: ");
                var input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("프로그램을 종료합니다.");
                    break;
                }

                RunTest(input);
                Console.WriteLine();
            }
        }

        private static void RunTest(string value)
        {
            Console.WriteLine($"테스트 함수가 호출되었습니다. 입력값은 '{value}' 입니다.");

            if (int.TryParse(value, out var number))
            {
                var square = number * number;
                Console.WriteLine($"추가 테스트 결과: {number}의 제곱은 {square} 입니다.");
            }
            else
            {
                Console.WriteLine("입력값을 숫자로 변환할 수 없어 추가 계산을 수행하지 않았습니다.");
            }
        }
    }
}
