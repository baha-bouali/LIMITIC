using Microsoft.EntityFrameworkCore;

namespace LIMTIC.Infrastructure.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {

    }
}
