using ZhmApi.Services;
using ZhmApi.Models;

namespace ZhmApi.Tests.POM
{
    public class InsuranceNotificationPom
    {
        private readonly NotificationService _notificationService;

        public InsuranceNotificationPom(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }
            public async Task<List<Notification>> GetNotifications(int insurerId)
            {
                return await _notificationService.GetForUser(insurerId);
            }
        }
    }
