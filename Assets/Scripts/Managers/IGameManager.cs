public interface IGameManager {
    ManagerStatus status { get; }

    // Start is called before the first frame update
    void Startup();
}

public enum ManagerStatus {
    Shutdown,
    Initializing,
    Started
}