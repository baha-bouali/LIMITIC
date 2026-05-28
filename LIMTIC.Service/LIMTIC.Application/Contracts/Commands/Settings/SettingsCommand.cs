namespace LIMTIC.Application.Contracts.Commands.Settings
{
    public class SettingsCommands
    {
        public class IdentityCommand
        {
            public string LabName { get; set; }
            public string LabSlogan { get; set; }
            public string ContactEmail { get; set; }
            public string Address { get; set; }
            public string Phone { get; set; }
            public string? LogoUrl { get; set; }
        }

        public class SmtpCommand
        {
            public string Host { get; set; }
            public int Port { get; set; }
            public string Username { get; set; }
            public string? Password { get; set; }
            public bool UseTls { get; set; }
        }

        public class SettingsCommand
        {
            public IdentityCommand Identity { get; set; }
            public SmtpCommand Smtp { get; set; }
        }
    }
}
