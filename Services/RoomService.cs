using HotelManagement.API.Common;
using HotelManagement.API.Data;
using HotelManagement.API.DTOs.Room;
using HotelManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.API.Services;

public interface IRoomService
{
    Task<List<RoomResponse>> GetAllRoomsAsync();
    Task<List<RoomResponse>> GetAdminRoomsAsync();
    Task<List<RoomResponse>> GetPartnerRoomsAsync(int partnerId);
    Task<RoomResponse?> GetRoomByIdAsync(int id);
    Task<(RoomResponse? room, string? error)> CreateRoomAsync(CreateRoomRequest req);
    Task<(RoomResponse? room, string? error)> UpdateRoomAsync(int id, UpdateRoomRequest req);
    Task<bool> ApproveRoomAsync(int id);
    Task<bool> RejectRoomAsync(int id, string? reason);
    Task<bool> DeleteRoomAsync(int id);
    Task<bool> SetHiddenAsync(int id, bool hidden);
    Task<List<int>> GetUnavailableRoomIdsAsync(DateTime checkIn, DateTime checkOut);
    Task<bool> IsOwnedByPartnerUserAsync(int roomId, int userId);
    Task<List<AmenityDto>> GetAmenitiesAsync();
}

public class RoomService : IRoomService
{
    private readonly AppDbContext _db;
    private readonly ILogger<RoomService> _logger;

