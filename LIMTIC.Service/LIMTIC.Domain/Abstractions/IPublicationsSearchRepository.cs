using LIMTIC.Domain.Models.Publications;

namespace LIMTIC.Domain.Abstractions
{
    public interface IPublicationsSearchRepository
    {
        Task IndexPublicationAsync(PublicationSearchDocument publication);

        Task DeleteFromIndexAsync(Guid publicationId);

        // Supports: Recherche plein texte (titre, auteurs, mots-clés).
        Task<IEnumerable<PublicationSearchDocument>> SearchAsync(string searchTerm);
    }
}
