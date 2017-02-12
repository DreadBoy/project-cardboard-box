using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AttackDetector : MonoBehaviour
{

    void Start()
    {
    }

    void Update()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        //Two players crashed
        if (GetComponent<PlayerBehaviour>() != null && other.GetComponent<PlayerBehaviour>() != null)
        {
            var player = GetComponent<PlayerBehaviour>();
            var target = other.GetComponent<PlayerBehaviour>();
            if (player.velocity.magnitude > 0)
            {
                Debug.Log(player.name + " is attacking " + target.name + " in direction " + player.velocity);
                target.BounceIntoDirection(player.velocity);
            }
        }
    }
}