    public RoomService(AppDbContext db, ILogger<RoomService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<RoomResponse>> GetAllRoomsAsync()
    {
        var list = await _db.Rooms
            .Include(r => r.Property)
            .Include(r => r.Images)
            .Include(r => r.RoomAmenities).ThenInclude(ra => ra.Amenity)
            .Where(r => r.IsApproved == true && r.Status != "rejected" && r.Status != "deleted" && r.Status != "hidden")
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
        
        return list.Select(MapToResponse).ToList();
    }

    public async Task<List<RoomResponse>> GetAdminRoomsAsync()
    {
        var list = await _db.Rooms
            .Include(r => r.Property)
            .Include(r => r.Images)
            .Include(r => r.RoomAmenities).ThenInclude(ra => ra.Amenity)
            .Where(r => r.Status != "deleted")
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
        
        return list.Select(MapToResponse).ToList();
    }

    /// <param name="partnerId">Id người dùng (nguoi_dung.id) của đối tác.</param>
    public async Task<List<RoomResponse>> GetPartnerRoomsAsync(int partnerId)
    {
        var partnerRecordId = await _db.Partners
            .Where(p => p.UserId == partnerId)
            .Select(p => p.Id)
            .FirstOrDefaultAsync();
        if (partnerRecordId == 0) return new List<RoomResponse>();

        var property = await _db.Properties.FirstOrDefaultAsync(p => p.PartnerId == partnerRecordId);
        if (property == null) return new List<RoomResponse>();

        var list = await _db.Rooms
            .Include(r => r.Property)
            .Include(r => r.Images)
            .Include(r => r.RoomAmenities).ThenInclude(ra => ra.Amenity)
            .Where(r => r.PropertyId == property.Id && r.Status != "deleted")
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return list.Select(MapToResponse).ToList();
    }

    public async Task<RoomResponse?> GetRoomByIdAsync(int id)
    {
        var room = await _db.Rooms
            .Include(r => r.Property)
            .Include(r => r.Images)
            .Include(r => r.RoomAmenities).ThenInclude(ra => ra.Amenity)
            .FirstOrDefaultAsync(r => r.Id == id);
            
        if (room is null) return null;
        
        return MapToResponse(room);
    }

    public async Task<(RoomResponse? room, string? error)> CreateRoomAsync(CreateRoomRequest req)
    {
        if (await _db.Rooms.AnyAsync(r => r.RoomCode == req.RoomCode))
        {
            return (null, "Mã phòng đã tồn tại");
        }

        var room = new Room
        {
            PropertyId = req.PropertyId,
            Name = req.Name,
            RoomCode = req.RoomCode,
            RoomType = req.RoomType,
            Capacity = req.Capacity,
            BedCount = req.BedCount,
            BasePrice = req.BasePrice,
            Area = req.Area,
            Description = req.Description,
            WeekendPrice = req.WeekendPrice is > 0 ? req.WeekendPrice : null,
            Status = "pending",
            IsApproved = false,
            CreatedAt = DateTime.Now
        };

        _db.Rooms.Add(room);
        await _db.SaveChangesAsync();

        if (req.ImageUrls != null && req.ImageUrls.Any())
        {
            var images = req.ImageUrls.Select((url, index) => new RoomImage
            {
                RoomId = room.Id,
                Url = url,
                IsMainImage = index == 0,
                DisplayOrder = index,
                CreatedAt = DateTime.Now
            }).ToList();
            
            _db.RoomImages.AddRange(images);
            await _db.SaveChangesAsync();
        }

        if (req.AmenityIds is { Count: > 0 })
        {
            await SetAmenitiesAsync(room.Id, req.AmenityIds);
            await _db.SaveChangesAsync();
        }

        return (await GetRoomByIdAsync(room.Id) ?? MapToResponse(room), null);
    }

    public async Task<(RoomResponse? room, string? error)> UpdateRoomAsync(int id, UpdateRoomRequest req)
    {
        var room = await _db.Rooms.FindAsync(id);
        if (room is null)
            return (null, "Phòng không tồn tại");

        if (req.RoomCode is not null && req.RoomCode != room.RoomCode)
        {
            if (await _db.Rooms.AnyAsync(r => r.RoomCode == req.RoomCode))
                return (null, "Mã phòng mới đã tồn tại");
            
            room.RoomCode = req.RoomCode;
        }

        if (req.Name is not null) room.Name = req.Name;
        if (req.RoomType is not null) room.RoomType = req.RoomType;
        if (req.Capacity.HasValue) room.Capacity = req.Capacity.Value;
        if (req.BedCount.HasValue) room.BedCount = req.BedCount.Value;
        if (req.BasePrice.HasValue) room.BasePrice = req.BasePrice.Value;
        if (req.Area.HasValue) room.Area = req.Area.Value;
        if (req.Description is not null) room.Description = req.Description;
        if (req.WeekendPrice.HasValue) room.WeekendPrice = req.WeekendPrice > 0 ? req.WeekendPrice : null;
        if (req.Status is not null) room.Status = req.Status;
        if (req.IsApproved.HasValue) room.IsApproved = req.IsApproved.Value;
        if (req.PropertyId.HasValue) room.PropertyId = req.PropertyId.Value;

        room.UpdatedAt = DateTime.Now;

        if (req.AmenityIds is not null)
            await SetAmenitiesAsync(id, req.AmenityIds);

        if (req.ImageUrls is { Count: > 0 })
        {
            var oldImages = await _db.RoomImages.Where(i => i.RoomId == id).ToListAsync();
            _db.RoomImages.RemoveRange(oldImages);
            _db.RoomImages.AddRange(req.ImageUrls.Select((url, index) => new RoomImage
            {
                RoomId = id,
                Url = url,
                IsMainImage = index == 0,
                DisplayOrder = index,
                CreatedAt = DateTime.Now
            }));
        }

        await _db.SaveChangesAsync();

        return (await GetRoomByIdAsync(id), null);
    }

    public async Task<List<AmenityDto>> GetAmenitiesAsync()
    {
        return await _db.Amenities
            .OrderBy(a => a.Group).ThenBy(a => a.Name)
            .Select(a => new AmenityDto { Id = a.Id, Name = a.Name, Group = a.Group, Icon = a.Icon })
            .ToListAsync();
    }

    /// <summary>Thay toàn bộ tiện ích của phòng; id không tồn tại bị bỏ qua.</summary>
    private async Task SetAmenitiesAsync(int roomId, IEnumerable<int> amenityIds)
    {
        var wanted = amenityIds.Distinct().ToList();
        var valid = await _db.Amenities.Where(a => wanted.Contains(a.Id)).Select(a => a.Id).ToListAsync();

        var current = await _db.RoomAmenities.Where(ra => ra.RoomId == roomId).ToListAsync();
        _db.RoomAmenities.RemoveRange(current.Where(ra => !valid.Contains(ra.AmenityId)));
        _db.RoomAmenities.AddRange(valid
            .Where(aid => current.All(ra => ra.AmenityId != aid))
            .Select(aid => new RoomAmenity { RoomId = roomId, AmenityId = aid }));
    }

    public async Task<bool> DeleteRoomAsync(int id)
    {
        var room = await _db.Rooms.FirstOrDefaultAsync(r => r.Id == id);
        if (room is null) return false;

        bool hasBookingsOrReviews = await _db.Bookings.AnyAsync(b => b.RoomId == id) ||
                                     await _db.Reviews.AnyAsync(r => r.RoomId == id);
                                     
        if (hasBookingsOrReviews)
        {
            room.Status = "deleted";
            room.IsApproved = false; // Hide from public
            room.UpdatedAt = DateTime.Now;
        }
        else
        {
            // Clear from legacy tables not mapped in EF Core but existing in DB
            // Using raw SQL to satisfy foreign key constraints.
            // Wrap in try-catch to ignore errors if tables don't exist in different DB environments.
            try {
                await _db.Database.ExecuteSqlRawAsync("DELETE FROM duyet_bai_dang WHERE phong_id = {0}", id);
            } catch (Exception ex) { _logger.LogWarning(ex, "Could not clear duyet_bai_dang for room {RoomId}", id); }

            _db.RoomAmenities.RemoveRange(await _db.RoomAmenities.Where(ra => ra.RoomId == id).ToListAsync());

            // Direct query for associated images by RoomId to ensure all are deleted
            // to satisfy foreign key constraints.
            var images = await _db.RoomImages.Where(i => i.RoomId == id).ToListAsync();
            if (images.Any())
            {
                _db.RoomImages.RemoveRange(images);
            }
            _db.Rooms.Remove(room);
        }

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SetHiddenAsync(int id, bool hidden)
    {
        var room = await _db.Rooms.FindAsync(id);
        if (room is null) return false;

        room.Status = hidden ? "hidden" : "approved";
        if (!hidden) room.IsApproved = true;
        room.UpdatedAt = DateTime.Now;

        await _db.SaveChangesAsync();
        _logger.LogInformation("Room {RoomId} {Action} by admin", id, hidden ? "hidden" : "shown");
        return true;
    }

    /// <summary>Phòng đã có đơn (chưa huỷ) chồng lên khoảng ngày [checkIn, checkOut).</summary>
    public async Task<List<int>> GetUnavailableRoomIdsAsync(DateTime checkIn, DateTime checkOut)
    {
        return await _db.Bookings
            .Where(b => b.Status != BookingStatus.Cancelled && b.Status != BookingStatus.CancelledVi &&
                        checkIn < b.CheckOut && checkOut > b.CheckIn)
            .Select(b => b.RoomId)
            .Distinct()
            .ToListAsync();
    }

    public async Task<bool> IsOwnedByPartnerUserAsync(int roomId, int userId)
    {
        return await _db.Rooms.AnyAsync(r => r.Id == roomId && r.Property.Partner != null && r.Property.Partner.UserId == userId);
    }

    public async Task<bool> ApproveRoomAsync(int id)
    {
        var room = await _db.Rooms.FindAsync(id);
        if (room is null) return false;

        room.IsApproved = true;
        room.Status = "approved"; // lowercase!
        room.RejectReason = null;
        room.UpdatedAt = DateTime.Now;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RejectRoomAsync(int id, string? reason)
    {
        var room = await _db.Rooms.FindAsync(id);
        if (room is null) return false;

        room.IsApproved = false;
        room.Status = "rejected"; // will be mapped to "Hidden" in UI
        room.RejectReason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
        room.UpdatedAt = DateTime.Now;

        await _db.SaveChangesAsync();
        return true;
    }

    private static RoomResponse MapToResponse(Room r) => new()
    {
        Id = r.Id,
        PropertyId = r.PropertyId,
        Name = r.Name,
        RoomCode = r.RoomCode,
        RoomType = r.RoomType,
        Capacity = r.Capacity,
        BedCount = r.BedCount,
        BasePrice = r.BasePrice,
        Area = r.Area,
        Description = r.Description,
        Status = r.Status,
        IsApproved = r.IsApproved,
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt,
        // Lấy ảnh chính, nếu không có thì lấy ảnh đầu tiên
        MainImageUrl = r.Images?.FirstOrDefault(i => i.IsMainImage)?.Url ?? r.Images?.FirstOrDefault()?.Url,
        // Lấy toàn bộ danh sách ảnh cho gallery
        ImageUrls = r.Images?.OrderByDescending(i => i.IsMainImage).Select(i => i.Url).ToList() ?? new List<string>(),
        PropertyLocation = r.Property?.City,
        FullAddress = string.Join(", ", new[] { r.Property?.DetailedAddress, r.Property?.Ward, r.Property?.District, r.Property?.City }
            .Where(p => !string.IsNullOrWhiteSpace(p) && p != "Chưa cập nhật")),
        PropertyName = r.Property?.Name,
        PropertyDescription = r.Property?.Description,
        Rating = r.Property?.AverageRating,
        Latitude = r.Property?.Latitude,
        Longitude = r.Property?.Longitude,
        WeekendPrice = r.WeekendPrice,
        RejectReason = r.RejectReason,
        PropertyReviewCount = r.Property?.TotalReviews,
        CheckInTime = (r.Property?.DefaultCheckInTime ?? new TimeSpan(14, 0, 0)).ToString(@"hh\:mm"),
        CheckOutTime = (r.Property?.DefaultCheckOutTime ?? new TimeSpan(12, 0, 0)).ToString(@"hh\:mm"),
        Amenities = r.RoomAmenities?
            .Where(ra => ra.Amenity != null)
            .Select(ra => new AmenityDto { Id = ra.Amenity.Id, Name = ra.Amenity.Name, Group = ra.Amenity.Group, Icon = ra.Amenity.Icon })
            .OrderBy(a => a.Name)
            .ToList() ?? new List<AmenityDto>()
    };
}
