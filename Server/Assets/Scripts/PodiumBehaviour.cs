using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PodiumBehaviour : MonoBehaviour
{
    List<PlayerBehaviour> players = new List<PlayerBehaviour>();

    public GameObject podium;

    public List<Vector3> playerPositions = new List<Vector3>()
    {
        new Vector3(-4.7f, -0.32f, -38.91f),
        new Vector3(2.4f, -1.99f, -40.27f),
        new Vector3(-10.09f, -2.16f, -39.35f),
        new Vector3(-5.28f,-2.41f,-40.38f),
        new Vector3(-1.63f, -2.292784f, -39.54443f),

    };
    public List<Quaternion> playerRotations = new List<Quaternion>() {
        Quaternion.Euler(0, 180, 0),
        Quaternion.Euler(0, 170, 0),
        Quaternion.Euler(15.754f, 300.308f, -8.096001f),
        Quaternion.Euler(0, 50.238f, 0),
        Quaternion.Euler(21.048f, 90.50401f, 0)
    };
    private void Start()
    {
        if (podium == null)
            podium = transform.Find("Podium").gameObject;
        podium.SetActive(false);
    }

    public void AddPlayerToPodium(PlayerBehaviour player, bool victor = false)
    {
        players.Add(player);
        player.transform.SetParent(transform);
        var index = players.Count;
        if (victor)
            index = 0;
        player.transform.localPosition = playerPositions[index];
        player.transform.localRotation = playerRotations[index];
        if (players.Count > 0)
            podium.SetActive(true);
    }

    public void RemovePlayerFromPodium(PlayerBehaviour player)
    {
        players.Remove(player);
    }
}