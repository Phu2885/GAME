# Story 004: Hai loại quái (bay và không bay)

> **Epic**: Heretic
> **Status**: Ready
> **Layer**: Feature
> **Type**: Logic
> **Estimate**: M
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: —

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

- `Assets/Scripts/Enemies/GroundEnemy.cs`
- `Assets/Scripts/Enemies/FlyingEnemy.cs`
- `Assets/Tests/EditMode/EnemyBehaviourTests.cs`

---

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
