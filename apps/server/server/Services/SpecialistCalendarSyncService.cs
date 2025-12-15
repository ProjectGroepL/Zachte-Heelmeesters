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

            if (specialistIcal == null || string.IsNullOrWhiteSpace(specialistIcal.IcalUrl))
                return;

            var icsContent = await _httpClient.GetStringAsync(specialistIcal.IcalUrl);

            var calendar = Calendar.Load(icsContent);

            var existing = _context.SpecialistPrivateAgendas
                .Where(a => a.UserId == userId);
            _context.SpecialistPrivateAgendas.RemoveRange(existing);

            var events = calendar.Events ?? new List<CalendarEvent>();

            foreach (var ev in events)
            {
                var start = ev.DtStart?.AsSystemLocal;
                var end = ev.DtEnd?.AsSystemLocal;

                if (start == null || end == null)
                    continue;
                
                // var debug = $"ICAL EVENT: UID={(ev.UID ?? "<null>")}, Start={start.Value:O}, End={end.Value:O}, Summary={(ev.Summary ?? "<null>")}, Location={(ev.Location ?? "<null>")}";
                // System.Diagnostics.Debug.WriteLine(debug);
                
                var entity = new SpecialistPrivateAgenda
                {
                    Uid = ev.UID,           // adapt if your PK is different
                    UserId = userId,
                    StartTime = start.Value,        // adapt field names
                    EndTime = end.Value,
                    Title = ev.Summary,
                    Description = ev.Description,
                    Location = ev.Location
                };
                
                _context.SpecialistPrivateAgendas.Add(entity);
            }

            await _context.SaveChangesAsync();
        }
    }
}