# Story 001: Slow-motion và QTE chuỗi mũi tên

> **Epic**: Heretic
> **Status**: Complete
> **Layer**: Core
> **Type**: Logic
> **Estimate**: L
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: 2026-11-02

## Context

**GDD**: `design/game-brief.md`
**Requirement**: Brief MVP feature 5

**ADR Governing Implementation**: N/A (minimal — no ADRs)
**ADR Decision Summary**: N/A (minimal — no ADRs)
**ADR Version**: N/A (minimal — no ADRs)

**Engine**: Unity 6000.3 | **Risk**: HIGH
**Engine Notes**: none (no ADR engine-compatibility analysis at minimal)

**Control Manifest Rules (this layer)**: N/A (minimal — no control manifest)

---

## Acceptance Criteria

*From the brief (Core loop; MVP feature 5), scoped to this story:*

- [ ] Một sự kiện "đòn mạnh của boss" làm thời gian chậm lại (`Time.timeScale` thấp, giá trị chỉnh được) trong một cửa sổ thời gian hữu hạn, rồi trở về bình thường.
- [ ] Trong slow-motion, một chuỗi mũi tên ngẫu nhiên hiện ra trên màn hình; chuỗi **ngắn** (3–4 phím) dùng để parry, chuỗi **dài** (6–8 phím) dùng để phản đòn.
- [ ] Bấm đúng toàn bộ chuỗi trước khi hết cửa sổ thì thành công: phát sự kiện `ParrySucceeded` (chuỗi ngắn) hoặc `CounterSucceeded` (chuỗi dài).
- [ ] Bấm sai một phím thì quay về phím đầu của chuỗi, đồng hồ vẫn chạy (không thua ngay). Hết giờ mà chưa xong thì thất bại: phát sự kiện `QteFailed` và người chơi mất máu (đủ để chạy thử bằng một nhân vật giả có thanh máu).
- [ ] Độ dài chuỗi, số phím, cửa sổ thời gian và hệ số chậm là các giá trị chỉnh được (ScriptableObject hoặc trường Inspector), không hard-code.
- [ ] Chuỗi mũi tên dùng phím mũi tên, độc lập với WASD để di chuyển; đầu vào được đọc bằng thời gian thực, không bị `timeScale` làm chậm.
- [ ] Thử được trong một phòng trống có boss giả bấm nút để kích hoạt đòn mạnh.

---

## Implementation Notes

- Đây là phần rủi ro nhất của game (xem Build order trong brief): làm trước, chứng minh là vui rồi mới làm tiếp.
- Tách logic chuỗi phím (sinh chuỗi, kiểm tra từng phím, kết quả) ra một lớp C# thuần, không phụ thuộc `MonoBehaviour`, để kiểm thử được bằng Edit Mode.
- Dùng thời gian thực (`Time.unscaledDeltaTime`) cho cửa sổ QTE.
- Dùng Input System có sẵn trong template (`Assets/Settings/InputSystem_Actions.inputactions`) cho các phím mũi tên.
- Unity 6.3 nằm ngoài phần mô hình được học: kiểm API trong `docs/engine-reference/unity/` trước khi dùng.

---

## Out of Scope

- Story 002: boss thật và mẫu đòn của nó.
- Story 003: nhân vật thật (ở đây chỉ cần nhân vật giả có thanh máu).

---

## Files

- `Assets/Scripts/Combat/QteSequence.cs`
- `Assets/Scripts/Combat/SlowMotionController.cs`
- `Assets/Scripts/Combat/QteTestRoom.cs` (test harness, added beyond the planned list for criterion 7)

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above.*

---

## Test Evidence

**Story Type**: Logic
**Required evidence**: `Assets/Tests/EditMode/QteSequenceTests.cs` (tests are waived at `qa.level: minimal`, advisory only)

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: None
- Unlocks: `production/epics/heretic/story-002-paladin-boss.md`

## Completion Notes
**Completed**: 2026-11-02
**Criteria**: 7/7 passing (none deferred)
**Deviations**: None advisory. Out of scope: test scene `Assets/Scenes/QteTestRoom.unity`; Unity-generated URP/project settings files; wrong key now restarts the sequence instead of failing (user decision, story and brief updated)
**Test Evidence**: waived at qa.level minimal; screenshot at `production/qa/evidence/story-001-slowmo-qte/01-qte-arrows-and-timer.png`
**Reviewed by**: self, no independent review
