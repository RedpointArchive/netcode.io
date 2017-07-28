using System;

namespace netcode.io.example.server
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
            const UInt64 testProtocolId = 0x1122334455667788L;

            double time = 0f;
            double deltaTime = 1.0 / 60.0;

            bool quit = false;

            Console.CancelKeyPress += (sender, a) =>
            {
                quit = true;
            };

            NetcodeLibrary.SetLogLevel(NetcodeLogLevel.Info);
            var server = new Server(
                "127.0.0.1:40000",
                testProtocolId,
                privateKey,
                time);

            var maxPacketSize = NetcodeLibrary.GetMaxPacketSize();
            var packetData = new byte[maxPacketSize];
            for (var i = 0; i < maxPacketSize; i++)
            {
                packetData[i] = (byte)i;
            }

            server.Start(NetcodeLibrary.GetMaxClients());
            
            while (!quit)
            {
                server.Update(time);

                if (server.ClientConnected(0))
                {
                    server.SendPacket(0, packetData);
                }

                for (var clientIndex = 0; clientIndex < NetcodeLibrary.GetMaxClients(); ++clientIndex)
                {
                    while (true)
                    {
                        var packet = server.ReceivePacket(clientIndex);
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
                }

                NetcodeLibrary.Sleep(deltaTime);

                time += deltaTime;
            }

            if (quit)
            {
                Console.WriteLine("Shutting down");
            }

            server.Dispose();
        }
    }
}