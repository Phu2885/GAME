# Epic: Heretic

> **Source**: `design/game-brief.md` (tier `minimal`: no GDD, no ADRs)
> **Status**: In progress

## Goal

Một tiểu quỷ chạy-nhảy-dash qua thế giới trung cổ do Giáo hội thống trị, và trong mỗi trận boss Paladin, thời gian chậm lại để bạn bấm đúng chuỗi mũi tên, parry đòn và đoạt lại bộ phận quỷ bị chúng biến thành vũ khí.

## Scope

Sáu mục MVP của brief. Ngoài phạm vi: kết hợp năng lực, nhiều hơn 1 boss hoặc 1 màn, cốt truyện dài, đa người chơi, hệ thống lưu game.

## Build order

1. Slow-motion + QTE + boss (rủi ro nhất, thử trước trên phòng trống)
2. Di chuyển nhân vật
3. Quái nhỏ + chướng ngại, ghép thành một màn
4. Năng lực nhận sau boss

## Stories

| # | Story | Type | Status | ADR |
|---|-------|------|--------|-----|
| 001 | Slow-motion và QTE chuỗi mũi tên | Logic | Ready | N/A |
| 002 | Boss Paladin có mẫu đòn đọc được | Integration | Ready | N/A |
| 003 | Nhân vật chạy, nhảy, dash, chém, chết | Visual/Feel | Ready | N/A |
| 004 | Hai loại quái (bay và không bay) | Logic | Ready | N/A |
| 005 | Một màn cuộn ngang có chướng ngại | Integration | Ready | N/A |
| 006 | Hạ boss nhận một năng lực mới | Logic | Ready | N/A |
