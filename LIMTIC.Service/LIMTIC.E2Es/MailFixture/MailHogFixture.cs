using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace LIMTIC.E2Es.MailFixture
{
    public class MailHogFixture : IAsyncLifetime
    {
        private IContainer _container;

        public int SmtpPort => _container.GetMappedPublicPort(1025);
        public string ApiUrl => $"http://localhost:{_container.GetMappedPublicPort(8025)}";

        public async Task InitializeAsync()
        {
            _container = new ContainerBuilder()
                .WithImage("mailhog/mailhog")
                .WithPortBinding(1025, assignRandomHostPort: true)
                .WithPortBinding(8025, assignRandomHostPort: true)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(8025).ForPath("/")))
                .Build();
            await _container.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await _container.DisposeAsync();
        }
    }
}