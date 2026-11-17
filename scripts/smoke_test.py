#!/usr/bin/env python3
"""
Kiểm thử nhanh API StayMaster trên môi trường local (KHÔNG chạy trên server thật: script tạo tài khoản và đơn thử).

    python3 scripts/smoke_test.py --api http://localhost:5000/api \
        --admin-email admin@staymaster.local --admin-password '...' --webhook-key local-dev-webhook-key

Mỗi bước in PASS/FAIL; thoát với mã 1 nếu có bước FAIL.
"""
import argparse
import json
import re
import sys
import time
import urllib.error
import urllib.request
import uuid
from datetime import date, timedelta

FAILS = []


def call(api, method, path, token=None, body=None, headers=None):
    data = json.dumps(body).encode() if body is not None else None
    req = urllib.request.Request(api + path, data=data, method=method)
    req.add_header("Content-Type", "application/json")
    if token:
        req.add_header("Authorization", f"Bearer {token}")
    for k, v in (headers or {}).items():
        req.add_header(k, v)
    try:
        with urllib.request.urlopen(req, timeout=30) as res:
            raw = res.read()
            return res.status, (json.loads(raw) if raw else None)
    except urllib.error.HTTPError as e:
        raw = e.read()
        try:
            return e.code, json.loads(raw) if raw else None
        except ValueError:
            return e.code, raw.decode(errors="replace")


def check(name, cond, detail=""):
    print(("PASS " if cond else "FAIL ") + name + ("" if cond else f"  -> {detail}"))
    if not cond:
        FAILS.append(name)


def expect(name, api, method, path, statuses, token=None, body=None, headers=None):
    status, data = call(api, method, path, token, body, headers)
    check(f"{name} [{method} {path} -> {status}]", status in statuses, f"expected {statuses}, body={str(data)[:200]}")
    return status, data


def fetch_raw(api, path, token):
    req = urllib.request.Request(api + path)
    req.add_header("Authorization", f"Bearer {token}")
    try:
        with urllib.request.urlopen(req, timeout=30) as res:
            return res.status, res.headers.get("Content-Type", ""), res.read()
    except urllib.error.HTTPError as e:
        return e.code, e.headers.get("Content-Type", ""), e.read()


