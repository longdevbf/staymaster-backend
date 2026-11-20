#!/bin/bash
# Tạo DB QL_NhaNghi từ database/QL_NhaNghi.sql nếu chưa có. Chạy một lần mỗi lần `docker compose up`;
# DB đã có thì bỏ qua. Các migration sau đó do API tự áp dụng khi khởi động (Database__MigrateOnStartup).
set -euo pipefail
SQLCMD=/opt/mssql-tools18/bin/sqlcmd
run() { "$SQLCMD" -C -S db -U sa -P "$MSSQL_SA_PASSWORD" "$@"; }

for _ in $(seq 1 60); do
  run -Q "SELECT 1" -b -o /dev/null && break
  sleep 2
done

exists=$(run -h -1 -W -Q "SET NOCOUNT ON; SELECT CASE WHEN DB_ID('QL_NhaNghi') IS NULL THEN 0 ELSE 1 END" | tr -d '[:space:]')
if [ "$exists" = "1" ]; then
  echo "QL_NhaNghi đã có, bỏ qua bước tạo DB."
  exit 0
fi

echo "Tạo QL_NhaNghi từ database/QL_NhaNghi.sql ..."
# -r0: lỗi SQL vẫn ra stderr (xem bằng `docker compose logs db-init`) dù bỏ phần output thường
if ! run -b -r0 -f 65001 -i /sql/QL_NhaNghi.sql -o /dev/null; then
  # Không để lại DB tạo dở: lần chạy sau sẽ thấy "đã có" và bỏ qua, API chạy với DB thiếu bảng
  echo "Tạo DB lỗi, xoá DB tạo dở để lần sau làm lại từ đầu." >&2
  run -Q "IF DB_ID('QL_NhaNghi') IS NOT NULL BEGIN ALTER DATABASE [QL_NhaNghi] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [QL_NhaNghi]; END" || true
  exit 1
fi
echo "Xong."
