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
    }
}
