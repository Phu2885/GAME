# Story 005: Một màn cuộn ngang có chướng ngại

> **Epic**: Heretic
> **Status**: Complete
> **Layer**: Feature
> **Type**: Integration
> **Estimate**: M
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: 2026-11-02

## Context

**GDD**: `design/game-brief.md`
**Requirement**: Brief MVP feature 2

**ADR Governing Implementation**: N/A (minimal — no ADRs)
**ADR Decision Summary**: N/A (minimal — no ADRs)
**ADR Version**: N/A (minimal — no ADRs)

**Engine**: Unity 6000.3 | **Risk**: HIGH
**Engine Notes**: none (no ADR engine-compatibility analysis at minimal)

**Control Manifest Rules (this layer)**: N/A (minimal — no control manifest)

---

## Acceptance Criteria

*From the brief (Core loop; Player goal & fail state), scoped to this story:*

- [ ] Một màn cuộn ngang từ điểm xuất phát đến cửa phòng boss, dựng bằng Tilemap, camera đi theo nhân vật.
- [ ] Có chướng ngại vật phải nhảy hoặc dash để vượt (hố, gai hoặc tường thấp), chạm gai thì mất máu.
- [ ] Có đặt cả hai loại quái của story 004 trong màn.
- [ ] Cửa cuối màn dẫn sang phòng boss (story 002).
- [ ] Chết giữa màn thì quay lại đầu màn.
- [ ] Màn chơi được từ đầu đến cuối bằng nhân vật thật.

---

## Implementation Notes

- Nền xanh đen theo moodboard của brief; dùng ô lát tạm nếu chưa có tileset.
- Giữ độ dài ngắn: mục tiêu là thử nhịp chạy-nhảy trước khi trận boss, không phải làm nội dung dài.

---

## Out of Scope

- Nhiều màn, cốt truyện, cắt cảnh (đã loại trong brief).

---

## Files

- `Assets/Scripts/Level/LevelOneBootstrap.cs` (builds the level from an ASCII map, the Tilemap, hazards, door, enemies, HUD and restart)
- `Assets/Scripts/Level/CameraFollow.cs` (smoothed follow clamped to the level)
- `Assets/Scripts/Level/Hazard.cs` (spikes and the kill zone)
- `Assets/Scripts/Level/LevelDoor.cs` (loads the boss room)
- `Assets/Scripts/Player/PlayerFactory.cs` (shared player construction)
- `Assets/Scripts/Player/PlayerController.cs`, `PlayerHealth.cs`, `PlayerTestBootstrap.cs` (Bounds, Halt, overload with ignoreInvulnerability, never-sleep body, use the factory)
- `Assets/Scripts/Boss/PaladinBoss.cs`, `BossArenaBootstrap.cs` (the boss fight now uses the real player)
- `Assets/Scripts/Enemies/EnemyBase.cs`, `GroundEnemy.cs` (TD-001: walkers turn at other enemies)
- `Assets/Scenes/Level01.unity` (new, built by the lead), `Assets/Scenes/BossArena.unity` (updated with the player sprites)
- `ProjectSettings/EditorBuildSettings.asset` (Level01 and BossArena in the build)

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above.*

---

## Test Evidence

**Story Type**: Integration
**Required evidence**: `Assets/Tests/PlayMode/Level01Tests.cs` OR a playtest doc (waived at `qa.level: minimal`, advisory only)

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: `production/epics/heretic/story-003-player-movement.md`, `production/epics/heretic/story-004-enemies.md`
- Unlocks: None

## Completion Notes
**Completed**: 2026-11-02
**Criteria**: 6/6 passing (none deferred)
**Deviations**: Out of scope, agreed with the user: the boss fight now uses the real player (changes to stories 002 and 003 code, `PlayerFactory`), the walker turns at other enemies (story 004 code, TD-001 and TD-002 resolved and closed in `docs/tech-debt-register.md`), `ProjectSettings/EditorBuildSettings.asset` lists Level01 and BossArena. The unused stand-in `Assets/Scripts/Boss/TempPlayer.cs` was deleted with the user's approval. Ground enemies walk through the spikes (spikes hurt only the player), left as is.
**Test Evidence**: waived at qa.level minimal; screenshots at `production/qa/evidence/story-005-level-one/`
**Reviewed by**: self, no independent review
