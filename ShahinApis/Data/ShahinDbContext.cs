using Microsoft.EntityFrameworkCore;
using ShahinApis.Data.Model;

namespace ShahinApis.Data;

public class ShahinDbContext : DbContext
{

    public ShahinDbContext(DbContextOptions<ShahinDbContext> dbContext) : base(dbContext)
    {

    }

    public DbSet<ShahinReqLog> Shahin_Req { get; set; }
    public DbSet<ShahinResLog> Shahin_Res { get; set; }
    public DbSet<AccessTokenEntity> AccessTokens { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ShahinDbContext).Assembly);
    }

}

