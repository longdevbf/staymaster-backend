using HotelManagement.API.Common;
using HotelManagement.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.API.Controllers;

public class AuditLogDto
{
    public long Id { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int? UserId { get; set; }
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? TableName { get; set; }
    public string? EntityKey { get; set; }
    public string? OldData { get; set; }
    public string? NewData { get; set; }
}

/// <summary>Nhật ký thao tác (log_he_thong) cho Admin (SRS 3f).</summary>
[ApiController]
[Route("api/admin/audit-logs")]
[Authorize(Roles = AppRoles.Admin)]
public class AuditLogController : ControllerBase
{
    private readonly AppDbContext _db;
    public AuditLogController(AppDbContext db) => _db = db;

    // GET /api/admin/audit-logs?page=1&pageSize=50&action=LOGIN_FAILED&q=email
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 50,
        [FromQuery] string? action = null, [FromQuery] string? q = null)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 10, 200);

        var query = _db.AuditLogs.AsQueryable();
        if (!string.IsNullOrWhiteSpace(action)) query = query.Where(l => l.Action == action);
        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            var userIds = _db.Users.Where(u => u.Email.Contains(term) || u.FullName.Contains(term)).Select(u => (int?)u.Id);
            query = query.Where(l => (l.EntityKey != null && l.EntityKey.Contains(term)) || userIds.Contains(l.UserId));
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(l => l.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(l => new AuditLogDto
            {
                Id = l.Id,
                CreatedAt = l.CreatedAt,
                UserId = l.UserId,
                UserName = _db.Users.Where(u => u.Id == l.UserId).Select(u => u.FullName).FirstOrDefault(),
                UserEmail = _db.Users.Where(u => u.Id == l.UserId).Select(u => u.Email).FirstOrDefault(),
                Action = l.Action,
                TableName = l.TableName,
                EntityKey = l.EntityKey,
                OldData = l.OldData,
                NewData = l.NewData,
            })
            .ToListAsync();

        var actions = await _db.AuditLogs.Select(l => l.Action).Distinct().OrderBy(a => a).ToListAsync();
        return Ok(new { total, page, pageSize, actions, items });
    }
}
