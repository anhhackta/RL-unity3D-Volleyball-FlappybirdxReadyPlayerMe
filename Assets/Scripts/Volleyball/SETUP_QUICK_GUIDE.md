# 🎮 Setup Đơn Giản Game Mode Panel

## Bước 1: Tạo GameModeManager
1. **Tạo Empty GameObject** tên "GameModeManager"
2. **Add Component** `GameModeManager`
3. **Gán:**
   - Blue Agent: VolleyballAgent màu xanh
   - Purple Agent: VolleyballAgent màu tím
   - Game Mode Panel: Panel có sẵn của bạn
   - AI Vs Ai Button: Button AI vs AI
   - AI Vs Player Button: Button AI vs Player  
   - Close Button: Button đóng
4. **Settings:** ✅ Show Game Mode On Start

## Bước 2: Setup Panel
1. **Ẩn panel** trong scene (SetActive = false)
2. **Khi play** → Panel sẽ tự động hiện
3. **Chọn mode** → Panel đóng → Game bắt đầu

## Kết Quả
- **Khi start**: Panel hiện, game pause
- **Chọn AI vs AI**: Xem 2 AI chơi
- **Chọn AI vs Player**: Điều khiển Purple agent (WASD + Space)
- **Nhấn M**: Mở lại panel bất cứ lúc nào
