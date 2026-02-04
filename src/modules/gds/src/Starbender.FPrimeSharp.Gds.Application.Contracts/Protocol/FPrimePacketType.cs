namespace Starbender.FPrimeSharp.Gds.Protocol;

public enum FPrimePacketType
{
    FW_PACKET_COMMAND, // !< Command packet type - incoming
    FW_PACKET_TELEM, // !< Telemetry packet type - outgoing
    FW_PACKET_LOG, // !< Log type - outgoing
    FW_PACKET_FILE, // !< File type - incoming and outgoing
    FW_PACKET_PACKETIZED_TLM, // !< Packetized telemetry packet type
    FW_PACKET_DP,
    FW_PACKET_IDLE, // !< Idle packet
    FW_PACKET_UNKNOWN = 0xFF // !< Unknown packet
}
