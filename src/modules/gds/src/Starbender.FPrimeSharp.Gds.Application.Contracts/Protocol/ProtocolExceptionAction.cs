namespace Starbender.FPrimeSharp.Gds.Protocol;

public enum ProtocolExceptionAction
{
    Continue,           // keep reading next frames
    CloseGracefully,    // complete pipes/close connection
    Abort               // hard abort (rethrow / fail connection)
}
