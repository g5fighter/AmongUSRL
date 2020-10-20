using Mirror;
    public class NetworkGamePlayerLobby : NetworkBehaviour
    {
        [SyncVar]
        public string displayName = "Loading...";
        public string color = "FFFFFF";

        private NetworkManagerLobby room;
        private NetworkManagerLobby Room
        {
            get
            {
                if (room != null) { return room; }
                return room = NetworkManager.singleton as NetworkManagerLobby;
            }
        }

        public override void OnStartClient()
        {
            DontDestroyOnLoad(gameObject);

            Room.GamePlayers.Add(this);
        }

        public override void OnStopClient()
        {
            Room.GamePlayers.Remove(this);
        }

        [Server]
        public void SetDisplayName(string displayName)
        {
            this.displayName = displayName;
        }

        [Server]
        public void SetColorPlayer(string color)
        {
            this.color = color;
        }
}
