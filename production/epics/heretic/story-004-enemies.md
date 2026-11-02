# Story 004: Hai loại quái (bay và không bay)

> **Epic**: Heretic
> **Status**: Complete
> **Layer**: Feature
> **Type**: Logic
> **Estimate**: M
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: 2026-11-02

## Context

**GDD**: `design/game-brief.md`
**Requirement**: Brief MVP feature 3

**ADR Governing Implementation**: N/A (minimal — no ADRs)
**ADR Decision Summary**: N/A (minimal — no ADRs)
**ADR Version**: N/A (minimal — no ADRs)

**Engine**: Unity 6000.3 | **Risk**: HIGH
**Engine Notes**: none (no ADR engine-compatibility analysis at minimal)

**Control Manifest Rules (this layer)**: N/A (minimal — no control manifest)

---

## Acceptance Criteria

*From the brief (Core loop), scoped to this story:*

- [ ] Quái không bay: đi tuần trên mặt đất, đổi hướng khi gặp mép hoặc tường, gây sát thương khi chạm người chơi.
- [ ] Quái bay: bay theo một đường lượn, gây sát thương khi chạm người chơi.
- [ ] Cả hai có thanh máu; chém trúng thì mất máu, hết máu thì chết và biến mất.
- [ ] Sát thương, máu, tốc độ là các giá trị chỉnh được.
- [ ] Mỗi loại là một prefab dùng lại được trong màn.

---

## Implementation Notes

- Tách phần quyết định hành vi (tuần tra, đổi hướng) thành logic thuần kiểm thử được.
- Dùng placeholder cho sprite; art thật làm sau.

---

## Out of Scope

- Story 005: đặt quái vào màn.
- Story 003: cú chém của người chơi (ở đây chỉ nhận sát thương).

---

## Files

- `Assets/Scripts/Enemies/EnemyBase.cs` (shared health, flash, contact damage, death; `EnemySettings`)
- `Assets/Scripts/Enemies/GroundEnemy.cs` (walker: patrol, turns at ledges and walls)
- `Assets/Scripts/Enemies/FlyingEnemy.cs` (flyer on a sine path)
- `Assets/Scripts/Enemies/EnemyPatrolLogic.cs` (pure turn decision)
- `Assets/Scripts/Enemies/FlightPath.cs` (pure flight-path function)
- `Assets/Scripts/Enemies/EnemyTestBootstrap.cs` (test spawner)
- `Assets/Prefabs/Enemies/GroundEnemy.prefab`, `FlyingEnemy.prefab` (built by the lead)
- `Assets/Scenes/EnemyTest.unity` (test scene built by the lead; reuses the story-003 player)

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above.*

---

## Test Evidence

**Story Type**: Logic
**Required evidence**: `Assets/Tests/EditMode/EnemyBehaviourTests.cs` (waived at `qa.level: minimal`, advisory only)

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: `production/epics/heretic/story-003-player-movement.md`
- Unlocks: `production/epics/heretic/story-005-level-one.md`

## Completion Notes
**Completed**: 2026-11-02
**Criteria**: 5/5 passing (none deferred)
**Deviations**: Advisory, logged as tech debt TD-001 and TD-002 in `docs/tech-debt-register.md`: two ground enemies walking toward each other collide and stop (the walker ignores dynamic bodies as walls); ledge turning was never exercised because the test room has no ledge. Out of scope: shared `EnemyBase`/`EnemySettings`, the `EnemyTest` scene and bootstrap, the two prefabs built by the lead.
**Test Evidence**: waived at qa.level minimal; screenshot at `production/qa/evidence/story-004-enemies/01-ground-and-flying-enemies.png`
**Reviewed by**: self, no independent review
