using Microsoft.EntityFrameworkCore;
using Volo.Abp;

namespace Starbender.FPrimeSharp.Bridge.EntityFrameworkCore;

public static class BridgeDbContextModelCreatingExtensions
{
    public static void ConfigureBridge(
        this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        /* Configure all entities here. Example:

        builder.Entity<Question>(b =>
        {
            //Configure table & schema name
            b.ToTable(BridgeDbProperties.DbTablePrefix + "Questions", BridgeDbProperties.DbSchema);

            b.ConfigureByConvention();

            //Properties
            b.Property(q => q.Title).IsRequired().HasMaxLength(QuestionConsts.MaxTitleLength);

            //Relations
            b.HasMany(question => question.Tags).WithOne().HasForeignKey(qt => qt.QuestionId);

            //Indexes
            b.HasIndex(q => q.CreationTime);
        });
        */
    }
}
