# Deploy StayMaster (backend + frontend)

Áp dụng cho các nhánh `feat/srs-day1` → `feat/srs-day3`. Làm theo đúng thứ tự.

## 0. Việc bắt buộc trước khi deploy

1. **Đổi khoá JWT.** Khoá cũ đã nằm công khai trong lịch sử git. Tạo khoá mới (`openssl rand -base64 48`) và đặt vào
   `Jwt:SecretKey` trên server. Mọi người dùng sẽ phải đăng nhập lại; đó là điều mong muốn.
2. **Đổi API key SePay** trong trang quản trị SePay, rồi đặt key mới vào `SePay:ApiKey` và cấu hình webhook bên SePay.
3. **Đổi mật khẩu tài khoản `admin@staymaster.com`** nếu tài khoản này có trên DB thật. Code cũ tạo nó với mật khẩu
   `Admin123!` viết cứng trong repo công khai. Code mới không còn tạo tài khoản này.
4. **Sao lưu DB thật** trước khi chạy migration.

## 1. Cấu hình backend trên server

`appsettings.json` không chứa khoá bí mật. Trên server, tạo `appsettings.Production.json` cạnh file `.dll`
(file này đã bị gitignore, không commit), hoặc đặt biến môi trường (dùng `__` thay cho `:`):

```json
{
  "ConnectionStrings": { "DefaultConnection": "<chuỗi kết nối DB thật>" },
  "Jwt": { "SecretKey": "<khoá mới ≥ 32 byte>" },
  "App": {
    "FrontendUrl": "https://staymaster-v1.vercel.app",
    "CorsOrigins": ["https://staymaster-v1.vercel.app"],
    "SupportEmail": "<email hỗ trợ>",
    "SupportPhone": "<hotline>"
  },
  "SePay": {
    "ApiKey": "<key mới>",
    "BankId": "<mã ngân hàng của tài khoản gắn SePay, vd. Sacombank, MB>",
    "AccountNumber": "<số tài khoản>",
    "AccountName": "<tên chủ tài khoản>"
  },
  "Ai": { "Provider": "deepseek", "Model": "deepseek-flash", "ApiKey": "<DeepSeek API key>", "TimeoutSeconds": 5 }
}
```

Biến môi trường tương đương cho phần AI: `Ai__Provider=deepseek`, `Ai__Model=deepseek-flash`, `Ai__ApiKey=<key>`.
`Ai:Provider` nhận `deepseek`, `anthropic`, `gemini` hoặc `offline`.

- Thiếu `ConnectionStrings:DefaultConnection` hoặc `Jwt:SecretKey` thì API dừng ngay khi khởi động và báo rõ lý do.
- **Email không bắt buộc.** Mọi thông báo (đơn mới, đã xác nhận, bị huỷ, thanh toán, chủ nhà trả lời, rút tiền) đều hiện
  trong chuông thông báo. Không có `Email` thì không gửi email; người quên mật khẩu liên hệ hỗ trợ và admin cấp mật khẩu
  tạm ở trang **Người dùng** (nút chìa khoá). Muốn gửi thêm email thì thêm:
  `"Email": { "SmtpHost": "smtp.gmail.com", "SmtpPort": 587, "UseSsl": true, "Username": "<gmail>", "Password": "<App Password>", "FromAddress": "<gmail>", "FromName": "StayMaster" }`
  — khi đó trang Quên mật khẩu tự chuyển sang gửi link qua email.
- Bỏ trống `Ai:Provider` thì chatbot luôn trả lời thay thế (kèm số hotline / email hỗ trợ).
- Mỗi tin nhắn chatbot là một lượt gọi API AI có tính phí; giới hạn 20 tin nhắn/phút/IP đã bật sẵn.

## 2. Migration DB

Chạy lần lượt trên DB thật (mỗi file kiểm tra `__EFMigrationsHistory` nên chạy lại không sao):

1. `database/migrations/2026-11-16_AddPasswordResetTokens.sql` — bảng `ma_dat_lai_mat_khau`
2. `database/migrations/2026-11-16_AddInboxQuestions.sql` — bảng `hop_thu_cau_hoi`
3. `database/migrations/2026-11-16_AddInboxMessages.sql` — bảng `hop_thu_tin_nhan` (cuộc trò chuyện khách ↔ chủ nhà), chuyển câu hỏi/trả lời cũ thành tin nhắn
4. `database/migrations/2026-11-17_Round1Features.sql` — đánh giá có trả lời / ẩn (`danh_gia`), cột `duong_dan` cho `thong_bao`,
   lý do từ chối và giá cuối tuần (`phong`), bảng `ngay_chan_phong` và `cau_hinh_he_thong`

Hoặc từ máy có `dotnet-ef` và kết nối được DB: `dotnet ef database update`.
Bảng `tien_ich` / `phong_tien_ich` (tiện nghi) đã có sẵn trong DB, không cần tạo.

## 3. Build và upload backend

```bash
dotnet publish -c Release -o out
```

- Upload nội dung thư mục `out/`. Kiểm tra có thư mục `out/Fonts/`: thiếu nó thì hoá đơn PDF lỗi.
- **Không ghi đè `wwwroot/uploads/`** trên server: đó là ảnh phòng người dùng đã tải lên.
- Không commit `out/` hay `publish/` vào git.

## 4. Deploy frontend (Vercel)

- Biến môi trường: `NEXT_PUBLIC_API_URL=https://<domain backend>/api`. **Không** đặt `NEXT_PUBLIC_DEV_TOKEN` ở production.
- Tuỳ chọn: `NEXT_PUBLIC_SUPPORT_EMAIL`, `NEXT_PUBLIC_SUPPORT_PHONE` (email / số hỗ trợ in trên các trang Liên hệ, chính
  sách). Phí dịch vụ, thuế, thời gian giữ phòng và liên hệ trong ứng dụng sửa ở trang admin **Cấu hình**, không cần
  deploy lại.
- Deploy ngay sau backend. Frontend cũ không chạy đúng với backend mới: trang thanh toán cũ hỏi trạng thái mà không
  gửi token (giờ bị 401), và trang tìm kiếm cũ đọc danh sách đơn của admin.
- Domain frontend phải nằm trong `App:CorsOrigins` của backend.

## 5. Kiểm tra sau deploy (thủ công, trên môi trường thật)

**Không chạy `scripts/smoke_test.py` trên server thật**: script tạo tài khoản và đơn thử.

- [ ] Mở `/api/users` khi chưa đăng nhập → 401.
- [ ] Đăng nhập bằng email và bằng số điện thoại.
- [ ] "Quên mật khẩu" → nhận email trong vài giây → đặt mật khẩu mới.
- [ ] Đặt một phòng → nhận email "Đã nhận đơn"; chuyển khoản đúng nội dung `BK<mã đơn>` → đơn tự xác nhận.
- [ ] Đối tác check-in → check-out → khách tải được hoá đơn PDF.
- [ ] Chatbot trả lời một câu tìm phòng trong vòng 5 giây; hỏi "có cho mang thú cưng không" thì khung chat chuyển sang nhắn với chủ nhà,
      chủ nhà trả lời trong Inbox và khách thấy ngay trong khung chat.
- [ ] Admin: trang Báo cáo doanh thu và Hòm thư hỗ trợ có dữ liệu.
