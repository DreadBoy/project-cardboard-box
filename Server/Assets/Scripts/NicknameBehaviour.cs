using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using State = PlayerBehaviour.State;

public class NicknameBehaviour : MonoBehaviour
{
    public Canvas nicknamePrefab;
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
    Canvas canvas;

    public void UpdateNickname(string nickname)
    {
        canvas.GetComponentInChildren<Text>().text = nickname;
    }

    public void UpdatePosition(Vector3 position, PlayerBehaviour player)
    {
        if (canvas == null)
        {
            canvas = Instantiate(nicknamePrefab);
        }
        canvas.transform.SetParent(player.transform.parent);
        var rectTransform = canvas.GetComponent<RectTransform>();
        if (player.state == State.waiting || player.state == State.ready)
        {
            rectTransform.position = new Vector3(position.x, 3.2f, position.z);
            rectTransform.rotation = player.transform.rotation * Quaternion.Euler(0, 180, 0);
            rectTransform.localScale = Vector3.one * 0.6f;

        }
        if (player.state == State.ingame)
        {
            rectTransform.position = position + new Vector3(0, 0.7f, -2.73f);
            rectTransform.rotation = Quaternion.Euler(70, 0, 0);
            rectTransform.localScale = Vector3.one;
        }
        if(player.state == State.ending)
        {
            canvas.GetComponentInChildren<Text>().text = "";
        }
    }
}
