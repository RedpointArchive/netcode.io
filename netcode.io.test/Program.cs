using System;

namespace netcode.io.test
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

            bool validatedServer = false;
            bool validatedClient = false;
            DateTime startTime = DateTime.UtcNow;

            NetcodeLibrary.SetLogLevel(NetcodeLogLevel.Info);
            
            var server = new Server(
                "[::1]:40000",
                testProtocolId,
                privateKey,
                time);

            server.Start(NetcodeLibrary.GetMaxClients());

            var client = new Client(
                "::",
                time);

            var clientId = NetcodeLibrary.GetRandomUInt64();
            Console.WriteLine($"client id is {clientId:X}");

            var serverAddress = "[::1]:40000";

            byte[] connectToken = NetcodeLibrary.GenerateConnectTokenFromPrivateKey(
                new[] { serverAddress },
                testConnectTokenExpiry,
                clientId,
                testProtocolId,
                0,
                privateKey);

            client.Connect(connectToken);

            var maxPacketSize = NetcodeLibrary.GetMaxPacketSize();
            var serverPacketData = new byte[maxPacketSize];
            var clientPacketData = new byte[maxPacketSize];
            for (var i = 0; i < maxPacketSize; i++)
            {
                serverPacketData[i] = (byte)i;
                clientPacketData[i] = (byte)i;
            }

            while ((!validatedServer || !validatedClient) && (DateTime.UtcNow - startTime).TotalSeconds < 30)
            {
                server.Update(time);

                if (server.ClientConnected(0))
                {
                    server.SendPacket(0, serverPacketData);
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

                        var matches = true;

                        if (packet.Length != clientPacketData.Length)
                        {
                            Console.Error.WriteLine("Packet doesn't have expected length!");
                            matches = false;
                        }
                        else
                        {
                            for (var i = 0; i < maxPacketSize; i++)
                            {
                                if (packet[i] != clientPacketData[i])
                                {
                                    Console.Error.WriteLine($"Packet differs at index {i}!");
                                    matches = false;
                                }
                            }
                        }

                        if (matches)
                        {
                            validatedServer = true;
                        }
                    }
                }

                client.Update(time);

                if (client.State == ClientState.Connected)
                {
                    client.SendPacket(clientPacketData);
                }

                while (true)
                {
                    var packet = client.ReceivePacket();
                    if (packet == null)
                    {
                        break;
                    }

                    var matches = true;

                    if (packet.Length != serverPacketData.Length)
                    {
                        Console.Error.WriteLine("Packet doesn't have expected length!");
                        matches = false;
                    }
                    else
                    {
                        for (var i = 0; i < maxPacketSize; i++)
                        {
                            if (packet[i] != serverPacketData[i])
                            {
                                Console.Error.WriteLine($"Packet differs at index {i}!");
                                matches = false;
                            }
                        }
                    }

                    if (matches)
                    {
                        validatedClient = true;
                    }
                }

                if (client.State <= ClientState.Disconnected)
                {
                    break;
                }

                NetcodeLibrary.Sleep(deltaTime);

                time += deltaTime;
            }

            if ((DateTime.UtcNow - startTime).TotalSeconds > 30)
            {
                Console.Error.WriteLine("Timed out after 30 seconds.");
                Environment.Exit(1);
            }

            if (!validatedClient || !validatedServer)
            {
                Console.Error.WriteLine("Not in a valid state when exiting loop.");
                Environment.Exit(1);
            }

            client.Dispose();
            server.Dispose();

            Console.WriteLine("Test passed.");
            Environment.Exit(0);
        }
    }
}