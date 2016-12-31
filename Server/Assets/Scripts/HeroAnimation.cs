using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class HeroAnimation : MonoBehaviour {

    Animator animator;

    public void Idle()
    {
        animator.SetTrigger("Idle");
    }

    public void Taunt()
    {
        animator.SetTrigger("Taunt");
    }

    void Start () {
        animator = GetComponent<Animator>();
	}
}
