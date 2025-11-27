using IncidentsTI.Domain.Entities;
using IncidentsTI.Domain.Interfaces;
using IncidentsTI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IncidentsTI.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio de notificaciones
/// </summary>
public class NotificationRepository : INotificationRepository
{
    private readonly ApplicationDbContext _context;

    public NotificationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Notification?> GetByIdAsync(int id)
    {
        return await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == id && n.IsActive);
    }

    public async Task<IEnumerable<Notification>> GetByUserIdAsync(string userId, bool includeRead = true)
    {
        var query = _context.Notifications
            .Where(n => n.UserId == userId && n.IsActive);

        if (!includeRead)
        {
            query = query.Where(n => !n.IsRead);
        }

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(string userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead && n.IsActive)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(string userId)
    {
        return await _context.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead && n.IsActive);
    }

    public async Task<Notification> CreateAsync(Notification notification)
    {
        notification.CreatedAt = DateTime.UtcNow;
        notification.IsActive = true;
        notification.IsRead = false;
        
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
        
        return notification;
    }

    public async Task<IEnumerable<Notification>> CreateManyAsync(IEnumerable<Notification> notifications)
    {
        var notificationList = notifications.ToList();
        var now = DateTime.UtcNow;
        
        foreach (var notification in notificationList)
        {
            notification.CreatedAt = now;
            notification.IsActive = true;
            notification.IsRead = false;
        }
        
        _context.Notifications.AddRange(notificationList);
        await _context.SaveChangesAsync();
        
        return notificationList;
    }

    public async Task<bool> MarkAsReadAsync(int notificationId)
    {
        var notification = await _context.Notifications.FindAsync(notificationId);
        
        if (notification == null || !notification.IsActive)
        {
            return false;
        }

        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> MarkAllAsReadAsync(string userId)
    {
        var now = DateTime.UtcNow;
        
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead && n.IsActive)
            .ToListAsync();

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
            notification.ReadAt = now;
        }

        await _context.SaveChangesAsync();
        return notifications.Count;
    }

    public async Task<bool> DeleteAsync(int notificationId)
    {
        var notification = await _context.Notifications.FindAsync(notificationId);
        
        if (notification == null)
        {
            return false;
        }

        // Soft delete
        notification.IsActive = false;
        
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<(IEnumerable<Notification> Items, int TotalCount)> GetPaginatedAsync(
        string userId, 
        int page, 
        int pageSize, 
        bool includeRead = true)
    {
        var query = _context.Notifications
            .Where(n => n.UserId == userId && n.IsActive);

        if (!includeRead)
        {
            query = query.Where(n => !n.IsRead);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
