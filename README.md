# StayMaster — Backend API

ASP.NET Core 9 Web API + Entity Framework Core + SQL Server. Frontend nằm ở repo `Hotel-Manager`.

## Cấu hình

`appsettings.json` **không chứa khoá bí mật**. Đặt các giá trị sau bằng một trong hai cách:

- Máy dev: tạo `appsettings.Development.json` (đã nằm trong `.gitignore`).
- Server: biến môi trường, dùng `__` thay cho `:` (ví dụ `Jwt__SecretKey`), hoặc file `appsettings.Production.json` đặt trực tiếp trên server (cũng đã bị gitignore).

| Khoá | Bắt buộc | Ý nghĩa |
|------|----------|---------|
| `ConnectionStrings:DefaultConnection` | Có | Chuỗi kết nối SQL Server |
| `Jwt:SecretKey` | Có | Khoá ký JWT, tối thiểu 32 byte. Tạo bằng `openssl rand -base64 48` |
| `SeedAdmin:Email`, `SeedAdmin:Password` | Không | Tạo tài khoản admin đầu tiên nếu email đó chưa tồn tại |
| `SePay:ApiKey` | Khi dùng SePay | Key SePay gửi kèm webhook (header `Authorization: Apikey <key>`) và dùng để tra giao dịch |
| `SePay:BankId`, `SePay:AccountNumber`, `SePay:AccountName` | Có | Tài khoản nhận tiền hiển thị trên mã VietQR (`GET /api/payment/bank-info`); phải là tài khoản gắn với SePay |
| `App:CorsOrigins` | Có | Danh sách origin của frontend |
| `App:FrontendUrl` | Có | URL frontend, dùng trong link gửi qua email |
| `Email:*` | Khi gửi email | SMTP: `SmtpHost`, `SmtpPort`, `UseSsl`, `Username`, `Password`, `FromAddress`, `FromName` |
| `Ai:*` | Khi bật chatbot | `Provider`, `Model`, `ApiKey`, `TimeoutSeconds` |

Ví dụ `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,14333;Database=QL_NhaNghi;User Id=sa;Password=<mật khẩu>;TrustServerCertificate=True;"
  },
  "Jwt": { "SecretKey": "<chuỗi ngẫu nhiên ≥ 32 byte>" },
  "SeedAdmin": { "Email": "admin@staymaster.local", "Password": "<mật khẩu>" },
  "SePay": { "ApiKey": "local-dev-webhook-key" }
}
```

## Chạy local

```bash
# 1. SQL Server trong Docker (trên Mac Apple Silicon cần bật Rosetta trong Docker Desktop)
docker run -d --name staymaster-sql --platform linux/amd64 -e ACCEPT_EULA=Y \
  -e "MSSQL_SA_PASSWORD=<mật khẩu>" -p 14333:1433 mcr.microsoft.com/mssql/server:2022-latest

# 2. Tạo DB + dữ liệu mẫu
docker cp database/QL_NhaNghi.sql staymaster-sql:/tmp/init.sql
docker exec staymaster-sql /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "<mật khẩu>" -C -i /tmp/init.sql

# 3. Áp dụng migration mới
dotnet tool install --global dotnet-ef   # một lần
dotnet ef database update
#    Trên server không có dotnet-ef: chạy lần lượt các file trong database/migrations/ (idempotent, chạy lại không sao)

# 4. Chạy API (ảnh mẫu trong DB trỏ tới localhost:5000)
ASPNETCORE_ENVIRONMENT=Development ASPNETCORE_URLS=http://localhost:5000 dotnet run
```

Swagger: http://localhost:5000/swagger

### Dữ liệu mẫu

```bash
ASPNETCORE_ENVIRONMENT=Development dotnet run -- --seed-demo
```

Xoá sạch mọi bảng rồi tạo dữ liệu mẫu tiếng Việt có ảnh (`Data/DemoSeeder.cs`, ảnh ở `wwwroot/demo/`): 13 cơ sở ở 12 thành phố,
32 phòng, 102 đơn ở đủ trạng thái trong 12 tháng, đánh giá, rút tiền, hòm thư. Chỉ chạy với môi trường Development.
Tài khoản: `admin@staymaster.vn` / `Admin@123`; chủ nhà và khách `*@demo.vn` / `Demo@123`; `chunha@test.local` và
`khach@test.local` / `Test123!`.

