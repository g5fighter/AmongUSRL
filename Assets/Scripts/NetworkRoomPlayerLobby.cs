using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkRoomPlayerLobby : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private PlayerCard[] playerTargetList = null;
    [SerializeField] private ColorButton[] colorButton = null;
    [SerializeField] private Button buttonStart = null;
    [SerializeField] private Image bodyImage = null;

    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "Loading...";
    [SyncVar(hook = nameof(HandleColorChanged))]
    public string Color = "FFFFFF";
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool IsReady = false;

    private bool isLeader;

    public bool IsLeader
    {
        set
        {
            isLeader = value;
            buttonStart.gameObject.SetActive(value);
        }
    }

    private NetworkManagerLobby room;

    private NetworkManagerLobby Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerLobby;
        }
    }

    public override void OnStartAuthority()
    {
        CmdSetDisplayName(PlayerNameInput.DisplayName);
        lobbyUI.SetActive(true);
    }
    public override void OnStartClient()
    {
        Room.RoomPlayers.Add(this);
        if (!isLeader) { Room.coloresUsados.Add("FFFFFF"); }
        UpdateDisplay();
        if (hasAuthority)
        {
            CmdSetColor(Color);
        }
    }

    public override void OnStopClient()
    {
        Room.RoomPlayers.Remove(this);

        UpdateDisplay();
    }

    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();
    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();

    public void HandleColorChanged(string oldValue, string newValue) => UpdateDisplay();

    private void UpdateDisplay()
    {
        if (!hasAuthority)
        {
            foreach (var player in Room.RoomPlayers)
            {
                if (player.hasAuthority)
                {
                    player.UpdateDisplay();
                    break;
                }
            }

            return;
        }

        for (int i = 0; i < playerTargetList.Length; i++)
        {
            playerTargetList[i].playerName.text = "Waiting For Player...";
            playerTargetList[i].playerStatus.text = string.Empty;
            playerTargetList[i].foto.color = GetColorFromString("FFFFFF");
        }

        for (int i = 0; i < Room.RoomPlayers.Count; i++)
        {
            playerTargetList[i].foto.color = GetColorFromString(Room.RoomPlayers[i].Color);
            bodyImage.color = GetColorFromString(Color);
            playerTargetList[i].playerName.text = Room.RoomPlayers[i].DisplayName;
            playerTargetList[i].playerStatus.text = Room.RoomPlayers[i].IsReady ?
                "<color=green>Ready</color>" :
                "<color=red>Not Ready</color>";
        }
        disableColorButtons();
    }


    public void disableColorButtons()
    {
        if (hasAuthority)
        {
            List<string> coloresUsados = new List<string>();
            foreach (NetworkRoomPlayerLobby el in Room.RoomPlayers)
            {
                coloresUsados.Add(el.Color);
            }
            for (int x = 0; x < colorButton.Length; x++)
            {
                bool esta = !isColorIn(colorButton[x].color, coloresUsados);
                colorButton[x].boton.interactable = esta;
            }
        }
    }

    public void checkColor(string color)
    {
        string tmp = color;
        while (isColorIn(tmp))
        {
            int randomIndex = Random.Range(0, colorButton.Length - 1);
            tmp = colorButton[randomIndex].color;
        }
        Room.coloresUsados.Remove(Color);
        Color = tmp;
        Room.coloresUsados.Add(tmp);
    }

    public bool isColorIn(string color)
    {
        return isColorIn(color, Room.coloresUsados);
    }
    public bool isColorIn(string color, List<string> colores)
    {
        if (colores.Count == 0)
        {
            return false;
        }
        bool encontrado = false;
        for (int x = 0; x < colores.Count & !encontrado; x++)
        {
            encontrado = colores[x] == color;
        }
        return encontrado;
    }

    public void HandleReadyToStart(bool readyToStart)
    {
        if (!isLeader) { return; }

        buttonStart.interactable = readyToStart;
    }

    [Command]
    private void CmdSetDisplayName(string displayName)
    {
        DisplayName = displayName;
    }

    [Command]
    public void CmdSetColor(string color)
    {
        checkColor(color);
    }

    [Command]
    public void CmdReadyUp()
    {
        IsReady = !IsReady;

        Room.NotifyPlayersOfReadyState();
    }

    [Command]
    public void CmdStartGame()
    {
        if (Room.RoomPlayers[0].connectionToClient != connectionToClient) { return; }

        Room.StartGame();
    }

    private float HexToDec(string hex)
    {
        return System.Convert.ToInt32(hex, 16) / 255f;
    }

    private Color GetColorFromString(string hex)
    {
        float red = HexToDec(hex.Substring(0, 2));
        float green = HexToDec(hex.Substring(2, 2));
        float blue = HexToDec(hex.Substring(4, 2));
        return new Color(red, green, blue);
    }
}
