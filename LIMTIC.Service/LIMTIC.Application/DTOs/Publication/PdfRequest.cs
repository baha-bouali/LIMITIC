namespace LIMTIC.Application.DTOs.Publications;

public record PdfRequest(string PdfUrl);

public record UpdateVisibilityRequest(string Visibility);

public record RejectPublicationRequest(string? Reason);