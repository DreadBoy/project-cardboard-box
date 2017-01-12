using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{

    public List<Vector3> positions = new List<Vector3>()
    {
        new Vector3(-0.77f, 4.5f, -2.94f),
        new Vector3(-0.77f,32.4f, -18.9f)
    };
    public List<Vector3> rotations = new List<Vector3>()
    {
        new Vector3(8.4f, 0, 0),
        new Vector3(70, 0, 0)
    };

    LerpHelper<Vector3> lerpPosition = null;
    LerpHelper<Quaternion> lerpRotation = null;

    void Start()
    {
        FindObjectOfType<GameBehaviour>().changeStateEvent.Event += ChangeStateEvent;
    }

    private void ChangeStateEvent(object sender, changeStateArgs e)
    {
        var index = -1;
        if (e.state == GameBehaviour.State.lobby)
            index = 0;
        if (e.state == GameBehaviour.State.game)
            index = 1;

        if (index > -1)
        {
            lerpPosition = new LerpHelper<Vector3>(transform.position, positions[index], Vector3.Lerp, 1.25f);
            lerpRotation = new LerpHelper<Quaternion>(transform.rotation, Quaternion.Euler(rotations[index]), Quaternion.Lerp, 1.25f);
        }
    }

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

    void Update()
    {
        if (lerpPosition != null)
        {
            lerpPosition.Update(Time.deltaTime);
            transform.position = lerpPosition.Lerp();
            if (lerpPosition.IsDone())
                lerpPosition = null;
        }
        if (lerpRotation != null)
        {
            lerpRotation.Update(Time.deltaTime);
            transform.rotation = lerpRotation.Lerp();
            if (lerpRotation.IsDone())
                lerpRotation = null;
        }
    }
}
