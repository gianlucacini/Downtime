using System;
using System.Net.Sockets;

namespace Downtime.Service.BusinessLogic
{
    /// <summary>
    /// Minimal SNTP client: single request/response against an NTP server.
    /// </summary>
    public class SntpClient
    {
        public const string DefaultServer = "time.nist.gov";
        public const int DefaultPort = 123;

        private const int PacketSize = 48;
        private const int TransmitTimestampOffset = 40;
        private static readonly DateTime NtpEpoch = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // NTP v4, mode 3 (client)
        private static readonly byte[] RequestPacket = new byte[]
        {
            0x23, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        };

        public DateTime? GetUtcDateTime(string server = DefaultServer, int receiveTimeoutMs = 3000)
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.ReceiveTimeout = receiveTimeoutMs;
                socket.Connect(server, DefaultPort);

                byte[] data = (byte[])RequestPacket.Clone();
                socket.Send(data);
                socket.Receive(data);

                return ParseTransmitTimestamp(data);
            }
        }

        private static DateTime? ParseTransmitTimestamp(byte[] data)
        {
            if (data == null || data.Length < TransmitTimestampOffset + 8)
                return null;

            byte[] integerPart = new byte[]
            {
                data[TransmitTimestampOffset + 3],
                data[TransmitTimestampOffset + 2],
                data[TransmitTimestampOffset + 1],
                data[TransmitTimestampOffset + 0]
            };

            byte[] fractPart = new byte[]
            {
                data[TransmitTimestampOffset + 7],
                data[TransmitTimestampOffset + 6],
                data[TransmitTimestampOffset + 5],
                data[TransmitTimestampOffset + 4]
            };

            long ms = (long)(
                (ulong)BitConverter.ToUInt32(integerPart, 0) * 1000
                + ((ulong)BitConverter.ToUInt32(fractPart, 0) * 1000)
                / 0x100000000L);

            return NtpEpoch.AddTicks(ms * TimeSpan.TicksPerMillisecond);
        }
    }
}
