using LIMTIC.Domain.Abstractions.Publications;
using LIMTIC.Domain.Entities.Publications;
using LIMTIC.Domain.Enums;
using LIMTIC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LIMTIC.Infrastructure.Repositories.Publications
{
    public class PublicationRepository : IPublicationRepository
    {
        private readonly AppDbContext _context;

        public PublicationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PublicationEntity?> GetByIdAsync(Guid id)
        {
            return await _context.Publications
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PublicationEntity?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.Publications
                .Include(p => p.User)
                .Include(p => p.ResearchAxis)
                .Include(p => p.JournalArticle)
                .Include(p => p.TechnicalReport)
                .Include(p => p.BookChapter)
                .Include(p => p.NationalConference)
                .Include(p => p.InternationalConference)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<(IEnumerable<PublicationEntity> Items, int TotalCount)> GetFilteredAsync(
            PublicationType? type,
            PublicationStatus? status,
            PublicationVisibility? visibility,
            Guid? userId,
            Guid? researchAxisId,
            int? year,
            string? search,
            int page,
            int limit)
        {
            var query = _context.Publications
                .Include(p => p.User)
                .Include(p => p.ResearchAxis)
                .AsQueryable();

            if (type.HasValue)
            {
                query = query.Where(p => p.Type == type.Value);

                switch (type.Value)
                {
                    case PublicationType.Journal:
                        query = query.Include(p => p.JournalArticle);
                        break;
                    case PublicationType.InternationalConference:
                        query = query.Include(p => p.InternationalConference);
                        break;
                    case PublicationType.NationalConference:
                        query = query.Include(p => p.NationalConference);
                        break;
                    case PublicationType.BookChapter:
                        query = query.Include(p => p.BookChapter);
                        break;
                    case PublicationType.TechnicalReport:
                        query = query.Include(p => p.TechnicalReport);
                        break;
                }
            }

            if (status.HasValue)
                query = query.Where(p => p.Status == status.Value);

            if (visibility.HasValue)
                query = query.Where(p => p.Visibility == visibility.Value);

            if (userId.HasValue)
                query = query.Where(p => p.UserId == userId.Value);

            if (researchAxisId.HasValue)
                query = query.Where(p => p.ResearchAxisId == researchAxisId.Value);

            if (year.HasValue)
                query = query.Where(p => p.Year == year.Value);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.ToLower();
                query = query.Where(p =>
                    p.Title.ToLower().Contains(lowerSearch) ||
                    (p.Abstract != null && p.Abstract.ToLower().Contains(lowerSearch)));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.Year)
                .ThenByDescending(p => p.CreatedAtUtc)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task AddAsync(PublicationEntity entity)
        {
            await _context.Publications.AddAsync(entity);
        }

        public void Update(PublicationEntity entity)
        {
            _context.Publications.Update(entity);
        }

        public void Remove(PublicationEntity entity)
        {
            _context.Publications.Remove(entity);
        }
    }
}