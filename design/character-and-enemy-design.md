# HERETIC — BẢN THIẾT KẾ CHI TIẾT NHÂN VẬT & KẺ THÙ
# PHONG CÁCH: BLEACH VS NARUTO (BVN 2D ANIME PIXEL FIGHTING)

---

## 1. TIÊU CHUẨN ĐỒ HỌA CHUNG (ART STYLE GUIDE)
- Phong cách: 2D Fighting Game Pixel Art (Bleach vs Naruto / Jump Ultimate Stars).
- Tỷ lệ cơ thể: 5 đến 5.5 đầu (cân đối phong cách anime hành động, không làm chibi lùn).
- Kỹ thuật đổ bóng: Cel-shading 3 tone màu (Highlight - Base - Shadow), viền outline tối màu.
- Dynamic Action: Chuyển động có độ co dãn (Squash & Stretch), vệt bóng mờ (Afterimage/Ghosting) khi Dash.
- Tương thích Unity Engine: 
  - Pivot điểm đặt chân: Bottom-Center (x=0.5, y=0.0).
  - Chuẩn Pixels Per Unit (PPU): 32 hoặc 64 (giữ đồng bộ cho toàn bộ spritesheet).

---

## 2. NHÂN VẬT CHÍNH: TIỂU QUỶ (THE HERETIC)

### 2.1. Chỉ số kích thước (Sprite Dimensions)
- Kích thước Canvas: 80 x 80 pixels.
- Chiều cao nhân vật: 
  - Đứng thẳng hoàn toàn: 64 pixels (~1.3 - 1.5 Unity world units).
  - Thế thủ chiến đấu (Combat Stance): 56 pixels.
- Chiều rộng thân: 28 - 32 pixels.
- Collider Box ước tính trong Unity: Size(0.6, 1.3), Offset(0, 0.65).

### 2.2. Đặc điểm nhận diện & Bảng màu (Color Palette)
- Làn da (Skin): 
  - Tone chủ đạo: Màu đỏ thẫm huyết quỷ (Crimson Red: `#8B1A1A`).
  - Đổ bóng tối: Đỏ thẫm bóng đêm (`#5A0E0E`).
  - Điểm sáng (Highlight): Đỏ tươi gân guốc (`#B82E2E`).
- Mái tóc (Hair): 
  - Màu sắc: Trắng tuyết ánh bạc (`#F0F4F8`), bóng xám xanh (`#9AA5B4`).
  - Kiểu dáng: Tóc dài chấm vai, xõa bung sau lưng. Phần mái trước tỉa nhọn che một phần trán theo phong cách anime sát thủ, bay bồng bềnh theo chuyển động.
- Đôi mắt (Eyes): 
  - Màu sắc: Xanh lam dạ quang phát sáng (Cyan Neon: `#00F0FF`).
  - Hình dáng: Mắt hẹp dài, đồng tử sắc sảo, phát sáng rõ rệt kể cả trong bóng tối ngục tù.
- Cặp sừng (Horns): 
  - Hai sừng quỷ nhô từ trán cong nhẹ ngược ra sau đỉnh đầu.
  - Màu đen pha đỏ thẫm, có đường nứt phát sáng li ti chạy dọc thân sừng.
- Đôi tai (Ears): Tai nhọn dài vểnh ngang (Elven/Demon ears).
- Chiếc đuôi (Tail): 
  - Đuôi quỷ đỏ dài, chóp nhọn hình mũi tên/kim cương màu đen.
  - Cử động linh hoạt theo từng bước chân để giữ thăng bằng.
- Trang phục (Costume): 
  - Áo sát nách rách tơi tả màu xám tro hoặc quấn băng đen quanh ngực/bụng (dấu tích tù ngục Giáo hội).
  - Cổ tay quấn băng vải sờn và mang mảnh xích sắt đứt của xiềng xích cổ đại.
  - Quần đùi chiến đấu màu đen nhám, ống chân quấn xà cạp đen.
- Đôi cánh (Sự tiến hóa ngoại hình qua Story):
  - Giai đoạn 1 (Story 001 - 005): Sau lưng chỉ có 2 vết sẹo sâu đỏ rực (đôi cánh đã bị Giáo hội chặt đứt để rèn vũ khí).
  - Giai đoạn 2 (Sau khi hạ Boss - Story 006): Đôi cánh dơi xương quỷ bốc cháy lửa xanh/đỏ mọc bung ra khỏi lưng -> Mở khóa vĩnh viễn kỹ năng Nhảy đôi (Double Jump).

