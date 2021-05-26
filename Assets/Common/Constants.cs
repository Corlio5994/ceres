public static class Constants {
    public const int ticksPerSecond = 32;
    public const int millisecondsPerTick = 1000 / ticksPerSecond;
#if DEVELOPMENT_BUILD || UNITY_EDITOR
    public const string serverIP = "127.0.0.1";
#else
    public const string serverIP = "139.59.121.112";
#endif

    public const string version = "1.0.1";
    public const ushort port = 60000;
    public const int maxPlayers = 20;
}