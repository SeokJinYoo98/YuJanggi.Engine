# YuJanggi.Engine

C#으로 작성한 장기 규칙 및 게임 진행 엔진입니다. `YuJanggi.Unity`와 `YuJanggi.Server.V2`에서 공통 타입과 규칙을 사용합니다.

## 프로젝트 개요

- Unity에 의존하지 않는 장기판, 이동 규칙 및 게임 진행 코드
- `JanggiOptions`로 포진, 제한시간, 판 크기 설정
- .NET 라이브러리와 Unity Package Manager 패키지 구성

## 기술 스택

- C# 9
- .NET 10 / .NET Standard 2.1
- Unity Package Manager
- MSTest

## 엔진 구조

```text
JanggiEngineFactory → JanggiEngine
                       ├─ JanggiBoard: 기물 배치와 이동
                       ├─ JanggiRule: 이동 후보와 합법성 판정
                       ├─ JanggiTurn: 차례와 시간
                       ├─ JanggiRecord: 수 기록
                       └─ JanggiScore: 점수
```

## 주요 기능

- 기물별 이동 후보 계산과 합법성 검사
- 궁성 이동, 장군 및 합법적인 다음 수 판정
- 기물 이동, 턴 전환, 시간 경과 처리
- 포획 점수, 기보 기록, 무르기
- 한 수 쉼, 기권, 게임 종료 이벤트
- 실시간/리플레이 기록 모드 전환

## 주요 코드

| 구성 요소 | 역할 |
| --- | --- |
| [`JanggiEngineFactory`](src/JanggiEngine/JanggiEngineFactory.cs) | `JanggiOptions`로 엔진 생성 |
| [`JanggiEngine`](src/JanggiEngine/JanggiEngine.cs) | 이동 요청과 게임 진행 처리 |
| [`JanggiRule`](src/JanggiRule/JanggiRule.cs) | 합법적인 이동 및 장군 판정 |
| [`JanggiBoard`](src/JanggiBoard/JanggiBoard.cs) | 장기판과 기물 상태 관리 |
| [`JanggiRecord`](src/JanggiRecord/JanggiRecord.cs) | 수 기록과 리플레이 모드 관리 |

## 사용 방법

.NET 10 프로젝트에서는 [`src/YuJanggi.Engine.csproj`](src/YuJanggi.Engine.csproj)을 참조합니다. Unity용 패키지 설정은 [`upm/package.json`](upm/package.json)에 있습니다.

```csharp
using YuJanggi.Engine.Domain;
using YuJanggi.Engine.JanggiEngine;
using YuJanggi.Engine.JanggiOption;

var options = new JanggiOptions
{
    GameMode = GameModeType.Local,
    PlayerCho = PlayerType.Local,
    PlayerHan = PlayerType.Local,
    TurnTime = 30f
};

IJanggiEngine engine = JanggiEngineFactory.CreateEngine(options);
engine.InitEngine();
engine.StartEngine();
```

## 테스트

[`YuJanggi.Engine.Tests`](YuJanggi.Engine.Tests)는 MSTest로 엔진 진행과 이동 규칙을 검사합니다.

## 관련 프로젝트

- `YuJanggi.Unity`: 게임 클라이언트
- `YuJanggi.Server.V2`: 게임 서버
- `YuJanggi.Protocol`: 통신 메시지와 DTO

## 포트폴리오

[상세 설계와 문제 해결 과정을 소개할 때, 포트폴리오 링크]
