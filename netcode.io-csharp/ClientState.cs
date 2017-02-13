namespace netcode.io
{
    public enum ClientState
    {
        ConnectTokenExpired = -6,

        InvalidConnectToken = -5,

        ConnectionTimedOut = -4,

        ConnectionResponseTimeout = -3,

        ConnectionRequestTimeout = -2,

        ConnectionDenied = -1,

        Disconnected = 0,

        SendingConnectionRequest = 1,

        SendingConnectionResponse = 2,

        Connected = 3
    }
}
