using Cardboard;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {
    public Player player;

    GridBehaviour grid;

    void Awake()
    {
        grid = FindObjectOfType<GridBehaviour>();
    }

    void Start ()
    {
        Debug.Log(player.position.ToString());
        transform.position = grid.FromPlayerPosition(player.position, player.offset);
    }
	
	void Update () {
        player.Update(Time.deltaTime);
        
        transform.position = grid.FromPlayerPosition(player.position, player.offset);
        transform.rotation = player.rotation * player.offsetrotation;
    }
}
