using System;
using System.Runtime.InteropServices;

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

        public static byte[] GetRandomBytes(int length)
        {
            var unmanagedPointer = Marshal.AllocHGlobal(length);
            netcodeNATIVE.netcode_random_bytes(unmanagedPointer, length);
            var bytes = new byte[length];
            Marshal.Copy(unmanagedPointer, bytes, 0, length);
            Marshal.FreeHGlobal(unmanagedPointer);
            return bytes;
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
            var length = sizeof(UInt64);
            var unmanagedPointer = Marshal.AllocHGlobal(length);
            netcodeNATIVE.netcode_random_bytes(unmanagedPointer, length);
            var bytes = new byte[length];
            Marshal.Copy(unmanagedPointer, bytes, 0, length);
            Marshal.FreeHGlobal(unmanagedPointer);
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
            
            var unmanagedPrivateKey = Marshal.AllocHGlobal(privateKey.Length);
            var unmanagedConnectToken = Marshal.AllocHGlobal(netcodeNATIVE.NETCODE_CONNECT_TOKEN_BYTES);
            Marshal.Copy(privateKey, 0, unmanagedPrivateKey, privateKey.Length);

            netcodeNATIVE.netcode_generate_connect_token(
                serverAddresses.Length,
                serverAddresses,
                expirySeconds,
                clientId,
                protocolId,
                sequence,
                unmanagedPrivateKey,
                unmanagedConnectToken);

            Marshal.FreeHGlobal(unmanagedPrivateKey);
            var connectToken = new byte[netcodeNATIVE.NETCODE_CONNECT_TOKEN_BYTES];
            Marshal.Copy(unmanagedConnectToken, connectToken, 0, connectToken.Length);
            Marshal.FreeHGlobal(unmanagedConnectToken);

            return connectToken;
        }
    }
}
