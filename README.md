# YuJanggi.Core.V2

Unity 클라이언트와 .NET 서버에서 공유할 수 있는 순수 C# 장기 규칙 라이브러리입니다.
보드 상태와 말의 이동 가능 여부를 관리하고, 대국 진행에 필요한 턴·점수·기록·이벤트를 제공합니다.

## 제공 기능

- 9×10 장기판과 초·한의 초기 기물 배치, 네 가지 마·상 차림
- 기물별 이동 후보, 궁성 이동, 장군 노출 여부를 고려한 합법 수 판정
- 현재 차례와 이동 유효성을 확인하는 `MatchModel.TryMove`
- 이동·포획 기록, 무르기, 한 수 쉼, 기권
- 제한 시간 갱신과 기물 점수 관리
- 이동·장군·턴 변경·대국 종료 이벤트

렌더링, Unity 입력 구현, AI 전략, 네트워크 송수신은 사용하는 애플리케이션에서 구성합니다.
Core는 UnityEngine과 Protocol 패키지를 참조하지 않습니다.

## 요구 환경

| 항목 | 설정 |
| --- | --- |
| 소스 빌드·테스트 | .NET SDK 10 |
| 라이브러리 타깃 | `net10.0`, `netstandard2.1` |
| 라이브러리 C# 버전 | 9.0 |
| NuGet 패키지 | `YuJanggi.Core.V2` 1.0.0 |
| Unity UPM | `com.seokjinyoo.yujanggi.core.v2` 1.0.0 |
| Unity 대상 | Unity 6 (`6000.0` 이상) |
| 외부 런타임 패키지 | 없음 |

## 빠른 시작

### .NET — 로컬 NuGet

저장소 루트에서 패키지를 생성합니다. nuget.org 공개 배포를 전제로 하지 않는 절차입니다.

```powershell
dotnet pack src/YuJanggi.Engine.csproj -c Release -o artifacts/nuget
```

`artifacts/nuget/YuJanggi.Core.V2.1.0.0.nupkg`가 생성됩니다.
같은 저장소 루트에서 예제 프로젝트를 만들고 패키지를 설치합니다.

```powershell
dotnet new console -n CoreDemo -o artifacts/CoreDemo -f net10.0
dotnet add artifacts/CoreDemo/CoreDemo.csproj package YuJanggi.Core.V2 --version 1.0.0 --source ./artifacts/nuget
```

아래 최소 예제를 `artifacts/CoreDemo/Program.cs`에 넣고 실행합니다.

```powershell
dotnet run --project artifacts/CoreDemo/CoreDemo.csproj
```

기존 .NET 프로젝트에는 프로젝트 경로와 `--source`를 실제 경로에 맞춰 설치합니다.

### Unity — 로컬 UPM

저장소 루트에서 PowerShell로 실행합니다. 생성에는 .NET SDK 10이 필요합니다.

```powershell
./scripts/Prepare-Upm.ps1
```

Unity 6 프로젝트의 API Compatibility Level을 .NET Standard 2.1로 설정하고,
Package Manager의 **Install package from disk**에서 `upm/package.json`을 선택합니다.
사용자 asmdef가 있다면 Assembly Definition References에 `YuJanggi.Core.V2`를 추가합니다.

원본은 `src/`에만 작성합니다. 스크립트가 `.csproj`의 Compile 항목을
`upm/Runtime/Generated/`에 복사하고 안정적인 GUID의 `.meta`를 생성합니다.
원본 수정 후 스크립트를 다시 실행하며 생성 파일을 직접 수정하지 않습니다.

생성물은 Git에서 제외되므로 이 저장소의 `?path=/upm` Git URL 직접 설치는 지원하지 않습니다.
배포할 때는 스크립트 실행 후 생성 소스와 `.meta`를 포함한 `upm/` 전체를 전달해야 합니다.
동일한 Core V2 DLL과 UPM 소스를 함께 설치하지 않습니다.

## 최소 사용 예제

표준 보드를 초기화하고 초의 졸을 한 칸 전진시킨 뒤 무르는 콘솔 예제입니다.

