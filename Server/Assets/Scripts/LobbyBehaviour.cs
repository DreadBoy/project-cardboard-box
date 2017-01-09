using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class LobbyBehaviour : MonoBehaviour
{
    List<PlayerBehaviour> players = new List<PlayerBehaviour>();

    public Vector3 offset = new Vector3(3.8f, 0, 0);

    public List<Vector3> playerPositions = new List<Vector3>()
    {
        new Vector3(-9, 0, -0.6f),
        new Vector3(-4.7f, 0, 0),
        new Vector3(0,0,0),
        new Vector3(4.7f, 0, 0),
        new Vector3(9, 0, -0.6f),

    };
    public List<Quaternion> playerRotations = new List<Quaternion>() {
        Quaternion.Euler(0, 132, 0),
        Quaternion.Euler(0, 170, 0),
        Quaternion.Euler(0, 180, 0),
        Quaternion.Euler(0, 190, 0),
        Quaternion.Euler(0, 228, 0)
    };

    public void AddPlayerToLobby(PlayerBehaviour player)
    {
        players.Add(player);
        player.transform.parent = transform;
        for (int i = 0; i < players.Count; i++)
        {
            players[i].transform.localPosition = playerPositions[i];
            players[i].transform.localRotation = playerRotations[i];
        }
    }

    public void RemovePlayerFromLobby(PlayerBehaviour player)
    {
        players.Remove(player);
    }
}