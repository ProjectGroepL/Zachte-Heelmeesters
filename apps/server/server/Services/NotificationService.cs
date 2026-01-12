using Microsoft.EntityFrameworkCore;
using ZhmApi.Data;
using ZhmApi.Models;

namespace ZhmApi.Services
{
    public class NotificationService
    {
        private readonly ApiContext _context;

        public NotificationService(ApiContext context)
        {
            _context = context;
        }

        public async Task Create(
            int userId,
            string message,
            NotificationType type,
            int? accessRequestId = null
        )
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                Type = type,
                AccessRequestId = accessRequestId
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Notification>> GetForUser(int userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkAsRead(int notificationId, int userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n =>
                    n.Id == notificationId &&
                    n.UserId == userId
                );

            if (notification == null)
                throw new UnauthorizedAccessException();

            notification.IsRead = true;
            await _context.SaveChangesAsync();
        }

        public async Task MarkAllAsRead(int userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var n in notifications)
            {
                n.IsRead = true;
            }

            await _context.SaveChangesAsync();
        }
            }
}