## Kiểm thử nhanh

`scripts/smoke_test.py` gọi API thật để kiểm tra phân quyền, đặt/huỷ đơn, webhook SePay và số dư Đối tác.
Script tạo tài khoản và đơn thử, **chỉ chạy trên DB local**.

```bash
python3 scripts/smoke_test.py --admin-email admin@staymaster.local --admin-password '<mật khẩu>' \
  --webhook-key local-dev-webhook-key --chat        # --chat cần Ai:Provider=offline
```

## Email

Email (xác nhận đơn, huỷ đơn, hoàn tất kèm hoá đơn, đặt lại mật khẩu, trả lời câu hỏi) được đưa vào hàng đợi và gửi
trong nền, nên request không phải chờ SMTP. Khi chưa cấu hình `Email:SmtpHost`/`Email:FromAddress`, email chỉ được
ghi log; ở Development log có cả nội dung để thử link đặt lại mật khẩu.

Gmail: bật xác minh 2 bước, tạo App Password, rồi đặt `SmtpHost=smtp.gmail.com`, `SmtpPort=587`, `UseSsl=true`,
`Username`/`FromAddress` = địa chỉ Gmail, `Password` = App Password.

## AI Chatbot và hòm thư

`POST /api/chat` trả lời khách (tìm phòng, chính sách) và phân loại câu hỏi: tự trả lời, chuyển chủ nhà, hoặc chuyển Admin.
Câu hỏi được chuyển nằm trong bảng `hop_thu_cau_hoi`; Đối tác/Admin trả lời ở `/api/inbox`, câu trả lời gửi email cho khách.

| `Ai:Provider` | Hành vi |
|---------------|---------|
| `anthropic` + `Ai:ApiKey` | Claude qua SDK chính thức (`Ai:Model`, mặc định `claude-opus-5-5`, effort thấp, structured output, server-side fallback khi bị từ chối) |
| `gemini` + `Ai:ApiKey` | Google Gemini qua REST (`Ai:Model`, mặc định `gemini-2.5-flash`, thinking tắt, JSON schema). Key tạo ở Google AI Studio |
| `deepseek` + `Ai:ApiKey` | DeepSeek (`Ai:Model`, mặc định `deepseek-flash`, thinking tắt, JSON mode) |
| `offline` | Nhận diện từ khoá, không gọi dịch vụ ngoài — cho máy dev và demo |
| rỗng | Tắt; luôn trả lời thay thế kèm `App:SupportEmail` / `App:SupportPhone` |

- Ngữ cảnh gửi cho AI chỉ có dữ liệu phòng công khai và chính sách, không có thông tin cá nhân (SRS 3c).
- Quá `Ai:TimeoutSeconds` (mặc định 5 giây) hoặc lỗi thì trả lời thay thế (SRS 3a, 3d).
- Giới hạn 20 tin nhắn/phút/IP.
- Đổi nhà cung cấp: thêm một lớp `IChatModel` trong `Services/Ai/` và một nhánh trong `Program.cs`.

## Nhật ký thao tác

Các thao tác quan trọng được ghi vào bảng `log_he_thong` (ai, lúc nào, bản ghi nào, trước/sau): đăng nhập (cả sai mật khẩu),
đổi/đặt lại mật khẩu, tạo/huỷ/xác nhận/check-in/hoàn tất đơn, tạo/sửa/duyệt/ẩn/xoá phòng, xoá người dùng, yêu cầu và duyệt rút tiền.
Admin xem ở `GET /api/admin/audit-logs` (lọc theo hành động, tìm theo email/tên). Lỗi khi ghi nhật ký không làm hỏng thao tác chính.

## Hoá đơn PDF

`GET /api/bookings/{id}/invoice` trả PDF khi đơn đã `completed` (khách đặt, đối tác sở hữu phòng, admin).
Tạo bằng QuestPDF (giấy phép Community). Font Be Vietnam Pro (OFL) nằm trong `Fonts/` và được copy ra thư mục build/publish;
QuestPDF không dùng font hệ thống nên thiếu thư mục này sẽ lỗi.

