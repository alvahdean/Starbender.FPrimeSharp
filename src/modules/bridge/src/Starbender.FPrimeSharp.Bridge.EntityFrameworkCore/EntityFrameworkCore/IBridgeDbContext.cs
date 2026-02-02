using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Starbender.FPrimeSharp.Bridge.EntityFrameworkCore;

[ConnectionStringName(BridgeDbProperties.ConnectionStringName)]
public interface IBridgeDbContext : IEfCoreDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * DbSet<Question> Questions { get; }
     */
}
