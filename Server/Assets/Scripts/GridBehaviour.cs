using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridBehaviour : MonoBehaviour
{

    public GameObject cell;
    public Vector3 offset = new Vector3(2.95f, 0, 2.95f);
    Vector3 origin;

    List<PlayerBehaviour> players = new List<PlayerBehaviour>();

    GameBehaviour game;

    System.Random random;

    void Start()
    {
        game = FindObjectOfType<GameBehaviour>();
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
                var c = Instantiate(cell, transform);
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
        SpawnPlayerOnGrid(player);
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

    public bool SpawnPlayerOnGrid(PlayerBehaviour player)
    {
        //you already need reference to that player
        if (players.Find(pl => pl == player) == null)
            return false;

        List<int> x_all = Enumerable.Range(0, game.gridSize).ToList();
        List<int> y_all = Enumerable.Range(0, game.gridSize).ToList();

        while (x_all.Count > 0 && y_all.Count > 0)
        {
            var x = random.Next(x_all.Count);
            var y = random.Next(y_all.Count);

            if (players.Find(p => p.IsOnSpot(x, y)) != null)
                //if (grid[x, y] == null)
                return false;

            x_all.Remove(x);
            y_all.Remove(y);
            return SpawnPlayerOnGrid(player, x, y);


        }

        return false;
    }

    public bool SpawnPlayerOnGrid(PlayerBehaviour player, int x, int y)
    {
        //you already need reference to that player
        if (players.Find(pl => pl == player) == null)
            return false;

        if (x < 0 || x >= game.gridSize)
            return false;
        if (y < 0 || y >= game.gridSize)
            return false;

        if (players.Find(p => p.IsOnSpot(x, y)) != null)
            //if (grid[x, y] != null)
            return false;

        player.SpawnPlayer(x, y);
        return true;
    }
}
