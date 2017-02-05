using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridBehaviour : MonoBehaviour
{

    public GameObject cell;
    public GameObject cellContainer;
    public Vector3 offset = new Vector3(2.95f, 0, 2.95f);
    Vector3 origin;

    public List<PlayerBehaviour> players = new List<PlayerBehaviour>();

    GameBehaviour game;
    PlayerSpawner playerSpawner;

    System.Random random;

    void Start()
    {
        if (game == null)
            game = FindObjectOfType<GameBehaviour>();
        if (playerSpawner == null)
            playerSpawner = GetComponents<PlayerSpawner>().First(p => p.enabled);

        game.changeStateEvent.Event += ChangeStateEvent;
        random = new System.Random();

        CreateGrid();
    }

    private void ChangeStateEvent(object sender, changeStateArgs e)
    {
        if (e.state == GameBehaviour.State.game)
            ShowGrid();
        else if (e.state == GameBehaviour.State.game)
            HideGrid();

    }

    void Update()
    {
    }

    public void CreateGrid()
    {
        if (!cell)
            return;

        var start = transform.position - (game.gridSize / 2 * offset);
        var current = Vector3.zero;
        for (int y = 0; y < game.gridSize; y++)
        {
            for (int x = 0; x < game.gridSize; x++)
            {
                current.Set(x, 0, y);
                current.x *= offset.x;
                current.z *= offset.z;
                var c = Instantiate(cell, cellContainer != null ? cellContainer.transform : transform);
                c.transform.position = start + current;
            }

        }
        origin = -(game.gridSize / 2 * offset);
    }

    public void ShowGrid()
    {
        foreach (Transform child in transform)
            if (child.GetComponent<PlayerBehaviour>() == null)
                child.gameObject.SetActive(true);
    }

    public void HideGrid()
    {
        foreach (Transform child in transform)
            if (child.GetComponent<PlayerBehaviour>() == null)
                child.gameObject.SetActive(false);
    }

    public void AddPlayerToGrid(PlayerBehaviour player)
    {
        players.Add(player);
        player.transform.parent = transform;
        player.gameObject.name = "Player " +  players.Count.ToString();
        playerSpawner.SpawnPlayerOnGrid(player);
    }

    public void RemovePlayerToGrid(PlayerBehaviour player)
    {
        players.Remove(player);
    }

    public Vector3 FromPlayerPosition(int x, int y)
    {
        var ret = origin + new Vector3((float)x * offset.x, 0, (float)y * offset.z);
        return ret;
    }

    public bool IsSpotFree(int x, int y)
    {
        //TODO check environment

        if (players.Find(p => p.IsOnSpot(x, y)) != null)
            return false;

        return true;
    }
}
