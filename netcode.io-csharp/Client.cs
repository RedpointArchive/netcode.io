using System;
using System.Runtime.InteropServices;

namespace netcode.io
{
    /// <summary>
    /// Represents a netcode.io client instance.
    /// </summary>
    public class Client : IDisposable
    {
        private SWIGTYPE_p_netcode_client_t _client;
        private bool _isDisposed;

        /// <summary>
        /// Create a new netcode.io client.
        /// </summary>
        /// <param name="bindAddress">The address to bind to.</param>
        /// <param name="time">The starting time for the server as a double value.  Normally this will be 0 in the constructor.</param>
        public Client(string bindAddress, double time)
        {
            NetcodeLibrary.Init();

            _client = netcodeNATIVE.netcode_client_create(bindAddress, time);
        }

        private void AssertNotDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("netcode.io server");
            }
        }

        /// <summary>
        /// Connects a client to a server using a connect token.
        /// </summary>
        /// <param name="connectToken">
        /// The connect token either provided by an authentication service or generated
        /// with <see cref="NetcodeLibrary.GenerateConnectTokenFromPrivateKey"/>.
        /// </param>
        public void Connect(byte[] connectToken)
        {
            AssertNotDisposed();

            var unmanagedPointer = Marshal.AllocHGlobal(connectToken.Length);
            Marshal.Copy(connectToken, 0, unmanagedPointer, connectToken.Length);
            netcodeNATIVE.netcode_client_connect(_client, unmanagedPointer);
            Marshal.FreeHGlobal(unmanagedPointer);
        }

        /// <summary>
        /// Ticks the netcode.io client over.
        /// </summary>
        /// <param name="time">The current client time.  This should ideally be increasing at the same rate on your server.</param>
        public void Update(double time)
        {
            AssertNotDisposed();

            netcodeNATIVE.netcode_client_update(_client, time);
        }

        /// <summary>
        /// Returns the current state of the netcode.io client.
        /// </summary>
        public ClientState State
        {
            get
            {
                AssertNotDisposed();

                return (ClientState) netcodeNATIVE.netcode_client_state(_client);
            }
        }
        
        /// <summary>
        /// Sends a packet to the server.
        /// </summary>
        /// <param name="packetData">The packet data.  Must be no larger than <see cref="NetcodeLibrary.GetMaxPacketSize()"/></param>
        public void SendPacket(byte[] packetData)
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
            netcodeNATIVE.netcode_client_send_packet(_client, unmanagedPointer, packetData.Length);
            Marshal.FreeHGlobal(unmanagedPointer);
        }

        /// <summary>
        /// Receives a packet from the server if available.  This function is non-blocking and returns <c>null</c> if no packet is currently available.
        /// </summary>
        /// <returns>The byte array of the packet data, or <c>null</c> if no packet is available.</returns>
        public byte[] ReceivePacket()
        {
            AssertNotDisposed();

            int packetBytes;
            ulong packetSequence;
            IntPtr packetRaw;
            byte[] packet;

            packetRaw = netcodeNATIVE.netcode_client_receive_packet(_client, out packetBytes, out packetSequence);
            if (packetRaw == IntPtr.Zero)
            {
                return null;
            }

            packet = new byte[packetBytes];
            Marshal.Copy(packetRaw, packet, 0, packetBytes);
            netcodeNATIVE.netcode_client_free_packet(_client, packetRaw);
            return packet;
        }

        private void ReleaseUnmanagedResources()
        {
            if (!_isDisposed)
            {
                netcodeNATIVE.netcode_client_destroy(_client);
            }

            _isDisposed = true;
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~Client()
        {
            ReleaseUnmanagedResources();
        }
    }
}
