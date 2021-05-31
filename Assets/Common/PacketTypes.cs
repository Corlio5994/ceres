public enum ServerPackets {
    // Server --> Client
    ConnectedTCP,
    VersionAccepted,
    VersionDenied,
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
    VersionCheck,
    Login,
    Logout,
    PlayerDataRequest,
    PlayerMoved,
    ChatMessage
}