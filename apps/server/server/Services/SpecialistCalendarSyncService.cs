using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Microsoft.EntityFrameworkCore;
using ZhmApi.Data;
using ZhmApi.Models;

namespace ZhmApi.Services
{
    public interface ISpecialistCalendarSyncService
    {
        Task SyncFromIcalAsync(int userId);
    }

    public class SpecialistCalendarSyncService : ISpecialistCalendarSyncService
    {
        private readonly ApiContext _context;
        private readonly HttpClient _httpClient;

        public SpecialistCalendarSyncService(ApiContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        public async Task SyncFromIcalAsync(int userId)
        {
            var specialistIcal = await _context.SpecialistIcals
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (specialistIcal == null || string.IsNullOrWhiteSpace(specialistIcal.Url))
                return;

            var icsContent = await _httpClient.GetStringAsync(specialistIcal.Url);

            var calendar = Calendar.Load(icsContent);

            var existing = _context.SpecialistPrivateAgendas
                .Where(a => a.UserId == userId);
            _context.SpecialistPrivateAgendas.RemoveRange(existing);

            var eventsCollection = calendar.Events;
            var events = eventsCollection != null
                ? eventsCollection.ToList()
                : new List<CalendarEvent>();

            foreach (var ev in events)
            {
                var start = ev.DtStart?.Value;
                var end = ev.DtEnd?.Value;

                if (start == null || end == null)
                    continue;

                // var debug = $"ICAL EVENT: UID={(ev.Uid ?? "<null>")}, Start={start.Value:O}, End={end.Value:O}, Summary={(ev.Summary ?? "<null>")}, Location={(ev.Location ?? "<null>")}";
                // System.Diagnostics.Debug.WriteLine(debug);

                var entity = new SpecialistPrivateAgenda
                {
                    Uid = ev.Uid,
                    Start = start.Value,
                    End = end.Value,
                    UserId = userId,
                    UpdatedAt = DateTime.UtcNow
                };
                
                _context.SpecialistPrivateAgendas.Add(entity);
            }

            await _context.SaveChangesAsync();
        }
    }
}