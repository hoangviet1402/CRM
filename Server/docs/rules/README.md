# Development Rules

Tài liệu này chứa các quy tắc phát triển cho dự án.

## Danh sách Rules

1. **[myrukr.mdc](./myrukr.mdc)**
   - Quy tắc tạo module/tính năng mới
   - Cấu trúc thư mục chuẩn
   - Mối quan hệ giữa các layers
   - Gợi ý tích hợp tools

2. **[storeprocedure.mdc](./storeprocedure.mdc)**
   - Quy tắc thực thi Stored Procedure
   - Pattern cho SP đơn giản và phức tạp
   - Quy tắc đặt tên và xử lý params
   - Xử lý connection và error handling

## Cách sử dụng

1. Khi tạo tính năng mới:
   - Tham khảo `myrukr.mdc`
   - Tạo cấu trúc thư mục theo hướng dẫn
   - Đặt tên namespace theo quy tắc
   - Tích hợp các tools được đề xuất

2. Khi làm việc với Database:
   - Tham khảo `storeprocedure.mdc`
   - Chọn pattern phù hợp (đơn giản/phức tạp)
   - Tuân thủ quy tắc đặt tên
   - Xử lý lỗi theo hướng dẫn

## Cập nhật Rules

Khi cần thêm hoặc cập nhật rules:
1. Tạo file `.mdc` mới trong thư mục `rules`
2. Cập nhật file README.md
3. Thông báo cho team về thay đổi 