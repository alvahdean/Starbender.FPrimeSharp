using System;

namespace Starbender.FPrimeSharp.Gds.Protocol;

public enum CommDriverType
{
    Unspecified=0,  // Driver Type not specified
    Tcp,            // IP Stream
    Udp,            // IP Datagram
    RS232           // Serial protocol
}
