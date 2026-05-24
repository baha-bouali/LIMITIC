using LIMTIC.Application.DTOs.Publications;
using LIMTIC.Domain.Entities.Publications;
using LIMTIC.Domain.Enums;

namespace LIMTIC.Application.Mappings
{
    public static class PublicationMappings
    {
        // ─── Entity → type-specific detail DTOs ───────────────────────────────

        public static JournalArticleDto ToDto(this JournalArticleEntity e) =>
            new(e.Id, e.JournalName, e.Volume, e.Number, e.Pages,
                e.Ranking, e.GetRankingLabel());

        public static TechnicalReportDto ToDto(this TechnicalReportEntity e) =>
            new(e.Id, e.ReportNumber, e.Institution);

        public static BookChapterDto ToDto(this BookChapterEntity e) =>
            new(e.Id, e.BookTitle, e.Publisher, e.Isbn, e.Pages);

        public static NationalConferenceDto ToDto(this NationalConferenceEntity e) =>
            new(e.Id, e.ConferenceName, e.Location, e.Pages);

        public static InternationalConferenceDto ToDto(this InternationalConferenceEntity e) =>
            new(e.Id, e.ConferenceName, e.Location, e.Pages,
                e.Ranking, e.GetRankingLabel());

        // ─── Entity → full DTO ─────────────────────────────────────────────────

        public static PublicationDto ToDto(this PublicationEntity e) =>
            new(e.Id,
                e.UserId,
                e.User is not null ? $"{e.User.FirstName} {e.User.LastName}".Trim() : string.Empty,
                e.ResearchAxisId,
                e.ResearchAxis?.Title ?? string.Empty,
                e.Title,
                e.Abstract,
                e.Keywords,
                e.AttachedPdfs ?? [],
                e.Doi,
                e.Venue,
                e.Type,
                e.Status,
                e.Visibility,
                e.Year,
                e.Authors,
                e.CreatedAtUtc,
                e.JournalArticle?.ToDto(),
                e.TechnicalReport?.ToDto(),
                e.BookChapter?.ToDto(),
                e.NationalConference?.ToDto(),
                e.InternationalConference?.ToDto());

        // ─── Entity → summary DTO ──────────────────────────────────────────────

        public static PublicationSummaryDto ToSummaryDto(this PublicationEntity e) =>
            new(e.Id,
                e.Title,
                e.Abstract,
                e.Doi,
                e.Venue,
                e.Authors,
                e.AttachedPdfs ?? [],
                e.Type,
                e.Status,
                e.Visibility,
                e.Year,
                e.UserId,
                e.User is not null ? $"{e.User.FirstName} {e.User.LastName}".Trim() : string.Empty,
                e.ResearchAxisId,
                e.ResearchAxis?.Title ?? string.Empty,
                // Expose ranking only for the matching type; null otherwise
                e.JournalArticle?.Ranking,
                e.InternationalConference?.Ranking);

        // ─── Collection helpers ────────────────────────────────────────────────

        public static IEnumerable<PublicationDto> ToDtos(
            this IEnumerable<PublicationEntity> entities) =>
            entities.Select(e => e.ToDto());

        public static IEnumerable<PublicationSummaryDto> ToSummaryDtos(
            this IEnumerable<PublicationEntity> entities) =>
            entities.Select(e => e.ToSummaryDto());

        // ─── Request DTOs → entities ───────────────────────────────────────────

        /// <summary>Maps a CreatePublicationRequest to a new PublicationEntity.
        /// UserId must be set by the caller (controller / service).</summary>
        public static PublicationEntity ToEntity(this CreatePublicationRequest r) =>
            new()
            {
                ResearchAxisId = r.ResearchAxisId,
                Title = r.Title,
                Abstract = r.Abstract,
                Keywords = r.Keywords,
                Doi = r.Doi,
                Venue = r.Venue,
                Type = r.Type,
                Visibility = r.Visibility,
                Year = r.Year,
                Authors = r.Authors,
                AttachedPdfs = [],
                JournalArticle = r.JournalArticle is null ? null : new JournalArticleEntity
                {
                    JournalName = r.JournalArticle.JournalName,
                    Volume = r.JournalArticle.Volume,
                    Number = r.JournalArticle.Number,
                    Pages = r.JournalArticle.Pages,
                    Ranking = r.JournalArticle.Ranking
                },
                TechnicalReport = r.TechnicalReport is null ? null : new TechnicalReportEntity
                {
                    ReportNumber = r.TechnicalReport.ReportNumber,
                    Institution = r.TechnicalReport.Institution
                },
                BookChapter = r.BookChapter is null ? null : new BookChapterEntity
                {
                    BookTitle = r.BookChapter.BookTitle,
                    Publisher = r.BookChapter.Publisher,
                    Isbn = r.BookChapter.Isbn,
                    Pages = r.BookChapter.Pages
                },
                NationalConference = r.NationalConference is null ? null : new NationalConferenceEntity
                {
                    ConferenceName = r.NationalConference.ConferenceName,
                    Location = r.NationalConference.Location,
                    Pages = r.NationalConference.Pages
                },
                InternationalConference = r.InternationalConference is null ? null : new InternationalConferenceEntity
                {
                    ConferenceName = r.InternationalConference.ConferenceName,
                    Location = r.InternationalConference.Location,
                    Pages = r.InternationalConference.Pages,
                    Ranking = r.InternationalConference.Ranking
                }
            };

        /// <summary>Maps an UpdatePublicationRequest onto an existing PublicationEntity id.</summary>
        public static PublicationEntity ToEntity(this UpdatePublicationRequest r, Guid id) =>
            new()
            {
                Id = id,
                ResearchAxisId = r.ResearchAxisId,
                Title = r.Title,
                Abstract = r.Abstract,
                Keywords = r.Keywords,
                Doi = r.Doi,
                Venue = r.Venue,
                Type = r.Type,
                Visibility = r.Visibility,
                Year = r.Year,
                Authors = r.Authors,
                AttachedPdfs = [],   // PDFs are managed via the dedicated PDF endpoints
                JournalArticle = r.JournalArticle is null ? null : new JournalArticleEntity
                {
                    JournalName = r.JournalArticle.JournalName,
                    Volume = r.JournalArticle.Volume,
                    Number = r.JournalArticle.Number,
                    Pages = r.JournalArticle.Pages,
                    Ranking = r.JournalArticle.Ranking
                },
                TechnicalReport = r.TechnicalReport is null ? null : new TechnicalReportEntity
                {
                    ReportNumber = r.TechnicalReport.ReportNumber,
                    Institution = r.TechnicalReport.Institution
                },
                BookChapter = r.BookChapter is null ? null : new BookChapterEntity
                {
                    BookTitle = r.BookChapter.BookTitle,
                    Publisher = r.BookChapter.Publisher,
                    Isbn = r.BookChapter.Isbn,
                    Pages = r.BookChapter.Pages
                },
                NationalConference = r.NationalConference is null ? null : new NationalConferenceEntity
                {
                    ConferenceName = r.NationalConference.ConferenceName,
                    Location = r.NationalConference.Location,
                    Pages = r.NationalConference.Pages
                },
                InternationalConference = r.InternationalConference is null ? null : new InternationalConferenceEntity
                {
                    ConferenceName = r.InternationalConference.ConferenceName,
                    Location = r.InternationalConference.Location,
                    Pages = r.InternationalConference.Pages,
                    Ranking = r.InternationalConference.Ranking
                }
            };
    }
}