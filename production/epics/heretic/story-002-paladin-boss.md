# Story 002: Boss Paladin có mẫu đòn đọc được

> **Epic**: Heretic
> **Status**: Complete
> **Layer**: Feature
> **Type**: Integration
> **Estimate**: L
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: 2026-11-02

## Context

**GDD**: `design/game-brief.md`
**Requirement**: Brief MVP feature 4

**ADR Governing Implementation**: N/A (minimal — no ADRs)
**ADR Decision Summary**: N/A (minimal — no ADRs)
**ADR Version**: N/A (minimal — no ADRs)

**Engine**: Unity 6000.3 | **Risk**: HIGH
**Engine Notes**: none (no ADR engine-compatibility analysis at minimal)

**Control Manifest Rules (this layer)**: N/A (minimal — no control manifest)

---

## Acceptance Criteria

*From the brief (Core loop; Player goal & fail state), scoped to this story:*

- [ ] Có một boss Paladin trong một phòng đấu riêng, có thanh máu.
- [ ] Boss có ít nhất 2 đòn thường và 1 đòn mạnh, mỗi đòn có dấu hiệu báo trước (ánh sáng/hoạt ảnh) để người chơi đọc được.
- [ ] Đòn thường: người chơi né được bằng nhảy hoặc dash; trúng thì mất máu.
- [ ] Đòn mạnh kích hoạt slow-motion và QTE của story 001.
- [ ] Parry thành công: đòn bị đỡ, người chơi không mất máu. Phản đòn thành công: boss mất máu. Thất bại: người chơi mất máu.
- [ ] Boss hết máu thì trận kết thúc thắng; người chơi hết máu thì thua và quay lại đầu trận boss.
- [ ] Mẫu đòn, tốc độ và sát thương là các giá trị chỉnh được.

---

## Implementation Notes

- Dùng máy trạng thái đơn giản (chờ, báo trước, tấn công, hồi) cho boss; chưa cần AI phức tạp.
- Dùng placeholder cho sprite boss; art thật làm sau.

---

## Out of Scope

- Story 001: cơ chế slow-motion/QTE (chỉ gọi lại).
- Story 006: phần thưởng sau khi hạ boss.

---

## Files

- `Assets/Scripts/Boss/PaladinBoss.cs`
- `Assets/Scripts/Boss/TempPlayer.cs` (temporary player stand-in; story 003 replaces it)
- `Assets/Scripts/Boss/BossArenaBootstrap.cs` (builds the arena at runtime and holds the tunable settings)
- `Assets/Scenes/BossArena.unity` (test scene built by the lead, not the programmer)

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above.*

---

## Test Evidence

**Story Type**: Integration
**Required evidence**: `Assets/Tests/PlayMode/PaladinFightTests.cs` OR a playtest doc (waived at `qa.level: minimal`, advisory only)

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: `production/epics/heretic/story-001-slowmo-qte.md`
- Unlocks: `production/epics/heretic/story-006-ability-reward.md`

## Completion Notes
**Completed**: 2026-11-02
**Criteria**: 7/7 passing (none deferred)
**Deviations**: Advisory: the boss start position (`_arenaRight - 3f` in `ResetFight`) is hard-coded, not tunable. Out of scope, approved: temporary player stand-in `TempPlayer` and `BossArenaBootstrap` (story 003 replaces the stand-in); the heavy attack ignores dash invulnerability by design. A lunge bug (boss stuck at the arena edge from a float rounding mismatch) was found in play and fixed.
**Test Evidence**: waived at qa.level minimal; screenshot at `production/qa/evidence/story-002-paladin-boss/01-heavy-qte-counter.png`
**Reviewed by**: self, no independent review
