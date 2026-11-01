# Story 003: Nhân vật chạy, nhảy, dash, chém, chết

> **Epic**: Heretic
> **Status**: Complete
> **Layer**: Core
> **Type**: Visual/Feel
> **Estimate**: M
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: 2026-11-02

## Context

**GDD**: `design/game-brief.md`
**Requirement**: Brief MVP feature 1

**ADR Governing Implementation**: N/A (minimal — no ADRs)
**ADR Decision Summary**: N/A (minimal — no ADRs)
**ADR Version**: N/A (minimal — no ADRs)

**Engine**: Unity 6000.3 | **Risk**: HIGH
**Engine Notes**: none (no ADR engine-compatibility analysis at minimal)

**Control Manifest Rules (this layer)**: N/A (minimal — no control manifest)

---

## Acceptance Criteria

*From the brief (Core loop; Player goal & fail state), scoped to this story:*

- [ ] Nhân vật chạy trái/phải bằng WASD (A/D) và nhảy (W hoặc Space), có phản hồi nhanh, không trễ.
- [ ] Dash: bấm phím dash thì lướt nhanh một đoạn ngắn theo hướng đang quay, có thời gian hồi.
- [ ] Chém: bấm phím tấn công thì đánh ra một cú, gây sát thương cho kẻ địch trong tầm.
- [ ] Nhân vật có thanh máu; hết máu thì chạy hoạt ảnh chết và người chơi thua.
- [ ] Sprite `Assets/Art/Characters/Imp/demon_sheet.png` được cắt thành từng khung (Sprite Mode Multiple, 32×48) và dùng cho hoạt ảnh đứng, chạy, nhảy.
- [ ] Tốc độ, lực nhảy, khoảng dash, thời gian hồi là các giá trị chỉnh được.
- [ ] Mô hình va chạm dùng Unity Physics 2D, không dùng PhysX 3D.

---

## Implementation Notes

- Ảnh đã được `PixelArtImporter` đặt Point, 32 PPU, không nén; chỉ còn cắt khung.
- Chưa có khung dash, chém và chết trong sprite sheet; dùng hình tạm hoặc bổ sung khung.
- Nhân vật quay mặt theo hướng di chuyển (sprite gốc quay sang phải).

---

## Out of Scope

- Story 001: slow-motion/QTE.
- Story 004: kẻ địch (ở đây chỉ cần bia tập để thử cú chém).

---

## Files

- `Assets/Scripts/Player/PlayerController.cs`
- `Assets/Scripts/Player/PlayerHealth.cs`
- `Assets/Scripts/Player/PlayerAnimator.cs`
- `Assets/Scripts/Player/TrainingDummy.cs` (test target for the slash; story 004 makes real enemies)
- `Assets/Scripts/Player/PlayerTestBootstrap.cs` (builds the test arena and holds the tunable settings)
- `Assets/Scripts/Combat/IDamageable.cs` (shared target interface)
- `Assets/Editor/DemonSheetSlicer.cs` (cuts the sheet into 14 sprites)
- `Assets/Art/Characters/Imp/demon_sheet.png` (+ `.meta`): dash, attack and death frames added, now 14 frames
- `Assets/Scenes/PlayerTest.unity` (test scene built by the lead)

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above.*

---

## Test Evidence

**Story Type**: Visual/Feel
**Required evidence**: một ảnh chụp giữ lại trong `production/qa/evidence/` + xác nhận trong `production/qa/evidence/story-003-player-movement-evidence.md` (bắt buộc kể cả ở `qa.level: minimal`)

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: None
- Unlocks: `production/epics/heretic/story-004-enemies.md`, `production/epics/heretic/story-005-level-one.md`

## Completion Notes
**Completed**: 2026-11-02
**Criteria**: 7/7 passing (none deferred)
**Deviations**: None advisory. Out of scope, agreed: the real player runs in its own test scene `PlayerTest` and is not yet in the boss arena (story 005 assembles it); extra files `TrainingDummy`, `PlayerTestBootstrap`, `IDamageable`, `DemonSheetSlicer`, `PlayerTest.unity` and an updated `design/art/demon-preview.png`. The dash, attack and death frames are placeholder art drawn in code.
**Test Evidence**: waived at qa.level minimal for tests; screenshots at `production/qa/evidence/story-003-player-movement/` and signed-off record at `production/qa/evidence/story-003-player-movement-evidence.md`
**Reviewed by**: self, no independent review
