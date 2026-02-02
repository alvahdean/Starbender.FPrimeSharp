using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Starbender.FPrimeSharp.Gds.EntityFrameworkCore;

[ConnectionStringName(GdsDbProperties.ConnectionStringName)]
public class GdsDbContext : AbpDbContext<GdsDbContext>, IGdsDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * public DbSet<Question> Questions { get; set; }
     */

    public GdsDbContext(DbContextOptions<GdsDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureGds();
    }
}
