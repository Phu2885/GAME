# Story 006: Hạ boss nhận một năng lực mới

> **Epic**: Heretic
> **Status**: Ready
> **Layer**: Feature
> **Type**: Logic
> **Estimate**: S
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: —

## Context

**GDD**: `design/game-brief.md`
**Requirement**: Brief MVP feature 6

**ADR Governing Implementation**: N/A (minimal — no ADRs)
**ADR Decision Summary**: N/A (minimal — no ADRs)
**ADR Version**: N/A (minimal — no ADRs)

**Engine**: Unity 6000.3 | **Risk**: HIGH
**Engine Notes**: none (no ADR engine-compatibility analysis at minimal)

**Control Manifest Rules (this layer)**: N/A (minimal — no control manifest)

---

## Acceptance Criteria

*From the brief (Core loop; MVP feature 6), scoped to this story:*

- [ ] Khi boss Paladin chết, người chơi nhận một năng lực mới (ví dụ trong brief: cánh → nhảy đôi) và có thông báo cho biết đã nhận.
- [ ] Năng lực có hiệu lực ngay trong game: người chơi dùng được nó sau trận boss.
- [ ] Trạng thái "đã mở khóa" được ghi nhớ trong phiên chơi (chưa cần lưu ra file).
- [ ] Năng lực chưa mở khóa thì không dùng được.

---

## Implementation Notes

- Chỉ làm đúng một năng lực. Việc kết hợp nhiều năng lực nằm ngoài phạm vi (xem Out of scope trong brief).
- Dùng một danh sách đơn giản các năng lực đã mở khóa; chưa cần hệ thống lưu game.

---

## Out of Scope

- Kết hợp năng lực, nhiều boss, lưu game (đã loại trong brief).

---

## Files

- `Assets/Scripts/Player/AbilityUnlocks.cs`
- `Assets/Tests/EditMode/AbilityUnlockTests.cs`

---

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above.*

---

## Test Evidence

**Story Type**: Logic
**Required evidence**: `Assets/Tests/EditMode/AbilityUnlockTests.cs` (waived at `qa.level: minimal`, advisory only)

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: `production/epics/heretic/story-002-paladin-boss.md`
- Unlocks: None
