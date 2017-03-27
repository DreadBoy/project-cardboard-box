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
        player.gameObject.name = "Player " + players.Count.ToString();
        playerSpawner.SpawnPlayerOnGrid(player);
    }

    public void RemovePlayerFromGrid(PlayerBehaviour player)
    {
        players.Remove(player);
    }

    public Vector3 FromCellNumbers(int x, int z)
    {
        var ret = transform.position + new Vector3(x * offset.x, 0, z * offset.z) + origin;
        return ret;
    }

    public Vector3 FromCellNumbers(Vector3 cellNumbers)
    {
        return FromCellNumbers((int)cellNumbers.x, (int)cellNumbers.z);
    }

    public bool IsSpotFree(int x, int y)
    {
        var center = offset;
        center.x *= x;
        center.z *= y;
        center.y = offset.x / 2;
        center += transform.position + origin;
        var colliders = Physics.OverlapBox(center, new Vector3(offset.x / 2, offset.x / 2, offset.x / 2), Quaternion.identity, int.MaxValue, QueryTriggerInteraction.Collide);
        foreach (var collider in colliders)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Returns cell numbers from object's position
    /// Use FromCellNumbers to get object's world position
    /// </summary>
    /// <param name="position">Global postion of object</param>
    /// <returns></returns>
    public Vector3 GetCellNumbers(Vector3 position)
    {
        position -= transform.position + origin;

        var x = Mathf.Round(position.x / offset.x);
        if (x < 0)
            x = 0;
        if (x >= game.gridSize)
            x = game.gridSize - 1;
        var z = Mathf.Round(position.z / offset.z);
        if (z < 0)
            z = 0;
        if (z >= game.gridSize)
            z = game.gridSize - 1;

        return new Vector3(x, 0, z);
    }

    /// <summary>
    /// Returns vector that is centered on nearest cell. 
    /// Always returns position inside grid.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Vector3 SnapToGrid(Vector3 position)
    {
        return FromCellNumbers(GetCellNumbers(position));
    }

    public bool IsInsideGrid(Vector3 position)
    {
        position -= transform.position + origin;

        var x = Mathf.Round(position.x / offset.x);
        if (x < 0)
            return false;
        if (x >= game.gridSize)
            return false;
        var z = Mathf.Round(position.z / offset.z);
        if (z < 0)
            return false;
        if (z >= game.gridSize)
            return false;

        return true;
    }

    public bool TrySnapToGrid(Vector3 position, out Vector3 snappedPosition)
    {
        if (!IsInsideGrid(position))
        {
            snappedPosition = position;
            return false;
        }
        snappedPosition = SnapToGrid(position);
        return true;
    }
}
