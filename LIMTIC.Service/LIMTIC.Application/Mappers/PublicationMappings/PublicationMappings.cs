using LIMTIC.Application.DTOs.Publications;
using LIMTIC.Domain.Entities.Publications;

namespace LIMTIC.Application.Mappings
{
    public static class PublicationMappings
    {
        public static PublicationDto ToDto(this PublicationEntity e) =>
            new PublicationDto{
                Id = e.Id,
                UserId = e.UserId,
                ResearchAxisId = e.ResearchAxisId,
                Title = e.Title,
                Abstract = e.Abstract,
                Keywords = e.Keywords,
                Doi = e.Doi,
                Venue = e.Venue,
                Type = e.Type,
                Status = e.Status,
                Visibility = e.Visibility,
                Year = e.Year,
                Authors = e.Authors,
                JournalArticle = e.JournalArticle?.ToDto(),
                TechnicalReport = e.TechnicalReport?.ToDto(),
                BookChapter = e.BookChapter?.ToDto(),
                NationalConference = e.NationalConference?.ToDto(),
                InternationalConference = e.InternationalConference?.ToDto() 
            };

        public static IEnumerable<PublicationDto> ToDtos(
            this IEnumerable<PublicationEntity> entities) =>
            entities.Select(e => e.ToDto());

        public static JournalArticleDto ToDto(this JournalArticleEntity j) =>
            new JournalArticleDto
            {
                JournalName = j.JournalName,
                Volume = j.Volume,
                Ranking = j.Ranking,
                Pages = j.Pages,
                Number = j.Number
            };

        public static TechnicalReportDto ToDto(this TechnicalReportEntity t) =>
            new TechnicalReportDto
            {
                Institution = t.Institution,
                ReportNumber = t.ReportNumber
            };

        public static BookChapterDto ToDto(this BookChapterEntity b) =>
            new BookChapterDto
            {
                BookTitle = b.BookTitle,
                Publisher = b.Publisher,
                Isbn = b.Isbn,
                Pages = b.Pages
            };

        public static NationalConferenceDto ToDto(this NationalConferenceEntity n) =>
            new NationalConferenceDto
            {
                ConferenceName = n.ConferenceName,
                Location = n.Location,
            };

        public static InternationalConferenceDto ToDto(this InternationalConferenceEntity i) =>
            new InternationalConferenceDto
            {
                ConferenceName = i.ConferenceName,
                Location = i.Location,
                Ranking = i.Ranking
            };

        public static PublicationEntity ToEntity(this PublicationDto d) =>
            new PublicationEntity
            {
                Id = d.Id.Value,
                UserId = d.UserId,
                ResearchAxisId = d.ResearchAxisId,
                Title = d.Title,
                Abstract = d.Abstract,
                Keywords = d.Keywords,
                Doi = d.Doi,
                Venue = d.Venue,
                Type = d.Type,
                Status = d.Status,
                Visibility = d.Visibility,
                Year = d.Year,
                Authors = d.Authors
            };

        public static JournalArticleEntity ToEntity(this JournalArticleDto j) =>
            new JournalArticleEntity
            {
                JournalName = j.JournalName,
                Volume = j.Volume,
                Ranking = j.Ranking,
                Pages = j.Pages,
                Number = j.Number
            };

        public static TechnicalReportEntity ToEntity(this TechnicalReportDto t) =>
            new TechnicalReportEntity
            {
                Institution = t.Institution,
                ReportNumber = t.ReportNumber
            };

        public static BookChapterEntity ToEntity(this BookChapterDto b) =>
            new BookChapterEntity
            {
                BookTitle = b.BookTitle,
                Publisher = b.Publisher,
                Isbn = b.Isbn,
                Pages = b.Pages
            };

        public static NationalConferenceEntity ToEntity(this NationalConferenceDto n) =>
            new NationalConferenceEntity
            {
                ConferenceName = n.ConferenceName,
                Location = n.Location,
            };

        public static InternationalConferenceEntity ToEntity(this InternationalConferenceDto i) =>
            new InternationalConferenceEntity
            {
                ConferenceName = i.ConferenceName,
                Location = i.Location,
                Ranking = i.Ranking
            };
    }
}