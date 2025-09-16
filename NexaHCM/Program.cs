using System;
using System.Collections.Generic;
using System.Xml.Linq;
using NexaHCM.Utils;

namespace NexaHCM
{
    internal static class Program
    {
        private const string SampleXml = @"<?xml version=\"1.0\" encoding=\"utf-8\"?>
<library xmlns=\"http://schemas.example.com/library\" xmlns:bk=\"http://schemas.example.com/book\">
  <libraryInfo location=\"Seoul\" established=\"1982\">
    <librarian>Jane Doe</librarian>
  </libraryInfo>
  <books>
    <bk:book id=\"1\">
      <title>XML 기초</title>
      <author>홍길동</author>
    </bk:book>
    <bk:book id=\"2\">
      <title>C# 마스터</title>
      <author>임꺽정</author>
    </bk:book>
  </books>
</library>";

        private static void Main()
        {
            Console.WriteLine("=== .NET Framework 4.7.2 Test Console ===");
            Console.WriteLine("실행할 모드를 선택하세요.\n");

            while (true)
            {
                Console.WriteLine("1. 기본 테스트 모드");
                Console.WriteLine("2. XML 유틸리티 테스트 모드");
                Console.WriteLine("Q. 종료");
                Console.Write("선택: ");
                var selection = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(selection) || selection.Equals("q", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("프로그램을 종료합니다.");
                    break;
                }

                Console.WriteLine();

                switch (selection)
                {
                    case "1":
                        RunBasicInputLoop();
                        break;
                    case "2":
                        RunXmlUtilityMode();
                        break;
                    default:
                        Console.WriteLine("알 수 없는 선택입니다. 다시 입력해주세요.\n");
                        break;
                }
            }
        }

        private static void RunBasicInputLoop()
        {
            Console.WriteLine("--- 기본 테스트 모드 ---");
            Console.WriteLine("값을 입력하면 테스트 함수를 수행합니다. 종료하려면 빈 줄을 입력하세요.\n");

            while (true)
            {
                Console.Write("입력값: ");
                var input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("기본 테스트 모드를 종료합니다.\n");
                    break;
                }

                RunBasicTest(input);
                Console.WriteLine();
            }
        }

        private static void RunBasicTest(string value)
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

        private static void RunXmlUtilityMode()
        {
            Console.WriteLine("--- XML 유틸리티 테스트 모드 ---");
            Console.WriteLine("샘플 XML 문서를 기반으로 XMLUtils 기능을 테스트할 수 있습니다.");
            Console.WriteLine("책(book) 요소는 'bk' 접두사를 사용해 접근할 수 있습니다.\n");

            var document = XMLUtils.LoadFromString(SampleXml);

            while (true)
            {
                Console.WriteLine("현재 XML 문서:");
                Console.WriteLine(XMLUtils.SaveToString(document, SaveOptions.None));
                Console.WriteLine();
                Console.WriteLine("실행할 작업을 선택하세요:");
                Console.WriteLine("1. 네임스페이스 확인");
                Console.WriteLine("2. XPath로 요소 값 조회");
                Console.WriteLine("3. XPath로 요소 값 설정");
                Console.WriteLine("4. 자식 요소 추가");
                Console.WriteLine("5. 요소 삭제");
                Console.WriteLine("6. XPath로 요소 개수 확인");
                Console.WriteLine("7. 요소를 Dictionary로 변환");
                Console.WriteLine("8. XML 문서 초기화");
                Console.WriteLine("0. 메인 메뉴로 돌아가기");
                Console.Write("선택: ");
                var input = Console.ReadLine();

                Console.WriteLine();

                switch (input)
                {
                    case "1":
                        ShowNamespaces(document);
                        break;
                    case "2":
                        QueryElementValue(document);
                        break;
                    case "3":
                        UpdateElementValue(document);
                        break;
                    case "4":
                        AddChildElement(document);
                        break;
                    case "5":
                        DeleteElement(document);
                        break;
                    case "6":
                        CountElements(document);
                        break;
                    case "7":
                        ConvertElementToDictionary(document);
                        break;
                    case "8":
                        document = XMLUtils.LoadFromString(SampleXml);
                        Console.WriteLine("XML 문서를 초기 상태로 되돌렸습니다.\n");
                        break;
                    case "0":
                        Console.WriteLine("XML 유틸리티 테스트 모드를 종료합니다.\n");
                        return;
                    default:
                        Console.WriteLine("알 수 없는 선택입니다. 다시 입력해주세요.\n");
                        break;
                }
            }
        }

        private static void ShowNamespaces(XDocument document)
        {
            var namespaces = XMLUtils.GetAllNamespaces(document);
            if (namespaces.Count == 0)
            {
                Console.WriteLine("등록된 네임스페이스가 없습니다.\n");
                return;
            }

            Console.WriteLine("네임스페이스 목록:");
            foreach (var ns in namespaces)
            {
                var prefix = string.IsNullOrEmpty(ns.Key) ? "(기본)" : ns.Key;
                Console.WriteLine($"- {prefix}: {ns.Value}");
            }

            Console.WriteLine();
        }

