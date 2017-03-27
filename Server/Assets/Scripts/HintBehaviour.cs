﻿using ProjectCardboardBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintBehaviour : MonoBehaviour
{

    public GameObject hintArrow;
    public GameObject hintPath;
    List<GameObject> hintObjects = new List<GameObject>();
    Vector3[] directions = new Vector3[] { Vector3.forward, Vector3.right, Vector3.back, Vector3.left };


    public void DisplayHint(Vector3 position, float angle, GridBehaviour grid, Command[] hints)
    {
        for (int h = 0; h < hints.Length; h++)
        {
            var hint = hints[h];
            if (hint.type == Action.TURN)
            {
                angle += hint.number * 90;

                if (h < hints.Length - 1)
                    if (hints[h + 1].type == Action.TURN)
                        continue;

                var obj = Instantiate(hintArrow);
                obj.transform.position = position;
                obj.transform.rotation = Quaternion.Euler(0, angle, 0);
                hintObjects.Add(obj);
            }
            if (hint.type == Action.MOVE)
            {
                Vector3 direction = directions[((int)angle / 90) % 4];
                var pos = position;

                for (int i = 1; i <= hint.number; i++)
                {
                    pos = new Vector3(position.x + direction.x * grid.offset.x * i, 0, position.z + direction.z * grid.offset.z * i);
                    pos = grid.SnapToGrid(pos);
                    var obj = Instantiate(hintPath);
                    obj.transform.position = pos;
                    hintObjects.Add(obj);
                }
                position = pos;
            }
        }

    }

    public void HideHint()
    {
        foreach (var hint in hintObjects)
            Destroy(hint);
    }

}