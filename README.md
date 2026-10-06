# YuJanggi.Engine

Unity에 의존하지 않는 C# 장기 규칙·게임 상태 라이브러리입니다. 

게임 화면과 네트워크 처리에서 규칙 코드를 분리하고, NuGet과 Unity UPM 패키지로 제공합니다.

## Highlights

- 한 대국의 Board / Turn / Record / Score 관리
- .NET 10 / .NET Standard 2.1 지원
- GitHub Actions로 검증·패키징·NuGet 배포 자동화

## Installation

### .NET — NuGet

GitHub Packages의 `YuJanggi.Engine` 패키지를 참조합니다. 

NuGet 소스는 `https://nuget.pkg.github.com/SeokJinYoo98/index.json`이며, 접근 인증은 사용하는 환경에서 설정합니다. 

`.nupkg`는 [GitHub Release](https://github.com/SeokJinYoo98/YuJanggi.Engine/releases)에서도 다운로드할 수 있습니다.

### Unity — UPM

1. [Release 실행 화면](https://github.com/SeokJinYoo98/YuJanggi.Engine/actions/workflows/package-release.yml)의 **Artifacts → `yujanggi-engine-upm`**을 다운로드합니다.
2. 압축을 풀고 Unity Package Manager의 **Install package from tarball**에서 내부 `.tgz`를 선택합니다. 패키지의 Unity 기준 버전은 6000.0입니다.

UPM은 현재 Registry나 GitHub Release Asset으로 배포하지 않습니다. 

생성된 Runtime 소스는 Git에서 제외되므로, 저장소의 `upm/` Git 경로를 직접 설치하는 방식은 사용할 수 없습니다.

### Usage

```csharp
using YuJanggi.Engine.JanggiEngine;
using YuJanggi.Engine.JanggiOption;

var engine = JanggiEngineFactory.CreateEngine(new JanggiOptions { TurnTime = 30f });
engine.BindEvents();
engine.InitEngine();
engine.StartEngine();
```

소비 프로젝트에서 `Tick(deltaTime)`을 호출해 시간을 진행합니다. 

이벤트 연결이 필요 없어지면 `UnBindEvents()`로 해제합니다.

## Features

- 기물별 이동 후보, 궁성 이동과 이동 경로 검사
- 차례·기물 소유·장군 해소 조건을 확인한 이동 처리
- 포획 점수와 기보 기록, 무르기와 리플레이 기록 모드
- 한 수 쉼·기권·게임 종료와 상태 변경 이벤트
- 복제 보드의 합법 이동 조회·이동·복원을 통한 AI 탐색 지원

## Architecture

[JanggiEngineFactory](src/JanggiEngine/JanggiEngineFactory.cs)가 `JanggiOptions`로 엔진을 생성합니다.

 [JanggiEngine](src/JanggiEngine/JanggiEngine.cs)은 Board·Rule·Turn·Record·Score를 조정합니다.

- **규칙 처리**: 이동 후보 생성 → 궁성 규칙 적용 → 왕 안전성 검사
- **상태 조회**: Session의 게임 진행, Controller의 조회, AI 탐색을 별도 계약으로 제공
- **표현 분리**: Engine은 상태와 이벤트를 제공하며 UI·입력·통신은 소비 프로젝트가 담당

## CI/CD

- **[CI](.github/workflows/ci.yml)**: `main` / `release/**` 대상 PR에서 Restore → Release Build → Test
- **[Release](.github/workflows/package-release.yml)**: `v*.*.*` Tag Push에서 버전 설정·검증 후 NuGet / UPM Artifact 생성. GitHub Release에는 NuGet만 첨부
- **[CD](.github/workflows/cd.yml)**: 동일 Repository의 성공한 Release Push 실행에서 해당 Run ID의 NuGet Artifact를 받아 GitHub Packages에 Publish

CD는 다시 빌드하지 않으며 `GITHUB_TOKEN`으로 인증합니다. UPM Artifact는 CD에서 처리하지 않습니다.

[workflowConfig.json](workflowConfig.json)에서 프로젝트 경로·SDK·Artifact 이름을 관리합니다.

 [SetVersion.ps1](scripts/SetVersion.ps1)은 Tag 버전을 `Version.cs`, `.csproj`, `upm/package.json`에 함께 적용합니다. Runner에서 변경한 파일은 저장소나 로컬 폴더에 자동 반영하지 않습니다.

MSTest는 기물 이동·궁성·장군 규칙과 이동 거부 시 상태 보존, 포획·점수·턴 전환, 무르기를 검증합니다.

## Project Structure

```text
src/                        # 장기 규칙, 게임 상태, 공개 계약
upm/                        # Unity 패키지 설정과 Runtime 구성
scripts/                    # 버전 갱신과 NuGet / UPM 패키징
YuJanggi.Engine.Tests/       # MSTest 규칙·진행 검증
```

로컬 패키징은 `scripts/LocalPackage.bat`에서 Target / Version을 입력해 실행합니다. 결과물은 `artifacts/nuget`과 `artifacts/upm`에 생성합니다.

## Related Projects

- [YuJanggi.Unity](https://github.com/SeokJinYoo98/YuJanggi.Unity) — Engine을 사용하는 게임 화면·입력·AI Controller
- [YuJanggi.Server](https://github.com/SeokJinYoo98/YuJanggi.Server) — 공용 패키지를 참조하는 연결·매칭·게임 Server
- [YuJanggi.Protocol](https://github.com/SeokJinYoo98/YuJanggi.Protocol) — 클라이언트·서버 공유 메시지 계약

