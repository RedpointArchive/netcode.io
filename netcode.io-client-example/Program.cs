using System;

namespace netcode.io.example.client
{
    public static class Program
    {
        private static byte[] privateKey = new byte[]
        {
            0x60, 0x6a, 0xbe, 0x6e, 0xc9, 0x19, 0x10, 0xea,
            0x9a, 0x65, 0x62, 0xf6, 0x6f, 0x2b, 0x30, 0xe4,
            0x43, 0x71, 0xd6, 0x2c, 0xd1, 0x99, 0x27, 0x26,
            0x6b, 0x3c, 0x60, 0xf4, 0xb7, 0x15, 0xab, 0xa1
        };

        public static void Main(string[] args)
        {
            const int testConnectTokenExpiry = 30;
            const UInt64 testProtocolId = 0x1122334455667788L;

            double time = 0f;
            double deltaTime = 1.0 / 60.0;

            bool quit = false;

            Console.CancelKeyPress += (sender, a) =>
            {
                quit = true;
            };

            NetcodeLibrary.SetLogLevel(NetcodeLogLevel.Info);

            Console.WriteLine("[client]");

            var client = new Client(
                "::",
                time);

            var clientId = NetcodeLibrary.GetRandomUInt64();
            Console.WriteLine($"client id is {clientId:X}");

            var serverAddress = "[::1]:40000";

            byte[] connectToken = NetcodeLibrary.GenerateConnectTokenFromPrivateKey(
                new[] {serverAddress},
                testConnectTokenExpiry,
                clientId,
                testProtocolId,
                0,
                privateKey);

            client.Connect(connectToken);

            var maxPacketSize = NetcodeLibrary.GetMaxPacketSize();
            var packetData = new byte[maxPacketSize];
            for (var i = 0; i < maxPacketSize; i++)
            {
                packetData[i] = (byte)i;
            }

            while (!quit)
            {
                client.Update(time);

                if (client.State == ClientState.Connected)
                {
                    client.SendPacket(packetData);
                }

                while (true)
                {
                    var packet = client.ReceivePacket();
                    if (packet == null)
                    {
                        break;
                    }

                    if (packet.Length != packetData.Length)
                    {
                        Console.Error.WriteLine("Packet doesn't have expected length!");
                    }
                    else
                    {
                        for (var i = 0; i < maxPacketSize; i++)
                        {
                            if (packet[i] != packetData[i])
                            {
                                Console.Error.WriteLine($"Packet differs at index {i}!");
                            }
                        }
                    }
                }

                if (client.State <= ClientState.Disconnected)
                {
                    break;
                }

                NetcodeLibrary.Sleep(deltaTime);

                time += deltaTime;
            }

            if (quit)
            {
                Console.WriteLine("Shutting down");
            }

            client.Dispose();
        }
    }
}