### 2.3. Bảng cử động chi tiết (Animation Frame Spec - BVN Style)
1. Combat Idle (Thế đứng thủ - 6 frames):
   - Chân trước chân sau, đầu gối chùng thấp, hai tay co trước ngực thủ thế.
   - Thân mình nhấp nhô theo nhịp thở; tóc và đuôi đung đưa nhịp nhàng.
2. Run (Chạy nước rút - 8 frames):
   - Thân mình đổ dốc 45 độ về phía trước (kiểu chạy ninja/BVN).
   - Chân guồng tròn xé gió; tóc bay dạt ra sau; bụi đất pixel bốc lên ở gót chân.
3. Jump & Fall (3 frames):
   - Frame 1: Co người nhún chân tích lực bật nhảy.
   - Frame 2: Vươn người lướt lên không trung (Rising).
   - Frame 3: Thả lỏng người rơi xuống (Falling), áo và tóc bay dựng ngược lên trên.
4. Dash (Lướt gió - 4 frames):
   - Người thu gọn như mũi tên lao thẳng phía trước không trọng lực.
   - Để lại 2-3 bóng mờ đỏ (Afterimages) tan dần phía sau lưng.
5. Attack Combo (Chém vuốt/kiếm quỷ - 6 frames chia 3 phase):
   - Phase 1 (Windup - 2 frames): Nghiêng người lấy đà ra sau, mắt lóe sáng tia cyan.
   - Phase 2 (Strike - 2 frames): Vung tay chém cực nhanh, xuất hiện lưỡi liềm chém đỏ rực (Slash FX) hình bán nguyệt rộng hơn cơ thể.
   - Phase 3 (Recovery - 2 frames): Thu đà, gạt tay về tư thế thủ.
6. Death (Tử trận - 5 frames):
   - Bị bật lùi ra sau, mắt tắt ánh sáng xanh, ngã quỵ xuống đất và cơ thể tan biến thành tàn tro đỏ bốc lên không trung.

---

## 3. KẺ THÙ THƯỜNG 1: QUÁI BỘ (GROUND ENEMY - TU SĨ CUỒNG TÍN)

### 3.1. Chỉ số kích thước
- Kích thước Canvas: 80 x 80 pixels.
- Chiều cao thực tế: 66 pixels.
- Chiều rộng thân: 34 pixels.

### 3.2. Ngoại hình & Trang bị
- Danh xưng: Zealot Inquisitor (Tu sĩ Thanh Trừng).
- Trang phục: 
  - Áo choàng tu sĩ màu nâu đất rách gấu, viền đỏ sẫm biểu trưng của Giáo hội cực đoan.
  - Trùm mũ kín mít (Hoodie), bên trong bóng tối chỉ lộ ra chiếc mặt nạ sắt hình đầu chim mỏ nhọn (Plague Doctor) với 2 lỗ mắt phát sáng đỏ rực căm phẫn.
- Vũ khí: Cầm một cây gậy sắt gắn dây xích mang quả chùy gai sắt ở đầu, kéo lê trên mặt đất tạo tia lửa khi di chuyển.

### 3.3. Hoạt cảnh chiến đấu
- Patrol Walk (6 frames): Bước đi lừ đừ, dáng lưng còng, tay kéo lê quả chùy gai kêu loảng xoảng.
- Turn Around (2 frames): Dừng lại, xoay áo choàng đổi hướng khi chạm tường hoặc gặp quái khác.
- Hitstun/Death (3 frames): Vỡ mặt nạ sắt, văng chùy gai, ngã sấp về phía trước và tan biến.

---

## 4. KẺ THÙ THƯỜNG 2: QUÁI BAY (FLYING ENEMY - DỰNG TƯỢNG ĐÁ THÁNH / GARGOYLE)

### 4.1. Chỉ số kích thước
- Kích thước Canvas: 80 x 80 pixels.
- Chiều cao thực tế: 42 pixels.
- Sải cánh: 56 pixels.