def register(api, kind):
    tag = uuid.uuid4().hex[:8]
    body = {"fullName": f"Smoke {kind} {tag}", "email": f"smoke-{kind}-{tag}@example.test",
            "phone": "09" + str(uuid.uuid4().int)[:8], "password": "Smoke123!"}
    status, data = call(api, "POST", f"/auth/register/{kind}", body=body)
    check(f"register {kind}", status == 200 and data and data.get("token"), data)
    return data["token"], data["user"], body


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--api", default="http://localhost:5000/api")
    ap.add_argument("--admin-email", required=True)
    ap.add_argument("--admin-password", required=True)
    ap.add_argument("--webhook-key", required=True)
    ap.add_argument("--api-log", help="API log file (Development); used to read the password reset link")
    ap.add_argument("--save-invoice", help="Write the generated invoice PDF here")
    ap.add_argument("--chat", action="store_true", help="Test chatbot + inbox (needs Ai:Provider=offline or a real key)")
    ap.add_argument("--fallback-api", help="Second API instance running with the AI disabled, to test the fallback reply")
    a = ap.parse_args()
    api = a.api.rstrip("/")

    print("== Anonymous access must be refused")
    for method, path in [("GET", "/users"), ("DELETE", "/users/1"), ("GET", "/bookings"), ("GET", "/bookings/1"),
                         ("PUT", "/bookings/1/cancel"), ("PUT", "/rooms/1/approve"), ("DELETE", "/rooms/1"),
                         ("GET", "/rooms/admin"), ("GET", "/finance/admin"), ("POST", "/finance/payout/1/process"),
                         ("GET", "/statistics/revenue"), ("GET", "/partner/profile")]:
        expect("anonymous refused", api, method, path, {401})
    boundary = "smokeboundary"
    form = (f"--{boundary}\r\nContent-Disposition: form-data; name=\"file\"; filename=\"a.png\"\r\n"
            f"Content-Type: image/png\r\n\r\nx\r\n--{boundary}--\r\n").encode()
    req = urllib.request.Request(api + "/upload", data=form, method="POST")
    req.add_header("Content-Type", f"multipart/form-data; boundary={boundary}")
    try:
        urllib.request.urlopen(req, timeout=30)
        check("anonymous upload refused", False, "upload accepted")
    except urllib.error.HTTPError as e:
        check(f"anonymous upload refused [-> {e.code}]", e.code == 401, e.code)

    print("== Public endpoints stay open")
    d1 = (date.today() + timedelta(days=30)).isoformat()
    d2 = (date.today() + timedelta(days=32)).isoformat()
    expect("public rooms", api, "GET", "/rooms", {200})
    expect("availability", api, "GET", f"/rooms/availability?checkIn={d1}&checkOut={d2}", {200})
    expect("recommendations", api, "GET", "/statistics/recommendations", {200})

    print("== Login")
    status, admin = call(api, "POST", "/auth/login", body={"email": a.admin_email, "password": a.admin_password})
    check("admin login", status == 200, admin)
    admin_token = admin["token"]
    cust_token, cust, _ = register(api, "customer")
    other_token, _, _ = register(api, "customer")
    partner_token, _, _ = register(api, "partner")

    print("== Role checks")
    expect("customer cannot list users", api, "GET", "/users", {403}, cust_token)
    expect("admin lists users", api, "GET", "/users", {200}, admin_token)
    expect("customer cannot edit rooms", api, "PUT", "/rooms/1", {403}, cust_token, {"name": "hacked"})
    expect("partner cannot edit others' room", api, "PUT", "/rooms/1", {403}, partner_token, {"name": "hacked"})
    expect("customer cannot approve", api, "PUT", "/rooms/1/approve", {403}, cust_token)
    expect("customer cannot read admin finance", api, "GET", "/finance/admin", {403}, cust_token)

    print("== Partner room flow")
    code = "SMK-" + uuid.uuid4().hex[:6].upper()
    status, room = expect("partner creates room", api, "POST", "/rooms", {201}, partner_token,
                          {"name": "Smoke room", "roomCode": code, "roomType": "Standard", "capacity": 2,
                           "basePrice": 500000, "area": 20, "propertyId": 1})
    room_id = room["id"]
    check("new room awaits approval", room.get("isApproved") is False, room)
    status, upd = expect("partner cannot self-approve", api, "PUT", f"/rooms/{room_id}", {200}, partner_token,
                         {"isApproved": True, "status": "approved", "description": "updated"})
    check("isApproved unchanged after partner update", upd.get("isApproved") is False, upd)
    expect("unapproved room not bookable", api, "POST", "/bookings", {400}, cust_token,
           {"roomId": room_id, "checkIn": d1, "checkOut": d2, "guestCount": 1, "isDeposit": False})
    expect("admin approves", api, "PUT", f"/rooms/{room_id}/approve", {200}, admin_token)

    print("== Booking flow")
    status, bk = expect("customer books", api, "POST", "/bookings", {200}, cust_token,
                        {"roomId": room_id, "checkIn": d1, "checkOut": d2, "guestCount": 1, "isDeposit": False})
    bid = bk["id"]
    check("fees included in total", bk.get("totalPrice") == 500000 * 2 * 1.15, bk)
    status, av = call(api, "GET", f"/rooms/availability?checkIn={d1}&checkOut={d2}")
    check("booked room reported unavailable", room_id in (av or {}).get("unavailableRoomIds", []), av)
    expect("other customer cannot read booking", api, "GET", f"/bookings/{bid}", {403}, other_token)
    expect("other customer cannot cancel", api, "PUT", f"/bookings/{bid}/cancel", {403}, other_token)
    expect("customer cannot confirm own booking", api, "PUT", f"/bookings/{bid}/confirm", {403}, cust_token)
    expect("owner cancels pending booking", api, "PUT", f"/bookings/{bid}/cancel", {200}, cust_token)

    print("== SePay webhook")
    # Nhận phòng hôm nay để thử được check-in (chỉ cho phép từ ngày nhận phòng)
    t0 = date.today().isoformat()
    t2 = (date.today() + timedelta(days=2)).isoformat()
    status, early = expect("customer books a future stay", api, "POST", "/bookings", {200}, cust_token,
                           {"roomId": room_id, "checkIn": d1, "checkOut": d2, "guestCount": 1, "isDeposit": False})
    auth = {"Authorization": f"Apikey {a.webhook_key}"}
    call(api, "POST", "/payment/sepay-webhook", body={"transferType": "in", "content": f"BK{early['id']}", "transferAmount": early["deposit"]}, headers=auth)
    status, err = expect("check-in before the check-in date refused", api, "PUT", f"/bookings/{early['id']}/checkin", {400}, partner_token)
    check("check-in refusal explains the date", "Chưa đến ngày nhận phòng" in (err or {}).get("message", ""), err)
    status, bk2 = expect("customer books again", api, "POST", "/bookings", {200}, cust_token,
                         {"roomId": room_id, "checkIn": t0, "checkOut": t2, "guestCount": 1, "isDeposit": False})
    bid2, due = bk2["id"], bk2["deposit"]
    hook = {"transferType": "in", "content": f"thanh toan BK{bid2}", "transferAmount": due - 1000}
    expect("webhook without key refused", api, "POST", "/payment/sepay-webhook", {401}, body=hook)
    call(api, "POST", "/payment/sepay-webhook", body=hook, headers=auth)
    status, after = call(api, "GET", f"/bookings/{bid2}", cust_token)
    check("underpaid transfer does not confirm", after.get("status") == "pending", after)
    hook["transferAmount"] = due
    call(api, "POST", "/payment/sepay-webhook", body=hook, headers=auth)
    status, after = call(api, "GET", f"/bookings/{bid2}", cust_token)
    check("full transfer confirms booking", after.get("status") == "confirmed", after)
    expect("customer cannot cancel confirmed booking", api, "PUT", f"/bookings/{bid2}/cancel", {400}, cust_token)
    expect("partner checks in", api, "PUT", f"/bookings/{bid2}/checkin", {200}, partner_token)
    expect("partner checks out", api, "PUT", f"/bookings/{bid2}/complete", {200}, partner_token)
    status, fin = expect("partner balance", api, "GET", "/finance/business", {200}, partner_token)
    check("host earns total minus service fee", fin.get("totalEarnings") == bk2["totalPrice"] - bk2["serviceFee"], fin)

    print("== Invoice PDF")
    started = time.time()
    status, ctype, pdf = fetch_raw(api, f"/bookings/{bid2}/invoice", cust_token)
    elapsed = time.time() - started
    check(f"invoice for completed booking [{status}, {elapsed:.2f}s]", status == 200 and ctype.startswith("application/pdf")
          and pdf[:4] == b"%PDF", (status, ctype))
    check("invoice generated in under 5 s", elapsed < 5, elapsed)
    if a.save_invoice and status == 200:
        open(a.save_invoice, "wb").write(pdf)
    status, _, _ = fetch_raw(api, f"/bookings/{bid2}/invoice", other_token)
    check(f"other customer cannot download invoice [{status}]", status == 403, status)
    expect("no invoice before completion", api, "GET", f"/bookings/{bid}/invoice", {400}, cust_token)

    print("== Login with phone number")
    _, reset_user, creds = register(api, "customer")
    local = creds["phone"]
    for variant in (local, "+84" + local[1:], local[:4] + " " + local[4:7] + " " + local[7:]):
        status, _ = call(api, "POST", "/auth/login", body={"email": variant, "password": creds["password"]})
        check(f"login with phone '{variant}' [{status}]", status == 200, status)
    expect("wrong password refused", api, "POST", "/auth/login", {401}, body={"email": local, "password": "nope123"})

    print("== Forgot / reset password")
    msg_known = call(api, "POST", "/auth/forgot-password", body={"email": creds["email"]})
    msg_unknown = call(api, "POST", "/auth/forgot-password", body={"email": "nobody-" + uuid.uuid4().hex[:6] + "@example.test"})
    check("same answer for known and unknown email", msg_known == msg_unknown and msg_known[0] == 200, (msg_known, msg_unknown))
    email_on = not (isinstance(msg_known[1], dict) and msg_known[1].get("emailEnabled") is False)
    status, public = call(api, "GET", "/settings/public")
    check("public settings tell whether email is on", status == 200 and public.get("emailEnabled") == email_on, public)
    if not email_on:
        # Không có SMTP: người dùng liên hệ hỗ trợ, admin cấp mật khẩu tạm
        status, res = expect("admin issues a temporary password", api, "PUT", f"/users/{reset_user['id']}/reset-password",
                             {200}, admin_token)
        temp = (res or {}).get("temporaryPassword", "")
        expect("old password no longer works", api, "POST", "/auth/login", {401},
               body={"email": creds["email"], "password": creds["password"]})
        status, login = expect("login with temporary password", api, "POST", "/auth/login", {200},
                               body={"email": creds["email"], "password": temp})
        temp_token = (login or {}).get("token")
        expect("user changes the temporary password", api, "POST", "/auth/change-password", {200}, temp_token,
               {"currentPassword": temp, "newPassword": creds["password"], "confirmPassword": creds["password"]})
        expect("customer cannot issue temporary passwords", api, "PUT", f"/users/{reset_user['id']}/reset-password",
               {403}, temp_token)
    elif a.api_log:
        token = None
        for _ in range(20):
            log = open(a.api_log, encoding="utf-8", errors="replace").read()
            m = re.findall(re.escape(creds["email"]) + r".*?reset-password\?token=([A-Za-z0-9_-]+)", log, re.S)
            if m:
                token = m[-1]
                break
            time.sleep(0.5)
        check("reset link e-mailed (logged in Development)", token is not None, "no link in log")
        if token:
            expect("reset password", api, "POST", "/auth/reset-password", {200},
                   body={"token": token, "newPassword": "NewSmoke123!", "confirmPassword": "NewSmoke123!"})
            expect("reset link is single-use", api, "POST", "/auth/reset-password", {400},
                   body={"token": token, "newPassword": "Again123!", "confirmPassword": "Again123!"})
            expect("login with new password", api, "POST", "/auth/login", {200},
                   body={"email": creds["email"], "password": "NewSmoke123!"})
    expect("bogus reset token refused", api, "POST", "/auth/reset-password", {400},
           body={"token": "bogus", "newPassword": "Whatever1!", "confirmPassword": "Whatever1!"})

    print("== Amenities")
    status, amenities = expect("amenity list", api, "GET", "/amenities", {200})
    picked = [x["id"] for x in amenities[:3]]
    expect("partner sets amenities", api, "PUT", f"/rooms/{room_id}", {200}, partner_token, {"amenityIds": picked})
    status, detail = call(api, "GET", f"/rooms/{room_id}")
    check("room detail lists amenities", sorted(x["id"] for x in detail.get("amenities", [])) == sorted(picked), detail.get("amenities"))

    print("== Recommendations")
    status, recs = expect("recommendations near Hanoi", api, "GET", "/statistics/recommendations?lat=21.03&lng=105.85", {200})
    check("at most 8 recommendations", 0 < len(recs) <= 8, len(recs))
    check("recommendations carry a reason", all(r.get("reason") for r in recs), recs[:2])
    status, report = expect("admin revenue report", api, "GET", "/statistics/revenue/report", {200}, admin_token)
    check("report has 12 months", len(report.get("monthly", [])) == 12, report.get("monthly"))
    expect("customer cannot read revenue report", api, "GET", "/statistics/revenue/report", {403}, cust_token)

    if a.chat:
        print("== Chatbot + inbox")
        status, r = expect("anonymous room search", api, "POST", "/chat", {200},
                           body={"messages": [{"role": "user", "content": "Tìm phòng ở Đà Nẵng dưới 2 triệu"}]})
        check("chat answers without fallback", not r.get("fallback") and r.get("reply"), r)
        check("chat suggests at most 4 rooms", len(r.get("rooms", [])) <= 4, r.get("rooms"))
        status, r = expect("anonymous question for the host", api, "POST", "/chat", {200},
                           body={"messages": [{"role": "user", "content": "Phòng này có cho mang chó theo không?"}], "roomId": room_id})
        check("forwarding asks anonymous visitor to log in", r.get("needsLogin") and not r.get("forwarded"), r)
        status, r = expect("customer question for the host", api, "POST", "/chat", {200}, cust_token,
                           {"messages": [{"role": "user", "content": "Phòng này có cho mang chó theo không?"}], "roomId": room_id})
        check("question forwarded to partner", r.get("forwarded") and r.get("intent") == "forward_partner", r)
        check("forward returns the conversation to continue in", r.get("conversationId") and r.get("conversationWith") == "host", r)
        expect("customer cannot read partner inbox", api, "GET", "/inbox", {403}, cust_token)
        status, inbox = expect("partner inbox", api, "GET", "/inbox", {200}, partner_token)
        mine = [q for q in inbox if q.get("roomId") == room_id and q.get("source") == "chatbot"]
        check("forwarded question in partner inbox", len(mine) == 1, inbox[:3])
        check("partner sees no customer email", all("email" not in json.dumps(q).lower() for q in inbox), inbox[:1])
        other_partner, _, _ = register(api, "partner")
        if mine:
            qid = mine[0]["id"]
            expect("other partner cannot answer", api, "POST", f"/inbox/{qid}/answer", {403}, other_partner, {"answer": "x"})
            expect("partner answers", api, "POST", f"/inbox/{qid}/answer", {200}, partner_token,
                   {"answer": "Được bạn nhé, phụ thu 100.000đ/đêm."})
            status, own = expect("customer sees answer", api, "GET", "/inbox/mine", {200}, cust_token)
            check("answer visible to customer", any(q["id"] == qid and q["status"] == "answered" for q in own), own[:2])

            print("== Conversation with the host")
            expect("customer follows up", api, "POST", f"/inbox/{qid}/messages", {200}, cust_token, {"content": "Mèo có được không ạ?"})
            status, conv = expect("thread readable by customer", api, "GET", f"/inbox/{qid}", {200}, cust_token)
            check("follow-up reopens the conversation", conv.get("status") == "open", conv.get("status"))
            expect("host answers again", api, "POST", f"/inbox/{qid}/messages", {200}, partner_token, {"content": "Mèo cũng được nhé."})
            expect("host sends a second message", api, "POST", f"/inbox/{qid}/messages", {200}, partner_token, {"content": "Nhớ mang cát vệ sinh."})
            status, conv = expect("thread readable by host", api, "GET", f"/inbox/{qid}", {200}, partner_token)
            senders = [m["sender"] for m in conv.get("messages", [])]
            check(f"thread has every message in order {senders}", senders == ["customer", "host", "customer", "host", "host"], senders)
            expect("other partner cannot read thread", api, "GET", f"/inbox/{qid}", {403}, other_partner)
            expect("other customer cannot post", api, "POST", f"/inbox/{qid}/messages", {403}, other_token, {"content": "x"})
            expect("empty message refused", api, "POST", f"/inbox/{qid}/messages", {400}, cust_token, {"content": "  "})
            if a.api_log:
                time.sleep(1)
                check("answer e-mailed to customer", "Phản hồi cho câu hỏi" in open(a.api_log, encoding="utf-8", errors="replace").read(), "no email in log")
        status, r = expect("complaint goes to admin", api, "POST", "/chat", {200}, cust_token,
                           {"messages": [{"role": "user", "content": "Tôi muốn khiếu nại và yêu cầu hoàn tiền"}]})
        check("complaint forwarded to admin", r.get("forwarded") and r.get("intent") == "forward_admin", r)
        status, admin_inbox = expect("admin inbox", api, "GET", "/inbox", {200}, admin_token)
        check("admin inbox has the complaint", any("hoàn tiền" in q["question"] for q in admin_inbox), admin_inbox[:2])
        expect("direct question to host", api, "POST", "/inbox", {200}, cust_token, {"roomId": room_id, "question": "Có chỗ đậu ô tô không?"})
        codes = [call(api, "POST", "/chat", body={"messages": [{"role": "user", "content": "xin chào"}]})[0] for _ in range(25)]
        check(f"chat is rate limited ({codes.count(429)} x 429)", 429 in codes, codes)

    if a.fallback_api:
        print("== Chatbot fallback (AI disabled)")
        started = time.time()
        status, r = call(a.fallback_api.rstrip("/"), "POST", "/chat", body={"messages": [{"role": "user", "content": "Tìm phòng"}]})
        check(f"fallback reply when AI is down [{status}, {time.time() - started:.2f}s]", status == 200 and r.get("fallback") and "1900" in r.get("reply", ""), r)

    print("== Round 1: quote, reviews, notifications, favourites, settings, locking")
    status, q = expect("price quote", api, "GET", f"/rooms/{room_id}/quote?checkIn={d1}&checkOut={d2}", {200})
    check("quote = subtotal + fee + tax", q and q["total"] == q["subtotal"] + q["serviceFee"] + q["tax"], q)
    expect("quote rejects reversed dates", api, "GET", f"/rooms/{room_id}/quote?checkIn={d2}&checkOut={d1}", {400})
    status, ranges = expect("booked ranges", api, "GET", f"/rooms/{room_id}/booked-ranges", {200})
    check("future stay shows as booked", any(r["from"].startswith(d1) for r in ranges or []), ranges)
    expect("review needs a completed stay", api, "POST", f"/bookings/{early['id']}/review", {400}, cust_token, {"stars": 5})
    expect("other customer cannot review", api, "POST", f"/bookings/{bid2}/review", {400}, other_token, {"stars": 5})
    status, rv = expect("customer reviews completed stay", api, "POST", f"/bookings/{bid2}/review", {200}, cust_token,
                        {"stars": 4, "comment": "Phòng sạch sẽ, chủ nhà thân thiện, sẽ quay lại."})
    expect("only one review per booking", api, "POST", f"/bookings/{bid2}/review", {400}, cust_token, {"stars": 5})
    status, rr = expect("room reviews", api, "GET", f"/rooms/{room_id}/reviews", {200})
    check("review listed with average", rr and rr["count"] == 1 and rr["average"] == 4 and rr["distribution"][3] == 1, rr)
    expect("other host cannot reply", api, "PUT", f"/reviews/{rv['id']}/reply", {403}, register(api, "partner")[0], {"reply": "x"})
    expect("host replies", api, "PUT", f"/reviews/{rv['id']}/reply", {200}, partner_token, {"reply": "Cảm ơn bạn!"})
    expect("hiding needs a reason", api, "PUT", f"/admin/reviews/{rv['id']}/hide", {400}, admin_token, {"reason": ""})
    expect("admin hides review", api, "PUT", f"/admin/reviews/{rv['id']}/hide", {200}, admin_token, {"reason": "Thử kiểm duyệt"})
    status, rr = call(api, "GET", f"/rooms/{room_id}/reviews")
    check("hidden review not public", rr["count"] == 0, rr)
    expect("admin shows review again", api, "PUT", f"/admin/reviews/{rv['id']}/show", {200}, admin_token)
    status, n = expect("host notifications", api, "GET", "/notifications", {200}, partner_token)
    titles = [i["title"] for i in (n or {}).get("items", [])]
    check("host notified of paid booking and review", any("đã thanh toán" in t for t in titles) and any("Đánh giá mới" in t for t in titles), titles)
    expect("mark all read", api, "PUT", "/notifications/read-all", {204}, partner_token)
    status, c = call(api, "GET", "/notifications/unread-count", partner_token)
    check("no unread after read-all", c.get("unread") == 0, c)
    expect("save favourite", api, "POST", f"/favorites/{room_id}", {200}, cust_token)
    status, ids = call(api, "GET", "/favorites/ids", cust_token)
    check("favourite saved", room_id in ids, ids)
    expect("remove favourite", api, "DELETE", f"/favorites/{room_id}", {200}, cust_token)
    expect("public settings", api, "GET", "/settings/public", {200})
    expect("customer cannot change settings", api, "PUT", "/admin/settings", {403}, cust_token, {})
    expect("invalid fee refused", api, "PUT", "/admin/settings", {400}, admin_token,
           {"serviceFeePercent": 90, "taxPercent": 5, "paymentHoldMinutes": 30, "supportEmail": "a@b.vn", "supportPhone": "1"})
    expect("admin to-do", api, "GET", "/admin/todo", {200}, admin_token)
    expect("admin booking search", api, "GET", f"/admin/bookings?search={bid2}", {200}, admin_token)
    expect("host revenue report", api, "GET", "/partner/revenue-report?months=6", {200}, partner_token)
    expect("host calendar", api, "GET", f"/partner/calendar?from={t0}&days=14", {200}, partner_token)
    status, blk = expect("host blocks free dates", api, "POST", f"/rooms/{room_id}/blocks", {200}, partner_token,
                         {"from": (date.today() + timedelta(days=50)).isoformat(), "to": (date.today() + timedelta(days=52)).isoformat(), "reason": "Bảo trì"})
    expect("block over a booking refused", api, "POST", f"/rooms/{room_id}/blocks", {400}, partner_token, {"from": d1, "to": d2})
    expect("booking on blocked dates refused", api, "POST", "/bookings", {400}, cust_token,
           {"roomId": room_id, "checkIn": (date.today() + timedelta(days=50)).isoformat(),
            "checkOut": (date.today() + timedelta(days=51)).isoformat(), "guestCount": 1, "isDeposit": False})
    expect("host removes block", api, "DELETE", f"/rooms/{room_id}/blocks/{blk['id']}", {204}, partner_token)
    lock_token, lock_user, lock_body = register(api, "customer")
    expect("admin locks account", api, "PUT", f"/users/{lock_user['id']}/lock", {200}, admin_token, {"reason": "smoke"})
    expect("locked account cannot log in", api, "POST", "/auth/login", {403}, body={"email": lock_body["email"], "password": "Smoke123!"})
    expect("locked account's token refused", api, "GET", "/bookings/my", {401}, lock_token)
    expect("admin unlocks account", api, "PUT", f"/users/{lock_user['id']}/unlock", {200}, admin_token)
    expect("admin cannot lock an admin", api, "PUT", f"/users/{admin['user']['id']}/lock", {400}, admin_token, {})

    print()
    print(f"{len(FAILS)} failed" if FAILS else "All checks passed")
    sys.exit(1 if FAILS else 0)


if __name__ == "__main__":
    main()
