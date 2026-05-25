using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities.Logs;
using LIMTIC.Domain.Enums;
using LIMTIC.UnitTests.Base;
using Microsoft.Extensions.DependencyInjection;

namespace LIMTIC.UnitTests.Tests
{
    public class AuditLogsRepositoryTests : BaseTests
    {
        [Fact]
        public async Task AddLog_PersistsAuditLog()
        {
            // Steps:
            // 1. Resolve audit logs repository
            // 2. Create an audit log entity
            // 3. Save the log
            // 4. Assert the log is persisted

            var repo = ServiceProvider.GetRequiredService<IAuditLogsRepository>();
            var timestamp = DateTime.UtcNow;
            var log = new AuditLogsEntity
            {
                Id = Guid.NewGuid(),
                ActorId = Guid.NewGuid(),
                Action = ActionType.CREATE,
                Resource = ResourceType.User,
                Timestamp = timestamp
            };

            var created = await repo.AddLog(log);
            Assert.True(created);

            var saved = DbContext.AuditLogs.FirstOrDefault(l => l.Id == log.Id);
            Assert.NotNull(saved);
            Assert.Equal(log.ActorId, saved.ActorId);
            Assert.Equal(ActionType.CREATE, saved.Action);
            Assert.Equal(ResourceType.User, saved.Resource);
            Assert.Equal(timestamp, saved.Timestamp);
        }

        [Fact]
        public async Task GetLogsByPeriodAsync_ReturnsOnlyLogsInRange()
        {
            // Steps:
            // 1. Resolve audit logs repository
            // 2. Seed logs before/inside/after target range
            // 3. Query logs by period
            // 4. Assert only expected logs are returned

            var repo = ServiceProvider.GetRequiredService<IAuditLogsRepository>();

            var baseTime = DateTime.UtcNow;
            var before = baseTime.AddHours(-5);
            var inRangeA = baseTime.AddHours(-2);
            var inRangeB = baseTime.AddHours(-1);
            var after = baseTime.AddHours(1);

            var user = new Domain.Entities.Users.UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "Test",
                LastName = "User",
                Email = "Test@User.com",
                PasswordHash = "hashedpassword",
                Role = UserRole.Admin
            };
            await UserRepository.AddUserAsync(user);

            var logs = new[]
            {
                new AuditLogsEntity
                {
                    Id = Guid.NewGuid(),
                    ActorId = user.Id,
                    Action = ActionType.LOGIN,
                    Resource = ResourceType.User,
                    Timestamp = before
                },
                new AuditLogsEntity
                {
                    Id = Guid.NewGuid(),
                    ActorId = user.Id,
                    Action = ActionType.CREATE,
                    Resource = ResourceType.Contact,
                    Timestamp = inRangeA
                },
                new AuditLogsEntity
                {
                    Id = Guid.NewGuid(),
                    ActorId = user.Id,
                    Action = ActionType.UPDATE,
                    Resource = ResourceType.User,
                    Timestamp = inRangeB
                },
                new AuditLogsEntity
                {
                    Id = Guid.NewGuid(),
                    ActorId = user.Id,
                    Action = ActionType.LOGOUT,
                    Resource = ResourceType.User,
                    Timestamp = after
                }
            };

            DbContext.AuditLogs.AddRange(logs);
            await DbContext.SaveChangesAsync();

            var from = baseTime.AddHours(-3);
            var to = baseTime;
            var result = await repo.GetLogsByPeriodAsync(from, to);

            Assert.Equal(2, result.Count);
            Assert.All(result, l => Assert.InRange(l.Timestamp, from, to));
            Assert.DoesNotContain(result, l => l.Timestamp == before);
            Assert.DoesNotContain(result, l => l.Timestamp == after);
        }
    }
}
