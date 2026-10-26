# Kế hoạch 49 — Ổn định Build .NET MAUI

**Ngày:** 2026-10-25  
**Phạm vi:** `mobile-project/dotnet-maui-ver/`

## Bối cảnh

Windows target đã build thành công, nhưng Android target thất bại trong AAPT2 vì đường dẫn build trung gian vượt ngưỡng xử lý đường dẫn của Windows. Tài liệu frontend cũng chỉ tới port `5000`, trái với backend và mã nguồn dùng `5018`.

## Thay đổi

1. Ghim SDK .NET 10 bằng `global.json` để mọi máy dùng cùng feature band.
2. Đặt `BaseIntermediateOutputPath` riêng cho Android vào thư mục Temp của người dùng, ngăn AAPT2 gặp đường dẫn build trung gian quá sâu trên Windows.
3. Đồng bộ tài liệu frontend với port `5018`, bổ sung prerequisites và lệnh Android CLI có `BuildingForAndroid=true`.

## Tiêu chí nghiệm thu

- Windows build vẫn thành công với `net10.0-windows10.0.19041.0`.
- Android restore/build dùng target `net10.0-android` và không còn lỗi AAPT2 liên quan `fileprovider_file_paths.xml.flat`.
- README và hướng dẫn frontend không còn tham chiếu tới port `5000`.
