using HotelManagement.API.Common;
using HotelManagement.API.Models;
using HotelManagement.API.Services;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.API.Data;

/// <summary>
/// Xoá sạch dữ liệu và tạo bộ dữ liệu mẫu tiếng Việt có ảnh, để chạy thử và demo.
/// Chạy: <c>dotnet run -- --seed-demo</c> (chỉ ở Development). KHÔNG BAO GIỜ chạy trên DB thật.
/// Ảnh nằm ở wwwroot/demo và được lưu dạng đường dẫn tương đối "/demo/...".
/// </summary>
public static class DemoSeeder
{
    public const string DemoPassword = "Demo@123";
    public const string AdminEmail = "admin@staymaster.vn";
    public const string AdminPassword = "Admin@123";
    /// <summary>Tài khoản thử đã dùng trong quá trình phát triển, giữ lại cho tiện.</summary>
    public const string TestPassword = "Test123!";

    private const decimal ServiceFeeRate = 0.10m;
    private const decimal TaxRate = 0.05m;

    private static string Img(string name) => $"/demo/{name}.jpg";

    // ── Ảnh ──────────────────────────────────────────────────
    private static readonly string[] Interiors =
    {
        "modern-apartment-interior", "minimalist-urban-loft-apartment-interior", "cozy-mountain-cabin-in-snow",
        "modern-loft-apartment-with-high-ceilings", "modern-interior-of-a-boutique-guesthouse",
        "modern-rustic-bedroom-interior", "guesthouse-living-room-with-fireplace",
        "luxurious-beachfront-resort-suite-interior", "modern-bright-office-space-interior-background",
        "modern-boutique-hotel-lobby-with-elegant-furniture", "traditional-japanese-style-guesthouse",
    };

    private static readonly string[] Avatars =
    {
        "portrait-of-a-smiling-professional-man-with-glasse", "portrait-of-a-professional-woman-with-a-warm-expre",
        "portrait-of-a-young-man-looking-directly-at-the-ca", "portrait-of-a-female-guest",
        "professional-headshot-of-a-business-host", "user-profile-avatar-of-a-business-host",
        "professional-profile-headshot-of-a-male-business-o", "user-profile-avatar-photo",
        "user-profile-avatar-photo-2", "user-profile-avatar-of-a-business-manager",
        "user-profile-circular-headshot-thumbnail", "corporate-headshot-of-executive-administrator",
    };

    // ── Tiện ích (bảng tien_ich) — khoá icon khớp lib/amenities.ts ở frontend ──
    private static readonly (string Icon, string Name, string Group)[] AmenityList =
    {
        ("wifi", "Wifi miễn phí", "Cơ bản"), ("ac", "Máy lạnh", "Cơ bản"), ("tv", "TV màn hình phẳng", "Giải trí"),
        ("fridge", "Tủ lạnh", "Tiện nghi"), ("balcony", "Ban công", "Phong cảnh"), ("pool", "Hồ bơi", "Giải trí"),
        ("kitchen", "Bếp riêng", "Tiện nghi"), ("washer", "Máy giặt", "Tiện nghi"), ("parking", "Chỗ đỗ xe", "Dịch vụ"),
        ("elevator", "Thang máy", "Dịch vụ"), ("reception", "Lễ tân 24/7", "Dịch vụ"), ("breakfast", "Bữa sáng", "Ẩm thực"),
        ("gym", "Phòng gym", "Sức khoẻ"), ("security", "Bảo vệ 24/7", "An ninh"), ("camera", "Camera an ninh", "An ninh"),
        ("desk", "Bàn làm việc", "Công việc"), ("bath", "Bồn tắm", "Tiện nghi"), ("dryer", "Máy sấy tóc", "Tiện nghi"),
        ("iron", "Bàn ủi", "Tiện nghi"), ("sea-view", "Hướng biển", "Phong cảnh"),
    };

    private record RoomSeed(string Name, string Type, int Capacity, int Beds, decimal Price, decimal Area, string Description, string[] Amenities);

    private record PropertySeed(
        string Business, string Owner, string Email, string Phone, string Type,
        string City, string District, string Ward, string Address, decimal Lat, decimal Lng,
        string Cover, string Description, RoomSeed[] Rooms);

    private static readonly string[] BaseAmenities = { "wifi", "ac", "tv", "dryer", "camera" };

