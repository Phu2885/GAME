# Game Brief: Heretic

**One-sentence pitch:** Một tiểu quỷ chạy-nhảy-dash qua thế giới trung cổ do Giáo hội thống trị, và trong mỗi trận boss Paladin, thời gian chậm lại để bạn bấm đúng chuỗi mũi tên, parry đòn và đoạt lại bộ phận quỷ bị chúng biến thành vũ khí.

## Core loop
- Đi qua màn: chạy, nhảy, dash, tránh chướng ngại, chém quái nhỏ.
- Gặp boss Paladin: né đòn thường; khi hắn tung chiêu mạnh thì slow-motion và hiện chuỗi mũi tên.
- Bấm đúng: parry (chuỗi ngắn) hoặc phản đòn gây sát thương (chuỗi dài). Bấm sai: mất máu.
- Hạ boss: lấy lại một bộ phận quỷ, mở một năng lực mới, rồi sang màn kế.

## Player goal & fail state — what "working" looks like
- Mục tiêu: hạ hết Paladin, thu hồi sức mạnh của loài quỷ. MVP: hạ được 1 boss.
- Thua khi hết máu (trong màn hoặc trong trận boss); chết thì quay lại đầu màn hoặc đầu trận boss.

## MVP — what must exist to be the game
1. Nhân vật: chạy, nhảy, dash, chém, hoạt ảnh chết (WASD + phím tấn công)
2. Một màn cuộn ngang có chướng ngại vật
3. Hai loại quái: một bay, một không bay
4. Một boss Paladin có mẫu đòn đánh đọc được
5. Slow-motion khi boss tung chiêu mạnh + QTE chuỗi mũi tên (ngắn = parry, dài = phản đòn)
6. Hạ boss thì nhận một năng lực mới (ví dụ: cánh → nhảy đôi)

## Out of scope — not building this
- Kết hợp nhiều năng lực với nhau, nhiều hơn 1 boss, nhiều hơn 1 màn
- Cốt truyện dài, tẩy não thường dân, giáo hoàng, cắt cảnh
- Co-op, đa người chơi, hệ thống lưu game phức tạp

## Build order
1. Boss + slow-motion + QTE parry (phần rủi ro và là lõi vui của game): thử trên một phòng trống
2. Di chuyển nhân vật (chạy, nhảy, dash, chém)
3. Quái nhỏ + chướng ngại, rồi ghép thành một màn
4. Năng lực nhận sau boss

---
**Who it's for / what they feel:** Người thích game thử thách kiểu Cuphead và Celeste; cảm giác nghẹt thở rồi hả hê khi parry đúng nhịp.

**Art & audio direction:** Pixel-art 2D, trung cổ tối (xanh đen), nhạc nền dồn dập kiểu hợp xướng thánh ca bị bóp méo.

**Reference game:** Cuphead (trận boss đọc đòn) + Hollow Knight (hạ boss mở năng lực), chỉ giữ phần đánh boss và một năng lực mở khóa.
