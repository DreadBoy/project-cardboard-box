using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class PlayersTurn : MonoBehaviour {

    public Text playerText;
    Animator animator;
    bool started = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void NewTurn(string playerName)
    {
        playerText.text = playerName;
        if (started)
            animator.SetTrigger("Hide");
        else
            started = true;
        animator.SetTrigger("Show");
    }
}