    private static readonly PropertySeed[] Properties =
    {
        new("Khách sạn Phố Cổ Hoàn Kiếm", "Nguyễn Thu Hà", "chunha@test.local", "0901000002", "Khách sạn",
            "Hà Nội", "Hoàn Kiếm", "Hàng Bạc", "35 Hàng Bạc", 21.0341m, 105.8524m, "modern-guesthouse-interior-design",
            "Khách sạn nhỏ giữa lòng phố cổ, đi bộ 5 phút ra Hồ Gươm và phố đi bộ cuối tuần.",
            new RoomSeed[]
            {
                new("Phòng Đôi Hướng Phố", "Tiêu chuẩn", 2, 1, 650_000, 22, "Cửa sổ nhìn ra phố Hàng Bạc, cách âm tốt.", new[] { "desk", "reception", "elevator", "breakfast" }),
                new("Phòng Gia Đình Phố Cổ", "Gia đình", 4, 2, 1_100_000, 34, "Hai giường đôi, phù hợp gia đình có trẻ nhỏ.", new[] { "fridge", "reception", "elevator", "breakfast", "bath" }),
                new("Phòng Đơn Tiết Kiệm", "Tiêu chuẩn", 1, 1, 420_000, 14, "Phòng gọn gàng cho khách đi một mình.", new[] { "desk", "reception" }),
            }),
        new("Saigon Riverside Boutique", "Trần Minh Khoa", "riverside.saigon@demo.vn", "0902000011", "Khách sạn",
            "TP. Hồ Chí Minh", "Quận 1", "Bến Nghé", "12 Tôn Đức Thắng", 10.7769m, 106.7066m, "luxury-boutique-hotel-exterior-with-pool",
            "Khách sạn boutique ven sông Sài Gòn, hồ bơi tầng thượng nhìn ra Thủ Thiêm.",
            new RoomSeed[]
            {
                new("Phòng Cao Cấp Hướng Sông", "Cao cấp", 2, 1, 1_650_000, 30, "Cửa kính lớn nhìn ra sông Sài Gòn.", new[] { "pool", "gym", "elevator", "breakfast", "bath", "desk" }),
                new("Suite Thượng Lưu", "Suite", 3, 1, 3_200_000, 55, "Phòng khách riêng, bồn tắm và quầy bar mini.", new[] { "pool", "gym", "elevator", "breakfast", "bath", "fridge", "iron" }),
                new("Phòng Đôi Thành Phố", "Tiêu chuẩn", 2, 1, 1_150_000, 24, "Phòng hiện đại, thuận tiện đi Phố đi bộ Nguyễn Huệ.", new[] { "pool", "elevator", "desk" }),
            }),
        new("Tây Hồ Lakeview Hotel", "Lê Quang Huy", "tayho.lakeview@demo.vn", "0903000012", "Khách sạn",
            "Hà Nội", "Tây Hồ", "Quảng An", "88 Xuân Diệu", 21.0679m, 105.8238m, "luxury-hotel-facade",
            "Khách sạn yên tĩnh bên Hồ Tây, gần phủ Tây Hồ và các quán cà phê ven hồ.",
            new RoomSeed[]
            {
                new("Phòng Ban Công Hồ Tây", "Cao cấp", 2, 1, 1_250_000, 28, "Ban công riêng nhìn thẳng ra Hồ Tây.", new[] { "balcony", "elevator", "breakfast", "bath" }),
                new("Phòng Đôi Sân Vườn", "Tiêu chuẩn", 2, 1, 900_000, 22, "Nhìn ra sân vườn trong khuôn viên.", new[] { "elevator", "breakfast", "parking" }),
            }),
        new("Mỹ Khê Ocean Resort", "Phạm Thị Lan", "mykhe.resort@demo.vn", "0904000013", "Resort",
            "Đà Nẵng", "Sơn Trà", "Phước Mỹ", "120 Võ Nguyên Giáp", 16.0612m, 108.2468m, "luxury-coastal-guesthouse-with-infinity-pool",
            "Resort sát biển Mỹ Khê với hồ bơi vô cực, 15 phút tới cầu Rồng.",
            new RoomSeed[]
            {
                new("Phòng Hướng Biển Mỹ Khê", "Cao cấp", 2, 1, 1_900_000, 32, "Nhìn trọn bãi biển Mỹ Khê từ ban công.", new[] { "sea-view", "balcony", "pool", "breakfast", "bath" }),
                new("Bungalow Sân Vườn", "Bungalow", 3, 2, 2_400_000, 40, "Bungalow riêng tư giữa vườn dừa.", new[] { "pool", "breakfast", "fridge", "parking" }),
                new("Suite Gia Đình Hướng Biển", "Suite", 5, 3, 3_800_000, 70, "Hai phòng ngủ, phòng khách và bếp nhỏ.", new[] { "sea-view", "balcony", "pool", "kitchen", "washer", "breakfast" }),
            }),
        new("Nhà Cổ Hội An Homestay", "Võ Thị Mai", "nhaco.hoian@demo.vn", "0905000014", "Homestay",
            "Hội An", "Minh An", "Minh An", "45 Nguyễn Thái Học", 15.8768m, 108.3277m, "traditional-japanese-style-guesthouse",
            "Homestay trong nhà gỗ cổ, đi bộ tới chùa Cầu và chợ đêm Hội An.",
            new RoomSeed[]
            {
                new("Phòng Gỗ Truyền Thống", "Tiêu chuẩn", 2, 1, 780_000, 20, "Nội thất gỗ, trang trí đèn lồng Hội An.", new[] { "breakfast", "parking" }),
                new("Phòng Đôi Sân Trong", "Tiêu chuẩn", 2, 1, 690_000, 18, "Mở ra sân trong có giếng trời.", new[] { "breakfast" }),
                new("Phòng Gia Đình Nhà Cổ", "Gia đình", 4, 2, 1_250_000, 32, "Rộng rãi, hai giường đôi, có xe đạp miễn phí.", new[] { "breakfast", "parking", "fridge" }),
            }),
        new("Villa Đồi Thông Đà Lạt", "Đặng Quốc Bảo", "doithong.dalat@demo.vn", "0906000015", "Biệt thự",
            "Đà Lạt", "Phường 3", "Phường 3", "22 Trần Hưng Đạo", 11.9332m, 108.4421m, "large-suburban-house-with-manicured-lawn",
            "Biệt thự Pháp cổ giữa đồi thông, có lò sưởi và vườn hoa.",
            new RoomSeed[]
            {
                new("Phòng Lò Sưởi", "Cao cấp", 2, 1, 1_350_000, 30, "Lò sưởi riêng, nhìn ra đồi thông.", new[] { "balcony", "breakfast", "parking", "bath" }),
                new("Phòng Gác Mái", "Tiêu chuẩn", 2, 1, 850_000, 22, "Gác mái gỗ ấm cúng, cửa sổ trời.", new[] { "breakfast", "parking" }),
                new("Nguyên Căn Villa 4 Phòng Ngủ", "Biệt thự", 10, 5, 5_500_000, 220, "Thuê nguyên căn cho nhóm bạn, có BBQ ngoài vườn.", new[] { "kitchen", "washer", "parking", "fridge", "iron" }),
            }),
        new("Sa Pa Mountain Lodge", "Hoàng Văn Sơn", "sapa.lodge@demo.vn", "0907000016", "Nhà nghỉ",
            "Sa Pa", "Sa Pa", "Cầu Mây", "15 Mường Hoa", 22.3364m, 103.8438m, "cozy-mountain-cabin-in-snowy-woods",
            "Nhà gỗ trên sườn núi, nhìn ra thung lũng Mường Hoa và ruộng bậc thang.",
            new RoomSeed[]
            {
                new("Phòng Gỗ Nhìn Thung Lũng", "Tiêu chuẩn", 2, 1, 720_000, 20, "Cửa sổ lớn nhìn ra ruộng bậc thang.", new[] { "balcony", "breakfast" }),
                new("Phòng Gia Đình Có Lò Sưởi", "Gia đình", 4, 2, 1_300_000, 35, "Ấm áp mùa đông, có lò sưởi.", new[] { "breakfast", "parking", "bath" }),
            }),
        new("Nha Trang Bay Resort", "Bùi Thanh Tùng", "nhatrang.bay@demo.vn", "0908000017", "Resort",
            "Nha Trang", "Lộc Thọ", "Lộc Thọ", "50 Trần Phú", 12.2389m, 109.1967m, "beachfront-resort-aerial-view",
            "Resort ven vịnh Nha Trang, bãi biển riêng và hồ bơi lớn.",
            new RoomSeed[]
            {
                new("Phòng Đôi Hướng Vịnh", "Cao cấp", 2, 1, 1_450_000, 30, "Nhìn ra vịnh Nha Trang và đảo Hòn Tre.", new[] { "sea-view", "pool", "gym", "breakfast", "elevator" }),
                new("Phòng Hai Giường Đơn", "Tiêu chuẩn", 2, 2, 1_050_000, 26, "Phù hợp bạn bè đi cùng.", new[] { "pool", "breakfast", "elevator" }),
                new("Suite Hướng Biển", "Suite", 3, 1, 2_900_000, 52, "Phòng khách riêng và ban công rộng hướng biển.", new[] { "sea-view", "balcony", "pool", "gym", "breakfast", "bath" }),
            }),
        new("Phú Quốc Pool Villa", "Ngô Gia Hân", "phuquoc.villa@demo.vn", "0909000018", "Biệt thự",
            "Phú Quốc", "Dương Tơ", "Dương Tơ", "Bãi Trường", 10.1590m, 103.9774m, "modern-luxury-villa-with-pool-thumbnail",
            "Biệt thự có hồ bơi riêng gần Bãi Trường, ngắm hoàng hôn trên biển.",
            new RoomSeed[]
            {
                new("Villa Hồ Bơi Riêng 2 Phòng Ngủ", "Biệt thự", 4, 2, 4_800_000, 120, "Hồ bơi riêng, bếp và phòng khách mở.", new[] { "pool", "kitchen", "washer", "parking", "breakfast" }),
                new("Bungalow Vườn Nhiệt Đới", "Bungalow", 2, 1, 1_800_000, 35, "Bungalow giữa vườn, cách biển 200 m.", new[] { "pool", "breakfast", "parking" }),
            }),
        new("Nhà Biển Bãi Sau", "Lý Hoàng Nam", "baisau.vungtau@demo.vn", "0910000019", "Nhà nghỉ",
            "Vũng Tàu", "Phường 2", "Phường 2", "08 Thùy Vân", 10.3460m, 107.0980m, "beachfront-cottage-with-blue-shutters",
            "Nhà nghỉ đối diện bãi Sau, tiện đi Mũi Nghinh Phong và tượng Chúa.",
            new RoomSeed[]
            {
                new("Phòng Đôi Gần Biển", "Tiêu chuẩn", 2, 1, 600_000, 18, "Đi bộ 1 phút ra biển.", new[] { "parking" }),
                new("Phòng Nhóm 6 Người", "Gia đình", 6, 3, 1_500_000, 40, "Phù hợp nhóm bạn đi chơi cuối tuần.", new[] { "parking", "fridge", "kitchen" }),
            }),
        new("Tràng An Ecolodge", "Trịnh Văn Phúc", "trangan.ecolodge@demo.vn", "0911000020", "Homestay",
            "Ninh Bình", "Hoa Lư", "Trường Yên", "Thôn Tụ An", 20.2530m, 105.9140m, "main-exterior-of-the-mountain-guesthouse",
            "Homestay sinh thái giữa núi đá vôi Tràng An, có xe đạp miễn phí.",
            new RoomSeed[]
            {
                new("Phòng Nhìn Núi Đá", "Tiêu chuẩn", 2, 1, 750_000, 22, "Cửa sổ nhìn ra núi đá vôi.", new[] { "breakfast", "parking", "balcony" }),
                new("Bungalow Bên Hồ", "Bungalow", 3, 2, 1_200_000, 30, "Bungalow sát hồ sen.", new[] { "breakfast", "parking" }),
            }),
        new("Mũi Né Sand Dunes Homestay", "Đỗ Thị Thảo", "muine.dunes@demo.vn", "0912000021", "Homestay",
            "Phan Thiết", "Mũi Né", "Mũi Né", "Hòn Rơm", 10.9333m, 108.2870m, "eco-friendly-modern-home-in-the-desert",
            "Homestay gần đồi cát Mũi Né, có tour xem bình minh trên đồi cát.",
            new RoomSeed[]
            {
                new("Phòng Đôi Đồi Cát", "Tiêu chuẩn", 2, 1, 680_000, 20, "Thiết kế tối giản, yên tĩnh.", new[] { "parking", "breakfast" }),
                new("Phòng Gia Đình Sân Vườn", "Gia đình", 4, 2, 1_150_000, 32, "Sân vườn riêng, gần biển.", new[] { "parking", "breakfast", "fridge" }),
            }),
        new("Quy Nhơn Beach House", "Huỳnh Minh Trí", "quynhon.beach@demo.vn", "0913000022", "Nhà nghỉ",
            "Quy Nhơn", "Nhơn Lý", "Nhơn Lý", "Kỳ Co", 13.8000m, 109.2800m, "serene-beach-house-on-white-sands",
            "Nhà trên cát trắng gần Kỳ Co - Eo Gió, biển trong xanh.",
            new RoomSeed[]
            {
                new("Phòng Đôi Bên Biển", "Tiêu chuẩn", 2, 1, 900_000, 22, "Ngay trên bãi cát.", new[] { "sea-view", "breakfast", "parking" }),
                new("Phòng Gia Đình Hướng Biển", "Gia đình", 4, 2, 1_600_000, 36, "Hướng biển, có bếp nhỏ.", new[] { "sea-view", "kitchen", "breakfast", "parking" }),
            }),
    };

