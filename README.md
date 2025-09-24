# NAF-SRC-SHAREPOINT

사내 코드 공유 및 예제 관리를 위한 퍼블릭 저장소입니다. 현재는 `.NET Framework 4.7.2` 콘솔 응용 프로그램인 **NexaHCM** 샘플이 포함되어 있으며, XML 조작과 관련된 유틸리티 함수 사용 예시를 제공합니다.

## 저장소 구성

```
NAF-SRC-SHAREPOINT/
├── NexaHCM/           # 콘솔 앱 프로젝트 (net472)
│   ├── Program.cs     # 실행 진입점 및 데모 시나리오
│   └── Utils/
│       └── XMLUtils.cs # XML 유틸리티 클래스 라이브러리
└── README.md
```

## NexaHCM 콘솔 앱 개요

- 실행 시 "기본 테스트 모드"와 "XML 유틸리티 테스트 모드" 두 가지 메뉴를 제공합니다.
- 기본 모드에서는 사용자가 입력한 값을 단순 출력하고 숫자라면 제곱 계산을 수행합니다.
- XML 모드에서는 샘플 XML(`SampleXml`)을 로드하여 XPath 기반 조회/수정, 요소 추가/삭제, Dictionary 변환 등 다양한 XML 처리 기능을 체험할 수 있습니다.

## 요구 사항

- 운영 체제: Windows 10 이상 권장 ( .NET Framework 기반 프로젝트 )
- .NET Framework 4.7.2 Developer Pack 또는 Visual Studio 2019 이상 (개발 및 빌드용)
- 선택 사항: 최신 .NET SDK (예: .NET 6/7 이상) — `dotnet` CLI에서 .NET Framework 타겟을 빌드하려면 해당 타겟팅 팩이 설치되어 있어야 합니다.

## 빌드 및 실행 방법

### Visual Studio 사용 시
1. `NexaHCM` 폴더를 Visual Studio에서 "기존 프로젝트 열기"로 로드합니다.
2. 솔루션 구성/플랫폼을 확인한 뒤 **F5**(디버그) 또는 **Ctrl+F5**(디버그 없이 실행)로 콘솔 앱을 실행합니다.

### dotnet CLI 사용 시
> 사전에 .NET Framework 4.7.2 타겟팅 팩이 설치되어 있어야 합니다.

```bash
# 패키지 복원 (필요한 경우)
dotnet restore NexaHCM/NexaHCM.csproj

# 빌드
dotnet build NexaHCM/NexaHCM.csproj -c Release

# 실행
dotnet run --project NexaHCM/NexaHCM.csproj -c Release
```

## XMLUtils 주요 기능 요약

- **XML 로드/저장**: 문자열, 파일, 스트림에서 `XDocument`로 로드하고, 다시 문자열 또는 파일로 저장.
- **네임스페이스 관리**: 문서에 포함된 모든 네임스페이스를 추출하거나 기본 접두사를 포함한 `XmlNamespaceManager` 생성.
- **XPath 지원**: 기본 네임스페이스를 자동으로 처리하며 단일/다중 요소 선택 지원, XPath 함수/키워드 보호.
- **요소 값 조작**: XPath로 요소 값을 조회·설정하고, 자식 요소 추가, 요소 삭제, 요소 개수 계산.
- **구조 변환**: 특정 요소를 재귀적으로 순회하여 `Dictionary<string, object>` 구조로 변환.

## 추가 참고 사항

- `Program.cs`는 XMLUtils 기능을 실습할 수 있는 인터랙티브 콘솔 메뉴를 제공합니다.
- 향후 다른 샘플 프로젝트를 추가할 수 있으므로, 공용 유틸리티나 가이드는 이 README에 계속 정리해주세요.
- 버그 또는 개선 아이디어가 있다면 Pull Request를 통해 공유해주세요.

