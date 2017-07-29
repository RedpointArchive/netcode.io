using System;
using System.Runtime.InteropServices;

namespace netcode.io
{
    /// <summary>
    /// Represents a netcode.io server instance.
    /// </summary>
    public class Server : IDisposable
    {
        private readonly SWIGTYPE_p_netcode_server_t _server;
        private bool _isDisposed;

        /// <summary>
        /// Create a new netcode.io server.
        /// </summary>
        /// <param name="bindAddress">The address to bind to.</param>
        /// <param name="protocolId">The protocol ID.  This should be unique to your game or application.</param>
        /// <param name="privateKey">The symmetric private key used between your clients and the dedicated servers.</param>
        /// <param name="time">The starting time for the server as a double value.  Normally this will be 0 in the constructor.</param>
        public Server(string bindAddress, UInt64 protocolId, byte[] privateKey, double time)
        {
            NetcodeLibrary.Init();

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
            Marshal.Copy(privateKey, 0, unmanagedPrivateKey, privateKey.Length);
            _server = netcodeNATIVE.netcode_server_create(
                bindAddress,
                protocolId,
                unmanagedPrivateKey,
                time);
            Marshal.FreeHGlobal(unmanagedPrivateKey);

            if (_server == null)
            {
                throw new InvalidOperationException("Unable to create native netcode.io server");
            }
        }

        private void AssertNotDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("netcode.io server");
            }
        }

        /// <summary>
        /// Start the netcode.io server.
        /// </summary>
        /// <param name="maxClients">The maximum number of clients.  Must be greater than 0 and less than or equal to <see cref="NetcodeLibrary.GetMaxClients()"/>.</param>
        public void Start(int maxClients)
        {
            AssertNotDisposed();

            if (maxClients <= 0 || maxClients > NetcodeLibrary.GetMaxClients())
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxClients),
                    $"Must be greater than 0 and less than or equal to {NetcodeLibrary.GetMaxClients()}");
                ;
            }

            netcodeNATIVE.netcode_server_start(_server, maxClients);
        }

        /// <summary>
        /// Ticks the netcode.io server over.
        /// </summary>
        /// <param name="time">The current server time.  This should ideally be increasing at the same rate on your clients.</param>
        public void Update(double time)
        {
            AssertNotDisposed();

            netcodeNATIVE.netcode_server_update(_server, time);
        }

        /// <summary>
        /// Returns if there is a client connected at the specified client index.
        /// </summary>
        /// <param name="clientIndex">The client index.</param>
        /// <returns>Whether there is a client connected.</returns>
        public bool ClientConnected(int clientIndex)
        {
            AssertNotDisposed();

            return netcodeNATIVE.netcode_server_client_connected(_server, clientIndex) != 0;
        }

        /// <summary>
        /// Sends a packet to a specific client.
        /// </summary>
        /// <param name="clientIndex">The client index.</param>
        /// <param name="packetData">The packet data.  Must be no larger than <see cref="NetcodeLibrary.GetMaxPacketSize()"/></param>
        public void SendPacket(int clientIndex, byte[] packetData)
        {
            AssertNotDisposed();

            if (packetData == null)
            {
                throw new ArgumentNullException(nameof(packetData));
            }

            var maxPacketSize = NetcodeLibrary.GetMaxPacketSize();
            if (packetData.Length > maxPacketSize)
            {
                throw new ArgumentException($"Packet data can not be longer than {maxPacketSize} bytes.", nameof(packetData));
            }

            var unmanagedPointer = Marshal.AllocHGlobal(packetData.Length);
            Marshal.Copy(packetData, 0, unmanagedPointer, packetData.Length);
            netcodeNATIVE.netcode_server_send_packet(_server, clientIndex, unmanagedPointer, packetData.Length);
            Marshal.FreeHGlobal(unmanagedPointer);
        }

        /// <summary>
        /// Receives a packet from a client if available.  This function is non-blocking and returns <c>null</c> if no packet is currently available.
        /// </summary>
        /// <param name="clientIndex">The client index.</param>
        /// <returns>The byte array of the packet data, or <c>null</c> if no packet is available.</returns>
        public byte[] ReceivePacket(int clientIndex)
        {
            AssertNotDisposed();

            int packetBytes;
            ulong packetSequence;
            IntPtr packetRaw;
            byte[] packet;

            packetRaw = netcodeNATIVE.netcode_server_receive_packet(_server, clientIndex, out packetBytes, out packetSequence);
            if (packetRaw == IntPtr.Zero)
            {
                return null;
            }

            packet = new byte[packetBytes];
            Marshal.Copy(packetRaw, packet, 0, packetBytes);
            netcodeNATIVE.netcode_server_free_packet(_server, packetRaw);
            return packet;
        }

        private void ReleaseUnmanagedResources()
        {
            if (!_isDisposed)
            {
                netcodeNATIVE.netcode_server_destroy(_server);
            }

            _isDisposed = true;
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~Server()
        {
            ReleaseUnmanagedResources();
        }
    }
}