    private record CustomerSeed(string Name, string Email, string Phone, string Address);

    private static readonly CustomerSeed[] Customers =
    {
        new("Khách Thử", "khach@test.local", "0901000001", "Hà Nội"),
        new("Nguyễn Văn An", "nguyen.van.an@demo.vn", "0931000001", "Cầu Giấy, Hà Nội"),
        new("Trần Thị Bích", "tran.thi.bich@demo.vn", "0931000002", "Quận 3, TP. Hồ Chí Minh"),
        new("Lê Hoàng Cường", "le.hoang.cuong@demo.vn", "0931000003", "Hải Châu, Đà Nẵng"),
        new("Phạm Ngọc Dung", "pham.ngoc.dung@demo.vn", "0931000004", "Ninh Kiều, Cần Thơ"),
        new("Hoàng Minh Đức", "hoang.minh.duc@demo.vn", "0931000005", "Đống Đa, Hà Nội"),
        new("Vũ Thị Giang", "vu.thi.giang@demo.vn", "0931000006", "Bình Thạnh, TP. Hồ Chí Minh"),
        new("Đặng Văn Hải", "dang.van.hai@demo.vn", "0931000007", "Lê Chân, Hải Phòng"),
        new("Bùi Thu Hương", "bui.thu.huong@demo.vn", "0931000008", "Thanh Xuân, Hà Nội"),
        new("Ngô Quang Khải", "ngo.quang.khai@demo.vn", "0931000009", "Thủ Đức, TP. Hồ Chí Minh"),
        new("Đỗ Mai Linh", "do.mai.linh@demo.vn", "0931000010", "Biên Hòa, Đồng Nai"),
        new("Phan Tuấn Minh", "phan.tuan.minh@demo.vn", "0931000011", "Vinh, Nghệ An"),
        new("Trương Thảo Nhi", "truong.thao.nhi@demo.vn", "0931000012", "Huế"),
    };

