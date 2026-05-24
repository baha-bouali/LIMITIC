using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities.Events;
using LIMTIC.Domain.Enums;
using LIMTIC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LIMTIC.Infrastructure.Repositories
{
    public class EventsRepository : IEventsRepository
    {
        private readonly AppDbContext _context;

        public EventsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<EventEntity>> GetEventsAsync(string? status, string? type, int page, int limit, string? q)
        {
            var query = _context.Events.AsQueryable();

            if (!string.IsNullOrWhiteSpace(type))
            {
                if (Enum.TryParse<EventType>(type, ignoreCase: true, out var parsedType))
                    query = query.Where(e => e.Type == parsedType);
                else
                    query = query.Where(_ => false);
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                var searchTerm = q.ToLower();
                query = query.Where(e => e.Title.ToLower().Contains(searchTerm) ||
                                         e.Description.ToLower().Contains(searchTerm));
            }

            // Fetch data first, then filter by status in memory (since Status is computed)
            var events = await query
                .OrderByDescending(e => e.StartDate)
                .Include(e => e.Speakers)
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(status))
                events = events.Where(e => e.Status.ToString().ToLower() == status.ToLower()).ToList();

            // Apply pagination after filtering
            var paginatedEvents = events
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToList();

            return paginatedEvents;
        }

        public async Task<int> GetTotalEventsCountAsync(string? status, string? type, string? q)
        {
            var query = _context.Events.AsQueryable();

            if (!string.IsNullOrWhiteSpace(type))
            {
                if (Enum.TryParse<EventType>(type, ignoreCase: true, out var parsedType))
                    query = query.Where(e => e.Type == parsedType);
                else
                    query = query.Where(_ => false);
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                var searchTerm = q.ToLower();
                query = query.Where(e => e.Title.ToLower().Contains(searchTerm) ||
                                         e.Description.ToLower().Contains(searchTerm));
            }

            // Fetch data first, then filter by status in memory (since Status is computed)
            var events = await query.ToListAsync();

            if (!string.IsNullOrWhiteSpace(status))
                events = events.Where(e => e.Status.ToString().ToLower() == status.ToLower()).ToList();

            return events.Count;
        }
    }
}