        private static void QueryElementValue(XDocument document)
        {
            Console.Write("조회할 XPath를 입력하세요: ");
            var xpath = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(xpath))
            {
                Console.WriteLine("XPath를 입력하지 않아 작업을 취소합니다.\n");
                return;
            }

            var value = XMLUtils.GetElementValue(document, xpath);
            if (value == null)
            {
                Console.WriteLine("해당 XPath에 일치하는 요소가 없습니다.\n");
            }
            else
            {
                Console.WriteLine($"요소 값: {value}\n");
            }
        }

        private static void UpdateElementValue(XDocument document)
        {
            Console.Write("값을 변경할 XPath를 입력하세요: ");
            var xpath = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(xpath))
            {
                Console.WriteLine("XPath를 입력하지 않아 작업을 취소합니다.\n");
                return;
            }

            Console.Write("새 값을 입력하세요: ");
            var value = Console.ReadLine() ?? string.Empty;

            var updated = XMLUtils.SetElementValue(document, xpath, value);
            Console.WriteLine(updated
                ? "요소 값을 변경했습니다.\n"
                : "요소를 찾을 수 없어 값을 변경하지 못했습니다.\n");
        }

        private static void AddChildElement(XDocument document)
        {
            Console.Write("부모 요소 XPath를 입력하세요: ");
            var parentXPath = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(parentXPath))
            {
                Console.WriteLine("XPath를 입력하지 않아 작업을 취소합니다.\n");
                return;
            }

            Console.Write("추가할 요소 이름을 입력하세요: ");
            var elementName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(elementName))
            {
                Console.WriteLine("요소 이름을 입력하지 않아 작업을 취소합니다.\n");
                return;
            }

            Console.Write("요소 값(선택)을 입력하세요: ");
            var value = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(value))
            {
                value = null;
            }

            var newElement = XMLUtils.AddChildElement(document, parentXPath, elementName, value);
            if (newElement == null)
            {
                Console.WriteLine("부모 요소를 찾지 못했습니다.\n");
            }
            else
            {
                Console.WriteLine($"새 요소 '{newElement.Name}' 을(를) 추가했습니다.\n");
            }
        }

        private static void DeleteElement(XDocument document)
        {
            Console.Write("삭제할 요소의 XPath를 입력하세요: ");
            var xpath = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(xpath))
            {
                Console.WriteLine("XPath를 입력하지 않아 작업을 취소합니다.\n");
                return;
            }

            var removed = XMLUtils.RemoveElements(document, xpath);
            Console.WriteLine(removed > 0
                ? $"{removed}개의 요소를 삭제했습니다.\n"
                : "삭제할 요소를 찾을 수 없습니다.\n");
        }

        private static void CountElements(XDocument document)
        {
            Console.Write("개수를 확인할 XPath를 입력하세요: ");
            var xpath = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(xpath))
            {
                Console.WriteLine("XPath를 입력하지 않아 작업을 취소합니다.\n");
                return;
            }

            var count = XMLUtils.CountElements(document, xpath);
            Console.WriteLine($"요소 개수: {count}\n");
        }

        private static void ConvertElementToDictionary(XDocument document)
        {
            Console.Write("Dictionary로 변환할 XPath를 입력하세요 (기본값: /library): ");
            var xpath = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(xpath))
            {
                xpath = "/library";
            }

            var element = XMLUtils.SelectElement(document, xpath);
            if (element == null)
            {
                Console.WriteLine("요소를 찾을 수 없습니다.\n");
                return;
            }

            var dictionary = XMLUtils.ElementToDictionary(element);
            Console.WriteLine("변환 결과:");
            PrintDictionary(dictionary, 0);
            Console.WriteLine();
        }

        private static void PrintDictionary(IDictionary<string, object> dictionary, int indent)
        {
            var indentString = new string(' ', indent);

            foreach (var pair in dictionary)
            {
                switch (pair.Value)
                {
                    case Dictionary<string, object> childDictionary:
                        Console.WriteLine($"{indentString}{pair.Key}:");
                        PrintDictionary(childDictionary, indent + 2);
                        break;
                    case object[] array:
                        Console.WriteLine($"{indentString}{pair.Key}:");
                        var itemIndent = new string(' ', indent + 2);
                        foreach (var item in array)
                        {
                            if (item is Dictionary<string, object> nestedDictionary)
                            {
                                Console.WriteLine($"{itemIndent}-");
                                PrintDictionary(nestedDictionary, indent + 4);
                            }
                            else
                            {
                                Console.WriteLine($"{itemIndent}- {item}");
                            }
                        }

                        break;
                    default:
                        Console.WriteLine($"{indentString}{pair.Key}: {pair.Value}");
                        break;
                }
            }
        }
    }
}