    private static readonly string[] GoodReviews =
    {
        "Phòng sạch sẽ, chủ nhà thân thiện, sẽ quay lại.", "Vị trí rất thuận tiện, đi bộ ra trung tâm.",
        "Giường êm, cách âm tốt, ngủ rất ngon.", "Bữa sáng ngon, nhân viên nhiệt tình.",
        "View đẹp hơn ảnh, rất đáng tiền.", "Check-in nhanh, phòng đúng như mô tả.",
        "Không gian yên tĩnh, phù hợp nghỉ dưỡng gia đình.", "Giá hợp lý so với chất lượng.",
    };

    private static readonly string[] HostReplies =
    {
        "Cảm ơn bạn đã chọn ở cùng chúng tôi, hẹn gặp lại bạn lần sau nhé!",
        "Rất vui vì bạn có kỳ nghỉ thoải mái. Lần sau ghé nhớ nhắn trước để được phòng đẹp nhé.",
        "Cảm ơn lời khen của bạn, cả nhà mình sẽ cố gắng giữ phòng sạch đẹp như vậy.",
    };

    private static readonly string[] HostApologies =
    {
        "Cảm ơn góp ý của bạn. Bên mình đã nhắc nhân viên và sửa lại chỗ bạn nói, mong được đón bạn lần sau.",
        "Xin lỗi vì chưa làm bạn hài lòng hoàn toàn. Góp ý của bạn đã được ghi nhận để cải thiện.",
    };

