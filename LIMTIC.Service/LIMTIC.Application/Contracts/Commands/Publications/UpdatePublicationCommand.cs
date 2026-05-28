using LIMTIC.Application.DTOs.Publications;

namespace LIMTIC.Application.Contracts.Commands.Publications
{
    public class UpdatePublicationCommand
    {
        public PublicationDto Publication { get; set; }
    }
}

