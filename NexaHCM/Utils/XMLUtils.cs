using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace NexaHCM.Utils
{
    public static class XMLUtils
    {
        private const string DefaultNamespacePrefix = "ns";

        #region XML 로드/저장

        /// <summary>
        /// 문자열에서 XDocument 로드
        /// </summary>
        public static XDocument LoadFromString(string xmlString)
        {
            return XDocument.Parse(xmlString);
        }

        /// <summary>
        /// 파일에서 XDocument 로드
        /// </summary>
        public static XDocument LoadFromFile(string filePath)
        {
            return XDocument.Load(filePath);
        }

        /// <summary>
        /// 스트림에서 XDocument 로드
        /// </summary>
        public static XDocument LoadFromStream(Stream stream)
        {
            return XDocument.Load(stream);
        }

        /// <summary>
        /// XDocument를 문자열로 저장 (포맷팅 옵션 포함)
        /// </summary>
        public static string SaveToString(XDocument doc, SaveOptions options = SaveOptions.None)
        {
            return doc.ToString(options);
        }

        /// <summary>
        /// XDocument를 파일로 저장
        /// </summary>
        public static void SaveToFile(XDocument doc, string filePath, SaveOptions options = SaveOptions.None)
        {
            doc.Save(filePath, options);
        }

        #endregion

        #region 네임스페이스 관리

        /// <summary>
        /// XDocument에서 모든 네임스페이스 추출
        /// </summary>
        public static Dictionary<string, string> GetAllNamespaces(XDocument doc)
        {
            var namespaces = new Dictionary<string, string>();
            var navigator = doc.CreateNavigator();
            var namespacesInScope = navigator.GetNamespacesInScope(XmlNamespaceScope.All);

            if (namespacesInScope != null)
            {
                foreach (var ns in namespacesInScope)
                {
                    namespaces[ns.Key] = ns.Value;
                }
            }

            return namespaces;
        }

        /// <summary>
        /// 네임스페이스 매니저 생성 (기본 네임스페이스 포함)
        /// </summary>
        public static XmlNamespaceManager CreateNamespaceManager(XDocument doc, string defaultPrefix = DefaultNamespacePrefix)
        {
            var navigator = doc.CreateNavigator();
            var nsManager = new XmlNamespaceManager(navigator.NameTable);
            var namespacesInScope = navigator.GetNamespacesInScope(XmlNamespaceScope.All);

            if (namespacesInScope != null)
            {
                foreach (var ns in namespacesInScope)
                {
                    if (string.IsNullOrEmpty(ns.Key))
                    {
                        if (!string.IsNullOrWhiteSpace(defaultPrefix) && !string.IsNullOrWhiteSpace(ns.Value))
                        {
                            nsManager.AddNamespace(defaultPrefix, ns.Value);
                        }
                    }
                    else if (nsManager.LookupNamespace(ns.Key) == null)
                    {
                        nsManager.AddNamespace(ns.Key, ns.Value);
                    }
                }
            }

            return nsManager;
        }

        #endregion

        #region XPath 쿼리 (단일/다중)

        /// <summary>
        /// XPath로 단일 XElement 선택 (네임스페이스 자동 처리)
        /// </summary>
        public static XElement SelectElement(XDocument doc, string xpath, XmlNamespaceManager nsManager = null)
        {
            var processedXPath = PrepareXPath(doc, xpath, ref nsManager);
            var element = doc.XPathSelectElement(processedXPath, nsManager);

            if (element == null && !string.Equals(processedXPath, xpath, StringComparison.Ordinal))
            {
                element = doc.XPathSelectElement(xpath, nsManager);
            }

            return element;
        }

        /// <summary>
        /// XPath로 다중 XElement 선택 (네임스페이스 자동 처리)
        /// </summary>
        public static List<XElement> SelectElements(XDocument doc, string xpath, XmlNamespaceManager nsManager = null)
        {
            var processedXPath = PrepareXPath(doc, xpath, ref nsManager);
            var elements = doc.XPathSelectElements(processedXPath, nsManager).ToList();

            if (elements.Count == 0 && !string.Equals(processedXPath, xpath, StringComparison.Ordinal))
            {
                elements = doc.XPathSelectElements(xpath, nsManager).ToList();
            }

            return elements;
        }

        /// <summary>
        /// XPath 표현식에 기본 네임스페이스 접두사 자동 추가
        /// </summary>
        private static string ProcessXPathForDefaultNamespace(string xpath, string defaultPrefix)
        {
            if (string.IsNullOrWhiteSpace(xpath))
            {
                return xpath;
            }

            var tokens = Regex.Split(xpath, @"(/+)");
            for (var i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i];
                if (string.IsNullOrEmpty(token))
                {
                    continue;
                }

                if (token.All(c => c == '/'))
                {
                    continue;
                }

                tokens[i] = AddPrefixToSegment(token, defaultPrefix);
            }

            return string.Concat(tokens);
        }

        private static string AddPrefixToSegment(string segment, string defaultPrefix)
        {
            return Regex.Replace(segment, @"(?<![@:])\b([a-zA-Z_][\w\-]*)\b(?!:)", match =>
            {
                var element = match.Value;
                if (IsXPathFunction(element) || IsXPathKeyword(element))
                {
                    return element;
                }

                return $"{defaultPrefix}:{element}";
            });
        }

        /// <summary>
        /// XPath 함수 여부 확인
        /// </summary>
        private static bool IsXPathFunction(string name)
        {
            var functions = new[]
            {
                "text", "node", "comment", "processing-instruction", "last", "position", "count", "name",
                "local-name", "namespace-uri", "string", "number", "boolean", "concat", "starts-with",
                "contains", "substring-before", "substring-after", "substring", "string-length",
                "normalize-space", "translate", "sum", "floor", "ceiling", "round"
            };
            return functions.Contains(name);
        }

        private static bool IsXPathKeyword(string name)
        {
            switch (name)
            {
                case "and":
                case "or":
                case "div":
                case "mod":
                case "true":
                case "false":
                    return true;
                default:
                    return false;
            }
        }

        private static string PrepareXPath(XDocument doc, string xpath, ref XmlNamespaceManager nsManager)
        {
            if (nsManager != null)
            {
                return xpath;
            }

            nsManager = CreateNamespaceManager(doc, DefaultNamespacePrefix);
            var defaultNamespace = nsManager.LookupNamespace(DefaultNamespacePrefix);
            if (!string.IsNullOrEmpty(defaultNamespace))
            {
                var processed = ProcessXPathForDefaultNamespace(xpath, DefaultNamespacePrefix);
                return processed;
            }

            return xpath;
        }

        #endregion

        #region 값 조회/설정

        /// <summary>
        /// XPath로 요소 값 조회
        /// </summary>
        public static string GetElementValue(XDocument doc, string xpath, XmlNamespaceManager nsManager = null)
        {
            var element = SelectElement(doc, xpath, nsManager);
            return element?.Value;
        }

        /// <summary>
        /// XPath로 요소 값 설정
        /// </summary>
        public static bool SetElementValue(XDocument doc, string xpath, string value, XmlNamespaceManager nsManager = null)
        {
            var element = SelectElement(doc, xpath, nsManager);
            if (element == null)
            {
                return false;
            }

            element.Value = value;
            return true;
        }

        /// <summary>
        /// XPath로 속성 값 조회
        /// </summary>
        public static string GetAttributeValue(XDocument doc, string xpath, string attributeName, XmlNamespaceManager nsManager = null)
        {
            var element = SelectElement(doc, xpath, nsManager);
            return element?.Attribute(attributeName)?.Value;
        }

        /// <summary>
        /// XPath로 속성 값 설정
        /// </summary>
        public static bool SetAttributeValue(XDocument doc, string xpath, string attributeName, string value, XmlNamespaceManager nsManager = null)
        {
            var element = SelectElement(doc, xpath, nsManager);
            if (element == null)
            {
                return false;
            }

            element.SetAttributeValue(attributeName, value);
            return true;
        }

        /// <summary>
        /// XPath로 여러 요소의 값들을 배열로 조회
        /// </summary>
        public static string[] GetElementValues(XDocument doc, string xpath, XmlNamespaceManager nsManager = null)
        {
            var elements = SelectElements(doc, xpath, nsManager);
            return elements.Select(e => e.Value).ToArray();
        }

        #endregion

        #region 요소 생성/삭제/수정

        /// <summary>
        /// 지정된 부모 요소에 자식 요소 추가
        /// </summary>
        public static XElement AddChildElement(XDocument doc, string parentXPath, string elementName, string value = null, XmlNamespaceManager nsManager = null)
        {
            var parent = SelectElement(doc, parentXPath, nsManager);
            if (parent == null)
            {
                return null;
            }

            XElement newElement;
            if (elementName.Contains(":"))
            {
                var parts = elementName.Split(new[] { ':' }, 2);
                var prefix = parts[0];
                var localName = parts[1];
                var namespaceUri = parent.GetNamespaceOfPrefix(prefix);
                newElement = namespaceUri == null
                    ? new XElement(parent.GetDefaultNamespace() + localName)
                    : new XElement(namespaceUri + localName);
            }
            else
            {
                newElement = new XElement(parent.GetDefaultNamespace() + elementName);
            }

            if (!string.IsNullOrEmpty(value))
            {
                newElement.Value = value;
            }

            parent.Add(newElement);
            return newElement;
        }

        /// <summary>
        /// XPath로 요소 삭제
        /// </summary>
        public static bool RemoveElement(XDocument doc, string xpath, XmlNamespaceManager nsManager = null)
        {
            var element = SelectElement(doc, xpath, nsManager);
            if (element == null)
            {
                return false;
            }

            element.Remove();
            return true;
        }

        /// <summary>
        /// XPath로 여러 요소 삭제
        /// </summary>
        public static int RemoveElements(XDocument doc, string xpath, XmlNamespaceManager nsManager = null)
        {
            var elements = SelectElements(doc, xpath, nsManager);
            foreach (var element in elements)
            {
                element.Remove();
            }

            return elements.Count;
        }

        /// <summary>
        /// 요소 복제 (깊은 복사)
        /// </summary>
        public static XElement CloneElement(XElement element)
        {
            return new XElement(element);
        }

        #endregion

        #region 고급 쿼리 기능

        /// <summary>
        /// XPath 조건부 쿼리 (조건에 따른 다른 XPath 실행)
        /// </summary>
        public static XElement ConditionalSelect(XDocument doc, string conditionXPath, string trueXPath, string falseXPath, XmlNamespaceManager nsManager = null)
        {
            var conditionResult = SelectElement(doc, conditionXPath, nsManager);
            var xpath = conditionResult != null ? trueXPath : falseXPath;
            return SelectElement(doc, xpath, nsManager);
        }

        /// <summary>
        /// 요소 존재 여부 확인
        /// </summary>
        public static bool ElementExists(XDocument doc, string xpath, XmlNamespaceManager nsManager = null)
        {
            return SelectElement(doc, xpath, nsManager) != null;
        }

        /// <summary>
        /// 요소 개수 조회
        /// </summary>
        public static int CountElements(XDocument doc, string xpath, XmlNamespaceManager nsManager = null)
        {
            return SelectElements(doc, xpath, nsManager).Count;
        }

        /// <summary>
        /// XPath 표현식 평가 (다양한 결과 타입 지원)
        /// </summary>
        public static object EvaluateXPath(XDocument doc, string xpath, XmlNamespaceManager nsManager = null)
        {
            var processedXPath = PrepareXPath(doc, xpath, ref nsManager);
            var result = doc.XPathEvaluate(processedXPath, nsManager);

            if (result == null && !string.Equals(processedXPath, xpath, StringComparison.Ordinal))
            {
                result = doc.XPathEvaluate(xpath, nsManager);
            }

            return result;
        }

        #endregion

        #region 유틸리티 메서드

        /// <summary>
        /// XML 유효성 검사
        /// </summary>
        public static bool IsValidXml(string xmlString)
        {
            try
            {
                XDocument.Parse(xmlString);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// XML 포맷팅 (들여쓰기 적용)
        /// </summary>
        public static string FormatXml(string xmlString)
        {
            try
            {
                var doc = XDocument.Parse(xmlString);
                return doc.ToString();
            }
            catch
            {
                return xmlString;
            }
        }

        /// <summary>
        /// 두 XElement 비교
        /// </summary>
        public static bool AreElementsEqual(XElement element1, XElement element2)
        {
            return XNode.DeepEquals(element1, element2);
        }

        /// <summary>
        /// XElement를 Dictionary로 변환 (속성과 자식 요소 포함)
        /// </summary>
        public static Dictionary<string, object> ElementToDictionary(XElement element)
        {
            var result = new Dictionary<string, object>();

            foreach (var attr in element.Attributes())
            {
                result[$"@{attr.Name.LocalName}"] = attr.Value;
            }

            if (!element.HasElements && !string.IsNullOrWhiteSpace(element.Value))
            {
                result["#text"] = element.Value;
            }

            var childGroups = element.Elements().GroupBy(e => e.Name.LocalName);
            foreach (var group in childGroups)
            {
                if (group.Count() == 1)
                {
                    var child = group.First();
                    result[child.Name.LocalName] = child.HasElements
                        ? (object)ElementToDictionary(child)
                        : child.Value;
                }
                else
                {
                    result[group.Key] = group
                        .Select(e => e.HasElements ? ElementToDictionary(e) : (object)e.Value)
                        .ToArray();
                }
            }

            return result;
        }

        #endregion
    }
}