    private static readonly string[] OkReviews =
    {
        "Phòng ổn nhưng wifi hơi yếu buổi tối.", "Vị trí đẹp, phòng tắm hơi nhỏ.",
        "Nhân viên dễ thương, nhưng chỗ đỗ xe hơi chật.",
    };

    private static readonly string[] BookingNotes =
    {
        "Cho mình phòng tầng cao nếu được.", "Mình đến khoảng 10 giờ tối.", "Có trẻ em 5 tuổi đi cùng.",
        "Kỷ niệm ngày cưới, nhờ trang trí giúp ạ.", "", "", "",
    };

    public static async Task RunAsync(AppDbContext db, ILogger logger, string? adminPassword = null)
    {
        var adminPwd = string.IsNullOrWhiteSpace(adminPassword) ? AdminPassword : adminPassword;
        var rng = new Random(20261117);
        var today = DateTime.Today;

        logger.LogWarning("Demo seed: wiping every table in {Database}", db.Database.GetDbConnection().Database);
        await WipeAsync(db);

        // ── Vai trò ──
        var roles = new[] { AppRoles.Admin, AppRoles.Partner, AppRoles.Customer }
            .ToDictionary(r => r, r => new Role { RoleName = r });
        db.Roles.AddRange(roles.Values);

        // ── Tiện ích ──
        var amenities = AmenityList.ToDictionary(a => a.Icon, a => new Amenity { Icon = a.Icon, Name = a.Name, Group = a.Group });
        db.Amenities.AddRange(amenities.Values);
        await db.SaveChangesAsync();

        var avatarIndex = 0;
        string NextAvatar() => Img(Avatars[avatarIndex++ % Avatars.Length]);

        User NewUser(string name, string email, string phone, string password, string role, string? address, DateTime createdAt)
        {
            var user = new User
            {
                FullName = name, Email = email, Username = email, Phone = phone, Address = address,
                Password = BCrypt.Net.BCrypt.HashPassword(password), Avatar = NextAvatar(),
                Status = "active", CreatedAt = createdAt,
            };
            user.UserRoles.Add(new UserRole { User = user, Role = roles[role] });
            db.Users.Add(user);
            return user;
        }

        // ── Admin ──
        NewUser("Quản trị StayMaster", AdminEmail, "0900000000", adminPwd, AppRoles.Admin, "Hà Nội", today.AddMonths(-14));

        // ── Đối tác, cơ sở, phòng ──
        var allRooms = new List<(Room Room, Property Property, Partner Partner)>();
        var partners = new List<Partner>();
        var interiorIndex = 0;
        var roomNo = 100;
        for (var pi = 0; pi < Properties.Length; pi++)
        {
            var p = Properties[pi];
            var owner = NewUser(p.Owner, p.Email, p.Phone, p.Email.EndsWith("@test.local") ? TestPassword : DemoPassword,
                AppRoles.Partner, p.City, today.AddMonths(-13).AddDays(pi * 3));
            var partner = new Partner
            {
                User = owner, PartnerCode = $"DT{pi + 1:D3}", BusinessName = p.Business, BusinessType = p.Type,
                Address = $"{p.Address}, {p.Ward}, {p.District}, {p.City}", Description = p.Description,
                Status = "active", CreatedAt = owner.CreatedAt,
            };
            db.Partners.Add(partner);
            partners.Add(partner);

            var property = new Property
            {
                Partner = partner, Name = p.Business, Type = p.Type, City = p.City, District = p.District, Ward = p.Ward,
                DetailedAddress = p.Address, Latitude = p.Lat, Longitude = p.Lng, Description = p.Description,
                ImageUrl = Img(p.Cover), DefaultCheckInTime = new TimeSpan(14, 0, 0), DefaultCheckOutTime = new TimeSpan(12, 0, 0),
                Status = "active", CreatedAt = owner.CreatedAt,
            };
            db.Properties.Add(property);

            db.PartnerBankAccounts.Add(new PartnerBankAccount
            {
                Partner = partner, BankName = new[] { "Vietcombank", "Techcombank", "MB Bank", "ACB", "BIDV" }[pi % 5],
                AccountNumber = $"10{pi + 1:D2}{rng.Next(10_000_000, 99_999_999)}", AccountHolder = RemoveDiacritics(p.Owner).ToUpperInvariant(),
                IsPrimary = true, CreatedAt = owner.CreatedAt!.Value,
            });

            foreach (var r in p.Rooms)
            {
                roomNo++;
                var room = new Room
                {
                    Property = property, Name = r.Name, RoomCode = $"P{roomNo}", RoomType = r.Type, Capacity = r.Capacity,
                    BedCount = r.Beds, BasePrice = r.Price, Area = r.Area, Description = r.Description,
                    Status = "approved", IsApproved = true, CreatedAt = owner.CreatedAt!.Value.AddDays(1),
                    // Một phần ba số phòng có giá cuối tuần cao hơn ~20%
                    WeekendPrice = roomNo % 3 == 0 ? Math.Round(r.Price * 1.2m / 10_000) * 10_000 : null,
                };
                // 5 ảnh mỗi phòng: phòng đầu tiên của cơ sở lấy ảnh mặt tiền làm ảnh chính, các phòng khác lấy ảnh nội thất
                var interiors = Enumerable.Range(0, 4).Select(_ => Interiors[interiorIndex++ % Interiors.Length]).ToList();
                var images = r == p.Rooms[0]
                    ? new[] { p.Cover }.Concat(interiors).ToArray()
                    : new[] { interiors[0], p.Cover }.Concat(interiors.Skip(1)).ToArray();
                for (var i = 0; i < images.Length; i++)
                    room.Images.Add(new RoomImage { Room = room, Url = Img(images[i]), IsMainImage = i == 0, DisplayOrder = i, CreatedAt = room.CreatedAt });
                foreach (var icon in BaseAmenities.Concat(r.Amenities).Distinct())
                    room.RoomAmenities.Add(new RoomAmenity { Room = room, Amenity = amenities[icon] });
                db.Rooms.Add(room);
                allRooms.Add((room, property, partner));
            }
        }

        // Vài phòng ở các trạng thái khác để admin/đối tác có việc để xem
        void SetState(int index, string status, bool approved) { allRooms[index].Room.Status = status; allRooms[index].Room.IsApproved = approved; }
        SetState(2, "pending", false);    // Phòng Đơn Tiết Kiệm (chủ nhà thử) chờ duyệt
        SetState(15, "pending", false);
        SetState(23, "hidden", true);
        SetState(28, "rejected", false);

        // ── Khách hàng ──
        var customers = Customers.Select((c, i) => NewUser(c.Name, c.Email, c.Phone,
            c.Email.EndsWith("@test.local") ? TestPassword : DemoPassword, AppRoles.Customer, c.Address, today.AddMonths(-12).AddDays(i * 5))).ToList();
        await db.SaveChangesAsync();

        // ── Đơn đặt phòng: 12 tháng trước tới 2 tháng sau, không chồng lịch trên cùng phòng ──
        var bookable = allRooms.Where(x => x.Room.IsApproved == true && x.Room.Status == "approved").ToList();
        var booked = bookable.ToDictionary(x => x.Room.Id, _ => new List<(DateTime In, DateTime Out)>());
        var bookings = new List<(Booking Booking, Partner Partner)>();

        bool TryBook(User customer, (Room Room, Property Property, Partner Partner) target, DateTime checkIn, int nights, string status)
        {
            var checkOut = checkIn.AddDays(nights);
            if (booked[target.Room.Id].Any(b => checkIn < b.Out && checkOut > b.In)) return false;
            booked[target.Room.Id].Add((checkIn, checkOut));

            var quote = Pricing.Quote(target.Room, checkIn, checkOut,
                new SystemSettingsSnapshot(ServiceFeeRate, TaxRate, 30, "", ""));
            var (serviceFee, tax, total) = (quote.ServiceFee, quote.Tax, quote.Total);
            var created = checkIn.AddDays(-rng.Next(3, 30)).AddHours(rng.Next(7, 23)).AddMinutes(rng.Next(60));
            if (created > DateTime.Now) created = DateTime.Now.AddHours(-rng.Next(1, 48));
            // Đơn chờ thanh toán chỉ giữ phòng 30 phút: tạo trong vài phút gần đây để còn thấy khi mở demo
            if (status == BookingStatus.Pending) created = DateTime.Now.AddMinutes(-rng.Next(1, 10));

            var booking = new Booking
            {
                User = customer, Room = target.Room, CheckIn = checkIn, CheckOut = checkOut, Nights = nights,
                GuestCount = Math.Max(1, Math.Min(target.Room.Capacity, rng.Next(1, target.Room.Capacity + 1))),
                CheckInTime = new TimeSpan(14, 0, 0), CheckOutTime = new TimeSpan(12, 0, 0),
                PricePerNight = target.Room.BasePrice, ServiceFee = serviceFee, Tax = tax, TotalPrice = total, Deposit = total,
                Status = status, Note = BookingNotes[rng.Next(BookingNotes.Length)], CreatedAt = created, UpdatedAt = created,
            };
            db.Bookings.Add(booking);
            bookings.Add((booking, target.Partner));
            return true;
        }

        // Khách thử luôn có đủ trạng thái để thử nút huỷ / tải hoá đơn
        var tester = customers[0];
        var testerRoom = bookable.First();
        TryBook(tester, testerRoom, today.AddDays(-40), 2, BookingStatus.Completed);
        TryBook(tester, bookable[3], today.AddDays(20), 2, BookingStatus.Pending);
        TryBook(tester, bookable[5], today.AddDays(35), 3, BookingStatus.Confirmed);

        // Quá khứ: hoàn thành hoặc huỷ
        for (var i = 0; i < 70; i++)
        {
            var customer = customers[rng.Next(customers.Count)];
            var target = bookable[rng.Next(bookable.Count)];
            var checkIn = today.AddDays(-rng.Next(5, 360));
            TryBook(customer, target, checkIn, rng.Next(1, 5), rng.NextDouble() < 0.85 ? BookingStatus.Completed : BookingStatus.Cancelled);
        }
        // Đang lưu trú hôm nay
        foreach (var target in bookable.OrderBy(_ => rng.Next()).Take(5))
            TryBook(customers[rng.Next(customers.Count)], target, today.AddDays(-rng.Next(0, 2)), rng.Next(2, 4), BookingStatus.CheckedIn);
        // Sắp tới: đã xác nhận / chờ duyệt / huỷ
        for (var i = 0; i < 25; i++)
        {
            var roll = rng.NextDouble();
            var status = roll < 0.55 ? BookingStatus.Confirmed : roll < 0.9 ? BookingStatus.Pending : BookingStatus.Cancelled;
            TryBook(customers[rng.Next(customers.Count)], bookable[rng.Next(bookable.Count)], today.AddDays(rng.Next(1, 60)), rng.Next(1, 5), status);
        }
        await db.SaveChangesAsync();

        // ── Đánh giá cho đơn đã hoàn thành ──
        foreach (var (b, _) in bookings.Where(x => x.Booking.Status == BookingStatus.Completed))
        {
            if (rng.NextDouble() > 0.8) continue;
            var stars = rng.NextDouble() < 0.75 ? 5 : rng.NextDouble() < 0.8 ? 4 : 3;
            db.Reviews.Add(new Review
            {
                UserId = b.UserId, RoomId = b.RoomId, BookingId = b.Id, Stars = stars,
                Comment = stars >= 4 ? GoodReviews[rng.Next(GoodReviews.Length)] : OkReviews[rng.Next(OkReviews.Length)],
                CreatedAt = b.CheckOut.AddDays(rng.Next(0, 3)),
            });
        }
        await db.SaveChangesAsync();

        // Chủ nhà trả lời khoảng 40% đánh giá
        foreach (var review in await db.Reviews.ToListAsync())
        {
            if (rng.NextDouble() > 0.4) continue;
            review.Reply = review.Stars >= 4 ? HostReplies[rng.Next(HostReplies.Length)] : HostApologies[rng.Next(HostApologies.Length)];
            review.ReplyAt = review.CreatedAt?.AddDays(rng.Next(0, 2)).AddHours(rng.Next(1, 10));
        }
        await db.SaveChangesAsync();

        // Điểm trung bình theo cơ sở
        foreach (var property in allRooms.Select(x => x.Property).Distinct())
        {
            var stars = await db.Reviews.Where(r => r.Room.PropertyId == property.Id).Select(r => r.Stars).ToListAsync();
            property.TotalReviews = stars.Count;
            property.AverageRating = stars.Count > 0 ? Math.Round((decimal)stars.Average(), 2) : null;
        }

        // ── Số dư và rút tiền của đối tác ──
        foreach (var partner in partners)
        {
            var earned = bookings.Where(x => x.Partner == partner && x.Booking.Status == BookingStatus.Completed)
                .Sum(x => Pricing.PartnerEarning(x.Booking));
            partner.WalletBalance = earned;
            if (earned < 1_000_000) continue;

            var bank = await db.PartnerBankAccounts.FirstAsync(a => a.PartnerId == partner.Id);
            var paid = Math.Round(earned * 0.4m / 100_000) * 100_000;
            db.Payouts.Add(new Payout
            {
                PartnerId = partner.Id, Amount = paid, BankName = bank.BankName, AccountNumber = bank.AccountNumber, AccountHolder = bank.AccountHolder,
                Status = "paid", TransactionId = $"FT{rng.Next(10_000_000, 99_999_999)}", AdminNote = "Đã chuyển khoản",
                CreatedAt = today.AddDays(-rng.Next(20, 60)), UpdatedAt = today.AddDays(-rng.Next(10, 19)), PaidAt = today.AddDays(-rng.Next(10, 19)),
            });
            if (rng.NextDouble() < 0.6)
                db.Payouts.Add(new Payout
                {
                    PartnerId = partner.Id, Amount = Math.Round(earned * 0.2m / 100_000) * 100_000, BankName = bank.BankName,
                    AccountNumber = bank.AccountNumber, AccountHolder = bank.AccountHolder, Status = "pending", CreatedAt = today.AddDays(-rng.Next(0, 4)),
                });
        }
        await db.SaveChangesAsync();

        // ── Hòm thư: vài cuộc trò chuyện mẫu ──
        var hostOf = allRooms.ToDictionary(x => x.Room.Id, x => x.Partner);
        async Task Conversation(User customer, Room? room, string source, params (string Sender, string Text, int MinutesAgo)[] messages)
        {
            var partner = room is null ? null : hostOf[room.Id];
            var start = DateTime.Now.AddMinutes(-messages[0].MinutesAgo);
            var last = messages[^1];
            var q = new InboxQuestion
            {
                CustomerUserId = customer.Id, PartnerId = partner?.Id, RoomId = room?.Id, Question = messages[0].Text, Source = source,
                Status = last.Sender == InboxSender.Customer ? "open" : "answered", CreatedAt = start,
                LastMessageAt = DateTime.Now.AddMinutes(-last.MinutesAgo),
            };
            var lastReply = messages.LastOrDefault(m => m.Sender != InboxSender.Customer);
            if (lastReply.Text is not null)
            {
                q.Answer = lastReply.Text;
                q.AnsweredAt = DateTime.Now.AddMinutes(-lastReply.MinutesAgo);
                q.AnsweredByUserId = partner?.UserId;
            }
            foreach (var m in messages)
                q.Messages.Add(new InboxMessage
                {
                    Sender = m.Sender, Content = m.Text, CreatedAt = DateTime.Now.AddMinutes(-m.MinutesAgo),
                    SenderUserId = m.Sender == InboxSender.Customer ? customer.Id : partner?.UserId,
                });
            db.InboxQuestions.Add(q);
            await db.SaveChangesAsync();
        }

        var hoanKiemRoom = allRooms[0].Room;
        await Conversation(customers[0], hoanKiemRoom, "chatbot",
            (InboxSender.Customer, "Phòng có cho mang theo mèo nhỏ không ạ?", 600),
            (InboxSender.Host, "Chào bạn, mèo nhỏ được nhé, phụ thu 50.000đ/đêm.", 540),
            (InboxSender.Customer, "Dạ vậy mình đặt 2 đêm cuối tuần này nhé.", 30));
        await Conversation(customers[1], hoanKiemRoom, "direct",
            (InboxSender.Customer, "Mình nhận phòng sớm lúc 9 giờ sáng được không?", 1500),
            (InboxSender.Host, "Được bạn nhé nếu phòng trống, bạn nhắn trước 1 ngày giúp mình.", 1400));
        await Conversation(customers[2], allRooms[4].Room, "chatbot",
            (InboxSender.Customer, "Từ sân bay Tân Sơn Nhất về khách sạn mất bao lâu?", 90));
        await Conversation(customers[3], allRooms[10].Room, "direct",
            (InboxSender.Customer, "Resort có xe đưa đón sân bay Đà Nẵng không?", 300),
            (InboxSender.Host, "Có xe 7 chỗ, phí 250.000đ/chiều. Bạn gửi giờ bay giúp mình nhé.", 240));
        await Conversation(customers[4], null, "chatbot",
            (InboxSender.Customer, "Mình chuyển khoản rồi nhưng đơn vẫn chờ duyệt, nhờ kiểm tra giúp.", 45));

        // ── Yêu thích, ngày chặn, thông báo mẫu ──
        foreach (var x in bookable.Skip(6).Take(3))
            db.Favorites.Add(new Favorite { UserId = tester.Id, RoomId = x.Room.Id, CreatedAt = DateTime.Now.AddDays(-rng.Next(1, 20)) });
        var hostRoom = allRooms[1].Room;
        var blockFrom = today.AddDays(10);
        if (!booked[hostRoom.Id].Any(b => blockFrom < b.Out && blockFrom.AddDays(2) > b.In))
            db.RoomBlocks.Add(new RoomBlock { RoomId = hostRoom.Id, FromDate = blockFrom, ToDate = blockFrom.AddDays(2), Reason = "Sơn lại tường, thay rèm", CreatedAt = DateTime.Now });

        var adminUser = await db.Users.FirstAsync(u => u.Email == AdminEmail);
        var hostUser = allRooms[0].Partner.UserId;
        void Notify(int userId, string title, string content, string type, string link, int minutesAgo, bool read = false) =>
            db.Notifications.Add(new Notification
            {
                UserId = userId, Title = title, Content = content, Type = type, Link = link,
                IsRead = read, CreatedAt = DateTime.Now.AddMinutes(-minutesAgo),
            });
        Notify(tester.Id, "Chủ nhà đã trả lời bạn", "Chào bạn, mèo nhỏ được nhé, phụ thu 50.000đ/đêm.", "inbox", "chat:1", 540);
        Notify(tester.Id, "Chuyến đi thế nào?", "Cảm ơn bạn đã ở Phố Cổ. Viết vài dòng đánh giá để giúp khách sau nhé.", "review", "/my-bookings", 60 * 24 * 30, read: true);
        Notify(hostUser, $"{tester.FullName} đang chờ trả lời", "Dạ vậy mình đặt 2 đêm cuối tuần này nhé.", "inbox", "/business/inbox?c=1", 30);
        Notify(hostUser, "Đã chuyển tiền cho bạn", "Yêu cầu rút tiền đã được duyệt.", "payout", "/business/payments", 60 * 24 * 12, read: true);
        Notify(adminUser.Id, "Phòng mới chờ duyệt", "Có phòng mới cần kiểm tra trước khi hiển thị.", "room", "/admin/listings?tab=pending", 120);
        Notify(adminUser.Id, $"{customers[4].FullName} đang chờ trả lời", "Mình chuyển khoản rồi nhưng đơn vẫn chờ duyệt, nhờ kiểm tra giúp.", "inbox", "/admin/inbox?c=5", 45);

        await db.SaveChangesAsync();
        logger.LogWarning("Demo seed done: {Partners} partners, {Rooms} rooms, {Customers} customers, {Bookings} bookings, {Reviews} reviews",
            partners.Count, allRooms.Count, customers.Count, bookings.Count, await db.Reviews.CountAsync());
    }

