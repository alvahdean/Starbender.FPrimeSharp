using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Starbender.FPrimeSharp.Gds.Protocol;

public interface IFPrimePacketHandler
{
    IReadOnlyCollection<FPrimePacketType> SupportedPacketTypes { get; }

    ValueTask HandlePacketAsync(FPrimePacket packet, CancellationToken ct);
}
