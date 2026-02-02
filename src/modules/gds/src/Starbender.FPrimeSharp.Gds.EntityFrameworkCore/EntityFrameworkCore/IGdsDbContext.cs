using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Starbender.FPrimeSharp.Gds.EntityFrameworkCore;

[ConnectionStringName(GdsDbProperties.ConnectionStringName)]
public interface IGdsDbContext : IEfCoreDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * DbSet<Question> Questions { get; }
     */
}