    /// <summary>Xoá dữ liệu mọi bảng (trừ lịch sử migration) và đặt lại bộ đếm id.</summary>
    private static async Task WipeAsync(AppDbContext db)
    {
        var tables = await db.Database.SqlQueryRaw<string>(
            "SELECT name AS Value FROM sys.tables WHERE name <> '__EFMigrationsHistory'").ToListAsync();

        foreach (var t in tables) await db.Database.ExecuteSqlRawAsync($"ALTER TABLE [{t}] NOCHECK CONSTRAINT ALL");
        foreach (var t in tables) await db.Database.ExecuteSqlRawAsync($"DELETE FROM [{t}]");
        foreach (var t in tables) await db.Database.ExecuteSqlRawAsync($"ALTER TABLE [{t}] WITH CHECK CHECK CONSTRAINT ALL");

        var withIdentity = await db.Database.SqlQueryRaw<string>(
            "SELECT OBJECT_NAME(object_id) AS Value FROM sys.identity_columns WHERE last_value IS NOT NULL " +
            "AND OBJECT_NAME(object_id) <> '__EFMigrationsHistory'").ToListAsync();
        foreach (var t in withIdentity) await db.Database.ExecuteSqlRawAsync($"DBCC CHECKIDENT ('[{t}]', RESEED, 0)");
        db.ChangeTracker.Clear();
    }

    private static string RemoveDiacritics(string text) => RecommendationService.Normalize(text);
}
