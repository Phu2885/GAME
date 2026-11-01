# Test Evidence: Nhân vật chạy, nhảy, dash, chém, chết

> **Story**: `production/epics/heretic/story-003-player-movement.md`
> **Story Type**: Visual/Feel
> **Date**: 2026-11-02
> **Tester**: Min (project owner)
> **Build / Commit**: 9349fd8 + uncommitted story 003 changes (Unity 6000.3.27f2, scene `Assets/Scenes/PlayerTest.unity`)

---

## What Was Tested

Nhân vật thật chạy trong scene thử `PlayerTest` (sàn, tường, hai bia tập): chạy, nhảy, dash, chém, mất máu và chết, dùng sprite sheet `demon_sheet.png` đã cắt thành 14 khung. Người chơi thử trực tiếp trong Unity và xác nhận mọi thứ chạy đúng; hai ảnh chụp giữ lại bên dưới.

**Acceptance criteria covered**: AC-1 đến AC-7 (xem bảng).

---

## Acceptance Criteria Results

| # | Criterion (from story) | Result | Notes |
|---|----------------------|--------|-------|
| AC-1 | Nhân vật chạy trái/phải bằng WASD (A/D) và nhảy (W hoặc Space), có phản hồi nhanh, không trễ. | PASS | Người chơi xác nhận khi chơi thử; ảnh 1 chụp lúc đang chạy |
| AC-2 | Dash: bấm phím dash thì lướt nhanh một đoạn ngắn theo hướng đang quay, có thời gian hồi. | PASS | Ảnh 1 thấy khung dash với vệt tốc độ |
| AC-3 | Chém: bấm phím tấn công thì đánh ra một cú, gây sát thương cho kẻ địch trong tầm. | PASS | Ảnh 1 thấy bia tập nháy trắng khi bị chém |
| AC-4 | Nhân vật có thanh máu; hết máu thì chạy hoạt ảnh chết và người chơi thua. | PASS | Ảnh 2: Health 0/5, thanh máu trống, nhân vật nằm xuống, "YOU DIED" |
| AC-5 | Sprite `demon_sheet.png` được cắt thành từng khung (Multiple, 32×48) và dùng cho hoạt ảnh đứng, chạy, nhảy. | PASS | File `.meta` có đúng 14 sprite `demon_0`…`demon_13`; nhân vật hiện đúng hình trong cả hai ảnh |
| AC-6 | Tốc độ, lực nhảy, khoảng dash, thời gian hồi là các giá trị chỉnh được. | PASS | Là trường `[SerializeField]` trên `PlayerTestBootstrap`, lưu trong scene |
| AC-7 | Mô hình va chạm dùng Unity Physics 2D, không dùng PhysX 3D. | PASS | `Rigidbody2D`, `BoxCollider2D`, `Physics2D.OverlapBox` |

---

## Screenshots / Video

| # | Filename | What It Shows | Acceptance Criterion |
|---|----------|--------------|----------------------|
| 1 | `story-003-player-movement/01-run-dash-and-hit-dummy.webp` | Cửa sổ Unity đang chạy PlayerTest: nhân vật ở khung dash (vệt tốc độ) sát bia tập đang nháy trắng khi bị chém, Health 5/5 | AC-1, AC-2, AC-3 |
| 2 | `story-003-player-movement/02-death-you-died.webp` | Health 0/5, thanh máu trống, nhân vật nằm xuống, "YOU DIED / Press R to restart" | AC-4 |

---

## Test Conditions

- **Game state at start**: scene `PlayerTest` mới, đầy máu, hai bia tập đứng yên
- **Platform / hardware**: macOS, Unity 6000.3.27f2 Editor (Metal), cửa sổ Game scale 2x
- **Framerate during test**: không đo
- **Any special setup required**: phím K tự trừ 1 máu để xem hoạt ảnh chết (không có kẻ địch gây sát thương trong scene này)

---

## Observations

- Các khung dash, chém, chết là hình tạm vẽ bằng code, khung chết còn thô.
- Nhân vật lệch nhẹ khoảng 0,1 đơn vị khi quay mặt, vì hình không nằm giữa khung 32 px (chỉ là thẩm mỹ).
- Cảm giác điều khiển (độ nhanh nhạy, trọng lượng cú nhảy) do người chơi đánh giá khi chơi trực tiếp; ảnh tĩnh không chứng minh được phần này.

---

## Sign-Off

| Role | Name | Date | Signature |
|------|------|------|-----------|
| Lead (art-director / designer) | Min | 2026-11-02 | [x] Approved |

---

*Template: `.claude/docs/templates/test-evidence.md`*
*Used for: Visual/Feel and UI story type evidence records*
*Location: `production/qa/evidence/[story-slug]-evidence.md`*
