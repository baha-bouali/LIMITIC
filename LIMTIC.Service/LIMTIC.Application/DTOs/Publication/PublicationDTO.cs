using LIMTIC.Domain.Enums;

namespace LIMTIC.Application.DTOs.Publications
{
    // ─── Type-specific detail DTOs ─────────────────────────────────────────────

    public sealed record JournalArticleDto(
        Guid Id,
        string JournalName,
        string Volume,
        string Number,
        string Pages,
        JournalRanking Ranking,
        string RankingLabel);

    public sealed record TechnicalReportDto(
        Guid Id,
        long ReportNumber,
        string Institution);

    public sealed record BookChapterDto(
        Guid Id,
        string BookTitle,
        string Publisher,
        string? Isbn,
        string? Pages);

    public sealed record NationalConferenceDto(
        Guid Id,
        string ConferenceName,
        string Location,
        string? Pages);

    public sealed record InternationalConferenceDto(
        Guid Id,
        string ConferenceName,
        string Location,
        string? Pages,
        CoreRanking Ranking,
        string RankingLabel);

    // ─── Research-axis nested DTO ──────────────────────────────────────────────

    public sealed record ResearchAxisSummaryDto(Guid Id, string Title);

    // ─── Full publication DTO (detail view) ────────────────────────────────────

    /// <summary>
    /// Complete representation returned by GetByIdAsync and write operations.
    /// Exactly one type-specific property will be non-null per publication.
    /// </summary>
    public sealed record PublicationDto(
        Guid Id,
        Guid UserId,
        string UserFullName,
        Guid ResearchAxisId,
        string ResearchAxisName,
        string Title,
        string Abstract,
        string[] Keywords,
        string[] AttachedPdfs,
        string? Doi,
        string? Venue,
        PublicationType Type,
        PublicationStatus Status,
        PublicationVisibility Visibility,
        int Year,
        string[] Authors,
        DateTime CreatedAtUtc,
        JournalArticleDto? JournalArticle,
        TechnicalReportDto? TechnicalReport,
        BookChapterDto? BookChapter,
        NationalConferenceDto? NationalConference,
        InternationalConferenceDto? InternationalConference);

    // ─── Summary DTO (list / paginated views) ──────────────────────────────────

    /// <summary>
    /// Enriched summary used by every list endpoint across all roles.
    /// Carries the ranking fields and common metadata all list views need,
    /// without pulling heavy navigation graphs.
    /// </summary>
    public sealed record PublicationSummaryDto(
        Guid Id,
        string Title,
        string Abstract,
        string? Doi,
        string? Venue,
        string[] Authors,
        string[] AttachedPdfs,
        PublicationType Type,
        PublicationStatus Status,
        PublicationVisibility Visibility,
        int Year,
        Guid UserId,
        string UserFullName,
        Guid ResearchAxisId,
        string ResearchAxisName,
        // Ranking shorthands — null when the publication is not of that type
        JournalRanking? JournalRanking,
        CoreRanking? CoreRanking);

    // ─── Write request DTOs ────────────────────────────────────────────────────

    /// <summary>
    /// Body accepted by POST (create) endpoints across all roles.
    /// Maps to a PublicationEntity before being handed to the service.
    /// </summary>
    public sealed record CreatePublicationRequest(
        Guid ResearchAxisId,
        string Title,
        string Abstract,
        string[] Keywords,
        string? Doi,
        string? Venue,
        PublicationType Type,
        PublicationVisibility Visibility,
        int Year,
        string[] Authors,
        // ── Type-specific payloads (at most one should be set) ──
        CreateJournalArticleRequest? JournalArticle,
        CreateTechnicalReportRequest? TechnicalReport,
        CreateBookChapterRequest? BookChapter,
        CreateNationalConferenceRequest? NationalConference,
        CreateInternationalConferenceRequest? InternationalConference);

    /// <summary>
    /// Body accepted by PUT (update) endpoints across all roles.
    /// Identical shape to CreatePublicationRequest; Id comes from the route.
    /// </summary>
    public sealed record UpdatePublicationRequest(
        Guid ResearchAxisId,
        string Title,
        string Abstract,
        string[] Keywords,
        string? Doi,
        string? Venue,
        PublicationType Type,
        PublicationVisibility Visibility,
        int Year,
        string[] Authors,
        UpdateJournalArticleRequest? JournalArticle,
        UpdateTechnicalReportRequest? TechnicalReport,
        UpdateBookChapterRequest? BookChapter,
        UpdateNationalConferenceRequest? NationalConference,
        UpdateInternationalConferenceRequest? InternationalConference);

    // Type-specific create payloads
    public sealed record CreateJournalArticleRequest(
        string JournalName, string Volume, string Number, string Pages, JournalRanking Ranking);
    public sealed record CreateTechnicalReportRequest(long ReportNumber, string Institution);
    public sealed record CreateBookChapterRequest(string BookTitle, string Publisher, string? Isbn, string? Pages);
    public sealed record CreateNationalConferenceRequest(string ConferenceName, string Location, string? Pages);
    public sealed record CreateInternationalConferenceRequest(
        string ConferenceName, string Location, string? Pages, CoreRanking Ranking);

    // Type-specific update payloads (same shape, kept separate for future divergence)
    public sealed record UpdateJournalArticleRequest(
        string JournalName, string Volume, string Number, string Pages, JournalRanking Ranking);
    public sealed record UpdateTechnicalReportRequest(long ReportNumber, string Institution);
    public sealed record UpdateBookChapterRequest(string BookTitle, string Publisher, string? Isbn, string? Pages);
    public sealed record UpdateNationalConferenceRequest(string ConferenceName, string Location, string? Pages);
    public sealed record UpdateInternationalConferenceRequest(
        string ConferenceName, string Location, string? Pages, CoreRanking Ranking);
}