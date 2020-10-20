using Mirror;
using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public NetworkGamePlayerLobby playerInfo;
    public PlayerUI playerUI;

    public void Start()
    {
        playerUI.playerName.text = playerInfo.name;
    }
}
