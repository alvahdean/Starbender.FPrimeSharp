using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Starbender.FPrimeSharp.Bridge.EntityFrameworkCore;

[ConnectionStringName(BridgeDbProperties.ConnectionStringName)]
public class BridgeDbContext : AbpDbContext<BridgeDbContext>, IBridgeDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * public DbSet<Question> Questions { get; set; }
     */

    public BridgeDbContext(DbContextOptions<BridgeDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureBridge();
    }
}
