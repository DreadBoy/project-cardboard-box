using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBehaviour : MonoBehaviour
{

    public GameObject cell;
    public Vector3 offset = new Vector3(2.95f, 0, 2.95f);

    void Start()
    {
        FindObjectOfType<GameBehaviour>().game.gridCompiledEvent.Event += DisplayGrid;
    }

    public void DisplayGrid(object sender, GridCompiledEventArgs args)
    {
        if (!cell)
            return;

        foreach (Transform child in transform)
            Destroy(child);

        var start = transform.position - (args.size / 2 * offset);
        var current = Vector3.zero;
        for (int y = 0; y < args.size; y++)
        {
            for (int x = 0; x < args.size; x++)
            {
                current.Set(x, 0, y);
                current.x *= offset.x;
                current.z *= offset.z;
                var c = Instantiate(cell);
                cell.transform.parent = transform;
                c.transform.position = start + current;
            }
        }

        FindObjectOfType<GameBehaviour>().game.gridCompiledEvent.Event -= DisplayGrid;
    }
}