### 4.2. Ngoại hình & Cấu tạo
- Danh xưng: Holy Gargoyle / Cherubim Sa Đọa.
- Cơ thể: Tạc từ đá cẩm thạch trắng pha xám tro, có những đường nứt nẻ phát sáng ánh vàng thánh chiến (Holy Light).
- Đầu: Mặt tượng thiên thần bị vỡ nát một nửa, để lộ khung sọ quái thú bằng đá hung tợn với con mắt vàng rực giữa trán.
- Đôi cánh: Đôi cánh lông vũ đá nhọn hoắt như những lưỡi dao xếp lớp. Không có chân, phần thân dưới thon nhọn lơ lửng giữa không trung.

### 4.3. Hoạt cảnh chiến đấu
- Fly Cycle (4 frames): Đập cánh nhịp nhàng theo biên độ sóng sin (lượn sóng dập dềnh).
- Dive Attack (3 frames): Cúp cánh thu người, lao cắm đầu chéo góc 45 độ xuống vị trí người chơi với vệt sáng vàng sau đuôi.

---

## 5. BOSS: PALADIN ĐAO PHỦ (GRAND INQUISITOR PALADIN)

### 5.1. Chỉ số kích thước
- Kích thước Canvas: 120 x 120 pixels.
- Chiều cao thực tế: 102 pixels (cao gấp 1.6 lần nhân vật chính, tạo cảm giác áp bức khổng lồ).
- Chiều rộng thân: 50 - 65 pixels (khi cầm đại kiếm).

### 5.2. Ngoại hình & Thiết kế uy áp
- Giáp trụ (Armor): 
  - Bộ đại thiết giáp toàn thân (Full Gothic Plate Armor) màu bạc sáng chói, các đường gờ mạ vàng ròng chạm khắc chữ cổ thánh kinh.
  - Cặp giáp vai (Pauldrons) khổng lồ có các lưỡi dao nhọn nhô lên.
- Áo choàng: Áo choàng đỏ huyết dụ dài quét đất, rách tua rua ở đuôi áo, tung bay dữ dội theo từng thế đánh.
- Mũ trụ (Helm): Great Helm kín mít, khe nhìn hình chữ thập phát ra ánh lửa vàng kim cuồng thịnh (Holy Wrath).
- Đại kiếm (Relic Greatsword): 
  - Chiều dài kiếm: 85 pixels.
  - Lưỡi kiếm được rèn từ chính mảnh xương và đôi cánh quỷ bị phong ấn trong pha lê thánh, luôn phát ra tia sét vàng xen lẫn tia năng lượng đỏ bạo liệt.

### 5.3. Bộ chiêu thức & Hoạt cảnh (Animation & Telegraph Specs)
1. Combat Stance: Cắm mũi đại kiếm xuống đất, hai tay đặt lên đốc kiếm, áo choàng bay phần phật uy nghiêm.
2. Ground Slam (Đập đất nổ sóng chấn động):
   - Telegraph: Nhấc bổng đại kiếm qua đầu, lưỡi kiếm phát sáng chói lòa màu vàng rực, thân mình rung giật tích lực trong 0.8s.
   - Attack: Bổ sấm sét đại kiếm xuống đất -> Bùng nổ 2 cột sóng chấn động (Shockwaves) vàng rực cao 25px xé toạc mặt đất lao về 2 bên võ đài.
3. Lunge (Lướt kiếm xuyên màn hình):
   - Telegraph: Hạ thấp trọng tâm, đưa đại kiếm ra sau hông, mắt mũ lóe tia sáng cam đỏ trong 0.8s.
   - Attack: Lao vụt ngang toàn bộ võ đài với tốc độ cực nhanh (16 units/s), để lại vệt chém vàng rực cắt ngang màn hình.
4. Heavy Attack & QTE State (Chiêu tất sát):
   - Telegraph: Vung kiếm tích tụ vòng xoáy ánh sáng thánh bao quanh người trong 1.2s.
   - QTE Trigger: Toàn bộ màn hình chuyển sang Slow-motion, Paladin chém xuống nhát kiếm trừng phạt; người chơi bấm đúng chuỗi phím mũi tên thì sẽ có hoạt cảnh đao kiếm va chạm tóe lửa parry/phản đòn.
5. Defeated: Quỳ một gối gục ngã, chống kiếm run rẩy, giáp trụ nứt toác bốc khói đen, thanh kiếm vỡ vụn giải phóng đôi cánh quỷ bay về cơ thể nhân vật chính.