```csharp
using System;
using YuJanggi.Core.V2.Board;
using YuJanggi.Core.V2.Domain;
using YuJanggi.Core.V2.Match;
using YuJanggi.Core.V2.Rule;

var match = new MatchModel(
    new Turn(30f),
    new Record(),
    new Score(),
    new BoardModel(),
    new JanggiRule());

match.BindEvents();
try
{
    match.InitGame(Formation.EHHE, Formation.EHHE);
    match.StartGame();

    bool moved = match.TryMove(new Pos(0, 3), new Pos(0, 4));
    Console.WriteLine($"이동: {moved}, 다음 차례: {match.PlayerTurn}");
    Console.WriteLine($"기록 수: {match.RecordCnt}");

    bool undone = match.TryUnDo(out _);
    Console.WriteLine($"무르기: {undone}, 현재 차례: {match.PlayerTurn}");
}
finally
{
    match.UnBindEvents();
}
```

예상 결과:

```text
이동: True, 다음 차례: Han
기록 수: 1
무르기: True, 현재 차례: Cho
```

Unity에서는 위 객체 구성과 초기화를 컴포넌트의 초기화 시점에 수행하고, 갱신 시점에
`match.Tick(Time.deltaTime)`을 호출합니다. 정리 시점에는 `UnBindEvents()`를 호출합니다.
서버에서는 서버의 갱신 루프에서 경과 초를 `Tick`에 전달합니다.

## 주요 API와 사용 조건

| API | 역할과 조건 |
| --- | --- |
| `InitGame(cho, han)` | 보드를 비우고 선택한 차림으로 기물을 배치 |
| `StartGame()` | 기록·점수·턴을 초기화하고 초부터 시작; 보드 배치는 별도 |
| `BindEvents()` / `UnBindEvents()` | 턴 이벤트 전달과 시간 만료 시 한 수 쉼을 연결·해제; 중복 연결을 피함 |
| `TryMove(from, to)` | 종료 상태, 좌표 범위, 기물 존재, 현재 차례, 합법 수를 검사하고 이동 |
| `TryUnDo(out context)` | 마지막 기록을 제거하고 이동·포획 점수·차례를 복원; 종료 후에는 false |
| `Handicap()` | 보드를 바꾸지 않고 한 수 쉼 기록을 추가하며 차례 변경 |
| `GiveUp()` | 현재 차례의 팀을 패자로 대국 종료 이벤트 발생 |
| `Tick(deltaTime)` | 초 단위 경과 시간을 받아 턴 제한 시간 갱신; 자동 실행되지 않음 |
| `Rule.GetMovableCells(board, from, legalCells, illegalCells)` | 지정 위치의 합법·불법 이동 후보를 호출자가 제공한 서로 다른 두 리스트에 덮어씀 |
| `Rule.HasAnyLegalMove(board, team)` | 해당 팀에 합법 수가 하나라도 있는지 확인 |

실제 대국 이동은 `MatchModel.TryMove`를 사용합니다. `BoardModel.DoMove`와 `SetPiece`는
상태를 직접 변경하는 저수준 API이므로 차례·합법 수·점수·기록 검증을 대신하지 않습니다.
규칙 API를 직접 사용할 때도 좌표 범위와 기물 존재 등 호출 조건을 확인해야 합니다.

`Pos`는 `(X, Z)` 좌표를 사용합니다. 기본 보드는 X가 0~8, Z가 0~9이며 초는 낮은 Z 쪽,
한은 높은 Z 쪽에서 시작합니다. 차림은 `HEHE`, `EHEH`, `EHHE`, `HEEH` 중 선택합니다.

`MatchEvent`는 `OnPieceMoved`, `OnCheckOccurred`, `OnCheckReleased`, `OnGameEnded`,
`OnTurnChanged`를 제공합니다. 남은 시간은 `Turn.OnTimeChanged`, 점수는
`Score.OnScoreChanged`, 기록 변화는 `Record.OnRecordChanged`에서 구독합니다.

## 네임스페이스와 버전

