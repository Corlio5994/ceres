public enum ServerPackets {
    // Server --> Client
    ConnectedTCP,
    ConnectedUDP,
    LoginAccepted,
    LoginDenied,
    LogoutSuccessful,
    PlayerData,
    OtherPlayerLoggedIn,
    OtherPlayerLoggedOut,
    PlayerPosition,
    OtherPlayerMoved,
    ChatMessage
}

public enum ClientPackets {
    // Client --> Server
    Login,
    Logout,
    PlayerDataRequest,
    PlayerMoved,
    ChatMessage
}