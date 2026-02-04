using System;
using Starbender.FPrimeSharp.Gds.Protocol;

namespace Starbender.FPrimeSharp.Gds.Options;

public class CommDriverOptions
{
    public string Name { get; set; } = string.Empty;
    public CommDriverType DriverType { get; set; } = CommDriverType.Tcp;
    public FrameType Protocol { get; set; } = FrameType.FPrime;
}

public class TcpDriverOptions : CommDriverOptions
{
    public TcpDriverOptions Options { get; set; } = new TcpDriverOptions();
}
