# YuJanggi.Engine

## Tech Stack

<p align="center">
  <img src="https://cdn.jsdelivr.net/gh/devicons/devicon@latest/icons/csharp/csharp-original.svg" height="40" alt="C#" title="C#" />
  <img src="https://cdn.jsdelivr.net/gh/devicons/devicon@latest/icons/dotnetcore/dotnetcore-original.svg" height="40" alt=".NET" title=".NET" />
  <img src="https://cdn.jsdelivr.net/gh/devicons/devicon@latest/icons/nuget/nuget-original.svg" height="40" alt="NuGet" title="NuGet" />
  <img src="https://cdn.jsdelivr.net/gh/devicons/devicon@latest/icons/unity/unity-original.svg" height="40" alt="Unity UPM" title="Unity UPM" />
  <img src="https://cdn.jsdelivr.net/gh/devicons/devicon@latest/icons/githubactions/githubactions-original.svg" height="40" alt="GitHub Actions" title="GitHub Actions" />
</p>

<p align="center">
  C# · .NET 10 / .NET Standard 2.1 · MSTest<br />
  CI/CD · GitHub Actions · PowerShell · NuGet / UPM / npm
</p>

## 라이브러리 소개

Unity에 의존하지 않는 C# 장기 규칙·게임 상태 라이브러리입니다.<br />
보드·규칙·턴·점수·기보를 관리하며, 화면·입력·네트워크 처리는 소비 프로젝트가 담당합니다.

.NET용 NuGet과 Unity용 UPM 패키지로 제공합니다.

## 주요 기능

- **이동 규칙**: 기물별 후보 생성 · 궁성 이동 · 경로 검사 · 왕 안전성 검증
- **대국 진행**: 차례·기물 소유 확인 · 포획·점수 · 턴 전환과 시간 진행
- **종료 처리**: 기권 · 한 수 쉼 · 대국 결과와 상태 변경 이벤트
- **기보 관리**: 턴 기록 · 무르기 · 리플레이 기록 조회
- **AI 탐색 지원**: 실제 보드와 분리된 복제 보드에서 합법 이동 조회·이동·복원

AI의 수 선택 전략과 UI 재생은 Engine 외부에서 구현합니다.

## 설치 방법

### .NET — NuGet

GitHub Packages의 `YuJanggi.Engine`을 참조합니다.<br />
NuGet 소스는 `https://nuget.pkg.github.com/SeokJinYoo98/index.json`이며 읽기 인증은 사용하는 환경에 설정합니다.

```bash
dotnet add package YuJanggi.Engine --source https://nuget.pkg.github.com/SeokJinYoo98/index.json
```

### Unity — UPM

Project Settings → Package Manager에서 Scoped Registry를 등록합니다.

| 항목 | 값 |
| --- | --- |
| Name | YuJanggi |
| URL | `https://registry.npmjs.org` |
| Scope | `com.yujanggi` |

Package Manager의 My Registries에서 **YuJanggi Engine**을 설치합니다.<br />
패키지 ID는 `com.yujanggi.engine`이며 최소 Unity 버전은 **6000.0**입니다.

