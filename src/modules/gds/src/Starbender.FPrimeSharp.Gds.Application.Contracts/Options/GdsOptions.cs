using System;

namespace Starbender.FPrimeSharp.Gds.Options;

public class GdsOptions
{
    public class Listeners
    {
        public TcpDriverOptions[] Tcp { get; set; } = [];
    }
}
