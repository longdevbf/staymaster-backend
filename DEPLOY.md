# Deploy StayMaster (backend + frontend)

Có hai cách: **Docker trên VPS** (mục D, gọn nhất: SQL Server + API + HTTPS trong một lệnh) hoặc host .NET có sẵn
như somee.com (mục 1–3). Frontend luôn deploy lên Vercel (mục 4). Làm mục 0 trước trong cả hai cách.

## 0. Việc bắt buộc trước khi deploy

1. **Đổi khoá JWT.** Khoá cũ đã nằm công khai trong lịch sử git. Tạo khoá mới (`openssl rand -base64 48`) và đặt vào
   `Jwt:SecretKey` trên server. Mọi người dùng sẽ phải đăng nhập lại; đó là điều mong muốn.
2. **Đổi API key SePay** trong trang quản trị SePay, rồi đặt key mới vào `SePay:ApiKey` và cấu hình webhook bên SePay.
3. **Đổi mật khẩu tài khoản `admin@staymaster.com`** nếu tài khoản này có trên DB thật. Code cũ tạo nó với mật khẩu
   `Admin123!` viết cứng trong repo công khai. Code mới không còn tạo tài khoản này.
4. **Sao lưu DB thật** trước khi chạy migration.

## D. Chạy bằng Docker trên VPS

Cần: VPS **x86_64** (image SQL Server không có bản ARM), còn trống **≥ 2 GB RAM** cho SQL Server, đã cài Docker và
Docker Compose. Muốn HTTPS tự động thì cần một domain (bản ghi A trỏ về IP VPS) và cổng 80, 443 chưa bị dịch vụ khác dùng.
Frontend trên Vercel chạy `https`, nên API bắt buộc phải có `https`.

```bash
git clone https://github.com/longdevbf/staymaster-backend.git && cd staymaster-backend
cp deploy/.env.example .env        # điền: MSSQL_SA_PASSWORD, API_DOMAIN, Jwt__SecretKey, SeedAdmin__Password, SePay, Ai
docker compose --profile https up -d --build
docker compose logs -f api         # đợi dòng "Now listening on: http://[::]:8080"
curl https://<API_DOMAIN>/api/settings/public
```

- Lần đầu: container `db-init` tạo DB từ `database/QL_NhaNghi.sql`, API tự chạy các migration khi khởi động
  (`Database__MigrateOnStartup`), **không cần** chạy tay các file trong `database/migrations/`.
- Dữ liệu nằm trong volume `mssql-data`, ảnh người dùng tải lên trong volume `uploads`; `docker compose down` không
  xoá chúng (`down -v` thì có).
- Cập nhật code: `git pull && docker compose --profile https up -d --build`.
- Sao lưu DB: `docker compose exec db /opt/mssql-tools18/bin/sqlcmd -C -S localhost -U sa -P "$MSSQL_SA_PASSWORD"
  -Q "BACKUP DATABASE QL_NhaNghi TO DISK='/var/opt/mssql/backup.bak'"` rồi `docker compose cp db:/var/opt/mssql/backup.bak .`

**VPS đã có nginx/Caddy chiếm cổng 80/443:** chạy `docker compose up -d --build` (không có `--profile https`) và cho proxy
sẵn có chuyển domain về `127.0.0.1:${API_PORT}`, giữ header `Host`, `X-Forwarded-For`, `X-Forwarded-Proto` (link ảnh
tải lên dựa vào hai header cuối để ra đúng `https://`). Ví dụ nginx:

```nginx
location / {
    proxy_pass http://127.0.0.1:8080;
    proxy_set_header Host $host;
    proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
    proxy_set_header X-Forwarded-Proto $scheme;
    client_max_body_size 10m;
}
```

**Dữ liệu mẫu (tuỳ chọn, XOÁ SẠCH dữ liệu hiện có):** phải dừng API trước, nếu không job tự huỷ đơn của API ghi vào DB
trong lúc đang xoá và lệnh seed lỗi.

```bash
docker compose stop api
docker compose run --rm --no-deps -e ASPNETCORE_ENVIRONMENT=Development -e Database__MigrateOnStartup=false api --seed-demo
docker compose start api
```

Admin mẫu `admin@staymaster.vn` dùng mật khẩu `SeedAdmin__Password` trong `.env`. Các tài khoản mẫu khác (`*@demo.vn` /
`Demo@123`, `khach@test.local` và `chunha@test.local` / `Test123!`) có mật khẩu công khai trong repo: trên server public ai
cũng đăng nhập được, nên chỉ dùng để demo.

Sau khi API chạy: đặt `NEXT_PUBLIC_API_URL=https://<API_DOMAIN>/api` trên Vercel (mục 4) và khai báo webhook SePay là
`https://<API_DOMAIN>/api/payment/sepay-webhook`.

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
