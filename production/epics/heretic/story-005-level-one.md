# Story 005: Một màn cuộn ngang có chướng ngại

> **Epic**: Heretic
> **Status**: Ready
> **Layer**: Feature
> **Type**: Integration
> **Estimate**: M
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: —

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

- `Assets/Scenes/Level01.unity`
- `Assets/Scripts/Level/CameraFollow.cs`
- `Assets/Scripts/Level/Hazard.cs`
- `Assets/Tests/PlayMode/Level01Tests.cs`

---

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
