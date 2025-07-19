# Hướng Dẫn Sử Dụng Scripts Volleyball

## Tổng Quan
Bộ script này cung cấp các tính năng xem game Volleyball với ML-Agents, **KHÔNG ẢNH HƯỞNG** đến logic AI:
- Chuyển đổi camera để xem từ các góc khác nhau
- Pause/Resume game để xem chi tiết
- Thoát về menu chính
- Chỉ để xem AI chơi, không can thiệp vào quá trình AI

## Các Script Chính

### 1. GameViewer.cs
**Chức năng:** Quản lý tất cả tính năng xem game (không ảnh hưởng AI)
**Phím tắt:** 
- R: Chuyển camera
- P: Pause/Resume
- Escape: Thoát về menu chính
**Cách sử dụng:**
- Gán các Camera vào mảng `cameras`
- Gán UI panels vào `pauseMenuUI` và `gameUI`
- Đặt tên scene menu chính vào `mainMenuSceneName`

## Cách Setup trong Unity

### Bước 1: Tạo GameObject cho Game Viewer
1. Tạo empty GameObject tên "GameViewer"
2. Thêm component `GameViewer`

### Bước 2: Setup Camera
1. Gán các Camera vào mảng `cameras` trong GameViewer
2. Đảm bảo có ít nhất 2 camera để chuyển đổi

### Bước 3: Setup UI (Tùy chọn)
1. Tạo UI panels cho pause menu và game UI
2. Gán vào `pauseMenuUI` và `gameUI` trong GameViewer
3. Đặt tên scene menu chính vào `mainMenuSceneName`

## Các Phím Tắt

| Phím | Chức Năng |
|------|-----------|
| R | Chuyển camera |
| P | Pause/Resume game |
| Escape | Thoát về menu chính |

## Lưu Ý Quan Trọng

1. **Scene Management:** Đảm bảo có scene "MainMenu" hoặc thay đổi tên trong `mainMenuSceneName`
2. **UI Setup:** Cần tạo các UI elements (Text, Button, Panel) trước khi gán vào scripts
3. **Camera Setup:** Cần có ít nhất 2 camera để sử dụng tính năng chuyển camera
4. **AI Unchanged:** Script này **KHÔNG ẢNH HƯỞNG** đến logic AI, chỉ để xem game
5. **Time Scale:** Script tự động xử lý `Time.timeScale` khi pause/resume

## Troubleshooting

### Lỗi thường gặp:
1. **"Scene MainMenu not found"**: Kiểm tra tên scene trong Build Settings
2. **"No cameras assigned"**: Gán camera vào GameViewer
3. **"UI elements not found"**: Tạo và gán các UI elements vào GameViewer

### Debug:
- Kiểm tra Console để xem các thông báo debug
- Đảm bảo GameViewer được gán đúng các references 