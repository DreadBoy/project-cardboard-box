using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{

    public List<Vector3> positions = new List<Vector3>();
    public List<Vector3> rotations = new List<Vector3>();

    public void GoToPoint(int index)
    {
        if (index >= positions.Count || index < 0 || positions.Count != rotations.Count)
            return;
        transform.position = positions[index];
        transform.rotation = Quaternion.Euler(rotations[index]);
    }

    public struct PointIndex
    {
        public static int lobby = 0;
    }
}
