using Lab5TestTask.Data;
using Lab5TestTask.Enums;
using Lab5TestTask.Models;
using Lab5TestTask.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lab5TestTask.Services.Implementations;

/// <summary>
/// SessionService implementation.
/// Implement methods here.
/// </summary>
public class SessionService : ISessionService
{
    private readonly ApplicationDbContext _dbContext;

    public SessionService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Session> GetSessionAsync()
    {
        return await _dbContext.Sessions.OrderBy(s => s.StartedAtUTC).FirstOrDefaultAsync();
    }

    public async Task<List<Session>> GetSessionsAsync()
    {
        var date2025 = new DateTime(2025, 1, 1);
        var activeUserIds = await _dbContext.Users.Where(u => u.Status == UserStatus.Active).Select(u => u.Id).ToListAsync();
        var sessionsBefore2025 = await _dbContext.Sessions.Where(s => s.EndedAtUTC < date2025 && activeUserIds.Contains(s.UserId))
            .Select(s => new Session
            {
                Id = s.Id,
                StartedAtUTC = s.StartedAtUTC,
                EndedAtUTC = s.EndedAtUTC,
                DeviceType = s.DeviceType,
                UserId = s.UserId
            })
            .ToListAsync();

        return sessionsBefore2025;
    }
}
