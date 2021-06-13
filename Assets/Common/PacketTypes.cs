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
    ChatMessage,

    ItemPickupData,
    ContainerData,
    ItemDropped,
    ItemPickedUp,
    ContainerDeposit,
    ContainerWithdraw
}

public enum ClientPackets {
    // Client --> Server
    VersionCheck,
    Login,
    Logout,
    PlayerDataRequest,
    PlayerMoved,
    ChatMessage,
    
    ItemPickupDataRequest,
    ContainerDataRequest,
    ItemDropped,
    ItemPickedUp,
    ContainerDeposit,
    ContainerWithdraw
}