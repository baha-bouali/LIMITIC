using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities.Publications;
using LIMTIC.Domain.Enums;
using LIMTIC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LIMTIC.Infrastructure.Repositories
{
    public class PublicationRepository : IPublicationRepository
    {
        private readonly AppDbContext _context;

        public PublicationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PublicationEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.Publications
                .FirstOrDefaultAsync(p => p.Id == id, ct);
        }

        public async Task<PublicationEntity?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.Publications
                .Include(p => p.User)
                .Include(p => p.ResearchAxis)
                .Include(p => p.JournalArticle)
                .Include(p => p.TechnicalReport)
                .Include(p => p.BookChapter)
                .Include(p => p.NationalConference)
                .Include(p => p.InternationalConference)
                .FirstOrDefaultAsync(p => p.Id == id, ct);
        }

        public async Task<IEnumerable<PublicationEntity>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            return await _context.Publications
                .Where(p => p.UserId == userId)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<PublicationEntity>> GetByUserIdAndStatusAsync(
            Guid userId, PublicationStatus status, CancellationToken ct = default)
        {
            return await _context.Publications
                .Where(p => p.UserId == userId && p.Status == status)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<PublicationEntity>> GetByStatusAndVisibilityAsync(
            PublicationStatus status, PublicationVisibility visibility, CancellationToken ct = default)
        {
            return await _context.Publications
                .Where(p => p.Status == status && p.Visibility == visibility)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<PublicationEntity>> GetByResearchAxisIdAsync(Guid researchAxisId, CancellationToken ct = default)
        {
            return await _context.Publications
                .Where(p => p.ResearchAxisId == researchAxisId)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<PublicationEntity>> GetByTypeAsync(PublicationType type, CancellationToken ct = default)
        {
            return await _context.Publications
                .Where(p => p.Type == type)
                .ToListAsync(ct);
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
            int pageSize, 
            CancellationToken ct = default)
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
                    case PublicationType.ArticleJournal:
                        query = query.Include(p => p.JournalArticle);
                        break;
                    case PublicationType.ConferenceInternational:
                        query = query.Include(p => p.InternationalConference);
                        break;
                    case PublicationType.ConferenceNational:
                        query = query.Include(p => p.NationalConference);
                        break;
                    case PublicationType.ChapterBook:
                        query = query.Include(p => p.BookChapter);
                        break;
                    case PublicationType.TechnicalReport:
                        query = query.Include(p => p.TechnicalReport);
                        break;
                }
            }
            else 
            {
                query = query
                    .Include(p => p.JournalArticle)
                    .Include(p => p.InternationalConference);
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

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(p => p.Year)
                .ThenByDescending(p => p.CreatedAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, totalCount);
        }

        public async Task<int> CountByStatusAndVisibilityAsync(
            PublicationStatus status, PublicationVisibility visibility, CancellationToken ct = default)
        {
            return await _context.Publications
                .CountAsync(p => p.Status == status && p.Visibility == visibility, ct);
        }

        public async Task AddAsync(PublicationEntity entity, CancellationToken ct = default)
        {
            await _context.Publications.AddAsync(entity, ct);
        }

        public void Update(PublicationEntity entity)
        {
            _context.Publications.Update(entity);
        }

        public void Remove(PublicationEntity entity)
        {
            _context.Publications.Remove(entity);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}