| 네임스페이스 | 주요 타입 |
| --- | --- |
| `YuJanggi.Core.V2` | `CoreVersion` |
| `YuJanggi.Core.V2.Domain` | `Pos`, `PlayerTeam`, `PieceType`, `Formation`, `Selection`, 이동·결과 모델 |
| `YuJanggi.Core.V2.Board` | `BoardModel`, `IBoardModel`, `PieceModel` |
| `YuJanggi.Core.V2.Match` | `MatchModel`, `Turn`, `Record`, `Score`, `MatchEvents` |
| `YuJanggi.Core.V2.Rule` | `JanggiRule`, `IJanggiRule` 외부 진입점 |
| `YuJanggi.Core.V2.JanggiRule` | 내부 `JanggiRulePipeline`, Context, Step, 장군 검사 구현 |
| `YuJanggi.Core.V2.MovementRule` | 기물별 이동 구현 |

이전 Core를 사용하던 코드는 `YuJanggi.Core.*` 참조를 실제 V2 네임스페이스에 맞춰 변경합니다.
`CoreVersion.Current`는 현재 `1.0.0`입니다. NuGet·UPM 버전도 현재 1.0.0이지만 별도 선언이므로
릴리스 시 각각 확인해야 합니다. Protocol의 통신 버전과도 구분합니다.

## 개발 및 검증

저장소 루트에서 실행합니다.

```powershell
dotnet build -c Release
dotnet test YuJanggi.Engine.Tests/YuJanggi.Engine.Tests.csproj -c Release
dotnet pack src/YuJanggi.Engine.csproj -c Release -o artifacts/nuget
```

테스트는 기물 이동과 막힘, 포의 다리·포획 조건, 궁성 이동, 자기 왕을 장군에 노출시키는 수,
이동·포획·턴·점수·기록 변화, 무르기와 한 수 쉼을 다룹니다.
.NET 테스트 통과만으로 Unity Player 또는 IL2CPP 검증 완료를 의미하지는 않습니다.

```text
YuJanggi.Engine/
  Board/                  # 보드와 기물 상태
  Domain/                 # 좌표·팀·이동·세션 계약
  Match/                  # 대국 진행과 규칙·기물 이동
  JanggiRule/             # 내부 이동 규칙 Pipeline과 재사용 계산 Context
YuJanggi.Engine.Tests/    # MSTest 기반 .NET 테스트
scripts/Prepare-Upm.ps1   # UPM 소스 생성
upm/                     # Unity 패키지 정의와 Runtime asmdef
```

## 사용 시 알아둘 점

- 보드 생성자는 크기를 받지만 궁성과 초기 배치는 표준 9×10 좌표를 전제로 하므로 기본 크기를 사용합니다.
- 규칙 판정은 내부 버퍼와 보드의 임시 이동·복원을 사용합니다. 같은 대국 인스턴스에 대한 호출은 직렬화합니다.
- `HasAnyLegalMove`는 첫 이동 가능한 기물을 찾으면 true를 반환합니다. 전체 합법 수의 개수를 세지 않습니다.
- `GameResult.Draw`와 `Score`가 정의되어 있어도 현재 `MatchModel`이 이 종료 결과를 자동 판정하는 것은 아닙니다.
- 리플레이 기록 API는 제공하지만 화면 재생과 보드 적용은 소비자 측에서 구성합니다.

### 로컬 패키지 갱신 후에도 V2 네임스페이스를 찾지 못할 때

같은 패키지 ID와 버전으로 다시 패키징하면 NuGet 전역 캐시의 이전 DLL이 사용될 수 있습니다.
위 예제 프로젝트에서는 저장소 루트에서 별도 캐시로 복원하여 새 패키지를 확인할 수 있습니다.
기존 전역 캐시는 삭제하지 않습니다.

```powershell
dotnet restore artifacts/CoreDemo/CoreDemo.csproj --packages artifacts/core-demo-packages --source ./artifacts/nuget --force
dotnet run --project artifacts/CoreDemo/CoreDemo.csproj --no-restore
```

패키지를 다른 프로젝트에 배포할 때는 변경된 내용에 맞춰 버전을 올려 구분합니다.

[프로토콜 V2](https://github.com/SeokJinYoo98/YuJanggi.Protocol.V2)
· [서버 V2](https://github.com/SeokJinYoo98/YuJanggi.Server.V2)
· [대국 구현](YuJanggi.Engine/Match/MatchModel.cs)
· [규칙 테스트](YuJanggi.Engine.Tests/JanggiRuleTests.cs)
