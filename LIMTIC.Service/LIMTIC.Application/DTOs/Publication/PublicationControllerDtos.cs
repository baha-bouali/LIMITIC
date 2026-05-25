using LIMTIC.Domain.Enums;

namespace LIMTIC.Application.DTOs.Publications
{
    public sealed record PublicationAxeDto(Guid Id, string Title);

    public sealed record PublicPublicationSummaryItemDto(
        Guid Id,
        string Type,
        string PublicationType,
        int Year,
        string Title,
        string[] Authors,
        string? Venue,
        string? Doi,
        string? Ranking,
        string? CoreRanking,
        PublicationAxeDto Axe);

    public sealed record PublicationStatsDto(int Total, int Journals, int Conferences);

    public sealed record PaginationDto(int Total, int Page, int Limit, int TotalPages);

    public sealed record PublicPublicationsListResultDto(
        List<PublicPublicationSummaryItemDto> Data,
        PublicationStatsDto Stats,
        PaginationDto Pagination);

    public sealed record PublicPublicationCardDto(
        Guid Id,
        string Type,
        string PublicationType,
        int Year,
        string Title,
        string[] Authors,
        string? Venue,
        string? Doi,
        string? Ranking,
        string? CoreRanking);

    public sealed record PublicPublicationDetailDto(
        Guid Id,
        string Type,
        string PublicationType,
        string Status,
        string Visibility,
        string Title,
        int Year,
        string[] Authors,
        string Abstract_,
        string[] Keywords,
        string? Doi,
        string? Venue,
        string? PdfUrl,
        PublicationAxeDto Axe,
        // Journal-specific
        string? JournalName,
        string? Volume,
        string? Number,
        string? Pages,
        string? Ranking,
        // International conference-specific
        string? CoreRanking,
        string? Location,
        // Book chapter-specific
        string? BookTitle,
        string? Publisher,
        string? Isbn,
        // Technical report-specific
        long? ReportNumber,
        string? Institution);

    // Dashboard list / detail DTOs

    public sealed record DashboardPublicationSummaryItemDto(
        Guid Id,
        string Type,
        string Title,
        int Year,
        string Status,
        string Visibility,
        string? Quartile,
        string? CoreRanking,
        string[] Authors,
        string? Venue,
        string? Doi,
        PublicationAxeDto Axe,
        string SubmittedBy,
        string? RejectionReason);

    public sealed record DashboardPublicationsListResultDto(
        List<DashboardPublicationSummaryItemDto> Data,
        PaginationDto Pagination);

    public sealed record CreateDashboardPublicationResultDto(
        string Message,
        Guid Id,
        string Status);

    public sealed record DashboardPublicationDetailDto(
        PublicPublicationDetailDto Publication,
        string SubmittedBy,
        string? RejectionReason);

    public sealed record PublicationStatusUpdateResultDto(
        string Message,
        Guid Id,
        string Status,
        string? Extra);
}