## Phân quyền

Vai trò trong token luôn là chữ thường: `admin`, `partner`, `customer`. Bảng `vai_tro` có nhiều tên cũ
(`Admin`, `Super Admin`, `Khach hang`, ...); `AppRoles.FromDbRoleName` chuẩn hoá chúng, tên lạ được coi là `customer`.

| Nhóm API | Ai được gọi |
|----------|-------------|
| `GET /rooms`, `/rooms/{id}`, `/rooms/availability`, `/rooms/{id}/quote`, `/rooms/{id}/booked-ranges`, `/rooms/{id}/reviews`, `/reviews/featured`, `/settings/public`, `/amenities`, `/statistics/recommendations`, `/auth/*`, `POST /chat` | Mọi người |
| Đơn đặt phòng của mình, huỷ đơn đang chờ thanh toán, đánh giá đơn đã hoàn thành, phòng yêu thích (`/favorites`), thông báo (`/notifications`) | Khách đặt đơn |
| Phòng, đơn, số dư, rút tiền, hòm thư, trả lời đánh giá, báo cáo doanh thu (`/partner/revenue-report`), lịch phòng (`/partner/calendar`), chặn ngày (`/rooms/{id}/blocks`) của cơ sở mình | Đối tác |
| Người dùng (khoá / mở khoá / xoá), duyệt/từ chối kèm lý do/ẩn/xoá phòng, mọi đơn (`/admin/bookings`), tài chính, báo cáo, duyệt rút tiền, ẩn đánh giá (`/admin/reviews`), việc cần xử lý (`/admin/todo`), cấu hình (`/admin/settings`) | Admin |
| `POST /payment/sepay-webhook` | SePay (kiểm tra `SePay:ApiKey`) |

## Trạng thái đơn

`pending` (chờ thanh toán) → `confirmed` (SePay báo đã nhận đủ tiền, hoặc đối tác/admin xác nhận) → `checked_in` → `completed`.
`pending`/`confirmed` có thể chuyển sang `cancelled` (đối tác/admin ghi được lý do, khách thấy trong email và thông báo).

- **Giữ phòng:** đơn `pending` giữ phòng trong `payment_hold_minutes` phút (mặc định 30, sửa ở trang Cấu hình hoặc
  `Booking:PaymentHoldMinutes`). `BookingExpiryService` chạy mỗi phút, huỷ đơn quá hạn (nhật ký `BOOKING_EXPIRE`).
  Trong lúc chờ job, đơn quá hạn cũng không còn chặn lịch. Tiền về trễ: nếu phòng còn trống thì đơn được khôi phục
  (`BOOKING_REINSTATE`), nếu không thì admin nhận thông báo cần hoàn tiền (`PAYMENT_NEEDS_REFUND`).
- **Check-in** chỉ từ ngày nhận phòng tới trước ngày trả phòng.
- **Tiền của chủ nhà:** khi `completed`, số dư đối tác được cộng `tong_tien − phi_dich_vu` (phí dịch vụ là phần của
  StayMaster; thuế đi cùng tiền phòng). Báo cáo của admin có thêm "Phí dịch vụ đã thu".
- **Giá:** đêm thứ Sáu, thứ Bảy dùng `gia_cuoi_tuan` nếu chủ nhà đặt; phí dịch vụ và thuế lấy từ cấu hình hệ thống.

## Thông báo trong app

Bảng `thong_bao` (thêm cột `duong_dan`). `InAppNotifier` ghi thông báo cho: đơn mới / đã thanh toán / huỷ / tự huỷ /
trả phòng, tin nhắn hòm thư, yêu cầu và kết quả rút tiền, duyệt / từ chối / ẩn phòng, đánh giá mới và trả lời đánh giá.
`duong_dan` dạng `chat:<id>` nghĩa là mở cuộc trò chuyện trong khung chat.
Dữ liệu cũ có `Confirmed`, `Cancelled`, `Hủy`, ...; API luôn trả về dạng chữ thường (`BookingStatus.Normalize`).
