using Mirror;
using UnityEngine;

    public class PlayerSpawnSystem : NetworkBehaviour
    {
        [SerializeField] private GameObject playerPrefab = null;

        int index = 0;
        public override void OnStartServer() => NetworkManagerLobby.OnServerReadied += SpawnPlayer;

    private NetworkManagerLobby room;
    private NetworkManagerLobby Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerLobby;
        }
    }

    [ServerCallback]
        private void OnDestroy() => NetworkManagerLobby.OnServerReadied -= SpawnPlayer;

        [Server]
        public void SpawnPlayer(NetworkConnection conn)
        {
            GameObject playerInstance = Instantiate(playerPrefab);
            playerInstance.GetComponent<Player>().playerInfo = Room.GamePlayers[index];
            NetworkServer.Spawn(playerInstance, conn);
            index++;
        }
    }
