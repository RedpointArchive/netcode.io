using System;

namespace netcode.io
{
    public static class NetcodeLibrary
    {
        static NetcodeLibrary()
        {
            netcodeNATIVE.netcode_init();
            SetLogLevel(NetcodeLogLevel.None);
        }

        internal static void Init()
        {
            // Ensures static constructor is called at least once.
        }

        public static void SetLogLevel(NetcodeLogLevel logLevel)
        {
            netcodeNATIVE.netcode_log_level((int)logLevel);
        }

        public static void GetRandomBytes(int length)
        {
            var bytes = new byte[length];
            netcodeNATIVE.netcode_random_bytes(bytes, length);
        }

        public static int GetMaxPacketSize()
        {
            return netcodeNATIVE.NETCODE_MAX_PACKET_SIZE;
        }

        public static int GetMaxClients()
        {
            return netcodeNATIVE.NETCODE_MAX_CLIENTS;
        }

        public static void Sleep(double seconds)
        {
            netcodeNATIVE.netcode_sleep(seconds);
        }

        public static UInt64 GetRandomUInt64()
        {
            var bytes = new byte[sizeof(UInt64)];
            netcodeNATIVE.netcode_random_bytes(bytes, bytes.Length);
            return BitConverter.ToUInt64(bytes, 0);
        }

        public static byte[] GenerateConnectTokenFromPrivateKey(
            string[] serverAddresses,
            int expirySeconds,
            UInt64 clientId,
            UInt64 protocolId,
            UInt64 sequence,
            byte[] privateKey)
        {
            if (privateKey == null)
            {
                throw new ArgumentNullException(nameof(privateKey));
            }

            if (privateKey.Length != netcodeNATIVE.NETCODE_KEY_BYTES)
            {
                throw new ArgumentException(
                    $"The private symmetric key must be {netcodeNATIVE.NETCODE_KEY_BYTES} bytes long.",
                    nameof(privateKey));
            }
            
            var connectToken = new byte[netcodeNATIVE.NETCODE_CONNECT_TOKEN_BYTES];

            netcodeNATIVE.netcode_generate_connect_token(
                serverAddresses.Length,
                serverAddresses,
                expirySeconds,
                clientId,
                protocolId,
                sequence,
                privateKey,
                connectToken);

            return connectToken;
        }
    }
}
