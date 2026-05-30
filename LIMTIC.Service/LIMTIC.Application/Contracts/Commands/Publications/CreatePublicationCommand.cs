using LIMTIC.Application.DTOs.Publications;

namespace LIMTIC.Application.Contracts.Commands.Publications
{
    public class CreatePublicationCommand
    {
        public PublicationDto Publication { get; set; }
    }
}