[GitHub Releases](https://github.com/SeokJinYoo98/YuJanggi.Engine/releases)의 `.nupkg` / `.tgz`로도 설치할 수 있습니다.

## 사용 예시

```csharp
using YuJanggi.Engine.JanggiEngine;
using YuJanggi.Engine.JanggiOption;

var engine = JanggiEngineFactory.CreateEngine(
    new JanggiOptions { TurnTime = 30f });

engine.BindEvents();
engine.InitEngine();
engine.StartEngine();
```

소비 프로젝트의 게임 루프에서 `engine.Tick(deltaTime)`을 호출해 시간을 진행합니다.<br />
이동은 `engine.TryProcessTurn(from, to)`로 요청하고, 종료 시 `EndEngine()`과 `UnBindEvents()`로 시간 진행과 내부 이벤트 연결을 정리합니다.

| 공개 API | 용도 |
| --- | --- |
| `References` | 읽기 전용 보드·턴·점수·기보와 게임 이벤트 참조 |
| `ControllerQuery` | 현재 차례 · 기물 선택 유효성 · 합법·불법 이동 위치 조회 |
| `ReplayRecord` | 리플레이 모드 진입·종료와 인덱스별 턴 기록 조회 |
| `CreateAIPosition()` | AI 탐색용 복제 보드 생성 |

외부에서 구독한 화면·입력 이벤트는 소비 프로젝트에서 해제합니다.

## 규칙 처리 구조

이동 후보 생성과 최종 합법성 검증을 분리합니다.

1. **MovementStep**: 기물별 이동 패턴과 경로를 바탕으로 후보를 생성합니다.
2. **PalaceStep**: 궁성 내부 이동 규칙을 적용합니다.
3. **LegalMoveStep**: 후보 이동을 임시 적용해 자기 왕의 장군 여부를 확인하고 보드를 복원합니다.

[CandidatePipeline](src/JanggiRule/CandidatePipeline.cs)은 앞의 두 단계를 담당하고, [JanggiRule](src/JanggiRule/JanggiRule.cs)이 합법성 검사까지 조합합니다.<br />
`TryProcessTurn`은 대국 종료·좌표·기물 존재·소유권을 확인한 뒤 합법 이동만 실제 대국에 적용합니다.

규칙 계산은 인스턴스 내부 작업 메모리를 재사용하므로 동일 Engine 인스턴스를 동시에 호출하지 않습니다.

## 게임 상태 · 기보 관리

[JanggiEngine](src/JanggiEngine/JanggiEngine.cs)이 Board / Rule / Turn / Record / Score를 조정합니다.

- **Board / Turn**: 기물 배치와 현재 차례·대국 시간 관리
- **Record / Score**: 이동·포획·턴 결과 기록과 점수 반영
- **Undo**: 이전 기록을 바탕으로 보드·점수·턴 복원
- **ReplayRecord**: 기록 탐색과 실제 대국 기록 추가를 구분
- **GameEvents**: 처리 결과를 소비 프로젝트의 화면·게임 흐름에 전달

`GiveUp`은 기권 결과를 생성하며, `EndEngine`은 외부에서 확정한 종료를 반영해 시간 진행만 멈춥니다.<br />
실제 화면의 Live / Replay 전환은 Unity 클라이언트에서 처리합니다.

## 테스트

[YuJanggi.Engine.Tests](YuJanggi.Engine.Tests)에 MSTest를 작성했습니다.

- 기물별 이동·궁성·장군 규칙
- 잘못된 이동 거부와 상태 보존
- 포획·점수·턴 전환
- 기보 기록과 무르기

```bash
dotnet test YuJanggi.Engine.slnx --configuration Release
```

## 패키지 배포

| 단계 | GitHub Actions 구성 |
| --- | --- |
| [CI](.github/workflows/ci.yml) | main / release 대상 PR에서 Restore → Release Build → MSTest |
| [Release](.github/workflows/package-release.yml) | 버전 Tag에서 검증 · NuGet / UPM 생성 · Artifact와 GitHub Release 제공 |
| [CD](.github/workflows/cd.yml) | 성공한 Release Run의 패키지를 GitHub Packages와 npm에 배포 |

CD는 패키지를 다시 빌드하지 않으며 npm 배포에는 Trusted Publishing을 사용합니다.<br />
[SetVersion.ps1](scripts/SetVersion.ps1)이 Tag 버전을 `Version.cs`, `.csproj`, `upm/package.json`에 함께 적용합니다. Runner의 변경은 저장소에 자동 커밋하지 않습니다.

프로젝트 경로·SDK·Artifact 이름은 [workflowConfig.json](workflowConfig.json)에서 관리합니다.<br />
로컬 패키징은 `scripts/LocalPackage.bat`에서 Target / Version을 입력하며, 결과는 `artifacts/nuget`과 `artifacts/upm`에 생성합니다.

## 관련 저장소 · 문서

- [YuJanggi.Unity](https://github.com/SeokJinYoo98/YuJanggi.Unity) — Engine 기반 게임 화면·입력·AI 전략·기보 재생
- [YuJanggi.Protocol](https://github.com/SeokJinYoo98/YuJanggi.Protocol) — 클라이언트·서버 공용 메시지 계약
- [YuJanggi.Server](https://github.com/SeokJinYoo98/YuJanggi.Server) — AWS EC2 기반 TCP 서버. 현재 Engine 공용 타입·버전을 사용하며 규칙 검증 연동은 향후 과제
