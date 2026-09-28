# YuJanggi.Engine

Unity와 서버에서 공통으로 사용하는 장기 게임 규칙 및 상태 전이 엔진입니다.

## 프로젝트 개요

- Unity 비의존 장기 게임 엔진
- 클라이언트 / 서버 공용 게임 규칙 제공
- Command 기반 게임 진행
- 결정론적 상태 전이
- 장기 기물 이동 및 규칙 검증
- 턴 관리
- 승패 판정
- 기보 및 리플레이에 필요한 상태 제공
- NuGet / UPM 패키지 제공

## 기술 스택

- C#
- .NET 10
- .NET Standard 2.1
- NuGet
- Unity Package Manager
- xUnit
- GitHub Actions

## 엔진 구조

    Client / Server
          ↓
       Command
          ↓
      Validation
          ↓
    YuJanggi.Engine
          ↓
    State Transition
          ↓
      Game State

## 주요 기능

- 게임 초기 상태 생성
- 기물 이동 가능 여부 검증
- 장기 규칙 검증
- Command 처리
- 턴 전환
- 체크 및 장군 판정
- 승패 판정
- 게임 상태 조회
- 기보 생성
- 리플레이 상태 재현

## 주요 구성 요소

| 구성 요소 | 역할 |
| --- | --- |
| `JanggiEngine` | 게임 진행 및 상태 전이 관리 |
| `GameState` | 현재 게임 상태 관리 |
| `Board` | 장기판 및 기물 상태 관리 |
| `Command` | 게임 상태 변경 요청 |
| `Rule` | 장기 규칙 및 이동 검증 |
| `Turn` | 현재 차례 관리 |
| `Record` | 게임 진행 기록 관리 |
| `Score` | 게임 결과 및 점수 관리 |

## 게임 진행 흐름

    Command
        ↓
    입력 검증
        ↓
    규칙 검증
        ↓
    상태 변경
        ↓
    턴 전환
        ↓
    승패 판정
        ↓
    GameState 갱신

## 패키지 구조

    YuJanggi.Engine/
    ├── Commands/
    ├── Game/
    ├── Board/
    ├── Rules/
    ├── Records/
    ├── Common/
    └── upm/
        ├── package.json
        └── Runtime/

## 테스트

- 기물 이동 규칙 테스트
- Command 검증 테스트
- 상태 전이 테스트
- 턴 전환 테스트
- 승패 판정 테스트
- 동일 Command 재현 테스트

## NuGet 사용

    dotnet add package YuJanggi.Engine

## Unity UPM 사용

    "com.seokjinyoo.yujanggi.engine": "Git Repository URL"

## 관련 프로젝트

- [YuJanggi.Unity](링크)
- [YuJanggi.Server](링크)
- [YuJanggi.Protocol](링크)

## 포트폴리오

- [YuJanggi 포트폴리오](노션 링크)